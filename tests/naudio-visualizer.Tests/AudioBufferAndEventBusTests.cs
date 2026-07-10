#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using FluentAssertions;
using Moq;
using NAudioVisualizer.Caching;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using Xunit;

namespace NAudioVisualizer.Tests;

/// <summary>
/// Contains unit tests for the <see cref="AudioBuffer"/> class and <see cref="EventBus"/> class,
/// covering audio sample buffering functionality and event bus subscription/publishing behavior.
/// </summary>
public class AudioBufferAndEventBusTests
{
    // -------------------------------------------------------------------------
    // AudioBuffer
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests for the <see cref="AudioBuffer"/> class, verifying audio sample buffering behavior including
    /// writing, reading, peeking, capacity management, clearing, and duration calculation.
    /// </summary>

    /// <summary>
    /// Tests that writing audio samples to the buffer correctly increases the sample count.
    /// </summary>
    [Fact]
    public void Write_SamplesWritten_CountIncreasesAccordingly()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);

        buffer.Write(new float[] { 0.1f, 0.2f, 0.3f });

        buffer.Count.Should().Be(3);
    }

    /// <summary>
    /// Tests that reading samples from the buffer correctly consumes them and decreases the count.
    /// </summary>
    [Fact]
    public void Read_SamplesConsumed_CountDecreasesAndDataReturned()
    {
        // Arrange
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 0.5f, 0.6f, 0.7f });

        // Act
        var result = buffer.Read(2, out _);

        // Assert
        result.Should().HaveCount(2);
        buffer.Count.Should().Be(1);
    }

    /// <summary>
    /// Tests that peeking at samples does not consume them, leaving the count unchanged.
    /// </summary>
    [Fact]
    public void Peek_DoesNotConsumeSamples_CountRemainsUnchanged()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 1f, 2f, 3f });

        buffer.Peek(3);

        buffer.Count.Should().Be(3);
    }

    /// <summary>
    /// Tests that when writing exceeds capacity, the oldest samples are overwritten.
    /// </summary>
    [Fact]
    public void Write_ExceedsCapacity_OldestSamplesAreOverwritten()
    {
        var buffer = new AudioBuffer(capacity: 4, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 1f, 2f, 3f, 4f });

        buffer.Write(new float[] { 5f });

        buffer.Count.Should().Be(4);
        buffer.IsFull().Should().BeTrue();
        // The oldest sample (1f) was evicted; newest sample (5f) is present
        buffer.GetAll().Should().Contain(5f);
    }

    /// <summary>
    /// Tests that clearing the buffer after writing returns it to an empty state.
    /// </summary>
    [Fact]
    public void Clear_AfterWriting_BufferReturnsEmpty()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 1f, 2f });

        buffer.Clear();

        buffer.IsEmpty().Should().BeTrue();
        buffer.Count.Should().Be(0);
    }

    /// <summary>
    /// Tests that duration calculation returns the correct duration based on sample count and rate.
    /// </summary>
    [Fact]
    public void GetDurationSeconds_KnownSampleCountAndRate_ReturnsCorrectDuration()
    {
        var buffer = new AudioBuffer(capacity: 44100, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[4410]); // 4410 / 44100 = 0.1 seconds

        var duration = buffer.GetDurationSeconds();

        duration.Should().BeApproximately(0.1, precision: 0.001);
    }

    /// <summary>
    /// Tests that reading with insufficient samples returns zero-padded results and reports correct actual samples read.
    /// </summary>
    [Fact]
    public void Read_InsufficientSamples_ReturnsZeroPaddedAndCorrectActualSamplesRead()
    {
        // Arrange
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 0.1f, 0.2f, 0.3f }); // Write 3 samples

        // Act - Request to read 5 samples, but only 3 are available
        var result = buffer.Read(5, out int actualSamplesRead);

        // Assert
        result.Should().HaveCount(5); // Should return an array of the requested size
        actualSamplesRead.Should().Be(3); // Should report 3 actual samples read
        result[0].Should().BeApproximately(0.1f, 0.0001f);
        result[1].Should().BeApproximately(0.2f, 0.0001f);
        result[2].Should().BeApproximately(0.3f, 0.0001f);
        result[3].Should().BeApproximately(0.0f, 0.0001f); // Padded with zero
        result[4].Should().BeApproximately(0.0f, 0.0001f); // Padded with zero
        buffer.Count.Should().Be(0); // All available samples should be consumed
    }

    // -------------------------------------------------------------------------
    // CacheManager
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tests for the <see cref="CacheManager"/> class, verifying cache storage and retrieval functionality.
    /// </summary>

    /// <summary>
    /// Tests that setting a value and then retrieving it returns the stored value.
    /// </summary>
    [Fact]
    public void Set_ThenTryGetValue_ReturnsStoredValue()
    {
        var cache = new CacheManager<string, int>();

        cache.Set("sampleRate", 44100);
        var found = cache.TryGetValue("sampleRate", out var value);

        found.Should().BeTrue();
        value.Should().Be(44100);
    }

    /// <summary>
    /// Tests that removing an existing key returns true and the entry is removed.
    /// </summary>
    [Fact]
    public void Remove_ExistingKey_ReturnsTrueAndEntryIsGone()
    {
        var cache = new CacheManager<string, string>();
        cache.Set("key", "value");

        var removed = cache.Remove("key");

        removed.Should().BeTrue();
        cache.Contains("key").Should().BeFalse();
    }

    /// <summary>
    /// Tests that trying to get an expired entry returns false.
    /// </summary>
    [Fact]
    public void TryGetValue_ExpiredEntry_ReturnsFalse()
    {
        // Arrange: set a 1 ms expiration so the entry expires immediately
        var cache = new CacheManager<string, string>(defaultExpiration: TimeSpan.FromMilliseconds(1));
        cache.Set("key", "value");

        System.Threading.Thread.Sleep(20);

        // Act & Assert
        cache.TryGetValue("key", out _).Should().BeFalse();
    }

    /// <summary>
    /// Tests that getting statistics after adding entries reflects the current size.
    /// </summary>
    [Fact]
    public void GetStatistics_AfterAddingEntries_ReflectsCurrentSize()
    {
        var cache = new CacheManager<string, string>(maxSize: 100);
        cache.Set("a", "1");
        cache.Set("b", "2");

        var stats = cache.GetStatistics();

        stats.CurrentSize.Should().Be(2);
        stats.MaxSize.Should().Be(100);
    }

    // -------------------------------------------------------------------------
    // EventBus — uses Moq to verify handler invocations
    // -------------------------------------------------------------------------

    public interface IAudioEventHandler
    {
        void OnEvent(string payload);
    }

    /// <summary>
    /// Tests that publishing an event with a registered subscriber invokes the handler once.
    /// </summary>
    [Fact]
    public void Publish_WithRegisteredSubscriber_HandlerIsInvokedOnce()
    {
        // Arrange
        var bus = new EventBus();
        var mockHandler = new Mock<IAudioEventHandler>();
        bus.Subscribe<string>(e => mockHandler.Object.OnEvent(e));

        // Act
        bus.Publish("spectrum-ready");

        // Assert
        mockHandler.Verify(h => h.OnEvent("spectrum-ready"), Times.Once);
    }

    /// <summary>
    /// Tests that publishing after unsubscribing all handlers never invokes any handler.
    /// </summary>
    [Fact]
    public void Publish_AfterUnsubscribeAll_HandlerIsNeverInvoked()
    {
        // Arrange
        var bus = new EventBus();
        var mockHandler = new Mock<IAudioEventHandler>();
        bus.Subscribe<string>(e => mockHandler.Object.OnEvent(e));
        bus.UnsubscribeAll<string>();

        // Act
        bus.Publish("spectrum-ready");

        // Assert
        mockHandler.Verify(h => h.OnEvent(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Tests that getting subscriber count after two subscriptions returns two.
    /// </summary>
    [Fact]
    public void GetSubscriberCount_AfterTwoSubscribes_ReturnsTwo()
    {
        var bus = new EventBus();

        bus.Subscribe<string>(_ => { });
        bus.Subscribe<string>(_ => { });

        bus.GetSubscriberCount<string>().Should().Be(2);
    }

    /// <summary>
    /// Tests that publishing with no subscribers registered does not throw an exception.
    /// </summary>
    [Fact]
    public void Publish_NoSubscribersRegistered_DoesNotThrow()
    {
        var bus = new EventBus();

        var act = () => bus.Publish("no-listeners");

        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that disposing a subscription token removes the subscription.
    /// </summary>
    [Fact]
    public void Subscribe_DisposeToken_RemovesSubscription()
    {
        var bus = new EventBus();
        var mockHandler = new Mock<IAudioEventHandler>();

        var token = bus.Subscribe<string>(e => mockHandler.Object.OnEvent(e));
        token.Dispose();
        bus.Publish("after-dispose");

        mockHandler.Verify(h => h.OnEvent(It.IsAny<string>()), Times.Never);
    }
}