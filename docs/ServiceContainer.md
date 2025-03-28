# ServiceContainer

The `ServiceContainer` class serves as a lightweight dependency injection container for the `naudio-visualizer` project. It manages the registration, resolution, and lifecycle of services, including audio processing and visualization components. The container supports both direct instance registration and factory-based registration, allowing flexibility in service creation and disposal.

## API

### `public void Register<T>()`
Registers a default instance of type `T` with the container. The container will create and manage the instance using the default constructor. If `T` is already registered, the existing registration is overwritten.

**Throws:**
- `InvalidOperationException` if `T` does not have a public parameterless constructor.

---

### `public void RegisterFactory<T>(Func<T> factory)`
Registers a factory delegate for type `T` with the container. The delegate is invoked when `T` is resolved, allowing custom instantiation logic.

**Parameters:**
- `factory` (`Func<T>`): A delegate that returns an instance of `T`.

**Throws:**
- `ArgumentNullException` if `factory` is `null`.

---

### `public T? Resolve<T>()`
Resolves and returns an instance of type `T` from the container. If `T` is not registered, returns `null`.

**Returns:**
- An instance of `T` if registered; otherwise, `null`.

**Throws:**
- `InvalidOperationException` if the factory delegate throws an exception during resolution.

---

### `public bool IsRegistered<T>()`
Checks whether type `T` is registered in the container.

**Returns:**
- `true` if `T` is registered; otherwise, `false`.

---

### `public bool Unregister<T>()`
Removes the registration for type `T` from the container. If `T` is not registered, this method has no effect.

**Returns:**
- `true` if `T` was registered and successfully unregistered; otherwise, `false`.

---

### `public void Clear()`
Removes all registered services from the container. Disposes any disposable instances managed by the container.

**Throws:**
- `InvalidOperationException` if disposal of a managed instance fails.

---

### `public void Dispose()`
Releases all resources managed by the container, including disposable instances. After disposal, the container cannot be reused.

**Throws:**
- `InvalidOperationException` if disposal of a managed instance fails.

---

### `public static ServiceContainer ConfigureServices()`
Creates and configures a new `ServiceContainer` instance with default services pre-registered. This includes audio processing and visualization components.

**Returns:**
- A configured `ServiceContainer` instance.

---

### `public static ServiceContainer ConfigureServices(Action<ServiceContainer> configure)`
Creates and configures a new `ServiceContainer` instance, allowing custom configuration via a delegate.

**Parameters:**
- `configure` (`Action<ServiceContainer>`): A delegate that performs additional configuration on the container.

**Returns:**
- A configured `ServiceContainer` instance.

**Throws:**
- `ArgumentNullException` if `configure` is `null`.

---

### `public int MaxAudioBufferSize`
Gets or sets the maximum size of the audio buffer used by the visualizer. Defaults to a predefined value if not explicitly set.

---

### `public int DefaultSampleRate`
Gets or sets the default sample rate for audio processing. Defaults to a predefined value if not explicitly set.

---

### `public int DefaultFftSize`
Gets or sets the default FFT size for frequency analysis. Defaults to a predefined value if not explicitly set.

---

### `public int TargetFps`
Gets or sets the target frames per second for visualization rendering. Defaults to a predefined value if not explicitly set.

---

### `public bool EnableLogging`
Gets or sets a value indicating whether logging is enabled for container operations. Defaults to `false`.

---

### `public int MaxFramesPerSession`
Gets or sets the maximum number of frames to process in a single visualization session. Defaults to a predefined value if not explicitly set.

---

### `public bool IsValid`
Gets a value indicating whether the container is in a valid state for service resolution. Returns `false` if the container has been disposed.

## Usage

### Example 1: Basic Service Registration and Resolution
