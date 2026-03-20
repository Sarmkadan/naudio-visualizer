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

public class AudioBufferAndEventBusTests
{
    // -------------------------------------------------------------------------
    // AudioBuffer
    // -------------------------------------------------------------------------

    [Fact]
    public void Write_SamplesWritten_CountIncreasesAccordingly()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);

        buffer.Write(new float[] { 0.1f, 0.2f, 0.3f });

        buffer.Count.Should().Be(3);
    }

    [Fact]
    public void Read_SamplesConsumed_CountDecreasesAndDataReturned()
    {
        // Arrange
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 0.5f, 0.6f, 0.7f });

        // Act
        var result = buffer.Read(2);

        // Assert
        result.Should().HaveCount(2);
        buffer.Count.Should().Be(1);
    }

    [Fact]
    public void Peek_DoesNotConsumeSamples_CountRemainsUnchanged()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 1f, 2f, 3f });

        buffer.Peek(3);

        buffer.Count.Should().Be(3);
    }

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

    [Fact]
    public void Clear_AfterWriting_BufferReturnsEmpty()
    {
        var buffer = new AudioBuffer(capacity: 10, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[] { 1f, 2f });

        buffer.Clear();

        buffer.IsEmpty().Should().BeTrue();
        buffer.Count.Should().Be(0);
    }

    [Fact]
    public void GetDurationSeconds_KnownSampleCountAndRate_ReturnsCorrectDuration()
    {
        var buffer = new AudioBuffer(capacity: 44100, sampleRate: 44100, channelCount: 1);
        buffer.Write(new float[4410]); // 4410 / 44100 = 0.1 seconds

        var duration = buffer.GetDurationSeconds();

        duration.Should().BeApproximately(0.1, precision: 0.001);
    }

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

    [Fact]
    public void Set_ThenTryGetValue_ReturnsStoredValue()
    {
        var cache = new CacheManager<string, int>();

        cache.Set("sampleRate", 44100);
        var found = cache.TryGetValue("sampleRate", out var value);

        found.Should().BeTrue();
        value.Should().Be(44100);
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrueAndEntryIsGone()
    {
        var cache = new CacheManager<string, string>();
        cache.Set("key", "value");

        var removed = cache.Remove("key");

        removed.Should().BeTrue();
        cache.Contains("key").Should().BeFalse();
    }

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

    [Fact]
    public void GetSubscriberCount_AfterTwoSubscribes_ReturnsTwo()
    {
        var bus = new EventBus();

        bus.Subscribe<string>(_ => { });
        bus.Subscribe<string>(_ => { });

        bus.GetSubscriberCount<string>().Should().Be(2);
    }

    [Fact]
    public void Publish_NoSubscribersRegistered_DoesNotThrow()
    {
        var bus = new EventBus();

        var act = () => bus.Publish("no-listeners");

        act.Should().NotThrow();
    }

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
