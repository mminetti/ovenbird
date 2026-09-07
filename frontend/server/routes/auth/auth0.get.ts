interface BackendMe {
	name: string
	email: string
	permissions: Array<{ name: string, module: string }>
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

		// Deliberately not using the `user` argument here — it comes from Auth0's own
		// /userinfo endpoint, which knows nothing about our domain's permissions and
		// isn't guaranteed to carry the namespaced name/email claims either. Our own
		// backend already returns everything the UI needs from the same access token.
		let me: BackendMe
		try {
			me = await $fetch<BackendMe>(`${backendUrl}/security/me`, {
				headers: { Authorization: `Bearer ${tokens.access_token}` }
			})
		} catch (error: unknown) {
			if ((error as { response?: { status?: number } })?.response?.status === 401) {
				// Expected on a brand-new user: the API JIT-creates the User row but
				// defaults it to IsActive = false until an admin approves it.
				return sendRedirect(event, '/login?error=inactive')
			}
			throw error
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

		return sendRedirect(event, '/')
	},
	onError(event, error) {
		console.error('Auth0 OAuth error:', error)
		return sendRedirect(event, '/login?error=oauth')
	}
})
