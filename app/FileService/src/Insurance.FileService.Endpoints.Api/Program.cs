using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FileServiceDashboard", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                || origin.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Storage root is resolved once so uploads, processed media artifacts and cleanup share the same base path.
var storageRoot = ResolveStorageRoot(app.Configuration, app.Environment);
var videoProcessing = VideoProcessingOptions.FromConfiguration(app.Configuration);
Directory.CreateDirectory(storageRoot);

app.UseCors("FileServiceDashboard");
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageRoot),
    RequestPath = "/media"
});

var policies = MediaPolicy.CreateDefaults();

app.MapPost("/api/FileService/files/upload", async (HttpRequest request, string policy) =>
{
    if (!policies.TryGetValue(policy ?? string.Empty, out var mediaPolicy))
        return Results.BadRequest(new { message = "Unsupported upload policy." });

    if (!request.HasFormContentType)
        return Results.BadRequest(new { message = "Multipart form-data is required." });

    var form = await request.ReadFormAsync();
    var file = form.Files["file"] ?? form.Files.FirstOrDefault();
    if (file is null || file.Length == 0)
        return Results.BadRequest(new { message = "No file was uploaded." });

    if (file.Length > mediaPolicy.MaxBytes)
        return Results.BadRequest(new { message = $"File is larger than {mediaPolicy.MaxBytes / (1024 * 1024)} MB." });

    var extension = NormalizeExtension(Path.GetExtension(file.FileName));
    if (!mediaPolicy.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        return Results.BadRequest(new { message = "This file type is not allowed for the selected policy." });

    var relativeFolder = Path.Combine(mediaPolicy.Name, DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM")).Replace('\\', '/');
    var safeName = Slugify(Path.GetFileNameWithoutExtension(file.FileName));
    var stamp = Guid.NewGuid().ToString("N")[..12];
    var baseName = $"{safeName}-{stamp}";

    var uploadResult = IsVideoExtension(extension)
        ? await SaveVideoMediaAsync(file, storageRoot, relativeFolder, baseName, extension, mediaPolicy, videoProcessing)
        : await SaveImageMediaAsync(file, storageRoot, relativeFolder, baseName, extension, mediaPolicy);

    return Results.Ok(new MediaUploadResult(
        Policy: mediaPolicy.Name,
        FileKey: uploadResult.FileKey,
        ThumbnailFileKey: uploadResult.ThumbnailFileKey,
        OriginalFileName: file.FileName,
        ContentType: uploadResult.ContentType,
        Size: uploadResult.Size,
        Width: uploadResult.Width,
        Height: uploadResult.Height,
        Url: $"/media/{uploadResult.FileKey}",
        ThumbnailUrl: $"/media/{uploadResult.ThumbnailFileKey}"));
});

app.MapDelete("/api/FileService/files", (string fileKey) =>
{
    if (string.IsNullOrWhiteSpace(fileKey))
        return Results.BadRequest(new { message = "fileKey is required." });

    var normalizedFileKey = fileKey.Trim().TrimStart('/', '\\');
    if (string.IsNullOrWhiteSpace(normalizedFileKey))
        return Results.BadRequest(new { message = "fileKey is required." });

    var originalFullPath = Path.Combine(storageRoot, normalizedFileKey.Replace('/', Path.DirectorySeparatorChar));
    DeleteDerivedFiles(originalFullPath);

    return Results.Ok(new { deleted = true, fileKey = normalizedFileKey });
});

app.Run();

static string ResolveStorageRoot(IConfiguration configuration, IWebHostEnvironment environment)
{
    var configured = configuration["FileStorage:RootPath"];
    if (!string.IsNullOrWhiteSpace(configured))
        return Path.GetFullPath(configured, environment.ContentRootPath);

    return Path.Combine(environment.ContentRootPath, "Storage", "Media");
}

static string Slugify(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return "file";

    var normalized = Regex.Replace(value.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-");
    normalized = normalized.Trim('-');
    return string.IsNullOrWhiteSpace(normalized) ? "file" : normalized;
}

static string NormalizeExtension(string? extension)
{
    if (string.IsNullOrWhiteSpace(extension))
        return ".bin";

    return extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
}

static bool IsVideoExtension(string extension)
    => extension is ".mp4" or ".webm" or ".mov" or ".m4v";

static string ResolveContentType(string extension)
{
    return extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        ".mov" => "video/quicktime",
        ".m4v" => "video/x-m4v",
        _ => "application/octet-stream"
    };
}

static string ResolveImageOutputExtension(Image image, string originalExtension)
{
    if (originalExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) || image.Metadata.DecodedImageFormat?.Name.Equals("PNG", StringComparison.OrdinalIgnoreCase) == true)
        return ".png";

    return ".jpg";
}

static async Task<StoredMediaResult> SaveImageMediaAsync(
    IFormFile file,
    string storageRoot,
    string relativeFolder,
    string baseName,
    string originalExtension,
    MediaPolicy mediaPolicy)
{
    await using var input = file.OpenReadStream();
    using var source = await Image.LoadAsync(input);

    var outputExtension = ResolveImageOutputExtension(source, originalExtension);
    var contentType = ResolveContentType(outputExtension);
    var originalRelativePath = $"{relativeFolder}/{baseName}{outputExtension}";
    var thumbRelativePath = $"{relativeFolder}/{baseName}_thumb{outputExtension}";
    var originalFullPath = Path.Combine(storageRoot, originalRelativePath.Replace('/', Path.DirectorySeparatorChar));
    var thumbFullPath = Path.Combine(storageRoot, thumbRelativePath.Replace('/', Path.DirectorySeparatorChar));

    Directory.CreateDirectory(Path.GetDirectoryName(originalFullPath)!);

    using var resizedOriginal = source.Clone(context => context.Resize(new ResizeOptions
    {
        Mode = ResizeMode.Max,
        Size = new Size(mediaPolicy.MaxWidth, mediaPolicy.MaxHeight)
    }));

    using var resizedThumb = source.Clone(context => context.Resize(new ResizeOptions
    {
        Mode = ResizeMode.Max,
        Size = new Size(mediaPolicy.ThumbnailWidth, mediaPolicy.ThumbnailHeight)
    }));

    await SaveImageAsync(resizedOriginal, originalFullPath, outputExtension, mediaPolicy.JpegQuality);
    await SaveImageAsync(resizedThumb, thumbFullPath, outputExtension, mediaPolicy.JpegQuality);

    var originalInfo = new FileInfo(originalFullPath);
    return new StoredMediaResult(
        originalRelativePath,
        thumbRelativePath,
        contentType,
        originalInfo.Length,
        source.Width,
        source.Height);
}

static async Task SaveImageAsync(Image image, string path, string extension, long jpegQuality)
{
    if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
    {
        await image.SaveAsPngAsync(path, new PngEncoder());
        return;
    }

    await image.SaveAsJpegAsync(path, new JpegEncoder
    {
        Quality = (int)Math.Clamp(jpegQuality, 1, 100)
    });
}

static async Task<StoredMediaResult> SaveVideoMediaAsync(
    IFormFile file,
    string storageRoot,
    string relativeFolder,
    string baseName,
    string originalExtension,
    MediaPolicy mediaPolicy,
    VideoProcessingOptions options)
{
    var tempDirectory = Path.Combine(storageRoot, "_temp");
    Directory.CreateDirectory(tempDirectory);

    var tempInputPath = Path.Combine(tempDirectory, $"{baseName}{originalExtension}");
    await using (var destination = File.Create(tempInputPath))
    await using (var input = file.OpenReadStream())
    {
        await input.CopyToAsync(destination);
    }

    try
    {
        if (!options.EnableTranscoding)
            return await SaveBinaryVideoFallbackAsync(tempInputPath, storageRoot, relativeFolder, baseName, originalExtension);

        var ffmpegAvailable = await CanExecuteAsync(options.FfmpegPath, "-version");
        var ffprobeAvailable = await CanExecuteAsync(options.FfprobePath, "-version");
        if (!ffmpegAvailable || !ffprobeAvailable)
            return await SaveBinaryVideoFallbackAsync(tempInputPath, storageRoot, relativeFolder, baseName, originalExtension);

        var outputExtension = ".mp4";
        var originalRelativePath = $"{relativeFolder}/{baseName}{outputExtension}";
        var thumbRelativePath = $"{relativeFolder}/{baseName}_thumb.jpg";
        var originalFullPath = Path.Combine(storageRoot, originalRelativePath.Replace('/', Path.DirectorySeparatorChar));
        var thumbFullPath = Path.Combine(storageRoot, thumbRelativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(originalFullPath)!);

        var ffmpegArgs =
            $"-y -i \"{tempInputPath}\" -vf \"scale='min({mediaPolicy.MaxWidth},iw)':'min({mediaPolicy.MaxHeight},ih)':force_original_aspect_ratio=decrease\" " +
            "-c:v libx264 -preset veryfast -crf 23 -pix_fmt yuv420p -movflags +faststart -c:a aac " +
            $"\"{originalFullPath}\"";

        if (!await RunProcessAsync(options.FfmpegPath, ffmpegArgs))
            return await SaveBinaryVideoFallbackAsync(tempInputPath, storageRoot, relativeFolder, baseName, originalExtension);

        var thumbArgs =
            $"-y -ss 00:00:01 -i \"{originalFullPath}\" -frames:v 1 -vf \"scale='min({mediaPolicy.ThumbnailWidth},iw)':'min({mediaPolicy.ThumbnailHeight},ih)':force_original_aspect_ratio=decrease\" " +
            $"\"{thumbFullPath}\"";

        if (!await RunProcessAsync(options.FfmpegPath, thumbArgs))
            thumbRelativePath = originalRelativePath;

        var dimensions = await TryReadVideoDimensionsAsync(options.FfprobePath, originalFullPath);
        var info = new FileInfo(originalFullPath);

        return new StoredMediaResult(
            originalRelativePath,
            thumbRelativePath,
            "video/mp4",
            info.Length,
            dimensions.Width,
            dimensions.Height);
    }
    finally
    {
        if (File.Exists(tempInputPath))
            File.Delete(tempInputPath);
    }
}

static async Task<StoredMediaResult> SaveBinaryVideoFallbackAsync(
    string tempInputPath,
    string storageRoot,
    string relativeFolder,
    string baseName,
    string extension)
{
    var relativePath = $"{relativeFolder}/{baseName}{extension}";
    var fullPath = Path.Combine(storageRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    File.Copy(tempInputPath, fullPath, overwrite: true);

    var info = new FileInfo(fullPath);
    return new StoredMediaResult(
        relativePath,
        relativePath,
        ResolveContentType(extension),
        info.Length,
        0,
        0);
}

static async Task<bool> CanExecuteAsync(string fileName, string arguments)
{
    try
    {
        return await RunProcessAsync(fileName, arguments);
    }
    catch
    {
        return false;
    }
}

static async Task<bool> RunProcessAsync(string fileName, string arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        }
    };

    process.Start();
    await process.WaitForExitAsync();
    return process.ExitCode == 0;
}

