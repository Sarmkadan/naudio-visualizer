#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.VST;

/// <summary>
/// Records, stores, and plays back parameter automation lanes for VST plugins.
/// </summary>
/// <remarks>
/// <para>
/// <b>Recording:</b> call <see cref="StartRecording"/>, then <see cref="RecordParameterChange"/>
/// from any UI or MIDI handler, then <see cref="StopRecording"/>.
/// </para>
/// <para>
/// <b>Playback:</b> call <see cref="StartPlayback"/> once, then drive <see cref="Tick"/> every
/// audio block. <see cref="Tick"/> evaluates all enabled lanes and writes values directly to the
/// target plugins via the supplied <see cref="IVstPluginHost"/>.
/// </para>
/// <para>All public members are thread-safe.</para>
/// </remarks>
public sealed class VstParameterAutomation : IDisposable
{
    private readonly ConcurrentDictionary<(Guid PluginId, int ParameterId), VstParameterAutomationLane> _lanes
        = new();
    private readonly Logger _logger;
    private readonly Stopwatch _clock = new();
    private double _playbackStartOffset;
    private volatile bool _isRecording;
    private bool _isDisposed;

    /// <summary>
    /// Returns <see langword="true"/> while the engine is stamping incoming parameter changes
    /// into their respective automation lanes.
    /// </summary>
    public bool IsRecording => _isRecording;

    /// <summary>
    /// Returns <see langword="true"/> while the engine is evaluating automation data and
    /// applying it to plugins via <see cref="Tick"/>.
    /// </summary>
    public bool IsPlayingBack => _clock.IsRunning;

    /// <summary>
    /// Current playback position in seconds from the start of the timeline.
    /// Returns zero when stopped.
    /// </summary>
    public double PlaybackPositionSeconds =>
        _clock.IsRunning
            ? _playbackStartOffset + _clock.Elapsed.TotalSeconds
            : 0d;

    /// <summary>
    /// All currently registered automation lanes, keyed by (PluginId, ParameterId).
    /// </summary>
    public IReadOnlyDictionary<(Guid PluginId, int ParameterId), VstParameterAutomationLane> Lanes
        => _lanes;

    /// <summary>
    /// Total number of registered lanes, regardless of whether they are enabled.
    /// </summary>
    public int LaneCount => _lanes.Count;

    /// <summary>
    /// Creates a new automation engine backed by <paramref name="logger"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public VstParameterAutomation(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ── Lane management ────────────────────────────────────────────────────

    /// <summary>
    /// Registers a new automation lane for the given plugin/parameter pair and returns it.
    /// If a lane already exists for that pair, the existing instance is returned unchanged.
    /// </summary>
    /// <param name="pluginId">Target plugin's unique identifier.</param>
    /// <param name="parameterId">Zero-based parameter index within the plugin.</param>
    /// <param name="parameterName">Optional display name copied from the parameter descriptor.</param>
    public VstParameterAutomationLane AddLane(
        Guid pluginId,
        int parameterId,
        string parameterName = "")
    {
        var key = (pluginId, parameterId);
        return _lanes.GetOrAdd(key, _ => new VstParameterAutomationLane
        {
            PluginId      = pluginId,
            ParameterId   = parameterId,
            ParameterName = parameterName
        });
    }

    /// <summary>
    /// Removes the lane for the given plugin/parameter pair and returns
    /// <see langword="true"/> if a lane was found and removed.
    /// </summary>
    public bool RemoveLane(Guid pluginId, int parameterId) =>
        _lanes.TryRemove((pluginId, parameterId), out _);

    /// <summary>
    /// Returns the automation lane for the given plugin/parameter pair, or
    /// <see langword="null"/> when no lane is registered for that pair.
    /// </summary>
    public VstParameterAutomationLane? GetLane(Guid pluginId, int parameterId) =>
        _lanes.TryGetValue((pluginId, parameterId), out var lane) ? lane : null;

