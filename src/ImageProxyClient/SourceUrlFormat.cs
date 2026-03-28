namespace ImageProxyClient;

/// <summary>
/// Specifies the format for encoding the source URL in imgproxy URLs.
/// </summary>
public enum SourceUrlFormat
{
    /// <summary>
    /// Plain text format: /plain/%source_url@%extension
    /// Example: /plain/http://example.com/img.jpg@webp
    /// Useful when you need human-readable URLs or when debugging.
    /// The source URL must be URL-safe encoded.
    /// </summary>
    Plain,
    
    /// <summary>
    /// Base64 URL-encoded format: /%encoded_source_url.%extension
    /// Example: /aHR0cDovL2V4YW1wbGUuY29tL2ltZy5qcGc=.webp
    /// This is the default and most commonly used format.
    /// </summary>
    Encoded,
    
    /// <summary>
    /// AES-CBC encrypted format: /enc/%encrypted_source_url.%extension
    /// Example: /enc/encrypted_base64_string.webp
    /// Provides additional security by encrypting the source URL.
    /// Requires AES-256-CBC encryption key and IV to be configured.
    /// </summary>
    Encrypted
}