static async Task<(int Width, int Height)> TryReadVideoDimensionsAsync(string ffprobePath, string videoPath)
{
    try
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffprobePath,
                Arguments = $"-v quiet -print_format json -show_streams \"{videoPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        var json = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(json))
            return (0, 0);

        var probe = JsonSerializer.Deserialize<FfprobeResult>(json);
        var stream = probe?.Streams?.FirstOrDefault(x => x.Width > 0 && x.Height > 0);
        return stream is null ? (0, 0) : (stream.Width, stream.Height);
    }
    catch
    {
        return (0, 0);
    }
}

static void DeleteDerivedFiles(string originalFullPath)
{
    var directory = Path.GetDirectoryName(originalFullPath);
    if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        return;

    var baseName = Path.GetFileNameWithoutExtension(originalFullPath);
    var thumbPrefix = $"{baseName}_thumb";

    if (File.Exists(originalFullPath))
        File.Delete(originalFullPath);

    foreach (var sibling in Directory.EnumerateFiles(directory, $"{thumbPrefix}.*", SearchOption.TopDirectoryOnly))
    {
        if (File.Exists(sibling))
            File.Delete(sibling);
    }
}

internal sealed record MediaUploadResult(
    string Policy,
    string FileKey,
    string ThumbnailFileKey,
    string OriginalFileName,
    string ContentType,
    long Size,
    int Width,
    int Height,
    string Url,
    string ThumbnailUrl);

