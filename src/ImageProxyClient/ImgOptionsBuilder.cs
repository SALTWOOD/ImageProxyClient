namespace ImageProxyClient;

/// <summary>
/// Builder for Imgproxy image processing options.
/// Provides a fluent API to configure image processing parameters.
/// </summary>
public class ImgOptionsBuilder
{
    private readonly List<string> _parts = [];

    #region Resize

    /// <summary>
    /// Resizes the image to specified dimensions.
    /// </summary>
    /// <param name="width">Target width.</param>
    /// <param name="height">Target height.</param>
    /// <param name="mode">Resize mode: fit, fill, fill-down, force, auto (default: fill).</param>
    public ImgOptionsBuilder Resize(int width, int height, string mode = "fill")
    {
        _parts.Add($"rs:{mode}:{width}:{height}");
        return this;
    }

    /// <summary>
    /// Sets the resize type.
    /// </summary>
    /// <param name="type">Type: fit, fill, fill-down, force, auto.</param>
    public ImgOptionsBuilder ResizingType(string type)
    {
        _parts.Add($"rt:{type}");
        return this;
    }

    /// <summary>
    /// Sets the resizing algorithm.
    /// </summary>
    /// <param name="algorithm">Algorithm: nearest, linear, cubic, lanczos2, lanczos3, lanczos.</param>
    public ImgOptionsBuilder ResizingAlgorithm(string algorithm)
    {
        _parts.Add($"ra:{algorithm}");
        return this;
    }

    /// <summary>
    /// Enables or disables image enlargement.
    /// </summary>
    public ImgOptionsBuilder Enlarge(bool enlarge = true)
    {
        _parts.Add($"el:{(enlarge ? "1" : "0")}");
        return this;
    }

    /// <summary>
    /// Sets the device pixel ratio.
    /// </summary>
    /// <param name="dpr">Pixel ratio (1-8).</param>
    public ImgOptionsBuilder Dpr(float dpr)
    {
        _parts.Add($"dpr:{dpr:F2}");
        return this;
    }

    #endregion

    #region Crop

    /// <summary>
    /// Crops the image to specified dimensions.
    /// </summary>
    /// <param name="width">Crop width.</param>
    /// <param name="height">Crop height.</param>
    public ImgOptionsBuilder Crop(int width, int height)
    {
        _parts.Add($"c:{width}:{height}");
        return this;
    }

    /// <summary>
    /// Crops the image with offset.
    /// </summary>
    /// <param name="width">Crop width.</param>
    /// <param name="height">Crop height.</param>
    /// <param name="xOffset">X offset.</param>
    /// <param name="yOffset">Y offset.</param>
    public ImgOptionsBuilder Crop(int width, int height, int xOffset, int yOffset)
    {
        _parts.Add($"c:{width}:{height}:{xOffset}:{yOffset}");
        return this;
    }

    /// <summary>
    /// Sets the gravity (crop focus point).
    /// </summary>
    /// <param name="type">Gravity type: no, ce, noea, ea, soea, so, sowe, we, nowe, no, sm, fp.</param>
    public ImgOptionsBuilder Gravity(string type)
    {
        _parts.Add($"g:{type}");
        return this;
    }

    /// <summary>
    /// Sets smart gravity (face detection).
    /// </summary>
    public ImgOptionsBuilder GravitySmart()
    {
        _parts.Add("g:sm");
        return this;
    }

    /// <summary>
    /// Sets center gravity.
    /// </summary>
    public ImgOptionsBuilder GravityCenter()
    {
        _parts.Add("g:ce");
        return this;
    }

    /// <summary>
    /// Sets focus point gravity.
    /// </summary>
    /// <param name="x">X coordinate (0-1).</param>
    /// <param name="y">Y coordinate (0-1).</param>
    public ImgOptionsBuilder GravityFocusPoint(float x, float y)
    {
        _parts.Add($"g:fp:{x:F4}:{y:F4}");
        return this;
    }

    #endregion

    #region Format and Quality

    /// <summary>
    /// Sets the output format.
    /// </summary>
    /// <param name="extension">Format extension: webp, jpg, png, gif, avif, ico.</param>
    public ImgOptionsBuilder Format(string extension)
    {
        _parts.Add($"ext:{extension}");
        return this;
    }

