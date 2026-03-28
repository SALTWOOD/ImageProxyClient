namespace ImageProxyClient;

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

/// <summary>
/// Client for generating Imgproxy URLs.
/// Supports signed and unsigned URL generation with multiple source URL formats.
/// </summary>
public class ImgproxyClient : IImgproxyClient
{
    private readonly byte[]? _key;
    private readonly byte[]? _salt;
    private readonly byte[]? _encryptionKey;
    private readonly byte[]? _encryptionIV;
    private readonly string _baseUrl;
    private readonly SourceUrlFormat _defaultFormat;

    /// <inheritdoc />
    public string BaseUrl => _baseUrl;

    /// <summary>
    /// Creates a new Imgproxy client instance.
    /// </summary>
    /// <param name="options">Configuration options.</param>
    public ImgproxyClient(ImgproxyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        _baseUrl = options.BaseUrl.TrimEnd('/');
        _defaultFormat = options.SourceUrlFormat;

        if (options.IsSigningEnabled)
        {
            _key = Convert.FromHexString(options.HexKey);
            _salt = Convert.FromHexString(options.HexSalt);
        }

        if (options.IsEncryptionEnabled)
        {
            _encryptionKey = Convert.FromHexString(options.HexEncryptionKey);
            _encryptionIV = Convert.FromHexString(options.HexEncryptionIV);
        }
    }

    /// <summary>
    /// Creates a new Imgproxy client instance using IOptions.
    /// </summary>
    public ImgproxyClient(IOptions<ImgproxyOptions> options) : this(options.Value)
    {
    }

    /// <summary>
    /// Creates a new Imgproxy client instance.
    /// </summary>
    /// <param name="baseUrl">Imgproxy service base URL.</param>
    /// <param name="hexKey">Hex-encoded signing key (optional).</param>
    /// <param name="hexSalt">Hex-encoded signing salt (optional).</param>
    public ImgproxyClient(string baseUrl, string? hexKey = null, string? hexSalt = null)
        : this(new ImgproxyOptions
        {
            BaseUrl = baseUrl,
            HexKey = hexKey ?? string.Empty,
            HexSalt = hexSalt ?? string.Empty
        })
    {
    }

    /// <inheritdoc />
    public string BuildUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp")
    {
        return BuildUrl(sourcePath, _defaultFormat, config, extension);
    }

    /// <inheritdoc />
    public string BuildUrl(string sourcePath, string extension = "webp")
    {
        return BuildUrl(sourcePath, _defaultFormat, extension);
    }

    /// <inheritdoc />
    public string BuildUrl(string sourcePath, SourceUrlFormat format, Action<ImgOptionsBuilder> config, string extension = "webp")
    {
        var builder = new ImgOptionsBuilder();
        config(builder);

        string options = builder.Build();
        return BuildSignedUrl(sourcePath, options, extension, format);
    }

    /// <inheritdoc />
    public string BuildUrl(string sourcePath, SourceUrlFormat format, string extension = "webp")
    {
        return BuildSignedUrl(sourcePath, string.Empty, extension, format);
    }

    /// <inheritdoc />
    public string BuildUnsignedUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp")
    {
        return BuildUnsignedUrl(sourcePath, _defaultFormat, config, extension);
    }

    /// <inheritdoc />
    public string BuildUnsignedUrl(string sourcePath, SourceUrlFormat format, Action<ImgOptionsBuilder> config, string extension = "webp")
    {
        var builder = new ImgOptionsBuilder();
        config(builder);

        string options = builder.Build();
        return BuildUnsignedUrlInternal(sourcePath, options, extension, format);
    }

    private string BuildSignedUrl(string sourcePath, string options, string extension, SourceUrlFormat format)
    {
        // Build the source URL part based on format
        string sourcePart = BuildSourcePart(sourcePath, extension, format);

        // Build path: /options/sourcePart or /sourcePart
        // For Plain format: /options/plain/source@ext
        // For Encoded format: /options/encoded_source.ext
        // For Encrypted format: /options/enc/encrypted_source.ext
        string pathAndExt = string.IsNullOrEmpty(options)
            ? sourcePart
            : $"/{options}{sourcePart}";

        // If no signing configured, return unsigned URL
        if (_key == null || _salt == null)
        {
            return $"{_baseUrl}{pathAndExt}";
        }

        // Calculate signature
        string signature = ComputeSignature(pathAndExt);

        // Final URL: {baseUrl}/{signature}/{options}/{source_part}
        return $"{_baseUrl}/{signature}{pathAndExt}";
    }

