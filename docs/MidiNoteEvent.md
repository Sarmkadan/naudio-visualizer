# MidiNoteEvent

The `MidiNoteEvent` class represents a singular MIDI note event within the `naudio-visualizer` project. It encapsulates relevant data concerning a note's properties, including timing, pitch, velocity, and the originating device, providing a structured representation for processing and visual feedback applications.

## API

### Instance Properties

- `Channel` (int): Gets or sets the MIDI channel associated with the event (typically 1–16).
- `NoteNumber` (int): Gets or sets the MIDI note number (0–127).
- `NoteName` (string): Gets or sets the human-readable string representation of the note (e.g., "C4").
- `Velocity` (int): Gets or sets the note velocity (0–127).
- `IsNoteOn` (bool): Indicates whether the event represents a note-on signal.
- `Frequency` (float): Gets or sets the calculated frequency of the note in Hertz.
- `Timestamp` (DateTime): The time at which the MIDI event occurred.
- `DeviceIndex` (int): The index of the MIDI device that produced the event.
- `Index` (int): The chronological index of this event within a stream or sequence.
- `ProductName` (string): The name of the MIDI hardware or software device that generated the event.
- `IsAvailable` (bool): Indicates the current availability status of the associated MIDI device.
- `IsValid` (bool): Indicates whether the event data is considered valid for processing.

### Static Methods

- `GetNoteName(int noteNumber)` (string): A static helper that returns the string representation (e.g., "C4") for a given MIDI note number.
- `GetFrequency(int noteNumber)` (float): A static helper that calculates and returns the frequency in Hertz for a given MIDI note number.

## Usage

### Example 1: Filtering and Processing
This example demonstrates how to filter incoming MIDI events to process only active Note-On events.

```csharp
public void ProcessMidiEvent(MidiNoteEvent midiEvent)
{
    if (midiEvent.IsValid && midiEvent.IsNoteOn)
    {
        Console.WriteLine($"Note {midiEvent.NoteName} (Frequency: {midiEvent.Frequency} Hz) " +
                          $"played on channel {midiEvent.Channel} with velocity {midiEvent.Velocity}.");
    }
}
```

### Example 2: Static Utility Usage
This example demonstrates how to use the static helper methods for direct MIDI note calculations.

```csharp
public void DisplayNoteInfo(int noteNumber)
{
    string name = MidiNoteEvent.GetNoteName(noteNumber);
    float freq = MidiNoteEvent.GetFrequency(noteNumber);

    Console.WriteLine($"MIDI Note {noteNumber} is {name} and has a frequency of {freq:F2} Hz.");
}
```

## Notes

- **Thread Safety**: This class is primarily a data container. Accessing properties from multiple threads concurrently is generally safe; however, callers should ensure that if instances are shared across threads, appropriate synchronization mechanisms are used if state is mutated.
- **Data Validation**: The `IsValid` property should be checked before consuming `Frequency` or `NoteName` values to ensure the `NoteNumber` was within valid MIDI ranges (0–127) at the time of calculation.
- **Static Methods**: The `GetNoteName` and `GetFrequency` methods are stateless and inherently thread-safe.
