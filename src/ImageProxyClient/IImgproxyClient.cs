namespace ImageProxyClient;

/// <summary>
/// Interface for Imgproxy URL generation client.
/// </summary>
public interface IImgproxyClient
{
    /// <summary>
    /// Gets the base URL of the Imgproxy service.
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Builds an Imgproxy URL with processing options using the default source URL format.
    /// </summary>
    /// <param name="sourcePath">Source image path (e.g., s3://bucket/key or HTTP URL).</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp");

    /// <summary>
    /// Builds an Imgproxy URL without processing options using the default source URL format.
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, string extension = "webp");

    /// <summary>
    /// Builds an Imgproxy URL with a specific source URL format.
    /// </summary>
    /// <param name="sourcePath">Source image path (e.g., s3://bucket/key or HTTP URL).</param>
    /// <param name="format">The format to use for encoding the source URL.</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, SourceUrlFormat format, Action<ImgOptionsBuilder> config, string extension = "webp");

    /// <summary>
    /// Builds an Imgproxy URL without processing options with a specific source URL format.
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="format">The format to use for encoding the source URL.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, SourceUrlFormat format, string extension = "webp");

    /// <summary>
    /// Builds an unsigned Imgproxy URL (requires signature verification disabled on server).
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The unsigned Imgproxy URL.</returns>
    string BuildUnsignedUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp");

    /// <summary>
    /// Builds an unsigned Imgproxy URL with a specific source URL format.
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="format">The format to use for encoding the source URL.</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The unsigned Imgproxy URL.</returns>
    string BuildUnsignedUrl(string sourcePath, SourceUrlFormat format, Action<ImgOptionsBuilder> config, string extension = "webp");
}
