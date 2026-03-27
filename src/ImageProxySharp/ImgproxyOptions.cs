namespace ImageProxySharp;

/// <summary>
/// Configuration options for Imgproxy client.
/// </summary>
public class ImgproxyOptions
{
    /// <summary>
    /// The base URL of the Imgproxy service (e.g., http://localhost:8080).
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The hex-encoded signing key. Leave empty to generate unsigned URLs.
    /// </summary>
    public string HexKey { get; set; } = string.Empty;

    /// <summary>
    /// The hex-encoded salt value. Leave empty to generate unsigned URLs.
    /// </summary>
    public string HexSalt { get; set; } = string.Empty;

    /// <summary>
    /// Returns true if both Key and Salt are provided.
    /// </summary>
    public bool IsSigningEnabled =>
        !string.IsNullOrWhiteSpace(HexKey) &&
        !string.IsNullOrWhiteSpace(HexSalt);

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("BaseUrl is required", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("BaseUrl must be a valid HTTP or HTTPS URL", nameof(BaseUrl));
        }

        if (!string.IsNullOrWhiteSpace(HexKey) && HexKey.Length % 2 != 0)
        {
            throw new ArgumentException("HexKey must have an even number of characters", nameof(HexKey));
        }

        if (!string.IsNullOrWhiteSpace(HexSalt) && HexSalt.Length % 2 != 0)
        {
            throw new ArgumentException("HexSalt must have an even number of characters", nameof(HexSalt));
        }
    }
}
