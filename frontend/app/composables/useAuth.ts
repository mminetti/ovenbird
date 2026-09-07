/**
 * useAuth — thin wrapper over nuxt-auth-utils' useUserSession.
 *
 * Exposes:
 *  - user         — reactive user object (id, name, email, permissions)
 *  - loggedIn     — whether the session is active
 *  - hasPermission — check if the user holds a given permission string
 *  - logout       — navigate to /auth/logout, which clears the session and
 *                   redirects through Auth0's own logout endpoint
 */
export function useAuth() {
	const { user, loggedIn, fetch } = useUserSession()

	async function logout() {
		await navigateTo('/auth/logout', { external: true })
	}

	function hasPermission(permission: string): boolean {
		return user.value?.permissions?.includes(permission) ?? false
	}

	return {
		user,
		loggedIn,
		hasPermission,
		logout,
		refreshSession: fetch
	}
}
