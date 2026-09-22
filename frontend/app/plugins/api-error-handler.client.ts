/**
 * Global handler for 401s coming back from our own `/api/**` server routes.
 *
 * `server/utils/handleBackendError.ts` already clears the sealed session
 * cookie server-side whenever the backend rejects the access token, but the
 * client's reactive session state (`useUserSession()`) and the URL don't
 * know that happened, so a page left sitting on a dead session would
 * otherwise just fail every fetch silently. This intercepts those 401s
 * once, syncs the client session state, and bounces to /login.
 */
export default defineNuxtPlugin(() => {
	const toast = useToast()
	const { clear } = useUserSession()

	let handling401 = false

	globalThis.$fetch = $fetch.create({
		async onResponseError({ request, response }) {
			if (response.status !== 401 || handling401) {
				return
			}

			const url = typeof request === 'string' ? request : request.url

			// Not one of our backend-proxying routes, or nuxt-auth-utils' own
			// session endpoint (avoid recursing through `clear()` below).
			if (!url.startsWith('/api/') || url.startsWith('/api/_auth/')) {
				return
			}

			handling401 = true

			try {
				await clear()

				toast.add({
					title: 'Session expired',
					description: 'Please sign in again.',
					color: 'error'
				})

				await navigateTo('/login')
			} finally {
				handling401 = false
			}
		}
	})
})
