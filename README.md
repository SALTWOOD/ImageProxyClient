# ImageProxyClient

A .NET client library for generating [Imgproxy](https://imgproxy.net/) image processing URLs.

## Features

- HMAC-SHA256 URL signing support
- Multiple source URL formats (Encoded, Plain, Encrypted)
- AES-256-CBC encryption for source URLs
- Fluent API for image processing options
- Dependency injection (DI) support
- Rich image processing options (resize, crop, filters, watermark, etc.)
- Lightweight, no external dependencies

## Installation

Install via NuGet:

```bash
dotnet add package ImageProxyClient
```

## Quick Start

### Basic Usage

```csharp
using ImageProxyClient;

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
builder.Services.AddImageProxyClient(
    builder.Configuration.GetSection("Imgproxy")
);

// Or register with delegate
builder.Services.AddImageProxyClient(options =>
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
options.Expires(1735689600L)                // Set expiration (Unix timestamp)
options.Expires(DateTime.UtcNow.AddHours(1)) // Set expiration (DateTime)
options.Expires(DateTimeOffset.UtcNow.AddHours(1)) // Set expiration (DateTimeOffset)
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

## Source URL Formats

Imgproxy supports three formats for encoding the source image URL. You can specify the format using `SourceUrlFormat` enum:

### 1. Encoded Format (Default)

Base64 URL-safe encoded source URL:

```
http://imgproxy.example.com/%signature/%processing_options/%encoded_source_url.%extension
```

```csharp
var client = new ImgproxyClient("http://localhost:8080", "key", "salt");

// Using default format (Encoded)
var url = client.BuildUrl("https://example.com/image.jpg", SourceUrlFormat.Encoded, 
    o => o.Resize(400, 300));

// Or specify in options
var options = new ImgproxyOptions
{
    BaseUrl = "http://localhost:8080",
    HexKey = "your-key",
    HexSalt = "your-salt",
    SourceUrlFormat = SourceUrlFormat.Encoded  // This is the default
};
var client = new ImgproxyClient(options);
```

### 2. Plain Format

Human-readable plain text source URL:

```
http://imgproxy.example.com/%signature/%processing_options/plain/%source_url@%extension
```

```csharp
var client = new ImgproxyClient("http://localhost:8080", "key", "salt");

var url = client.BuildUrl("https://example.com/image.jpg", SourceUrlFormat.Plain,
    o => o.Resize(400, 300));

// Output: http://localhost:8080/{signature}/rs:fill:400:300/plain/https%3A%2F%2Fexample.com%2Fimage.jpg@webp
```

### 3. Encrypted Format

AES-256-CBC encrypted source URL for additional security:

```
http://imgproxy.example.com/%signature/%processing_options/enc/%encrypted_source_url.%extension
```

```csharp
var options = new ImgproxyOptions
{
    BaseUrl = "http://localhost:8080",
    HexKey = "your-signing-key",
    HexSalt = "your-signing-salt",
    SourceUrlFormat = SourceUrlFormat.Encrypted,
    // AES-256 key (32 bytes = 64 hex chars)
    HexEncryptionKey = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
    // IV (16 bytes = 32 hex chars)
    HexEncryptionIV = "0123456789abcdef0123456789abcdef"
};

var client = new ImgproxyClient(options);

var url = client.BuildUrl("https://example.com/image.jpg", 
    o => o.Resize(400, 300));

// Output: http://localhost:8080/{signature}/rs:fill:400:300/enc/{encrypted_base64}.webp
```

> **Note:** When using `SourceUrlFormat.Encrypted`, you must configure `HexEncryptionKey` (64 hex characters) and `HexEncryptionIV` (32 hex characters). The encryption uses AES-256-CBC with PKCS7 padding.

## API Reference

### IImgproxyClient

| Method | Description |
|--------|-------------|
| `BuildUrl(sourcePath, config, extension)` | Generate signed URL with default format |
| `BuildUrl(sourcePath, extension)` | Generate signed URL without options |
| `BuildUrl(sourcePath, format, config, extension)` | Generate signed URL with specific format |
| `BuildUrl(sourcePath, format, extension)` | Generate signed URL without options, with specific format |
| `BuildUnsignedUrl(sourcePath, config, extension)` | Generate unsigned URL with default format |
| `BuildUnsignedUrl(sourcePath, format, config, extension)` | Generate unsigned URL with specific format |
| `BaseUrl` | Get service base URL |

### ImgproxyOptions

| Property | Type | Description |
|----------|------|-------------|
| `BaseUrl` | string | Imgproxy service URL |
| `HexKey` | string | Hex-encoded signing key |
| `HexSalt` | string | Hex-encoded signing salt |
| `SourceUrlFormat` | SourceUrlFormat | Format for encoding source URLs (default: Encoded) |
| `HexEncryptionKey` | string | Hex-encoded AES-256 key (64 chars) for encrypted format |
| `HexEncryptionIV` | string | Hex-encoded AES IV (32 chars) for encrypted format |

### SourceUrlFormat Enum

| Value | Description |
|-------|-------------|
| `Encoded` | Base64 URL-safe encoded (default) |
| `Plain` | Plain text with URL encoding (`/plain/url@ext`) |
| `Encrypted` | AES-256-CBC encrypted (`/enc/encrypted.ext`) |

## Development

### Build

```bash
git clone https://github.com/yourname/ImageProxyClient.git
cd ImageProxyClient
dotnet build
```

### Run Sample

```bash
cd samples/ImageProxyClient.Sample
dotnet run
```

## License

MIT License

## Links

- [Imgproxy Documentation](https://imgproxy.net/documentation)
- [Imgproxy GitHub](https://github.com/imgproxy/imgproxy)
