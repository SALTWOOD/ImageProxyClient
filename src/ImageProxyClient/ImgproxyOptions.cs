namespace ImageProxyClient;

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
    /// The format to use for encoding source URLs in generated URLs.
    /// Default is Encoded (Base64 URL-safe encoding).
    /// </summary>
    public SourceUrlFormat SourceUrlFormat { get; set; } = SourceUrlFormat.Encoded;

    /// <summary>
    /// The hex-encoded AES-256 encryption key for encrypting source URLs.
    /// Must be 64 hex characters (32 bytes) when using Encrypted format.
    /// Only required when SourceUrlFormat is Encrypted.
    /// </summary>
    public string HexEncryptionKey { get; set; } = string.Empty;

    /// <summary>
    /// The hex-encoded IV (Initialization Vector) for AES-CBC encryption.
    /// Must be 32 hex characters (16 bytes) when using Encrypted format.
    /// Only required when SourceUrlFormat is Encrypted.
    /// </summary>
    public string HexEncryptionIV { get; set; } = string.Empty;

    /// <summary>
    /// Returns true if both Key and Salt are provided for URL signing.
    /// </summary>
    public bool IsSigningEnabled =>
        !string.IsNullOrWhiteSpace(HexKey) &&
        !string.IsNullOrWhiteSpace(HexSalt);

    /// <summary>
    /// Returns true if encryption is configured for encrypted source URLs.
    /// </summary>
    public bool IsEncryptionEnabled =>
        !string.IsNullOrWhiteSpace(HexEncryptionKey) &&
        !string.IsNullOrWhiteSpace(HexEncryptionIV);

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

        // Validate encryption key and IV if encrypted format is selected
        if (SourceUrlFormat == SourceUrlFormat.Encrypted)
        {
            if (string.IsNullOrWhiteSpace(HexEncryptionKey))
            {
                throw new ArgumentException(
                    "HexEncryptionKey is required when using Encrypted source URL format",
                    nameof(HexEncryptionKey));
            }

            if (HexEncryptionKey.Length != 64)
            {
                throw new ArgumentException(
                    "HexEncryptionKey must be 64 hex characters (32 bytes for AES-256)",
                    nameof(HexEncryptionKey));
            }

            if (string.IsNullOrWhiteSpace(HexEncryptionIV))
            {
                throw new ArgumentException(
                    "HexEncryptionIV is required when using Encrypted source URL format",
                    nameof(HexEncryptionIV));
            }

            if (HexEncryptionIV.Length != 32)
            {
                throw new ArgumentException(
                    "HexEncryptionIV must be 32 hex characters (16 bytes for AES IV)",
                    nameof(HexEncryptionIV));
            }
        }
    }
}
