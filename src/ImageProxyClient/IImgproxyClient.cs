namespace ImageProxyClient;

/// <summary>
/// Interface for Imgproxy URL generation client.
/// </summary>
public interface IImgproxyClient
{
    /// <summary>
    /// Builds an Imgproxy URL with processing options.
    /// </summary>
    /// <param name="sourcePath">Source image path (e.g., s3://bucket/key or HTTP URL).</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp");

    /// <summary>
    /// Builds an Imgproxy URL without processing options.
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The generated Imgproxy URL.</returns>
    string BuildUrl(string sourcePath, string extension = "webp");

    /// <summary>
    /// Builds an unsigned Imgproxy URL (requires signature verification disabled on server).
    /// </summary>
    /// <param name="sourcePath">Source image path.</param>
    /// <param name="config">Action to configure image processing options.</param>
    /// <param name="extension">Output format extension (default: webp).</param>
    /// <returns>The unsigned Imgproxy URL.</returns>
    string BuildUnsignedUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp");

    /// <summary>
    /// Gets the base URL of the Imgproxy service.
    /// </summary>
    string BaseUrl { get; }
}
