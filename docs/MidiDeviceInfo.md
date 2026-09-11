# MidiDeviceInfo

Describes a MIDI input device available on the current system. Returned by `MidiInputService.GetAvailableDevicesAsync` as a read-only snapshot of every port visible to the operating system, and used to select which device to open for real-time note capture.

## API

### `public int Index`
The zero-based device index used when opening the device via `MidiInputService.StartAsync`. Corresponds to the position of the device in the underlying NAudio `MidiIn` enumeration.

### `public string ProductName`
The product name reported by the device driver (e.g., "USB MIDI Keyboard"). May be empty if the driver does not expose a name.

### `public bool IsAvailable`
Indicates whether the device is currently available for use. Defaults to `true` when the device is enumerated.

### `public bool IsValid()`
Determines whether this device info contains a usable product name and a valid index.

- **Return value**: `true` when `ProductName` is not null or whitespace and `Index` is greater than or equal to zero; otherwise, `false`.

### `public override string ToString()`
Returns a formatted string representation of the device, including `Index`, `ProductName`, and `IsAvailable`.

- **Return value**: A string in the format: `"[Index] ProductName (available|unavailable)"`.

## Usage

### Example 1: Enumerating devices and selecting one
```csharp
using var midiService = new MidiInputService();

IReadOnlyList<MidiDeviceInfo> devices = await midiService.GetAvailableDevicesAsync();
foreach (var device in devices)
{
    Console.WriteLine(device); // e.g. "[0] USB MIDI Keyboard (available)"
}

// Open the first valid device
var first = devices.FirstOrDefault(d => d.IsValid());
if (first is not null)
{
    await midiService.StartAsync(first.Index);
}
```

## Notes

- `MidiDeviceInfo` is a plain data container; its properties are immutable after construction via the object initializer used by `GetAvailableDevicesAsync`.
- `IsValid` should be checked before attempting to open a device, since a device with an empty `ProductName` or a negative `Index` cannot be opened reliably.
- `GetAvailableDevicesAsync` returns one `MidiDeviceInfo` per device reported by `MidiIn.NumberOfDevices`, with `Index` assigned sequentially from zero.