# Plan: Partial updates for the Update User endpoint

## Problem

`PUT /security/users/{UserId}` currently overwrites `Name`, `Email`, and
`IsActive` unconditionally on every call. There's no way to distinguish
"caller didn't send this field" from "caller sent this field as
default/null," so partial updates aren't possible and any future nullable
field can't be explicitly cleared.

## Goal

Update only the fields present in the request body. A field that is
present with value `null` should update the entity to `null` (once a
nullable field exists); a field absent from the body should be left
untouched. The Scalar/OpenAPI request shape must stay a flat object
(`{ "name": "...", "email": "...", "isActive": true }`) — no nested
`{ isSet, value }` wrapper visible to API consumers.

## Approach

Introduce an `Optional<T>` wrapper type with a `System.Text.Json`
converter that only sets `IsSet = true` when the JSON property is
actually present, then thread it through the request → command → handler.
Separately, teach NSwag/NJsonSchema (which backs FastEndpoints'
`SwaggerDocument()`) to render `Optional<T>` as plain `T` in the generated
schema, so Scalar shows no shape change.

## Steps

### 1. Add `Optional<T>` + JSON converter (shared/common location)

New file, e.g. `backend/src/Web/Common/Optional.cs`:

- `readonly struct Optional<T>` with `IsSet` (bool) and `Value` (`T?`),
  factory methods `Unset()` and `Of(T? value)`.
- `OptionalJsonConverterFactory : JsonConverterFactory` — matches the open
  generic `Optional<>`.
- `OptionalJsonConverter<T> : JsonConverter<Optional<T>>` — `Read`
  deserializes `T` and returns `Optional<T>.Of(value)` (only called when
  the property is present in the payload); `Write` serializes `Value`
  directly (for symmetry/response use, though requests are the primary
  use case).

### 2. Register the converter globally

In `backend/src/Web/Program.cs`, wherever FastEndpoints'
`SerializerOptions` are configured (add if not already present):

```csharp
builder.Services.AddFastEndpoints()
       .Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o =>
           o.SerializerOptions.Converters.Add(new OptionalJsonConverterFactory()));
```

(Exact hook point to confirm against FastEndpoints 8.2.0's JSON options
API — may be `.ConfigureFastEndpoints(c => c.Serializer.Options...)`
instead.)

### 3. Update the request DTO

`backend/src/Web/Security/Users/Update/UpdateUserRequest.cs`:

```csharp
public class UpdateUserRequest
{
    public const string Route = "/security/users/{UserId:int}";
    public static string BuildRoute(int userId) => Route.Replace("{UserId:int}", userId.ToString());

    public int UserId { get; set; }
    public int Id { get; set; }
    public Optional<string> Name { get; set; }
    public Optional<string> Email { get; set; }
    public Optional<bool> IsActive { get; set; }
}
```

Validator changes to only run when a field is set, and to require at
least one field to be present:

```csharp
public class UpdateUserValidator : Validator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name.Value)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200)
            .When(x => x.Name.IsSet);

        RuleFor(x => x.Email.Value)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200)
            .When(x => x.Email.IsSet);

        RuleFor(x => x.UserId)
            .Must((args, userId) => args.Id == userId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");

        RuleFor(x => x)
            .Must(x => x.Name.IsSet || x.Email.IsSet || x.IsActive.IsSet)
            .WithMessage("At least one field must be provided to update.");
    }
}
```

### 4. Update the command

`backend/src/UseCases/Security/Users/Update/UpdateUserCommand.cs`:

```csharp
public record UpdateUserCommand(int UserId, Optional<string> Name, Optional<string> Email, Optional<bool> IsActive);
```

(`Optional<T>` moves to a location both `Web` and `UseCases` can
reference — e.g. `Core.Common` — rather than staying under
`Web.Common`.)

### 5. Update the endpoint

`backend/src/Web/Security/Users/Update/UpdateUser.cs`: pass the
`Optional<T>` fields straight through to the command unchanged.

### 6. Update the handler

`backend/src/UseCases/Security/Users/Update/UpdateUserHandler.cs`:

```csharp
if (command.Name.IsSet) user.UpdateName(command.Name.Value!);
if (command.Email.IsSet) user.Email = command.Email.Value!;
if (command.IsActive.IsSet) user.IsActive = command.IsActive.Value;
```

### 7. Keep the OpenAPI/Scalar shape flat

In `Program.cs`, extend the `SwaggerDocument(o => ...)` configuration
with an NJsonSchema type mapper for the open generic `Optional<>` so it
serializes in the schema as the wrapped type `T`, not as an
`{ isSet, value }` object, and so the property is never marked
`required`:

```csharp
o.DocumentSettings = s =>
{
    ...
    s.SchemaSettings.TypeMappers.Add(new OptionalOpenApiTypeMapper());
};
```

New file `backend/src/Web/Common/OptionalOpenApiTypeMapper.cs`
implementing NJsonSchema's `ITypeMapper` (or a small set of concrete
mappers per field type if the open-generic mapping isn't supported
cleanly) — needs a short spike against NSwag 14.7.1 to confirm the exact
interface shape, since `ITypeMapper.MappedType` typically expects a
closed type rather than an open generic definition.

### 8. Tests

- Unit test for `OptionalJsonConverter<T>`: absent property → `IsSet ==
  false`; present with value → `IsSet == true, Value == x`; present as
  `null` (once a nullable field exists) → `IsSet == true, Value == null`.
- Unit test for `UpdateUserHandler`: partial payload (e.g. only `IsActive`
  set) leaves `Name`/`Email` unchanged.
- Integration test hitting `PUT /security/users/{id}` with a partial JSON
  body, asserting untouched fields are preserved in the DB.
- Manual check: load `/docs` (Scalar) and confirm the `UpdateUserRequest`
  schema still shows `name`/`email`/`isActive` as plain scalar
  properties, not nested objects.

## Open questions / risks

- Confirm the correct FastEndpoints 8.2.0 hook for registering a global
  `JsonConverter` (System.Text.Json options may be configured differently
  than a raw `AddFastEndpoints()` chain).
- Confirm NJsonSchema 14.x's `ITypeMapper`/`SchemaSettings.TypeMappers`
  actually supports matching an open generic type definition; if not,
  fall back to per-field closed-generic mappers (`Optional<string>`,
  `Optional<bool>`) registered explicitly.
- None of `User`'s current fields are nullable at the domain level, so
  "explicit null" has no real target yet — worth confirming with the
  user which field(s) should become nullable, or whether this is purely
  forward-looking infrastructure.
- Consider whether the route verb should change from `PUT` to `PATCH`
  now that semantics are partial-update; out of scope unless requested.
