namespace UseCases.Common;

public static class EntityReference
{
    private const char Separator = '#';

    public static string Format(string entityType, object entityId) =>
        $"{entityType}{Separator}{entityId}";

    public static bool TryParse(string? value, out string? entityType, out string? entityId)
    {
        entityType = null;
        entityId = null;

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        var separatorIndex = value.IndexOf(Separator);
        if (separatorIndex <= 0 || separatorIndex == value.Length - 1)
        {
            return false;
        }

        entityType = value[..separatorIndex];
        entityId = value[(separatorIndex + 1)..];

        return true;
    }
}
