# VisualizationException

`VisualizationException` is a custom exception type used within the `naudio-visualizer` project to signal errors specific to visualization operations. It extends the standard `Exception` class and includes an optional `VisualizationType` property to provide context about the type of visualization that encountered the error.

## API

### `VisualizationType` (property)
A read-only string property that indicates the type of visualization associated with the exception, if applicable. This property may be `null` if the visualization type is unknown or irrelevant.

### `VisualizationException(string message)`
Constructs a new `VisualizationException` with a specified error message.
- **Parameters**:
  - `message` (string): A human-readable description of the error.
- **Throws**: No exceptions.

### `VisualizationException()`
Constructs a new `VisualizationException` with a default error message.
- **Parameters**: None.
- **Throws**: No exceptions.

### `VisualizationException(string message, Exception innerException)`
Constructs a new `VisualizationException` with a specified error message and a reference to an inner exception that is the cause of this exception.
- **Parameters**:
  - `message` (string): A human-readable description of the error.
  - `innerException` (Exception): The exception that is the cause of the current exception.
- **Throws**: No exceptions.

### `VisualizationException(SerializationInfo info, StreamingContext context)`
Constructs a new `VisualizationException` during deserialization.
- **Parameters**:
  - `info` (SerializationInfo): The `SerializationInfo` that holds the serialized object data.
  - `context` (StreamingContext): The `StreamingContext` that contains contextual information about the source or destination.
- **Throws**: No exceptions.

## Usage
