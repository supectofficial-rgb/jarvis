using OysterFx.AppCore.Shared.DependencyInjections;
using System.Text;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.InputProcessing;

public sealed class InputNormalizer : IInputNormalizer, ISingletoneLifetimeMarker
{
    public NormalizedInput Normalize(string text)
    {
        var source = text ?? string.Empty;
        var normalized = source.Trim();
        normalized = normalized.Replace("\u200c", " ", StringComparison.Ordinal);
        normalized = ConvertPersianDigitsToEnglish(normalized);
        normalized = CollapseSpaces(normalized);

        return new NormalizedInput
        {
            OriginalText = source,
            Text = normalized
        };
    }

    private static string CollapseSpaces(string value)
    {
        var sb = new StringBuilder(value.Length);
        var prevSpace = false;
        foreach (var ch in value)
        {
            var isSpace = char.IsWhiteSpace(ch);
            if (isSpace)
            {
                if (!prevSpace)
                {
                    sb.Append(' ');
                }
            }
            else
            {
                sb.Append(ch);
            }
            prevSpace = isSpace;
        }
        return sb.ToString().Trim();
    }

    private static string ConvertPersianDigitsToEnglish(string value)
    {
        var persian = "??????????";
        var arabic = "??????????";
        var english = "0123456789";

        var chars = value.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            var p = persian.IndexOf(chars[i]);
            if (p >= 0)
            {
                chars[i] = english[p];
                continue;
            }

            var a = arabic.IndexOf(chars[i]);
            if (a >= 0)
            {
                chars[i] = english[a];
            }
        }

        return new string(chars);
    }
}




