// src/README.md

// ... existing content ...

## GradientStopExtensions

`GradientStopExtensions` provides a set of extension methods for working with `GradientStop` instances. These methods allow you to modify gradient stops, navigate through a collection of gradient stops, interpolate between stops, and analyze their properties.

### Usage Example

```csharp
using Domain.Models;

// Assume 'stop' is a GradientStop instance
var stop = new GradientStop { Color = Colors.Red, Position = 0.5 };

// Change the color of the gradient stop
var newStop = GradientStopExtensions.WithColor(stop, Colors.Blue);
Console.WriteLine($"New color: {newStop.Color}");

// Adjust the brightness of the gradient stop
var brighterStop = GradientStopExtensions.AdjustBrightness(stop, 0.2);
Console.WriteLine($"Brighter color: {brighterStop.Color}");

// Get the ARGB components of the gradient stop
GradientStopExtensions.GetArgbComponents(stop, out byte a, out byte r, out byte g, out byte b);
Console.WriteLine($"ARGB: {a}, {r}, {g}, {b}");

// Check if the gradient stop is dark or light
bool isDark = GradientStopExtensions.IsDark(stop);
bool isLight = GradientStopExtensions.IsLight(stop);
Console.WriteLine($"Is dark: {isDark}, Is light: {isLight}");

// Interpolate between two gradient stops
var nextStop = GradientStopExtensions.Next(stop);
var interpolatedStop = GradientStopExtensions.Interpolate(stop, nextStop, 0.7);
Console.WriteLine($"Interpolated color: {interpolatedStop.Color}");
```

// ... existing content ...
