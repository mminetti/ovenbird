import type { H3Event } from 'h3'
import { $fetch } from 'ofetch'
import type { FetchOptions } from 'ofetch'

/**
 * Makes an authenticated request to the .NET backend.
 *
 * The access token is read from the secure (server-only) session data —
 * it is never sent to the browser. Use this inside Nitro server routes
 * (`/server/api/**`) to proxy calls to the backend.
 *
 * @example
 * // server/api/customers.ts
 * export default defineEventHandler(async (event) => {
 *   return callBackend(event, '/api/customers')
 * })
 */
export async function callBackend<T = unknown>(
	event: H3Event,
	path: string,
	options?: FetchOptions<'json'>
): Promise<T> {
	const runtimeConfig = useRuntimeConfig(event)
	const baseURL = runtimeConfig.public.backendUrl as string
	const session = await getUserSession(event)
	const token = session.secure?.accessToken

	return $fetch<T>(path, {
		...options,
		baseURL,
		responseType: 'json',
		headers: {
			...(options?.headers as Record<string, string> | undefined),
			...(token ? { Authorization: `Bearer ${token}` } : {})
		}
	})
}
