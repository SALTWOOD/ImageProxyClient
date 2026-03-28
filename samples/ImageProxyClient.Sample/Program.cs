using ImageProxyClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

// ========================================
// Example 1: Direct instantiation
// ========================================

Console.WriteLine("=== Example 1: Direct instantiation ===\n");

var client = new ImgproxyClient(
    baseUrl: "http://localhost:8080",
    hexKey: "your-hex-key-here",
    hexSalt: "your-hex-salt-here"
);

// Basic usage: generate image processing URL
var imageUrl = client.BuildUrl("s3://mybucket/images/photo.jpg", options =>
    options.Resize(800, 600)
          .Format("webp")
          .Quality(80));

Console.WriteLine($"Basic image URL: {imageUrl}");

// Advanced usage: more processing options
var advancedUrl = client.BuildUrl("s3://mybucket/images/photo.png", options =>
    options.Resize(1920, 1080, "fill")
          .GravitySmart()
          .Format("webp")
          .Quality(85)
          .Sharpen(0.5f)
          .StripMetadata()
          .CacheBuster(DateTime.UtcNow.Ticks.ToString()));

Console.WriteLine($"Advanced image URL: {advancedUrl}");

// ========================================
// Example 2: Source URL formats
// ========================================

Console.WriteLine("\n=== Example 2: Source URL formats ===\n");

var sourceUrl = "https://example.com/images/photo.jpg";

// Format 1: Encoded (default) - Base64 URL-safe encoded source
// URL format: /%signature/%processing_options/%encoded_source_url.%extension
var encodedUrl = client.BuildUrl(sourceUrl, SourceUrlFormat.Encoded, options =>
    options.Resize(400, 300)
          .Format("webp")
          .Quality(80));
Console.WriteLine($"Encoded format:\n  {encodedUrl}\n");

// Format 2: Plain - Human-readable source URL
// URL format: /%signature/%processing_options/plain/%source_url@%extension
var plainUrl = client.BuildUrl(sourceUrl, SourceUrlFormat.Plain, options =>
    options.Resize(400, 300)
          .Format("webp")
          .Quality(80));
Console.WriteLine($"Plain format:\n  {plainUrl}\n");

// Format 3: Encrypted - AES-256-CBC encrypted source URL
// URL format: /%signature/%processing_options/enc/%encrypted_source_url.%extension
try
{
    var encryptedClient = new ImgproxyClient(new ImgproxyOptions
    {
        BaseUrl = "http://localhost:8080",
        HexKey = "your-hex-key-here",
        HexSalt = "your-hex-salt-here",
        SourceUrlFormat = SourceUrlFormat.Encrypted,
        // AES-256 key (32 bytes = 64 hex chars)
        HexEncryptionKey = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
        // IV (16 bytes = 32 hex chars)
        HexEncryptionIV = "0123456789abcdef0123456789abcdef"
    });

    var encryptedUrl = encryptedClient.BuildUrl(sourceUrl, options =>
        options.Resize(400, 300)
              .Format("webp")
              .Quality(80));
    Console.WriteLine($"Encrypted format:\n  {encryptedUrl}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"Encrypted format error: {ex.Message}\n");
}

// ========================================
// Example 3: Unsigned mode
// ========================================

Console.WriteLine("\n=== Example 3: Unsigned mode ===\n");

var unsignedClient = new ImgproxyClient("http://localhost:8080");

var unsignedUrl = unsignedClient.BuildUnsignedUrl("https://example.com/image.jpg", options =>
    options.Resize(400, 300)
          .Format("jpg")
          .Quality(70));

Console.WriteLine($"Unsigned URL: {unsignedUrl}");

// Unsigned with plain format
var unsignedPlainUrl = unsignedClient.BuildUnsignedUrl("https://example.com/image.jpg", 
    SourceUrlFormat.Plain, 
    options => options.Resize(400, 300));
Console.WriteLine($"Unsigned plain URL: {unsignedPlainUrl}");

// ========================================
// Example 4: Using dependency injection
// ========================================

Console.WriteLine("\n=== Example 4: Dependency injection ===\n");

var services = new ServiceCollection();

// Method A: Configure with delegate
services.AddImageProxyClient(options =>
{
    options.BaseUrl = "http://localhost:8080";
    options.HexKey = "your-hex-key-here";
    options.HexSalt = "your-hex-salt-here";
    // Optionally set default source URL format
    options.SourceUrlFormat = SourceUrlFormat.Encoded;
});

using var serviceProvider = services.BuildServiceProvider();
var injectedClient = serviceProvider.GetRequiredService<IImgproxyClient>();

var diUrl = injectedClient.BuildUrl("s3://mybucket/avatar.png", options =>
    options.Crop(200, 200)
          .GravityCenter()
          .Format("webp")
          .Quality(90));

Console.WriteLine($"DI client generated URL: {diUrl}");

// ========================================
// Example 5: Various image processing options
// ========================================

Console.WriteLine("\n=== Example 5: Image processing options ===\n");

// Blur effect
var blurUrl = client.BuildUrl("s3://bucket/private.jpg", options =>
    options.Blur(10.0f)
          .Format("webp"));

Console.WriteLine($"Blur effect: {blurUrl}");

// Rotation
var rotateUrl = client.BuildUrl("s3://bucket/rotated.jpg", options =>
    options.Rotate(90)
          .Format("webp"));

Console.WriteLine($"Rotate 90 degrees: {rotateUrl}");

// Adjust brightness, contrast, saturation
var adjustedUrl = client.BuildUrl("s3://bucket/dark.jpg", options =>
    options.Brightness(20)
          .Contrast(10)
          .Saturation(1.2f)
          .Format("webp"));

Console.WriteLine($"Adjusted brightness/contrast/saturation: {adjustedUrl}");

// Watermark
var watermarkUrl = client.BuildUrl("s3://bucket/photo.jpg", options =>
    options.Resize(1200, 800)
          .Watermark(0.5f, "so", 10, 10, 0.3f)
          .Format("webp")
          .Quality(85));

Console.WriteLine($"With watermark: {watermarkUrl}");

// Pixelate (privacy protection)
var pixelatedUrl = client.BuildUrl("s3://bucket/face.jpg", options =>
    options.Pixelate(20)
          .Format("webp"));

Console.WriteLine($"Pixelated: {pixelatedUrl}");

// ========================================
// Example 6: Get base URL
// ========================================

Console.WriteLine("\n=== Example 6: Get base URL ===\n");

Console.WriteLine($"Service base URL: {client.BaseUrl}");

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
