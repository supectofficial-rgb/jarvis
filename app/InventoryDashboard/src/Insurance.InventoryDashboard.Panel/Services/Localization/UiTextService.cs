using System.Collections.Concurrent;
using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Options;
using Microsoft.Extensions.Options;

namespace Insurance.InventoryDashboard.Panel.Services.Localization;

public sealed class UiTextService : IUiTextService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private readonly string _language;
    private readonly IReadOnlyDictionary<string, string> _resolvedTexts;

    public UiTextService(
        IWebHostEnvironment webHostEnvironment,
        IOptions<UiLocalizationOptions> options,
        ILogger<UiTextService> logger)
    {
        _language = NormalizeLanguage(options.Value.Language);
        var resourcesPath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources");

        var selectedTexts = LoadDictionary(Path.Combine(resourcesPath, $"ui.{_language}.json"), logger);
        var fallbackTexts = string.Equals(_language, "en", StringComparison.OrdinalIgnoreCase)
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : LoadDictionary(Path.Combine(resourcesPath, "ui.en.json"), logger);

        var merged = new Dictionary<string, string>(fallbackTexts, StringComparer.OrdinalIgnoreCase);
        foreach (var pair in selectedTexts)
        {
            merged[pair.Key] = pair.Value;
        }

        _resolvedTexts = merged;
    }

    public string Language => _language;

    public string this[string key] => Get(key);

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        return _resolvedTexts.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : key;
    }

    public IReadOnlyDictionary<string, string> GetAll() => _resolvedTexts;

    public string ToClientDictionaryJson()
    {
        return JsonSerializer.Serialize(_resolvedTexts, JsonOptions);
    }

    private static string NormalizeLanguage(string? language)
    {
        var normalized = (language ?? "fa").Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(normalized) ? "fa" : normalized;
    }

    private static Dictionary<string, string> LoadDictionary(string path, ILogger logger)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var json = File.ReadAllText(path);
            var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions);
            return payload is null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(payload, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load UI dictionary from {Path}", path);
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
