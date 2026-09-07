/**
 * Global auth guard — redirects unauthenticated users to /login.
 * All routes are protected by default; /login is explicitly excluded.
 */
export default defineNuxtRouteMiddleware((to) => {
	const { loggedIn } = useUserSession()

	if (!loggedIn.value && to.path !== '/login') {
		return navigateTo('/login')
	}
})
