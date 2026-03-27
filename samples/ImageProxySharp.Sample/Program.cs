using ImageProxySharp;
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
// Example 2: Unsigned mode
// ========================================

Console.WriteLine("\n=== Example 2: Unsigned mode ===\n");

var unsignedClient = new ImgproxyClient("http://localhost:8080");

var unsignedUrl = unsignedClient.BuildUnsignedUrl("https://example.com/image.jpg", options =>
    options.Resize(400, 300)
          .Format("jpg")
          .Quality(70));

Console.WriteLine($"Unsigned URL: {unsignedUrl}");

// ========================================
// Example 3: Using dependency injection
// ========================================

Console.WriteLine("\n=== Example 3: Dependency injection ===\n");

var services = new ServiceCollection();

// Method A: Configure with delegate
services.AddImageProxySharp(options =>
{
    options.BaseUrl = "http://localhost:8080";
    options.HexKey = "your-hex-key-here";
    options.HexSalt = "your-hex-salt-here";
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
// Example 4: Using IConfiguration
// ========================================

Console.WriteLine("\n=== Example 4: IConfiguration ===\n");

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["BaseUrl"] = "http://localhost:8080",
        ["HexKey"] = "your-hex-key-here",
        ["HexSalt"] = "your-hex-salt-here"
    })
    .Build();

var servicesWithConfig = new ServiceCollection();
servicesWithConfig.AddImageProxySharp(config);

using var providerWithConfig = servicesWithConfig.BuildServiceProvider();
var configClient = providerWithConfig.GetRequiredService<IImgproxyClient>();

var configUrl = configClient.BuildUrl("s3://bucket/image.jpg", "png");
Console.WriteLine($"URL from config: {configUrl}");

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