    private string BuildUnsignedUrlInternal(string sourcePath, string options, string extension, SourceUrlFormat format)
    {
        // Build the source URL part based on format (prefixed with /insecure)
        string sourcePart = BuildSourcePart(sourcePath, extension, format);

        // Unsigned URL format: {baseUrl}/insecure/{options}/{source_part}
        string pathAndExt = string.IsNullOrEmpty(options)
            ? $"/insecure{sourcePart}"
            : $"/insecure/{options}{sourcePart}";

        return $"{_baseUrl}{pathAndExt}";
    }

    /// <summary>
    /// Builds the source URL part based on the specified format.
    /// </summary>
    /// <param name="sourcePath">The source image URL/path.</param>
    /// <param name="extension">The output format extension.</param>
    /// <param name="format">The source URL format to use.</param>
    /// <returns>The formatted source part for the URL path.</returns>
    private string BuildSourcePart(string sourcePath, string extension, SourceUrlFormat format)
    {
        return format switch
        {
            SourceUrlFormat.Plain => BuildPlainSourcePart(sourcePath, extension),
            SourceUrlFormat.Encrypted => BuildEncryptedSourcePart(sourcePath, extension),
            _ => BuildEncodedSourcePart(sourcePath, extension)
        };
    }

    /// <summary>
    /// Builds the source part in encoded format: /encoded_source.ext
    /// </summary>
    private string BuildEncodedSourcePart(string sourcePath, string extension)
    {
        string encodedSource = Base64UrlEncode(Encoding.UTF8.GetBytes(sourcePath));
        return $"/{encodedSource}.{extension}";
    }

    /// <summary>
    /// Builds the source part in plain format: /plain/source_url@ext
    /// The source URL is URL-safe encoded (not Base64).
    /// </summary>
    private string BuildPlainSourcePart(string sourcePath, string extension)
    {
        // URL-encode the source path to make it URL-safe
        // imgproxy expects the source URL to be properly escaped
        string encodedSource = Uri.EscapeDataString(sourcePath);
        return $"/plain/{encodedSource}@{extension}";
    }

    /// <summary>
    /// Builds the source part in encrypted format: /enc/encrypted_source.ext
    /// Uses AES-256-CBC encryption with PKCS7 padding.
    /// </summary>
    private string BuildEncryptedSourcePart(string sourcePath, string extension)
    {
        if (_encryptionKey == null || _encryptionIV == null)
        {
            throw new InvalidOperationException(
                "Encryption key and IV must be configured in ImgproxyOptions to use Encrypted format. " +
                "Set HexEncryptionKey (64 hex chars) and HexEncryptionIV (32 hex chars).");
        }

        byte[] sourceBytes = Encoding.UTF8.GetBytes(sourcePath);
        byte[] encryptedBytes = EncryptAesCbc(sourceBytes, _encryptionKey, _encryptionIV);
        string encodedEncrypted = Base64UrlEncode(encryptedBytes);

        return $"/enc/{encodedEncrypted}.{extension}";
    }

    /// <summary>
    /// Encrypts data using AES-256-CBC with PKCS7 padding.
    /// </summary>
    /// <param name="plainBytes">The plaintext bytes to encrypt.</param>
    /// <param name="key">The 32-byte AES key.</param>
    /// <param name="iv">The 16-byte initialization vector.</param>
    /// <returns>The encrypted bytes.</returns>
    private static byte[] EncryptAesCbc(byte[] plainBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    }

    private string ComputeSignature(string path)
    {
        using var hmac = new HMACSHA256(_key!);
        var pathBytes = Encoding.UTF8.GetBytes(path);

        // Concatenate salt and path
        var dataToHash = new byte[_salt!.Length + pathBytes.Length];
        Buffer.BlockCopy(_salt, 0, dataToHash, 0, _salt.Length);
        Buffer.BlockCopy(pathBytes, 0, dataToHash, _salt.Length, pathBytes.Length);

        var hash = hmac.ComputeHash(dataToHash);
        return Base64UrlEncode(hash);
    }

    /// <summary>
    /// Encodes bytes to URL-safe Base64 string (no padding, +/ replaced with -_).
    /// </summary>
    private static string Base64UrlEncode(byte[] input) =>
        Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
}
