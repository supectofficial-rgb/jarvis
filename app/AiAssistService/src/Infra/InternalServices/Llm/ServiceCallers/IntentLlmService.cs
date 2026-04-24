using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Insurance.AiAssistService.Infra.InternalServices.LLM;

public sealed class IntentLlmService : IIntentLlmService
{
    private readonly HttpClient _httpClient;

    public IntentLlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LlmIntentResult> ResolveAsync(IntentLlmRequest request, CancellationToken cancellationToken)
    {
        var fallback = ResolveHeuristically(request);

        try
        {
            var candidateText = string.Join("\n", request.Candidates.Select(x => $"- {x.ActionName} ({x.Score:0.00})"));
            var sb = new StringBuilder();
            sb.AppendLine("You are intent resolver.");
            sb.AppendLine("Return JSON only with shape:");
            sb.AppendLine("{");
            sb.AppendLine("  \"action\": \"string or null\",");
            sb.AppendLine("  \"confidence\": 0.0,");
            sb.AppendLine("  \"parameters\": {},");
            sb.AppendLine("  \"missingFields\": [],");
            sb.AppendLine("  \"notes\": \"\"");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Available candidates:");
            sb.AppendLine(candidateText);
            sb.AppendLine();
            sb.AppendLine("User text:");
            sb.AppendLine(request.UserText);

            var payload = new
            {
                model = "glm-4.6:cloud",
                prompt = sb.ToString(),
                stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/generate", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return fallback;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(body))
            {
                return fallback;
            }

            using var envelope = JsonDocument.Parse(body);
            if (!envelope.RootElement.TryGetProperty("response", out var responseElement) || responseElement.ValueKind != JsonValueKind.String)
            {
                return fallback;
            }

            var responseJson = responseElement.GetString();
            if (string.IsNullOrWhiteSpace(responseJson))
            {
                return fallback;
            }

            using var parsed = JsonDocument.Parse(responseJson);
            var root = parsed.RootElement;

            var result = new LlmIntentResult
            {
                Action = root.TryGetProperty("action", out var actionElement) ? actionElement.GetString() : fallback.Action,
                Confidence = root.TryGetProperty("confidence", out var confidenceElement) && confidenceElement.TryGetDouble(out var confidence)
                    ? confidence
                    : fallback.Confidence,
                Notes = root.TryGetProperty("notes", out var notesElement) ? notesElement.GetString() ?? string.Empty : string.Empty,
                Parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
                MissingFields = new List<string>()
            };

            if (root.TryGetProperty("parameters", out var parametersElement) && parametersElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in parametersElement.EnumerateObject())
                {
                    result.Parameters[prop.Name] = prop.Value.ValueKind == JsonValueKind.String ? prop.Value.GetString() : prop.Value.ToString();
                }
            }

            if (root.TryGetProperty("missingFields", out var missingElement) && missingElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in missingElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(item.GetString()))
                    {
                        result.MissingFields.Add(item.GetString()!);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(result.Action))
            {
                result.Action = fallback.Action;
            }

            if (result.Confidence <= 0)
            {
                result.Confidence = fallback.Confidence;
            }

            if (result.Parameters.Count == 0)
            {
                foreach (var item in fallback.Parameters)
                {
                    result.Parameters[item.Key] = item.Value;
                }
            }

            if (result.MissingFields.Count == 0 && fallback.MissingFields.Count > 0)
            {
                result.MissingFields.AddRange(fallback.MissingFields);
            }

            return result;
        }
        catch
        {
            return fallback;
        }
    }

    private static LlmIntentResult ResolveHeuristically(IntentLlmRequest request)
    {
        var top = request.Candidates.OrderByDescending(x => x.Score).FirstOrDefault();
        return new LlmIntentResult
        {
            Action = top?.ActionName,
            Confidence = top?.Score ?? 0,
            Parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
            MissingFields = new List<string>(),
            Notes = "heuristic fallback"
        };
    }
}