    /// <summary>
    /// Sets the image quality.
    /// </summary>
    /// <param name="quality">Quality value (1-100).</param>
    public ImgOptionsBuilder Quality(int quality)
    {
        _parts.Add($"q:{quality}");
        return this;
    }

    /// <summary>
    /// Sets the maximum output file size in bytes.
    /// </summary>
    public ImgOptionsBuilder MaxBytes(int bytes)
    {
        _parts.Add($"mb:{bytes}");
        return this;
    }

    #endregion

    #region Visual Effects

    /// <summary>
    /// Sets the background color.
    /// </summary>
    /// <param name="color">Hex color value (e.g., FFF or FFFFFF).</param>
    public ImgOptionsBuilder Background(string color)
    {
        _parts.Add($"bg:{color}");
        return this;
    }

    /// <summary>
    /// Sets the background color with alpha.
    /// </summary>
    /// <param name="r">Red.</param>
    /// <param name="g">Green.</param>
    /// <param name="b">Blue.</param>
    /// <param name="a">Alpha (0-255).</param>
    public ImgOptionsBuilder Background(int r, int g, int b, int a = 255)
    {
        _parts.Add($"bg:{r}:{g}:{b}:{a}");
        return this;
    }

    /// <summary>
    /// Adjusts brightness.
    /// </summary>
    /// <param name="brightness">Brightness value (-100 to 100).</param>
    public ImgOptionsBuilder Brightness(int brightness)
    {
        _parts.Add($"br:{brightness}");
        return this;
    }

    /// <summary>
    /// Adjusts contrast.
    /// </summary>
    /// <param name="contrast">Contrast value (-100 to 100).</param>
    public ImgOptionsBuilder Contrast(int contrast)
    {
        _parts.Add($"co:{contrast}");
        return this;
    }

    /// <summary>
    /// Adjusts saturation.
    /// </summary>
    /// <param name="saturation">Saturation value.</param>
    public ImgOptionsBuilder Saturation(float saturation)
    {
        _parts.Add($"sa:{saturation:F2}");
        return this;
    }

    /// <summary>
    /// Applies blur effect.
    /// </summary>
    /// <param name="sigma">Blur strength (0-100).</param>
    public ImgOptionsBuilder Blur(float sigma)
    {
        _parts.Add($"bl:{sigma:F2}");
        return this;
    }

    /// <summary>
    /// Applies sharpen effect.
    /// </summary>
    /// <param name="sigma">Sharpen strength.</param>
    public ImgOptionsBuilder Sharpen(float sigma)
    {
        _parts.Add($"sh:{sigma:F2}");
        return this;
    }

    /// <summary>
    /// Applies pixelate effect.
    /// </summary>
    /// <param name="size">Pixel size.</param>
    public ImgOptionsBuilder Pixelate(int size)
    {
        _parts.Add($"pix:{size}");
        return this;
    }

    /// <summary>
    /// Rotates the image.
    /// </summary>
    /// <param name="angle">Rotation angle (0, 90, 180, 270).</param>
    public ImgOptionsBuilder Rotate(int angle)
    {
        _parts.Add($"rot:{angle}");
        return this;
    }

    /// <summary>
    /// Enables auto-rotation based on EXIF data.
    /// </summary>
    public ImgOptionsBuilder AutoRotate(bool autoRotate = true)
    {
        _parts.Add($"ar:{(autoRotate ? "1" : "0")}");
        return this;
    }

    /// <summary>
    /// Flips horizontally.
    /// </summary>
    public ImgOptionsBuilder FlipHorizontal()
    {
        _parts.Add("flip:1");
        return this;
    }

    /// <summary>
    /// Flips vertically.
    /// </summary>
    public ImgOptionsBuilder FlipVertical()
    {
        _parts.Add("flop:1");
        return this;
    }

    #endregion

    #region Watermark

    /// <summary>
    /// Adds a watermark.
    /// </summary>
    /// <param name="opacity">Opacity (0-1).</param>
    /// <param name="position">Position (ce, no, noea, ea, soea, so, sowe, we, nowe).</param>
    /// <param name="xOffset">X offset.</param>
    /// <param name="yOffset">Y offset.</param>
    /// <param name="scale">Scale factor.</param>
    public ImgOptionsBuilder Watermark(float opacity, string position = "ce", int xOffset = 0, int yOffset = 0, float scale = 1.0f)
    {
        _parts.Add($"wm:{opacity:F2}:{position}:{xOffset}:{yOffset}:{scale:F2}");
        return this;
    }

