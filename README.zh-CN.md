# ImageProxyClient

一个用于生成 [Imgproxy](https://imgproxy.net/) 图片处理 URL 的 .NET 客户端库。

## 特性

- 支持 HMAC-SHA256 URL 签名
- 多种源 URL 格式（编码、明文、加密）
- AES-256-CBC 加密源 URL
- 流式 API 配置图片处理选项
- 支持依赖注入（DI）
- 丰富的图片处理选项（调整大小、裁剪、滤镜、水印等）
- 轻量级，无外部依赖

## 安装

通过 NuGet 安装：

```bash
dotnet add package ImageProxyClient
```

## 快速开始

### 基本用法

```csharp
using ImageProxyClient;

// 创建客户端
var client = new ImgproxyClient(
    baseUrl: "http://localhost:8080",
    hexKey: "你的十六进制密钥",
    hexSalt: "你的十六进制盐值"
);

// 生成图片 URL
var url = client.BuildUrl("s3://mybucket/image.jpg", options =>
    options.Resize(800, 600)
          .Format("webp")
          .Quality(80));

Console.WriteLine(url);
// 输出: http://localhost:8080/{签名}/rs:fill:800:600/ext:webp/q:80/{编码后的源}.webp
```

### 依赖注入

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 从配置注册
builder.Services.AddImageProxyClient(
    builder.Configuration.GetSection("Imgproxy")
);

// 或使用委托配置
builder.Services.AddImageProxyClient(options =>
{
    options.BaseUrl = "http://localhost:8080";
    options.HexKey = "你的十六进制密钥";
    options.HexSalt = "你的十六进制盐值";
});

var app = builder.Build();

// 在 Controller/Service 中使用
app.MapGet("/image/{key}", (string key, IImgproxyClient client) =>
{
    return client.BuildUrl($"s3://bucket/{key}", o => o.Resize(400, 300));
});

app.Run();
```

### 配置文件

```json
{
  "Imgproxy": {
    "BaseUrl": "http://localhost:8080",
    "HexKey": "你的十六进制密钥",
    "HexSalt": "你的十六进制盐值"
  }
}
```

## 图片处理选项

### 调整大小

```csharp
options.Resize(800, 600)                    // 调整大小（默认 fill 模式）
options.Resize(800, 600, "fit")             // 指定模式：fit, fill, fill-down, force, auto
options.ResizingType("fill")                // 单独设置调整模式
options.ResizingAlgorithm("lanczos3")       // 算法：nearest, linear, cubic, lanczos2, lanczos3
options.Enlarge(false)                      // 禁止放大
options.Dpr(2.0f)                           // 设备像素比
```

### 裁剪

```csharp
options.Crop(200, 200)                      // 裁剪到 200x200
options.Crop(200, 200, 10, 10)              // 带偏移裁剪
options.Gravity("ce")                       // 重力：no, ce, noea, ea, soea, so, sowe, we, nowe
options.GravitySmart()                      // 智能裁剪（人脸检测）
options.GravityCenter()                     // 中心裁剪
options.GravityFocusPoint(0.5f, 0.3f)       // 焦点重力
```

### 格式和质量

```csharp
options.Format("webp")                      // 输出格式：webp, jpg, png, gif, avif, ico
options.Quality(85)                         // 质量 1-100
options.MaxBytes(50000)                     // 最大文件大小（字节）
```

### 视觉效果

```csharp
options.Brightness(20)                      // 亮度 -100 到 100
options.Contrast(-10)                       // 对比度 -100 到 100
options.Saturation(1.5f)                    // 饱和度
options.Blur(5.0f)                          // 模糊
options.Sharpen(2.0f)                       // 锐化
options.Pixelate(10)                        // 像素化
options.Rotate(90)                          // 旋转：0, 90, 180, 270
options.FlipHorizontal()                    // 水平翻转
options.FlipVertical()                      // 垂直翻转
```

### 水印

```csharp
options.Watermark(0.5f)                     // 不透明度 0-1
options.Watermark(0.7f, "so", 10, 10, 0.5f) // 完整参数：不透明度、位置、X偏移、Y偏移、缩放
```

### 其他选项

```csharp
options.Background("FFFFFF")                // 背景色
options.Background(255, 255, 255, 128)      // RGBA 背景色
options.StripMetadata()                     // 移除 EXIF 元数据
options.StripColorProfile()                 // 移除色彩配置
options.CacheBuster("v1")                   // 缓存破坏
options.Expires(1735689600L)                // 设置过期时间（Unix 时间戳）
options.Expires(DateTime.UtcNow.AddHours(1)) // 设置过期时间（DateTime）
options.Expires(DateTimeOffset.UtcNow.AddHours(1)) // 设置过期时间（DateTimeOffset）
options.Page(1)                             // PDF 页码
options.VideoFrame(5.0f)                    // 视频帧（秒）
```

## 不签名模式

如果 Imgproxy 服务端禁用了签名验证，可以生成未签名的 URL：

```csharp
var client = new ImgproxyClient("http://localhost:8080");

var url = client.BuildUnsignedUrl("https://example.com/image.jpg", options =>
    options.Resize(400, 300));

// 输出: http://localhost:8080/insecure/rs:fill:400:300/{编码后的源}.webp
```

## 源 URL 格式

Imgproxy 支持三种源图片 URL 编码格式。可以使用 `SourceUrlFormat` 枚举指定格式：

### 1. 编码格式（默认）

Base64 URL 安全编码的源 URL：

```
http://imgproxy.example.com/%signature/%processing_options/%encoded_source_url.%extension
```

```csharp
var client = new ImgproxyClient("http://localhost:8080", "key", "salt");

// 使用默认格式（编码格式）
var url = client.BuildUrl("https://example.com/image.jpg", SourceUrlFormat.Encoded, 
    o => o.Resize(400, 300));

// 或在选项中指定
var options = new ImgproxyOptions
{
    BaseUrl = "http://localhost:8080",
    HexKey = "your-key",
    HexSalt = "your-salt",
    SourceUrlFormat = SourceUrlFormat.Encoded  // 这是默认值
};
var client = new ImgproxyClient(options);
```

### 2. 明文格式

可读的明文源 URL：

```
http://imgproxy.example.com/%signature/%processing_options/plain/%source_url@%extension
```

```csharp
var client = new ImgproxyClient("http://localhost:8080", "key", "salt");

var url = client.BuildUrl("https://example.com/image.jpg", SourceUrlFormat.Plain,
    o => o.Resize(400, 300));

// 输出: http://localhost:8080/{签名}/rs:fill:400:300/plain/https%3A%2F%2Fexample.com%2Fimage.jpg@webp
```

### 3. 加密格式

AES-256-CBC 加密的源 URL，提供额外的安全性：

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
    // AES-256 密钥（32 字节 = 64 个十六进制字符）
    HexEncryptionKey = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
    // IV（16 字节 = 32 个十六进制字符）
    HexEncryptionIV = "0123456789abcdef0123456789abcdef"
};

var client = new ImgproxyClient(options);

var url = client.BuildUrl("https://example.com/image.jpg", 
    o => o.Resize(400, 300));

// 输出: http://localhost:8080/{签名}/rs:fill:400:300/enc/{加密后的base64}.webp
```

> **注意：** 使用 `SourceUrlFormat.Encrypted` 时，必须配置 `HexEncryptionKey`（64 个十六进制字符）和 `HexEncryptionIV`（32 个十六进制字符）。加密使用 AES-256-CBC 和 PKCS7 填充。

## API 参考

### IImgproxyClient

| 方法 | 说明 |
|------|------|
| `BuildUrl(sourcePath, config, extension)` | 生成签名 URL（使用默认格式） |
| `BuildUrl(sourcePath, extension)` | 生成签名 URL（无处理选项） |
| `BuildUrl(sourcePath, format, config, extension)` | 生成签名 URL（指定格式） |
| `BuildUrl(sourcePath, format, extension)` | 生成签名 URL（无处理选项，指定格式） |
| `BuildUnsignedUrl(sourcePath, config, extension)` | 生成未签名 URL（使用默认格式） |
| `BuildUnsignedUrl(sourcePath, format, config, extension)` | 生成未签名 URL（指定格式） |
| `BaseUrl` | 获取服务基础 URL |

### ImgproxyOptions

| 属性 | 类型 | 说明 |
|------|------|------|
| `BaseUrl` | string | Imgproxy 服务 URL |
| `HexKey` | string | 十六进制签名密钥 |
| `HexSalt` | string | 十六进制签名盐值 |
| `SourceUrlFormat` | SourceUrlFormat | 源 URL 编码格式（默认：Encoded） |
| `HexEncryptionKey` | string | 十六进制 AES-256 密钥（64 字符），用于加密格式 |
| `HexEncryptionIV` | string | 十六进制 AES IV（32 字符），用于加密格式 |

### SourceUrlFormat 枚举

| 值 | 说明 |
|----|------|
| `Encoded` | Base64 URL 安全编码（默认） |
| `Plain` | 明文格式，URL 编码（`/plain/url@ext`） |
| `Encrypted` | AES-256-CBC 加密（`/enc/encrypted.ext`） |

## 开发

### 构建

```bash
git clone https://github.com/yourname/ImageProxyClient.git
cd ImageProxyClient
dotnet build
```

### 运行示例

```bash
cd samples/ImageProxyClient.Sample
dotnet run
```

## 许可证

MIT License

## 相关链接

- [Imgproxy 官方文档](https://imgproxy.net/documentation)
- [Imgproxy GitHub](https://github.com/imgproxy/imgproxy)
