# ConfigurationManager

The `ConfigurationManager` class provides a centralized mechanism for managing application settings within the `naudio-visualizer` project. It supports type-safe retrieval and storage of configuration values, persistence to disk, and bulk operations such as exporting, importing, and resetting settings to their default states. This component acts as the primary interface for maintaining the state of user preferences and application parameters across sessions.

## API

### `public ConfigurationManager`
Initializes a new instance of the `ConfigurationManager` class. This constructor sets up the internal storage dictionary and prepares the instance for loading or defining default configurations.

### `public T? GetValue<T>(string key)`
Retrieves the value associated with the specified key, cast to the requested type `T`.
*   **Parameters**:
    *   `key`: The unique identifier for the configuration setting.
*   **Return Value**: The value associated with the key converted to type `T`, or `null` if the key does not exist or the value cannot be cast to `T`.
*   **Exceptions**: May throw an exception if the stored value exists but is fundamentally incompatible with the requested type `T` and cannot be safely converted.

### `public void SetValue<T>(string key, T value)`
Stores a value of type `T` associated with the specified key. If the key already exists, its value is overwritten.
*   **Parameters**:
    *   `key`: The unique identifier for the configuration setting.
    *   `value`: The value to store.
*   **Return Value**: None.
*   **Exceptions**: Throws an exception if the `key` is null or empty.

### `public bool Contains(string key)`
Determines whether the configuration manager contains a setting with the specified key.
*   **Parameters**:
    *   `key`: The key to check.
*   **Return Value**: `true` if the key exists in the current configuration; otherwise, `false`.
*   **Exceptions**: None.

### `public bool Remove(string key)`
Removes the setting associated with the specified key from the configuration.
*   **Parameters**:
    *   `key`: The key of the setting to remove.
*   **Return Value**: `true` if the item was successfully found and removed; `false` if the key did not exist.
*   **Exceptions**: None.

### `public IEnumerable<string> GetAllKeys()`
Returns a collection of all keys currently stored in the configuration manager.
*   **Parameters**: None.
*   **Return Value**: An enumerable collection of strings representing all registered configuration keys.
*   **Exceptions**: None.

### `public void LoadSettings()`
Loads configuration data from the default persistent storage location (e.g., a JSON or XML file) into the manager's memory.
*   **Parameters**: None.
*   **Return Value**: None.
*   **Exceptions**: Throws an exception if the storage file is corrupted, inaccessible, or contains invalid data formats.

### `public void SaveSettings()`
Writes the current in-memory configuration state to the default persistent storage location.
*   **Parameters**: None.
*   **Return Value**: None.
*   **Exceptions**: Throws an exception if the application lacks write permissions to the target directory or if the disk is full.

### `public void ResetToDefaults()`
Clears all current user-defined settings and restores the configuration to its predefined default values.
*   **Parameters**: None.
*   **Return Value**: None.
*   **Exceptions**: None.

### `public string GetConfigurationSummary()`
Generates a human-readable string summarizing the current state of all configuration keys and their values.
*   **Parameters**: None.
*   **Return Value**: A formatted string containing the configuration summary.
*   **Exceptions**: None.

### `public void ExportSettings(string filePath)`
Exports the current configuration settings to a specific file path, allowing for backup or transfer.
*   **Parameters**:
    *   `filePath`: The absolute or relative path where the settings file will be created.
*   **Return Value**: None.
*   **Exceptions**: Throws an exception if the path is invalid, the directory does not exist, or write access is denied.

### `public void ImportSettings(string filePath)`
Imports configuration settings from a specified file, overwriting the current in-memory state.
*   **Parameters**:
    *   `filePath`: The path to the file containing the exported settings.
*   **Return Value**: None.
*   **Exceptions**: Throws an exception if the file does not exist, is unreadable, or contains malformed data.

## Usage

### Example 1: Initializing and Managing Basic Settings
The following example demonstrates initializing the manager, setting typed values, checking for existence, and persisting the data.

```csharp
var config = new ConfigurationManager();

// Set various typed settings
config.SetValue("VolumeLevel", 0.75f);
config.SetValue("IsFullScreen", true);
config.SetValue("WindowTitle", "NAudio Visualizer");

// Retrieve a value with type safety
float volume = config.GetValue<float>("VolumeLevel") ?? 0.5f;

// Conditionally update a setting
if (!config.Contains("LastOpenedFile"))
{
    config.SetValue("LastOpenedFile", string.Empty);
}

// Persist changes to disk
config.SaveSettings();
```

### Example 2: Export, Import, and Reset Workflow
This example illustrates how to backup current settings, reset the application state, and restore from the backup file.

```csharp
var config = new ConfigurationManager();
config.LoadSettings();

string backupPath = "./backups/config_backup.json";

// Export current state
config.ExportSettings(backupPath);

// Reset all settings to factory defaults
config.ResetToDefaults();

// Verify reset
var keys = config.GetAllKeys(); 
// Note: Keys may still exist if defaults are pre-populated, but values will be reset

// Restore from the specific backup file
config.ImportSettings(backupPath);

// Output state for debugging
Console.WriteLine(config.GetConfigurationSummary());
```

## Notes

*   **Thread Safety**: The provided signatures do not indicate internal synchronization mechanisms. If `ConfigurationManager` is accessed concurrently from multiple threads (e.g., a UI thread updating settings while a background thread saves them), external locking (e.g., using the `lock` statement) is required around calls to `SetValue`, `Remove`, `LoadSettings`, and `SaveSettings` to prevent race conditions and data corruption.
*   **Type Safety**: The `GetValue<T>` method relies on runtime type casting or conversion. If a value was stored as an `int` but requested as a `string`, the behavior depends on the internal implementation; however, callers should ensure type consistency between `SetValue<T>` and `GetValue<T>` to avoid runtime exceptions or null returns.
*   **Persistence Latency**: Methods `LoadSettings`, `SaveSettings`, `ExportSettings`, and `ImportSettings` involve I/O operations. These should not be called on the main UI thread in a graphical application to prevent freezing the interface; asynchronous wrappers should be implemented by the consumer if necessary.
*   **Key Uniqueness**: Keys are case-sensitive depending on the underlying dictionary implementation. Consistent casing should be maintained when calling `SetValue`, `GetValue`, and `Contains`.
