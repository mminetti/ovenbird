using FluentValidation;
using UseCases.DataLists;

namespace Web.DataLists.Get;

public sealed class GetDataListRequest
{
    public const string Route = "/data-lists/{Type}";

    public DataListType Type { get; set; }
}

public sealed class GetDataListValidator : Validator<GetDataListRequest>
{
    public GetDataListValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage($"Type must be one of: {string.Join(", ", Enum.GetNames<DataListType>())}");
    }
}
