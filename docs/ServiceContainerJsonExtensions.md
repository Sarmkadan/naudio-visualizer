# ServiceContainerJsonExtensions

Provides JSON serialization and deserialization methods for `ServiceContainer` instances, enabling persistence and transfer of registered services and factories.

## API

### `ToJson`
Serializes the given `ServiceContainer` into a JSON string.

- **Parameters**
  - `container` (`ServiceContainer`): The container to serialize. May be `null`, in which case the method returns `null`.
- **Return value**
  - (`string`): A JSON representation of the container, or `null` if `container` is `null`.
- **Exceptions**
  - Throws `JsonException` if serialization fails due to an unregistered service type or other serialization error.

---

### `FromJson`
Deserializes a JSON string back into a `ServiceContainer` instance.

- **Parameters**
  - `json` (`string`): The JSON string to deserialize. May be `null` or empty, in which case the method returns `null`.
- **Return value**
  - (`ServiceContainer`): A new `ServiceContainer` instance populated with the deserialized services and factories, or `null` if `json` is `null` or empty.
- **Exceptions**
  - Throws `JsonException` if deserialization fails due to malformed JSON, missing type information, or unregistered service types.

---

### `TryFromJson`
Attempts to deserialize a JSON string into a `ServiceContainer` instance without throwing exceptions.

- **Parameters**
  - `json` (`string`): The JSON string to deserialize. May be `null` or empty, in which case the method returns `null`.
- **Return value**
  - (`ServiceContainer`): A new `ServiceContainer` instance if deserialization succeeds, otherwise `null`.
- **Exceptions**
  - None. Errors are suppressed and result in a `null` return value.

---
### `RegisteredServiceTypes`
Gets a list of the fully qualified type names of all registered services in the container.

- **Return value**
  - (`List<string>`): A list of type names, or `null` if the container is `null` or has no registered services.
- **Exceptions**
  - None.

---
### `RegisteredFactoryTypes`
Gets a list of the fully qualified type names of all registered factory types in the container.

- **Return value**
  - (`List<string>`): A list of type names, or `null` if the container is `null` or has no registered factories.
- **Exceptions**
  - None.

## Usage

### Serializing a container to JSON
```csharp
var container = new ServiceContainer();
container.AddService<IAudioRenderer>(new WaveOutRenderer());
container.AddFactory<IAudioSource>(() => new AudioFileReader("sample.wav"));

string json = ServiceContainerJsonExtensions.ToJson(container);
Console.WriteLine(json);
```

### Deserializing a container from JSON
```csharp
string json = /* ... */;
ServiceContainer? container = ServiceContainerJsonExtensions.FromJson(json);

if (container != null)
{
    var renderer = container.GetService<IAudioRenderer>();
    var source = container.GetFactory<IAudioSource>()?.Invoke();
}
```

## Notes

- Thread safety is not guaranteed by this class. Concurrent serialization and deserialization operations on the same `ServiceContainer` instance may lead to undefined behavior.
- Deserialization requires all service and factory types to be available in the current `AppDomain`. Missing types will cause `FromJson` to throw.
- `TryFromJson` is the preferred method for safe deserialization in scenarios where malformed or incompatible JSON may be encountered.
- Passing `null` to any method will result in a `null` return value, except for `ToJson`, which returns `null` only when the input container is `null`.