    /// <summary>
    /// Adds a simple watermark with opacity only.
    /// </summary>
    public ImgOptionsBuilder Watermark(float opacity)
    {
        _parts.Add($"wm:{opacity:F1}");
        return this;
    }

    #endregion

    #region Other Options

    /// <summary>
    /// Sets the expiration time for the image URL.
    /// When set, imgproxy will check the provided unix timestamp and return 404 when expired.
    /// </summary>
    /// <param name="timestamp">Unix timestamp (seconds since epoch). Use null to clear.</param>
    public ImgOptionsBuilder Expires(long? timestamp)
    {
        if (timestamp.HasValue)
        {
            _parts.Add($"exp:{timestamp.Value}");
        }
        return this;
    }

    /// <summary>
    /// Sets the expiration time for the image URL using a DateTime.
    /// When set, imgproxy will check the provided unix timestamp and return 404 when expired.
    /// </summary>
    /// <param name="expiresAt">Expiration DateTime (will be converted to Unix timestamp). Use null to clear.</param>
    public ImgOptionsBuilder Expires(DateTime? expiresAt)
    {
        if (expiresAt.HasValue)
        {
            var timestamp = new DateTimeOffset(expiresAt.Value).ToUnixTimeSeconds();
            _parts.Add($"exp:{timestamp}");
        }
        return this;
    }

    /// <summary>
    /// Sets the expiration time for the image URL using a DateTimeOffset.
    /// When set, imgproxy will check the provided unix timestamp and return 404 when expired.
    /// </summary>
    /// <param name="expiresAt">Expiration DateTimeOffset (will be converted to Unix timestamp). Use null to clear.</param>
    public ImgOptionsBuilder Expires(DateTimeOffset? expiresAt)
    {
        if (expiresAt.HasValue)
        {
            _parts.Add($"exp:{expiresAt.Value.ToUnixTimeSeconds()}");
        }
        return this;
    }

    /// <summary>
    /// Sets a cache buster value.
    /// </summary>
    public ImgOptionsBuilder CacheBuster(string value)
    {
        _parts.Add($"cb:{value}");
        return this;
    }

    /// <summary>
    /// Sets a cache buster value (numeric).
    /// </summary>
    public ImgOptionsBuilder CacheBuster(int value)
    {
        _parts.Add($"cb:{value}");
        return this;
    }

    /// <summary>
    /// Strips EXIF metadata.
    /// </summary>
    public ImgOptionsBuilder StripMetadata(bool strip = true)
    {
        _parts.Add($"sm:{(strip ? "1" : "0")}");
        return this;
    }

    /// <summary>
    /// Strips color profile.
    /// </summary>
    public ImgOptionsBuilder StripColorProfile(bool strip = true)
    {
        _parts.Add($"scp:{(strip ? "1" : "0")}");
        return this;
    }

    /// <summary>
    /// Enables auto quality optimization.
    /// </summary>
    public ImgOptionsBuilder AutoQuality(string method = "none")
    {
        _parts.Add($"aq:{method}");
        return this;
    }

    /// <summary>
    /// Sets the page number for multi-page documents (PDF).
    /// </summary>
    public ImgOptionsBuilder Page(int page)
    {
        _parts.Add($"pg:{page}");
        return this;
    }

    /// <summary>
    /// Sets the video frame for video thumbnails.
    /// </summary>
    public ImgOptionsBuilder VideoFrame(float second)
    {
        _parts.Add($"vf:{second:F2}");
        return this;
    }

    /// <summary>
    /// Adds a custom option.
    /// </summary>
    /// <param name="option">Option string (e.g., "q:80").</param>
    public ImgOptionsBuilder Custom(string option)
    {
        _parts.Add(option);
        return this;
    }

    #endregion

    /// <summary>
    /// Builds the options string.
    /// </summary>
    internal string Build() => string.Join("/", _parts);

    /// <summary>
    /// Resets all options.
    /// </summary>
    public void Reset() => _parts.Clear();

    /// <summary>
    /// Gets the current option count.
    /// </summary>
    public int Count => _parts.Count;
}
