# MidiInputService

Provides asynchronous access to MIDI input devices, allowing enumeration of available ports, starting and stopping reception of MIDI messages, and exposing incoming note events through a strongly‑typed event.

## API
### GetAvailableDevicesAsync
```csharp
public Task<IReadOnlyList<MidiDeviceInfo>> GetAvailableDevicesAsync()
```
**Purpose** – Retrieves a read‑only list of MIDI input devices currently present on the system.  
**Parameters** – None.  
**Return value** – A task that completes with an `IReadOnlyList<MidiDeviceInfo>` describing each available device.  
**Exceptions** –  
- `ObjectDisposedException` if the service has been disposed.  
- `InvalidOperationException` if the underlying MIDI subsystem cannot be accessed.  
- `System.ComponentModel.Win32Exception` or NAudio‑specific exceptions for low‑level driver errors.

### StartAsync
```csharp
public async Task StartAsync()
```
**Purpose** – Begins listening for MIDI input on the device selected via configuration (or the default device if none is specified).  
**Parameters** – None.  
**Return value** – A task that completes when the service has successfully started receiving MIDI data.  
**Exceptions** –  
- `ObjectDisposedException` if the service has been disposed.  
- `InvalidOperationException` if `StartAsync` is called while the service is already running.  
- `InvalidOperationException` if no MIDI input device is available or the selected device cannot be opened.  
- NAudio‑related exceptions (e.g., `MmException`) for driver initialization failures.

### StopAsync
```csharp
public async Task StopAsync()
```
**Purpose** – Stops MIDI input reception and releases any resources tied to the active device.  
**Parameters** – None.  
**Return value** – A task that completes when the service has stopped listening and the device is closed.  
**Exceptions** –  
- `ObjectDisposedException` if the service has been disposed.  
- `InvalidOperationException` if `StopAsync` is called while the service is not running.

### Dispose
```csharp
public void Dispose()
```
**Purpose** – Releases all unmanaged resources held by the service, including any open MIDI input ports.  
**Parameters** – None.  
**Return value** – None.  
**Exceptions** – None; calling `Dispose` multiple times is safe.

### Note
```csharp
public required MidiNoteEvent Note
```
**Purpose** – Provides an event that is raised for each MIDI note‑on or note‑off message received from the input device. Subscribers receive a `MidiNoteEvent` instance containing note number, velocity, channel, and timestamp.  
**Parameters** – None (event subscription).  
**Return value** – None.  
**Exceptions** –  
- Subscribing to the event after `Dispose` has been called may result in an `ObjectDisposedException` being raised internally by the event invocation mechanism.  
- The event itself does not throw; errors in user‑provided handlers propagate according to normal C# event invocation semantics.

## Usage
### Example 1: Enumerating devices and starting input
```csharp
using var midiService = new MidiInputService();

// List available input ports
IReadOnlyList<MidiDeviceInfo> devices = await midiService.GetAvailableDevicesAsync();
Console.WriteLine("Available MIDI inputs:");
foreach (var d in devices)
{
    Console.WriteLine($"- {d.Name} (ID: {d.Id})");
}

// Start listening on the default device
await midiService.StartAsync();
Console.WriteLine("MidiInputService started.");
```
### Example 2: Handling note events
```csharp
await using var midiService = new MidiInputService();
midiService.Note += (sender, e) =>
{
    // e is a MidiNoteEvent with properties: NoteNumber, Velocity, Channel, Timestamp
    Console.WriteLine($"Note {e.NoteNumber} velocity {e.Velocity} on channel {e.Channel}");
};

await midiService.StartAsync();

// Simulate work for a while
await Task.Delay(TimeSpan.FromSeconds(30));

await midiService.StopAsync();
// Dispose is called automatically by the await using statement
```

## Notes
- `GetAvailableDevicesAsync` may be called before or after `StartAsync`; however, after `Dispose` it will throw `ObjectDisposedException`.  
- The service is not thread‑safe for concurrent calls to `StartAsync` or `StopAsync`; invoking either method while the other is pending will result in an `InvalidOperationException`.  
- The `Note` event is raised on an internal background thread; UI‑bound handlers must marshal to the appropriate context (e.g., using `SynchronizationContext` or `Dispatcher`).  
- Calling `Dispose` while `StartAsync` or `StopAsync` is awaiting will cancel the pending operation and cause the returned task to complete with an `ObjectDisposedException`.  
- If the underlying MIDI device is disconnected while the service is running, the `Note` event will cease to fire and subsequent calls to `StopAsync` may throw a device‑specific exception; it is recommended to handle such exceptions in the calling code.  
- The `required` modifier on the `Note` property indicates that the property must be initialized by object initializers; after construction the event behaves like a standard .NET event and can be subscribed to or unsubscribed from at any time.
