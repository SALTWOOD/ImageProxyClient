namespace ImageProxySharp;

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

/// <summary>
/// Client for generating Imgproxy URLs.
/// Supports signed and unsigned URL generation.
/// </summary>
public class ImgproxyClient : IImgproxyClient
{
    private readonly byte[]? _key;
    private readonly byte[]? _salt;
    private readonly string _baseUrl;

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

        if (options.IsSigningEnabled)
        {
            _key = Convert.FromHexString(options.HexKey);
            _salt = Convert.FromHexString(options.HexSalt);
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
        var builder = new ImgOptionsBuilder();
        config(builder);

        string options = builder.Build();
        return BuildSignedUrl(sourcePath, options, extension);
    }

    /// <inheritdoc />
    public string BuildUrl(string sourcePath, string extension = "webp")
    {
        return BuildSignedUrl(sourcePath, string.Empty, extension);
    }

    /// <inheritdoc />
    public string BuildUnsignedUrl(string sourcePath, Action<ImgOptionsBuilder> config, string extension = "webp")
    {
        var builder = new ImgOptionsBuilder();
        config(builder);

        string options = builder.Build();
        return BuildUnsignedUrlInternal(sourcePath, options, extension);
    }

    private string BuildSignedUrl(string sourcePath, string options, string extension)
    {
        string encodedSource = Base64UrlEncode(Encoding.UTF8.GetBytes(sourcePath));

        // Build path: /options/encoded_source.ext or /encoded_source.ext
        string pathAndExt = string.IsNullOrEmpty(options)
            ? $"/{encodedSource}.{extension}"
            : $"/{options}/{encodedSource}.{extension}";

        // If no signing configured, return unsigned URL
        if (_key == null || _salt == null)
        {
            return $"{_baseUrl}{pathAndExt}";
        }

        // Calculate signature
        string signature = ComputeSignature(pathAndExt);

        // Final URL: {baseUrl}/{signature}/{options}/{encoded_source}.{extension}
        return $"{_baseUrl}/{signature}{pathAndExt}";
    }

    private string BuildUnsignedUrlInternal(string sourcePath, string options, string extension)
    {
        string encodedSource = Base64UrlEncode(Encoding.UTF8.GetBytes(sourcePath));

        // Unsigned URL format: {baseUrl}/insecure/{options}/{encoded_source}.{extension}
        string pathAndExt = string.IsNullOrEmpty(options)
            ? $"/insecure/{encodedSource}.{extension}"
            : $"/insecure/{options}/{encodedSource}.{extension}";

        return $"{_baseUrl}{pathAndExt}";
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

    private static string Base64UrlEncode(byte[] input) =>
        Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
}
