interface BackendMe {
	name: string
	email: string
	permissions: Array<{ name: string }>
}

export default defineOAuthAuth0EventHandler({
	config: {
		scope: ['openid', 'profile', 'email']
		// audience comes from NUXT_OAUTH_AUTH0_AUDIENCE — Auth0 only issues a JWT
		// access token (vs. an opaque string) when it's set. See docs/auth0-setup.md.
	},
	async onSuccess(event, { tokens }) {
		const runtimeConfig = useRuntimeConfig(event)
		const backendUrl = runtimeConfig.public.backendUrl as string

		console.log(`[auth0] onSuccess: backendUrl=${backendUrl} hasAccessToken=${Boolean(tokens.access_token)}`)

		// Deliberately not using the `user` argument here — it comes from Auth0's own
		// /userinfo endpoint, which knows nothing about our domain's permissions and
		// isn't guaranteed to carry the namespaced name/email claims either. Our own
		// backend already returns everything the UI needs from the same access token.
		let me: BackendMe
		try {
			me = await $fetch<BackendMe>(`${backendUrl}/security/me`, {
				headers: { Authorization: `Bearer ${tokens.access_token}` }
			})
			console.log(`[auth0] /security/me succeeded for ${me.email}`)
		} catch (error: unknown) {
			const status = (error as { response?: { status?: number } })?.response?.status
			const body = (error as { data?: unknown })?.data
			console.error(`[auth0] /security/me failed: status=${status} message=${(error as Error)?.message} body=${JSON.stringify(body)}`)
			if (status === 401) {
				// Expected on a brand-new user: the API JIT-creates the User row but
				// defaults it to IsActive = false until an admin approves it.
				return sendRedirect(event, '/login?error=inactive')
			}
			// Any other failure (backend down, DB unreachable, unexpected 500, network
			// error) — surface it as a login error instead of letting it throw uncaught.
			// An uncaught throw here doesn't render as a clean failure: Nitro logs it as
			// an unhandled request error and falls back to rendering the path as a page,
			// which the global auth middleware then bounces to a bare /login with no
			// indication anything went wrong.
			return sendRedirect(event, '/login?error=backend')
		}

		await setUserSession(event, {
			user: {
				id: me.email,
				name: me.name,
				email: me.email,
				permissions: me.permissions?.map(p => p.name) ?? []
			},
			secure: {
				accessToken: tokens.access_token
			}
		})

		console.log('[auth0] session set, redirecting to /')
		return sendRedirect(event, '/')
	},
	onError(event, error) {
		console.error(`[auth0] OAuth error: ${error?.message ?? error}`, error)
		return sendRedirect(event, '/login?error=oauth')
	}
})
