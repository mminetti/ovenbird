/**
 * Global permission guard — reads `definePageMeta({ permission: '...' })` off
 * the target route and blocks navigation with a 403 error page when the
 * signed-in user lacks that permission. No-op when a route sets no permission.
 *
 * Runs after auth.global.ts (alphabetical ordering), so the user is already
 * known to be logged in by the time this checks permissions.
 */
export default defineNuxtRouteMiddleware((to) => {
	const permission = to.meta.permission

	if (!permission) {
		return
	}

	const { hasPermission } = useAuth()

	if (!hasPermission(permission)) {
		return createError({
			statusCode: 403,
			statusMessage: 'You do not have permission to view this page.'
		})
	}
})
