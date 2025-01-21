// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models;

// ── Enumerations ──────────────────────────────────────────────────────────────

/// <summary>
/// Lifecycle states for a VST plugin instance managed by the host.
/// </summary>
public enum VstPluginState
{
    /// <summary>Plugin library not yet loaded into memory.</summary>
    Unloaded = 0,

    /// <summary>Library loaded; waiting for <c>Initialize()</c> to be called.</summary>
    Loaded = 1,

    /// <summary>One-time initialisation in progress.</summary>
    Initializing = 2,

    /// <summary>Fully initialised and accepting audio I/O.</summary>
    Active = 3,

    /// <summary>Temporarily suspended; state is preserved, real-time resources freed.</summary>
    Suspended = 4,

    /// <summary>Unrecoverable error; the plugin should be unloaded and discarded.</summary>
    Error = 5
}

/// <summary>
/// Functional categories used to classify and filter VST plugins in the browser.
/// </summary>
public enum VstPluginCategory
{
    /// <summary>Category has not been declared by the plugin.</summary>
    Undefined = 0,

    /// <summary>General audio effect (e.g. compressor, saturator).</summary>
    Effect = 1,

    /// <summary>Software synthesiser / instrument.</summary>
    Synth = 2,

    /// <summary>Spectrum or metering analyser.</summary>
    Analyzer = 3,

    /// <summary>Spatial / positioning effect (panner, binaural).</summary>
    Spatial = 4,

    /// <summary>Mastering-grade processing (limiter, stereo enhancer).</summary>
    Mastering = 5,

    /// <summary>Dynamic-range processor (compressor, gate, expander).</summary>
    Dynamics = 6,

    /// <summary>Equaliser or filter.</summary>
    EQ = 7,

    /// <summary>Reverberation or room simulation.</summary>
    Reverb = 8,

    /// <summary>Delay or echo.</summary>
    Delay = 9,

    /// <summary>Distortion, overdrive, or saturation.</summary>
    Distortion = 10,

    /// <summary>Modulation effect (chorus, flanger, phaser, tremolo).</summary>
    Modulation = 11
}

/// <summary>
/// Interpolation curves available when evaluating a <see cref="VstParameterAutomationLane"/>.
/// </summary>
public enum VstAutomationInterpolation
{
    /// <summary>Straight line between adjacent automation points.</summary>
    Linear = 0,

    /// <summary>Smooth Catmull-Rom cubic spline through surrounding points.</summary>
    CubicSpline = 1,

    /// <summary>Instant jump to the next point's value with no ramp.</summary>
    Step = 2,

    /// <summary>S-curve cosine crossfade between adjacent points.</summary>
    Cosine = 3
}

// ── Immutable plugin descriptor ───────────────────────────────────────────────

/// <summary>
/// Immutable snapshot of a loaded VST plugin's identity, capabilities, and provenance.
/// Produced by the host at load time and never mutated thereafter.
/// </summary>
public sealed record VstPluginInfo(
    Guid Id,
    string Name,
    string Vendor,
    string Version,
    string PluginPath,
    VstPluginCategory Category,
    int ParameterCount,
    bool IsSynth)
{
    /// <summary>UTC timestamp recorded when the plugin was first loaded into the host.</summary>
    public DateTime LoadedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Four-character unique ID reported by the plugin (VST2 <c>uniqueID</c> field),
    /// or an empty string for plugins that do not expose one.
    /// </summary>
    public string UniqueId { get; init; } = string.Empty;

    /// <summary>Human-readable VST SDK version string (e.g. "VST 2.4", "VST 3.7").</summary>
    public string SdkVersion { get; init; } = "VST 2.4";

    /// <summary>Maximum number of audio I/O channels the plugin supports per side.</summary>
    public int MaxChannels { get; init; } = 2;

    /// <summary>
    /// Checks that all required fields are present and consistent.
    /// </summary>
    public bool IsValid() =>
        Id != Guid.Empty &&
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(PluginPath) &&
        ParameterCount >= 0 &&
        MaxChannels > 0;

    /// <inheritdoc/>
    public override string ToString() => $"{Vendor} – {Name} v{Version} [{Category}]";
}

