using FluentValidation;

namespace Web.Security.Users.Update;

public class UpdateUserRequest
{
    public const string Route = "/security/users/{UserId:int}";
    public static string BuildRoute(int userId) => Route.Replace("{UserId:int}", userId.ToString());

    public int UserId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UpdateUserValidator : Validator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200);

        RuleFor(x => x.UserId)
          .Must((args, userId) => args.Id == userId)
          .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
