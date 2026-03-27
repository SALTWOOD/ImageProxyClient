# ImageProxySharp

一个用于生成 [Imgproxy](https://imgproxy.net/) 图片处理 URL 的 .NET 库。

## 特性

- 支持 HMAC-SHA256 URL 签名
- 流式 API 配置图片处理选项
- 支持依赖注入（DI）
- 丰富的图片处理选项（调整大小、裁剪、滤镜、水印等）
- 轻量级，无外部依赖

## 安装

通过 NuGet 安装：

```bash
dotnet add package ImageProxySharp
```

## 快速开始

### 基本用法

```csharp
using ImageProxySharp;

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
builder.Services.AddImageProxySharp(
    builder.Configuration.GetSection("Imgproxy")
);

// 或使用委托配置
builder.Services.AddImageProxySharp(options =>
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

## API 参考

### IImgproxyClient

| 方法 | 说明 |
|------|------|
| `BuildUrl(sourcePath, config, extension)` | 生成签名 URL |
| `BuildUrl(sourcePath, extension)` | 生成签名 URL（无处理选项） |
| `BuildUnsignedUrl(sourcePath, config, extension)` | 生成未签名 URL |
| `BaseUrl` | 获取服务基础 URL |

### ImgproxyOptions

| 属性 | 类型 | 说明 |
|------|------|------|
| `BaseUrl` | string | Imgproxy 服务 URL |
| `HexKey` | string | 十六进制签名密钥 |
| `HexSalt` | string | 十六进制签名盐值 |

## 开发

### 构建

```bash
git clone https://github.com/yourname/ImageProxySharp.git
cd ImageProxySharp
dotnet build
```

### 运行示例

```bash
cd samples/ImageProxySharp.Sample
dotnet run
```

## 许可证

MIT License

## 相关链接

- [Imgproxy 官方文档](https://imgproxy.net/documentation)
- [Imgproxy GitHub](https://github.com/imgproxy/imgproxy)
