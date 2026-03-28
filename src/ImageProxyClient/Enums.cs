namespace ImageProxyClient;

/// <summary>
/// Resize mode for image processing.
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// Resizes the image to fit within the specified dimensions while preserving aspect ratio.
    /// </summary>
    Fit,

    /// <summary>
    /// Resizes the image to fill the specified dimensions while preserving aspect ratio.
    /// The image will be cropped if necessary.
    /// </summary>
    Fill,

    /// <summary>
    /// Similar to fill, but only scales down the image if it's larger than the specified dimensions.
    /// </summary>
    FillDown,

    /// <summary>
    /// Resizes the image to the exact specified dimensions without preserving aspect ratio.
    /// </summary>
    Force,

    /// <summary>
    /// Automatically chooses between fit and fill based on the image orientation.
    /// </summary>
    Auto
}

/// <summary>
/// Resizing algorithm for image processing.
/// </summary>
public enum ResizingAlgorithm
{
    /// <summary>
    /// Nearest-neighbor interpolation. Fast but low quality.
    /// </summary>
    Nearest,

    /// <summary>
    /// Bilinear interpolation.
    /// </summary>
    Linear,

    /// <summary>
    /// Bicubic interpolation.
    /// </summary>
    Cubic,

    /// <summary>
    /// Lanczos filter with a=2.
    /// </summary>
    Lanczos2,

    /// <summary>
    /// Lanczos filter with a=3.
    /// </summary>
    Lanczos3,

    /// <summary>
    /// Lanczos filter (alias for Lanczos3).
    /// </summary>
    Lanczos
}

/// <summary>
/// Gravity type for crop focus point.
/// </summary>
public enum GravityType
{
    /// <summary>
    /// No gravity (use coordinates).
    /// </summary>
    None,

    /// <summary>
    /// Center gravity.
    /// </summary>
    Center,

    /// <summary>
    /// North (top).
    /// </summary>
    North,

    /// <summary>
    /// Northeast (top-right).
    /// </summary>
    NorthEast,

    /// <summary>
    /// East (right).
    /// </summary>
    East,

    /// <summary>
    /// Southeast (bottom-right).
    /// </summary>
    SouthEast,

    /// <summary>
    /// South (bottom).
    /// </summary>
    South,

    /// <summary>
    /// Southwest (bottom-left).
    /// </summary>
    SouthWest,

    /// <summary>
    /// West (left).
    /// </summary>
    West,

    /// <summary>
    /// Northwest (top-left).
    /// </summary>
    NorthWest,

    /// <summary>
    /// Smart gravity (face detection).
    /// </summary>
    Smart,

    /// <summary>
    /// Focus point gravity (use coordinates).
    /// </summary>
    FocusPoint
}

/// <summary>
/// Output image format.
/// </summary>
public enum ImageFormat
{
    /// <summary>
    /// WebP format.
    /// </summary>
    WebP,

    /// <summary>
    /// JPEG format.
    /// </summary>
    Jpg,

    /// <summary>
    /// PNG format.
    /// </summary>
    Png,

    /// <summary>
    /// GIF format.
    /// </summary>
    Gif,

    /// <summary>
    /// AVIF format.
    /// </summary>
    Avif,

    /// <summary>
    /// ICO format.
    /// </summary>
    Ico,

    /// <summary>
    /// JPEG XL format.
    /// </summary>
    Jxl,

    /// <summary>
    /// SVG format.
    /// </summary>
    Svg,

    /// <summary>
    /// HEIC format.
    /// </summary>
    Heic,

    /// <summary>
    /// BMP format.
    /// </summary>
    Bmp,

    /// <summary>
    /// TIFF format.
    /// </summary>
    Tiff,

    /// <summary>
    /// Let imgproxy decide the best format.
    /// </summary>
    Best
}

/// <summary>
/// Watermark position.
/// </summary>
public enum WatermarkPosition
{
    /// <summary>
    /// Center.
    /// </summary>
    Center,

    /// <summary>
    /// North (top).
    /// </summary>
    North,

    /// <summary>
    /// Northeast (top-right).
    /// </summary>
    NorthEast,