// ── Parameter descriptor ──────────────────────────────────────────────────────

/// <summary>
/// Live descriptor for a single automatable control exposed by a VST plugin.
/// The host updates <see cref="CurrentValue"/> as parameters change during playback.
/// </summary>
public sealed class VstParameter(
    int id,
    string name,
    string label,
    string units,
    float minValue,
    float maxValue,
    float defaultValue)
{
    /// <summary>Zero-based parameter index used in all VST API calls.</summary>
    public int Id { get; } = id;

    /// <summary>Short human-readable name displayed in the parameter list (e.g. "Cutoff", "Attack").</summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>Display label appended to the formatted value in a UI (e.g. "Hz", "ms", "%").</summary>
    public string Label { get; } = label ?? string.Empty;

    /// <summary>Measurement unit hint for tooltip display (e.g. "dB", "semitones").</summary>
    public string Units { get; } = units ?? string.Empty;

    /// <summary>Minimum raw (domain) value on the plugin's internal scale.</summary>
    public float MinValue { get; } = minValue;

    /// <summary>Maximum raw (domain) value on the plugin's internal scale.</summary>
    public float MaxValue { get; } = maxValue;

    /// <summary>Default raw value restored when the user resets the parameter.</summary>
    public float DefaultValue { get; } = defaultValue;

    /// <summary>Current raw value; updated by the host in real time from automation or UI gestures.</summary>
    public float CurrentValue { get; set; } = defaultValue;

    /// <summary>
    /// <see langword="true"/> when an automation lane is actively writing to this parameter.
    /// </summary>
    public bool IsAutomated { get; set; }

    /// <summary>Optional grouping name for organising parameters in a browser (e.g. "Envelope", "Filter").</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// When <see langword="true"/> the host must not write this parameter;
    /// it is updated exclusively by the plugin itself.
    /// </summary>
    public bool IsReadOnly { get; init; }

    /// <summary>
    /// Current value normalised to [0.0, 1.0].
    /// Returns <c>0</c> when the usable range is zero.
    /// </summary>
    public float NormalizedValue
    {
        get
        {
            float range = MaxValue - MinValue;
            return Math.Abs(range) < 0.0001f ? 0f : (CurrentValue - MinValue) / range;
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> when all field invariants hold and
    /// <see cref="CurrentValue"/> is within the declared range.
    /// </summary>
    public bool IsValid() =>
        MinValue <= MaxValue &&
        CurrentValue >= MinValue &&
        CurrentValue <= MaxValue &&
        !float.IsNaN(CurrentValue) &&
        !float.IsInfinity(CurrentValue);

    /// <summary>
    /// Converts a normalised [0, 1] value to the plugin's raw domain range.
    /// </summary>
    public float DenormalizeValue(float normalized) =>
        MinValue + Math.Clamp(normalized, 0f, 1f) * (MaxValue - MinValue);

    /// <inheritdoc/>
    public override string ToString() => $"{Name}: {CurrentValue:F3} {Units}".TrimEnd();
}

// ── Automation point ──────────────────────────────────────────────────────────

/// <summary>
/// A single timed value on an automation lane, identified by its position in seconds.
/// </summary>
public sealed record VstParameterAutomationPoint(double PositionSeconds, float Value)
{
    /// <summary>
    /// Interpolation shape applied between this point and the next one on the lane.
    /// </summary>
    public VstAutomationInterpolation Interpolation { get; init; } = VstAutomationInterpolation.Linear;

    /// <summary>
    /// Returns <see langword="true"/> when the point's position is non-negative and
    /// <see cref="Value"/> is a normal finite number.
    /// </summary>
    public bool IsValid() =>
        PositionSeconds >= 0d &&
        !float.IsNaN(Value) &&
        !float.IsInfinity(Value);
}

// ── Automation lane ───────────────────────────────────────────────────────────

/// <summary>
/// An ordered, thread-safe collection of <see cref="VstParameterAutomationPoint"/> values
/// that drives a single VST parameter over a timeline.
/// </summary>
public sealed class VstParameterAutomationLane
{
    private readonly List<VstParameterAutomationPoint> _points = [];
    private readonly object _lock = new();

    /// <summary>Plugin that owns the parameter driven by this lane.</summary>
    public required Guid PluginId { get; init; }

    /// <summary>Zero-based parameter index within the owning plugin.</summary>
    public required int ParameterId { get; init; }

    /// <summary>Display name copied from the parameter descriptor at creation time.</summary>
    public string ParameterName { get; init; } = string.Empty;

    /// <summary>
    /// When <see langword="false"/> the lane is ignored during playback evaluation.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Read-only snapshot of the automation points sorted ascending by position.
    /// </summary>
    public IReadOnlyList<VstParameterAutomationPoint> Points
    {
        get { lock (_lock) return _points.AsReadOnly(); }
    }

    /// <summary>
    /// Total number of automation points currently on the lane.
    /// </summary>
    public int PointCount { get { lock (_lock) return _points.Count; } }

    /// <summary>
    /// Inserts a point at <paramref name="positionSeconds"/>, replacing any existing point
    /// within 1 ms of that position.
    /// </summary>
    /// <param name="positionSeconds">Timeline position in seconds (must be ≥ 0).</param>
    /// <param name="value">Normalised parameter value to record at this position.</param>
    /// <param name="interpolation">Curve shape applied toward the next point.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="positionSeconds"/> is negative.
    /// </exception>
    public void AddPoint(
        double positionSeconds,
        float value,
        VstAutomationInterpolation interpolation = VstAutomationInterpolation.Linear)
    {
        if (positionSeconds < 0d)
            throw new ArgumentOutOfRangeException(nameof(positionSeconds), "Position must be non-negative.");

        var point = new VstParameterAutomationPoint(positionSeconds, value)
        {
            Interpolation = interpolation
        };

        lock (_lock)
        {
            _points.RemoveAll(p => Math.Abs(p.PositionSeconds - positionSeconds) < 0.001);
            _points.Add(point);
            _points.Sort((a, b) => a.PositionSeconds.CompareTo(b.PositionSeconds));
        }
    }

    /// <summary>
    /// Removes the point closest to <paramref name="positionSeconds"/> (within a 1 ms tolerance).
    /// </summary>
    public void RemovePoint(double positionSeconds)
    {
        lock (_lock)
            _points.RemoveAll(p => Math.Abs(p.PositionSeconds - positionSeconds) < 0.001);
    }

    /// <summary>
    /// Evaluates the lane at <paramref name="positionSeconds"/> using the interpolation shape
    /// stored on each point.
    /// </summary>
    /// <returns>
    /// The interpolated value, or <see langword="null"/> when the lane has no points.
    /// </returns>
    public float? Evaluate(double positionSeconds)
    {
        IReadOnlyList<VstParameterAutomationPoint> snapshot;
        lock (_lock) snapshot = _points.AsReadOnly();

        if (snapshot.Count == 0) return null;
        if (snapshot.Count == 1) return snapshot[0].Value;
        if (positionSeconds <= snapshot[0].PositionSeconds) return snapshot[0].Value;
        if (positionSeconds >= snapshot[^1].PositionSeconds) return snapshot[^1].Value;

        // Binary-search for the segment that contains positionSeconds
        int lo = 0, hi = snapshot.Count - 2;
        while (lo < hi)
        {
            int mid = (lo + hi + 1) / 2;
            if (snapshot[mid].PositionSeconds <= positionSeconds) lo = mid;
            else hi = mid - 1;
        }

        var p0 = snapshot[lo];
        var p1 = snapshot[lo + 1];
        double segLen = p1.PositionSeconds - p0.PositionSeconds;
        double t = segLen < 1e-12 ? 0d : (positionSeconds - p0.PositionSeconds) / segLen;

        return p0.Interpolation switch
        {
            VstAutomationInterpolation.Step       => p0.Value,
            VstAutomationInterpolation.Cosine     => InterpolateCosine(p0.Value, p1.Value, t),
            VstAutomationInterpolation.CubicSpline => InterpolateCubic(snapshot, lo, t),
            _                                     => p0.Value + (float)t * (p1.Value - p0.Value)
        };
    }

    /// <summary>Removes all points from the lane, leaving it empty but still registered.</summary>
    public void Clear()
    {
        lock (_lock) _points.Clear();
    }

    // ── Interpolation helpers ─────────────────────────────────────────────

    private static float InterpolateCosine(float v0, float v1, double t) =>
        v0 + (float)((1d - Math.Cos(t * Math.PI)) / 2d) * (v1 - v0);

    private static float InterpolateCubic(
        IReadOnlyList<VstParameterAutomationPoint> pts, int i, double t)
    {
        // Catmull-Rom: use neighbouring points for tangent estimation
        float p0 = i > 0              ? pts[i - 1].Value        : pts[i].Value;
        float p1 = pts[i].Value;
        float p2 = pts[i + 1].Value;
        float p3 = i + 2 < pts.Count ? pts[i + 2].Value        : pts[i + 1].Value;

        float ft = (float)t;
        float ft2 = ft * ft;
        float ft3 = ft2 * ft;

        return 0.5f * ((2f * p1)
            + (-p0 + p2)                            * ft
            + (2f * p0 - 5f * p1 + 4f * p2 - p3)   * ft2
            + (-p0 + 3f * p1 - 3f * p2 + p3)        * ft3);
    }
}

// ── Preset ────────────────────────────────────────────────────────────────────

/// <summary>
/// A complete, named snapshot of a VST plugin's parameter state that can be saved,
/// loaded, categorised, and applied back to the plugin at any time.
/// </summary>
public sealed class VstPreset
{
    /// <summary>Unique identifier assigned when the preset is created.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Identifier of the plugin this preset was captured from.</summary>
    public required Guid PluginId { get; init; }

    /// <summary>Human-readable name displayed in the preset browser.</summary>
    public required string Name { get; init; }

    /// <summary>Optional prose description of the sound or intended use.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Top-level category for browser organisation (e.g. "Pads", "Leads", "Bus Comp").
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Freeform tags that enable multi-dimensional filtering (e.g. "warm", "dark", "analog").
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>
    /// Raw opaque chunk captured via <c>IVstPlugin.GetChunk(isPreset: true)</c>.
    /// Takes precedence over <see cref="ParameterValues"/> when non-empty.
    /// </summary>
    public byte[]? PluginChunk { get; init; }

    /// <summary>
    /// Explicit parameter values keyed by zero-based parameter ID.
    /// Used when the plugin does not support chunk serialisation or for fine-grained overrides.
    /// Values are normalised to [0.0, 1.0].
    /// </summary>
    public IReadOnlyDictionary<int, float> ParameterValues { get; init; } =
        new Dictionary<int, float>();

    /// <summary>UTC timestamp when this preset was first created.</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// When <see langword="true"/> this is a factory preset shipped with the plugin
    /// and should be treated as read-only in the UI.
    /// </summary>
    public bool IsFactory { get; init; }

    /// <summary>
    /// Name of the author who created the preset, or empty for factory presets.
    /// </summary>
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Returns <see langword="true"/> when the preset contains the minimum data required
    /// for it to be applied: a non-empty name, a valid plugin reference, and at least one
    /// source of parameter state (chunk or individual values).
    /// </summary>
    public bool IsValid() =>
        PluginId != Guid.Empty &&
        !string.IsNullOrWhiteSpace(Name) &&
        (PluginChunk is { Length: > 0 } || ParameterValues.Count > 0);

    /// <inheritdoc/>
    public override string ToString() =>
        string.IsNullOrEmpty(Category) ? Name : $"[{Category}] {Name}";
}