    /// <summary>
    /// Returns all lanes registered for a specific plugin, regardless of enabled state.
    /// </summary>
    public IReadOnlyList<VstParameterAutomationLane> GetLanesForPlugin(Guid pluginId) =>
        _lanes.Values.Where(l => l.PluginId == pluginId).ToList();

    // ── Recording ──────────────────────────────────────────────────────────

    /// <summary>
    /// Starts the recording clock. Subsequent <see cref="RecordParameterChange"/> calls
    /// will stamp time-relative points into the appropriate lane.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the engine is disposed.</exception>
    public void StartRecording()
    {
        ThrowIfDisposed();
        _isRecording = true;
        _playbackStartOffset = 0d;
        _clock.Restart();

        _logger.Info("VST parameter automation recording started.");
        EventPublisher.Instance.Publish(new VstAutomationRecordingStartedEvent
        {
            LaneCount = _lanes.Count
        });
    }

    /// <summary>
    /// Stops the recording clock. Points already written to lanes are preserved.
    /// </summary>
    public void StopRecording()
    {
        _isRecording = false;
        _clock.Stop();

        int totalPoints = _lanes.Values.Sum(l => l.PointCount);
        _logger.Info($"VST automation recording stopped. {totalPoints} points recorded across {_lanes.Count} lane(s).");

        EventPublisher.Instance.Publish(new VstAutomationRecordingStoppedEvent
        {
            TotalPointsRecorded = totalPoints
        });
    }

    /// <summary>
    /// Writes a parameter change to the matching lane at the current recording position.
    /// A lane is created automatically if one does not yet exist.
    /// Has no effect when <see cref="IsRecording"/> is <see langword="false"/>.
    /// </summary>
    /// <param name="pluginId">Plugin whose parameter changed.</param>
    /// <param name="parameterId">Zero-based parameter index.</param>
    /// <param name="normalizedValue">Normalised value in [0.0, 1.0] to record.</param>
    /// <param name="parameterName">Optional display name used when creating a new lane.</param>
    public void RecordParameterChange(
        Guid pluginId,
        int parameterId,
        float normalizedValue,
        string parameterName = "")
    {
        if (!_isRecording) return;

        var lane = AddLane(pluginId, parameterId, parameterName);
        lane.AddPoint(PlaybackPositionSeconds, normalizedValue);
    }

    // ── Playback ───────────────────────────────────────────────────────────

    /// <summary>
    /// Begins playback from <paramref name="startPositionSeconds"/> on the timeline.
    /// Call <see cref="Tick"/> every audio block to apply automation values to plugins.
    /// </summary>
    /// <param name="startPositionSeconds">
    /// Timeline offset from which to begin; defaults to the beginning (0.0 s).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="startPositionSeconds"/> is negative.
    /// </exception>
    public void StartPlayback(double startPositionSeconds = 0d)
    {
        ThrowIfDisposed();
        if (startPositionSeconds < 0d)
            throw new ArgumentOutOfRangeException(nameof(startPositionSeconds), "Start position must be non-negative.");

        _playbackStartOffset = startPositionSeconds;
        _clock.Restart();

        int activeLanes = _lanes.Count(kv => kv.Value.IsEnabled);
        _logger.Info($"VST automation playback started at {startPositionSeconds:F3}s ({activeLanes} active lane(s)).");

        EventPublisher.Instance.Publish(new VstAutomationPlaybackStartedEvent
        {
            StartPositionSeconds = startPositionSeconds,
            ActiveLaneCount      = activeLanes
        });
    }

    /// <summary>
    /// Stops playback. Lane data is preserved and playback can be restarted at any position.
    /// </summary>
    public void StopPlayback()
    {
        double finalPos = PlaybackPositionSeconds;
        _clock.Stop();

        _logger.Info($"VST automation playback stopped at {finalPos:F3}s.");
        EventPublisher.Instance.Publish(new VstAutomationPlaybackStoppedEvent
        {
            FinalPositionSeconds = finalPos
        });
    }