    /// <summary>
    /// East (right).
    /// </summary>
    East,

    /// <summary>
    /// Southeast (bottom-right).
    /// </summary>
    SouthEast,

    /// <summary>
    /// South (bottom).
    /// </summary>
    South,

    /// <summary>
    /// Southwest (bottom-left).
    /// </summary>
    SouthWest,

    /// <summary>
    /// West (left).
    /// </summary>
    West,

    /// <summary>
    /// Northwest (top-left).
    /// </summary>
    NorthWest,

    /// <summary>
    /// Repeat and tile the watermark to fill the entire image.
    /// </summary>
    Repeat,

    /// <summary>
    /// Same as repeat but watermarks are placed in a chessboard order.
    /// </summary>
    Chessboard
}

/// <summary>
/// Auto quality optimization method.
/// </summary>
public enum AutoQualityMethod
{
    /// <summary>
    /// No auto quality optimization.
    /// </summary>
    None,

    /// <summary>
    /// Use ETM (Encoder Time Minimization) method.
    /// </summary>
    Etm
}

/// <summary>
/// Extension methods for converting enums to their string representations used by imgproxy.
/// </summary>
internal static class EnumExtensions
{
    internal static string ToImgproxyString(this ResizeMode mode) => mode switch
    {
        ResizeMode.Fit => "fit",
        ResizeMode.Fill => "fill",
        ResizeMode.FillDown => "fill-down",
        ResizeMode.Force => "force",
        ResizeMode.Auto => "auto",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };

    internal static string ToImgproxyString(this ResizingAlgorithm algorithm) => algorithm switch
    {
        ResizingAlgorithm.Nearest => "nearest",
        ResizingAlgorithm.Linear => "linear",
        ResizingAlgorithm.Cubic => "cubic",
        ResizingAlgorithm.Lanczos2 => "lanczos2",
        ResizingAlgorithm.Lanczos3 => "lanczos3",
        ResizingAlgorithm.Lanczos => "lanczos",
        _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
    };

    internal static string ToImgproxyString(this GravityType gravity) => gravity switch
    {
        GravityType.None => "no",
        GravityType.Center => "ce",
        GravityType.North => "no",
        GravityType.NorthEast => "noea",
        GravityType.East => "ea",
        GravityType.SouthEast => "soea",
        GravityType.South => "so",
        GravityType.SouthWest => "sowe",
        GravityType.West => "we",
        GravityType.NorthWest => "nowe",
        GravityType.Smart => "sm",
        GravityType.FocusPoint => "fp",
        _ => throw new ArgumentOutOfRangeException(nameof(gravity), gravity, null)
    };

    internal static string ToImgproxyString(this ImageFormat format) => format switch
    {
        ImageFormat.WebP => "webp",
        ImageFormat.Jpg => "jpg",
        ImageFormat.Png => "png",
        ImageFormat.Gif => "gif",
        ImageFormat.Avif => "avif",
        ImageFormat.Ico => "ico",
        ImageFormat.Jxl => "jxl",
        ImageFormat.Svg => "svg",
        ImageFormat.Heic => "heic",
        ImageFormat.Bmp => "bmp",
        ImageFormat.Tiff => "tiff",
        ImageFormat.Best => "best",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };

    internal static string ToImgproxyString(this WatermarkPosition position) => position switch
    {
        WatermarkPosition.Center => "ce",
        WatermarkPosition.North => "no",
        WatermarkPosition.NorthEast => "noea",
        WatermarkPosition.East => "ea",
        WatermarkPosition.SouthEast => "soea",
        WatermarkPosition.South => "so",
        WatermarkPosition.SouthWest => "sowe",
        WatermarkPosition.West => "we",
        WatermarkPosition.NorthWest => "nowe",
        WatermarkPosition.Repeat => "re",
        WatermarkPosition.Chessboard => "ch",
        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
    };

    internal static string ToImgproxyString(this AutoQualityMethod method) => method switch
    {
        AutoQualityMethod.None => "none",
        AutoQualityMethod.Etm => "etm",
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}
