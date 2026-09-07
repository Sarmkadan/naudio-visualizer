# MidiNoteEventExtensions

The `MidiNoteEventExtensions` class provides convenience methods for classifying a `MidiNoteEvent` as a press or release and for formatting it as a readable string. The `MidiInputServiceJsonExtensions` class serializes the small portion of `MidiInputService` state that can be represented as JSON and provides methods that create a new service instance from a string.

## API

### MidiNoteEventExtensions

*   **`public static bool IsNotePressed(this MidiNoteEvent event)`**
    Returns `true` only when `IsNoteOn` is `true` and `Velocity` is greater than zero. Throws `ArgumentNullException` when `event` is `null`.

*   **`public static bool IsNoteReleased(this MidiNoteEvent event)`**
    Returns `true` when `IsNoteOn` is `false` or `Velocity` is zero. Throws `ArgumentNullException` when `event` is `null`.

*   **`public static string ToReadableString(this MidiNoteEvent event)`**
    Returns a string in the format `"{NoteName} (Channel {Channel}, Velocity {Velocity})"`. Throws `ArgumentNullException` when `event` is `null`.

### MidiInputServiceJsonExtensions

*   **`public static string ToJson(this MidiInputService value, bool indented = false)`**
    Serializes the service's `IsDisposed` and `ActiveDeviceIndex` values using camel-case property names. The result is compact by default; passing `true` for `indented` produces indented JSON. Disposable resources, event handlers, and other runtime state are not serialized. Throws `ArgumentNullException` when `value` is `null`.

*   **`public static MidiInputService? FromJson(string json)`**
    Creates and returns a new `MidiInputService` for any non-null, non-whitespace string. The method does not parse the supplied JSON or restore the serialized state. It throws `ArgumentNullException` for `null` and `ArgumentException` for an empty or whitespace-only string. If construction of the service throws, the method catches the exception and returns `null`.

*   **`public static bool TryFromJson(string json, out MidiInputService? value)`**
    Calls `FromJson` and returns `true` when it produces a non-null service. For empty or whitespace-only input, or when service construction fails, it returns `false` and sets `value` to `null`. A `null` argument still throws `ArgumentNullException` because the null check occurs before the method's `try` block.

## Usage

```csharp
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

var note = new MidiNoteEvent
{
    NoteName = "A4",
    Channel = 1,
    Velocity = 96,
    IsNoteOn = true
};

if (note.IsNotePressed())
{
    Console.WriteLine(note.ToReadableString());
    // A4 (Channel 1, Velocity 96)
}

using var service = new MidiInputService();
string json = service.ToJson(indented: true);

if (MidiInputServiceJsonExtensions.TryFromJson(json, out var newService))
{
    // newService is a fresh instance; the JSON state is not restored.
    newService.Dispose();
}
```

## Notes

*   A note-on event with zero velocity is classified as released: `IsNotePressed` returns `false`, while `IsNoteReleased` returns `true`.
*   For a positive velocity, `IsNoteReleased` depends on `IsNoteOn`: it returns `true` when `IsNoteOn` is `false`.
*   `ToJson` represents only a state snapshot. `FromJson` and `TryFromJson` do not validate that their input is JSON and do not apply `IsDisposed` or `ActiveDeviceIndex` to the newly created service.
