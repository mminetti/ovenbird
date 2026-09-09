/**
 * Temporary tracer for the Azure login redirect-loop investigation.
 * Logs which auth-related cookies arrive on request and get set on response,
 * without logging their values. Safe to remove once the issue is diagnosed.
 */
const TRACKED_COOKIES = ['nuxt-auth-state', 'nuxt-session']

export default defineEventHandler((event) => {
	const path = event.path.split('?')[0]
	if ((!path || !path.startsWith('/auth/')) && path !== '/') return

	const cookieHeader = getHeader(event, 'cookie') ?? ''
	const incoming = TRACKED_COOKIES.filter(name => cookieHeader.includes(`${name}=`))
	console.log(`[cookie-trace] --> ${event.method} ${path} incomingCookies=[${incoming.join(', ')}]`)

	event.node.res.on('finish', () => {
		const setCookieHeader = event.node.res.getHeader('set-cookie')
		const setCookies = (Array.isArray(setCookieHeader) ? setCookieHeader : [setCookieHeader])
			.filter(Boolean)
			.map(c => String(c).split('=')[0])
		const location = event.node.res.getHeader('location')
		console.log(`[cookie-trace] <-- ${event.method} ${path} status=${event.node.res.statusCode} setCookies=[${setCookies.join(', ')}] location=${location ?? 'n/a'}`)
	})
})
