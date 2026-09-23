# Ovenbird

## Verification before declaring work done

- After any change under `frontend/`, run lint before saying the task is done:
  `cd frontend && pnpm lint`
- After any change under `backend/`, build the whole solution before saying the task is done:
  `cd backend && dotnet build Ovenbird.slnx`

Fix any errors these report before considering the change complete.