    /// <summary>
    /// Seeks the playback head to <paramref name="positionSeconds"/> without stopping.
    /// </summary>
    /// <param name="positionSeconds">New timeline position in seconds (must be ≥ 0).</param>
    public void Seek(double positionSeconds)
    {
        if (positionSeconds < 0d)
            throw new ArgumentOutOfRangeException(nameof(positionSeconds));

        _playbackStartOffset = positionSeconds;
        if (_clock.IsRunning) _clock.Restart();
    }

    /// <summary>
    /// Evaluates all enabled automation lanes at the current playback position and applies
    /// the resulting values to the target plugins via <paramref name="pluginHost"/>.
    /// </summary>
    /// <remarks>
    /// This method is designed to be called once per audio processing block on the audio
    /// thread. It is lock-free for lane evaluation; lane list access uses a snapshot.
    /// </remarks>
    /// <param name="pluginHost">
    /// The plugin host used to look up plugins and forward parameter writes.
    /// </param>
    public void Tick(IVstPluginHost pluginHost)
    {
        if (!IsPlayingBack || pluginHost is null) return;

        double position = PlaybackPositionSeconds;

        foreach (var (key, lane) in _lanes)
        {
            if (!lane.IsEnabled) continue;

            float? value = lane.Evaluate(position);
            if (value is null) continue;

            var plugin = pluginHost.FindPlugin(key.PluginId);
            if (plugin is null || plugin.State != VstPluginState.Active || plugin.IsBypassed)
                continue;

            try
            {
                plugin.SetParameter(key.ParameterId, value.Value);
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    $"Automation failed to write plugin {key.PluginId} " +
                    $"param {key.ParameterId}: {ex.Message}");
            }
        }
    }

    // ── Bulk operations ────────────────────────────────────────────────────

    /// <summary>
    /// Removes all recorded points from every lane without unregistering the lanes themselves.
    /// </summary>
    public void ClearAllRecordings()
    {
        foreach (var lane in _lanes.Values)
            lane.Clear();
    }

    /// <summary>
    /// Unregisters and discards every automation lane.
    /// </summary>
    public void ClearAllLanes() => _lanes.Clear();

    /// <summary>
    /// Sets <see cref="VstParameterAutomationLane.IsEnabled"/> on all lanes that belong
    /// to the specified plugin.
    /// </summary>
    public void SetPluginLanesEnabled(Guid pluginId, bool enabled)
    {
        foreach (var lane in _lanes.Values.Where(l => l.PluginId == pluginId))
            lane.IsEnabled = enabled;
    }

    // ── Lifecycle ──────────────────────────────────────────────────────────

    private void ThrowIfDisposed()
    {
        if (_isDisposed) throw new ObjectDisposedException(GetType().Name);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed) return;

        if (IsPlayingBack) StopPlayback();
        if (IsRecording)   StopRecording();

        _lanes.Clear();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

// ── Automation event classes ───────────────────────────────────────────────────

/// <summary>Raised when automation recording begins.</summary>
public class VstAutomationRecordingStartedEvent
{
    /// <summary>Number of lanes registered at the moment recording started.</summary>
    public int LaneCount { get; init; }

    /// <summary>UTC timestamp.</summary>
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when automation recording ends.</summary>
public class VstAutomationRecordingStoppedEvent
{
    /// <summary>Total number of automation points written across all lanes.</summary>
    public int TotalPointsRecorded { get; init; }

    /// <summary>UTC timestamp.</summary>
    public DateTime StoppedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when automation playback begins.</summary>
public class VstAutomationPlaybackStartedEvent
{
    /// <summary>Timeline position from which playback started, in seconds.</summary>
    public double StartPositionSeconds { get; init; }

    /// <summary>Number of enabled lanes at the moment playback started.</summary>
    public int ActiveLaneCount { get; init; }

    /// <summary>UTC timestamp.</summary>
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised when automation playback stops.</summary>
public class VstAutomationPlaybackStoppedEvent
{
    /// <summary>Timeline position at which playback was stopped, in seconds.</summary>
    public double FinalPositionSeconds { get; init; }

    /// <summary>UTC timestamp.</summary>
    public DateTime StoppedAt { get; init; } = DateTime.UtcNow;
}