internal sealed record MediaPolicy(
    string Name,
    long MaxBytes,
    int MaxWidth,
    int MaxHeight,
    int ThumbnailWidth,
    int ThumbnailHeight,
    long JpegQuality,
    IReadOnlyList<string> AllowedExtensions)
{
    public static Dictionary<string, MediaPolicy> CreateDefaults()
    {
        return new Dictionary<string, MediaPolicy>(StringComparer.OrdinalIgnoreCase)
        {
            ["category-image"] = new("category-image", 5 * 1024 * 1024, 1600, 1600, 320, 320, 85, new[] { ".jpg", ".jpeg", ".png", ".webp" }),
            ["product-image"] = new("product-image", 5 * 1024 * 1024, 1800, 1800, 420, 420, 85, new[] { ".jpg", ".jpeg", ".png", ".webp" }),
            ["variant-gallery"] = new("variant-gallery", 50 * 1024 * 1024, 2200, 2200, 480, 480, 85, new[] { ".jpg", ".jpeg", ".png", ".webp", ".mp4", ".webm", ".mov", ".m4v" }),
            ["slider-image"] = new("slider-image", 8 * 1024 * 1024, 2800, 1800, 640, 360, 82, new[] { ".jpg", ".jpeg", ".png", ".webp" })
        };
    }
}

internal sealed record StoredMediaResult(
    string FileKey,
    string ThumbnailFileKey,
    string ContentType,
    long Size,
    int Width,
    int Height);

internal sealed record VideoProcessingOptions(
    bool EnableTranscoding,
    string FfmpegPath,
    string FfprobePath)
{
    public static VideoProcessingOptions FromConfiguration(IConfiguration configuration)
    {
        return new VideoProcessingOptions(
            configuration.GetValue<bool>("MediaProcessing:Video:EnableTranscoding"),
            configuration["MediaProcessing:Video:FfmpegPath"] ?? "ffmpeg",
            configuration["MediaProcessing:Video:FfprobePath"] ?? "ffprobe");
    }
}

internal sealed class FfprobeResult
{
    public List<FfprobeStream> Streams { get; set; } = new();
}

internal sealed class FfprobeStream
{
    public int Width { get; set; }
    public int Height { get; set; }
}
