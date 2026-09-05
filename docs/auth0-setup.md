# Auth0 dashboard setup (manual steps)

These steps happen in the Auth0 dashboard, not in this repo. Complete them
before setting `Authentication:Provider = "Auth0"` in any environment.

## 1. Create an API

Dashboard → **Applications → APIs → Create API**.

- Give it a name (e.g. "Ovenbird API").
- Set an **Identifier** — a URI-shaped string that does not need to resolve
  to anything (e.g. `https://api.ovenbird.example.com`). This value becomes
  `Auth0:Audience`.
- Note the tenant **Domain** shown in the dashboard (e.g.
  `your-tenant.us.auth0.com`, no scheme prefix). This becomes `Auth0:Domain`.

## 2. Create an Application (optional — only for Swagger/Scalar "Authorize" login)

Dashboard → **Applications → Create Application** (Single Page Application,
or Regular Web Application — whichever matches an authorization-code + PKCE
flow).

- Note its **Client ID** — becomes `Auth0:ClientId`.
- Under the Application's **Settings**, add the Swagger UI / Scalar UI
  redirect URI(s) to **Allowed Callback URLs**. Confirm the exact path by
  running the API locally once this is wired up and watching what redirect
  URI the "Authorize" button requests. Add the same origin to **Allowed Web
  Origins** / **Allowed Logout URLs** if you hit CORS/origin errors.
- Under the API from step 1, authorize this Application to request its
  audience.

Skip this step if you only need machine-to-machine (client-credentials)
tokens for testing — those don't need an interactive Application.

## 3. Create a Post-Login Action to add name/email claims

Auth0 access tokens only carry `sub`, `aud`, `iss`, `iat`, `exp`, `azp`,
`scope` by default — **not** name or email, even if the `profile`/`email`
scopes are requested. To get those into the access token (which is what the
API actually validates), add a custom claim via a Post-Login Action.

Dashboard → **Actions → Flows → Login** → add a custom Action with this
source:

```javascript
exports.onExecutePostLogin = async (event, api) => {
  const namespace = 'https://ovenbird.example.com/'; // must exactly match Auth0:ClaimsNamespace below, including trailing slash
  if (event.user.email) {
    api.accessToken.setCustomClaim(`${namespace}email`, event.user.email);
  }
  if (event.user.name) {
    api.accessToken.setCustomClaim(`${namespace}name`, event.user.name);
  }
};
```

Drag the Action into the Login flow diagram and deploy it. Pick a real
namespace URI you control conceptually (it doesn't need to resolve) and use
the exact same string — trailing slash included — for `Auth0:ClaimsNamespace`
in the API's configuration.

**Note:** this Action only fires for interactive user logins, not
machine-to-machine client-credentials grants. A client-credentials token's
`sub` will look like `{client_id}@clients` and will never carry the
namespaced name/email claims.

## 4. Request tokens with the right audience

Auth0 only issues a JWT access token (rather than an opaque string) when the
token request includes an `audience` parameter matching the API identifier
from step 1. If `Auth0:Audience` is missing or wrong, tokens will be opaque
and the API's JWT bearer validation will reject them as malformed.

## Configuring the API with these values

Locally, from `backend/src/Web`:

```
dotnet user-secrets init
dotnet user-secrets set "Authentication:Provider" "Auth0"
dotnet user-secrets set "Auth0:Domain" "your-tenant.us.auth0.com"
dotnet user-secrets set "Auth0:Audience" "https://api.ovenbird.example.com"
dotnet user-secrets set "Auth0:ClientId" "your-swagger-client-id"
dotnet user-secrets set "Auth0:ClaimsNamespace" "https://ovenbird.example.com/"
```

In deployed environments, set the same keys via that environment's
configuration/secret store — never commit real values to `appsettings.json`.

## Verifying end-to-end (no frontend yet)

1. Get a client-credentials token (safe, no real user needed) to prove
   signature/issuer/audience validation and the `sub`-based identifier path:
   ```
   curl --request POST --url https://your-tenant.us.auth0.com/oauth/token \
     --header 'content-type: application/json' \
     --data '{"client_id":"...","client_secret":"...","audience":"https://api.ovenbird.example.com","grant_type":"client_credentials"}'
   ```
2. Call `GET /security/me` with `Authorization: Bearer {token}`. Expect a
   **401** on the very first call — the API JIT-creates a `User` row keyed by
   the token's `sub` claim, but new users default to `IsActive = false`.
   This is existing, correct behavior, not a bug.
3. Activate the user (via the Users admin endpoint, or directly in the DB),
   then re-call `/security/me` — expect **200** with an empty `Permissions`
   list (no roles assigned yet).
4. To see namespaced name/email flow through, use Swagger's or Scalar's
   "Authorize" button to complete a real interactive login, then re-check
   `/security/me` — `Name`/`Email` should now be populated from the Action's
   custom claims.
