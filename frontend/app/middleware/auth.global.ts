/**
 * Global auth guard — redirects unauthenticated users to /login.
 * All routes are protected by default; /login is explicitly excluded.
 */
export default defineNuxtRouteMiddleware((to) => {
	const { loggedIn, user } = useUserSession()

	if (import.meta.server) {
		console.log(`[auth.global] path=${to.path} loggedIn=${loggedIn.value} user=${user.value?.email ?? 'none'}`)
	}

	if (!loggedIn.value && to.path !== '/login') {
		if (import.meta.server) {
			console.log(`[auth.global] redirecting ${to.path} -> /login (not logged in)`)
		}
		return navigateTo('/login')
	}
})
