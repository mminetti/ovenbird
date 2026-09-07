/**
 * Logout must be a real browser navigation (GET), not a fetch — it needs to end in a
 * redirect to Auth0's own logout endpoint. Clearing only the local session cookie
 * would leave the Auth0 SSO session alive, so the next "Sign in" click would silently
 * re-authenticate the user instead of prompting for credentials.
 */
export default defineEventHandler(async (event) => {
	const runtimeConfig = useRuntimeConfig(event)
	const domain = runtimeConfig.oauth.auth0.domain as string
	const clientId = runtimeConfig.oauth.auth0.clientId as string

	await clearUserSession(event)

	const returnTo = `${getRequestProtocol(event)}://${getRequestHost(event)}/login`
	const logoutUrl = new URL(`https://${domain}/v2/logout`)
	logoutUrl.searchParams.set('client_id', clientId)
	logoutUrl.searchParams.set('returnTo', returnTo)

	return sendRedirect(event, logoutUrl.toString())
})
