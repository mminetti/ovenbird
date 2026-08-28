using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Common.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // 1. Convert to lowercase
        value = value.ToLowerInvariant();

        // 2. Remove diacritics / accents (e.g., "crème" -> "creme")
        value = RemoveDiacritics(value);

        // 3. Replace invalid characters and spaces with a hyphen
        // This regex ensures only letters, numbers, and existing hyphens survive
        value = Regex.Replace(value, @"[^a-z0-9\s-]", "");

        // 4. Convert multiple spaces or hyphens into a single hyphen
        value = Regex.Replace(value, @"[\s-]+", "-");

        // 5. Trim trailing/leading hyphens
        return value.Trim('-');
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
