using Core.Common;

namespace UseCases.Common;

public static class ErrorLogItemFactory
{
    public static TItem Create<TItem>(ValidationError error) where TItem : ErrorLogItem, new() => new()
    {
        SeverityId = MapSeverity(error.Severity),
        Field = error.Identifier,
        Message = error.ErrorMessage,
    };

    public static int MapSeverity(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Warning => Constants.ErrorSeverities.Warning,
        ValidationSeverity.Info => Constants.ErrorSeverities.Info,
        _ => Constants.ErrorSeverities.Error,
    };
}
