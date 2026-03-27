# ImageProxySharp

A .NET library for generating [Imgproxy](https://imgproxy.net/) image processing URLs.

## Features

- HMAC-SHA256 URL signing support
- Fluent API for image processing options
- Dependency injection (DI) support
- Rich image processing options (resize, crop, filters, watermark, etc.)
- Lightweight, no external dependencies

## Installation

Install via NuGet:

```bash
dotnet add package ImageProxySharp
```

## Quick Start

### Basic Usage

```csharp
using ImageProxySharp;

// Create client
var client = new ImgproxyClient(
    baseUrl: "http://localhost:8080",
    hexKey: "your-hex-key",
    hexSalt: "your-hex-salt"
);

// Generate image URL
var url = client.BuildUrl("s3://mybucket/image.jpg", options =>
    options.Resize(800, 600)
          .Format("webp")
          .Quality(80));

Console.WriteLine(url);
// Output: http://localhost:8080/{signature}/rs:fill:800:600/ext:webp/q:80/{encoded_source}.webp
```

### Dependency Injection

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register from configuration
builder.Services.AddImageProxySharp(
    builder.Configuration.GetSection("Imgproxy")
);

// Or register with delegate
builder.Services.AddImageProxySharp(options =>
{
    options.BaseUrl = "http://localhost:8080";
    options.HexKey = "your-hex-key";
    options.HexSalt = "your-hex-salt";
});

var app = builder.Build();

// Use in Controller/Service
app.MapGet("/image/{key}", (string key, IImgproxyClient client) =>
{
    return client.BuildUrl($"s3://bucket/{key}", o => o.Resize(400, 300));
});

app.Run();
```

### Configuration File

```json
{
  "Imgproxy": {
    "BaseUrl": "http://localhost:8080",
    "HexKey": "your-hex-key",
    "HexSalt": "your-hex-salt"
  }
}
```

## Image Processing Options

### Resize

```csharp
options.Resize(800, 600)                    // Resize (default fill mode)
options.Resize(800, 600, "fit")             // Specify mode: fit, fill, fill-down, force, auto
options.ResizingType("fill")                // Set resize type only
options.ResizingAlgorithm("lanczos3")       // Algorithm: nearest, linear, cubic, lanczos2, lanczos3
options.Enlarge(false)                      // Disable enlargement
options.Dpr(2.0f)                           // Device pixel ratio
```

### Crop

```csharp
options.Crop(200, 200)                      // Crop to 200x200
options.Crop(200, 200, 10, 10)              // Crop with offset
options.Gravity("ce")                       // Gravity: no, ce, noea, ea, soea, so, sowe, we, nowe
options.GravitySmart()                      // Smart gravity (face detection)
options.GravityCenter()                     // Center gravity
options.GravityFocusPoint(0.5f, 0.3f)       // Focus point gravity
```

### Format and Quality

```csharp
options.Format("webp")                      // Output format: webp, jpg, png, gif, avif, ico
options.Quality(85)                         // Quality 1-100
options.MaxBytes(50000)                     // Max file size in bytes
```

### Visual Effects

```csharp
options.Brightness(20)                      // Brightness -100 to 100
options.Contrast(-10)                       // Contrast -100 to 100
options.Saturation(1.5f)                    // Saturation
options.Blur(5.0f)                          // Blur
options.Sharpen(2.0f)                       // Sharpen
options.Pixelate(10)                        // Pixelate
options.Rotate(90)                          // Rotate: 0, 90, 180, 270
options.FlipHorizontal()                    // Flip horizontal
options.FlipVertical()                      // Flip vertical
```

### Watermark

```csharp
options.Watermark(0.5f)                     // Opacity 0-1
options.Watermark(0.7f, "so", 10, 10, 0.5f) // Full params: opacity, position, x, y, scale
```

### Other Options

```csharp
options.Background("FFFFFF")                // Background color
options.Background(255, 255, 255, 128)      // RGBA background
options.StripMetadata()                     // Remove EXIF metadata
options.StripColorProfile()                 // Remove color profile
options.CacheBuster("v1")                   // Cache buster
options.Page(1)                             // PDF page number
options.VideoFrame(5.0f)                    // Video frame (seconds)
```

## Unsigned Mode

If your Imgproxy server has signature verification disabled, you can generate unsigned URLs:

```csharp
var client = new ImgproxyClient("http://localhost:8080");

var url = client.BuildUnsignedUrl("https://example.com/image.jpg", options =>
    options.Resize(400, 300));

// Output: http://localhost:8080/insecure/rs:fill:400:300/{encoded_source}.webp
```

## API Reference

### IImgproxyClient

| Method | Description |
|--------|-------------|
| `BuildUrl(sourcePath, config, extension)` | Generate signed URL |
| `BuildUrl(sourcePath, extension)` | Generate signed URL without options |
| `BuildUnsignedUrl(sourcePath, config, extension)` | Generate unsigned URL |
| `BaseUrl` | Get service base URL |

### ImgproxyOptions

| Property | Type | Description |
|----------|------|-------------|
| `BaseUrl` | string | Imgproxy service URL |
| `HexKey` | string | Hex-encoded signing key |
| `HexSalt` | string | Hex-encoded signing salt |

## Development

### Build

```bash
git clone https://github.com/yourname/ImageProxySharp.git
cd ImageProxySharp
dotnet build
```

### Run Sample

```bash
cd samples/ImageProxySharp.Sample
dotnet run
```

## License

MIT License

## Links

- [Imgproxy Documentation](https://imgproxy.net/documentation)
- [Imgproxy GitHub](https://github.com/imgproxy/imgproxy)
