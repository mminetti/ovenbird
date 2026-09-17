import type { H3Event } from 'h3'

interface BackendErrorShape {
	status?: number
	statusCode?: number
	statusMessage?: unknown
	message?: unknown
	data?: unknown
	response?: {
		status?: number
		statusText?: unknown
		_data?: unknown
	}
}

/**
 * Normalizes errors from backend API calls into H3 errors so every
 * `server/api/**` route surfaces failures the same way.
 *
 * - A 401 from the backend means the session's access token is no longer
 *   valid (expired, revoked, user deactivated) — the local session is
 *   cleared so the next request re-authenticates instead of looping on a
 *   dead token.
 * - Any other 4xx/5xx is re-thrown with the backend's own status code and
 *   message (extracted from its JSON payload) plus the raw payload as
 *   `data`, so field-level validation errors reach the client.
 * - Anything without a usable status code (backend unreachable, timed out,
 *   DNS failure, etc.) is logged here — where we know which backend call
 *   failed — and reported to the client as a generic 502 rather than
 *   letting the raw network error bubble up unformatted.
 *
 * Always throws — never returns normally.
 */
export async function handleBackendError(error: unknown, event: H3Event): Promise<never> {
	const candidate = (error ?? {}) as BackendErrorShape
	const statusCode = candidate.status ?? candidate.statusCode ?? candidate.response?.status
	const payload = candidate.data ?? candidate.response?._data
	const payloadMessage = getMessageFromPayload(payload)
	const fallbackMessage = typeof candidate.statusMessage === 'string' && candidate.statusMessage.trim()
		? candidate.statusMessage.trim()
		: (typeof candidate.response?.statusText === 'string' && candidate.response.statusText.trim()
				? candidate.response.statusText.trim()
				: (typeof candidate.message === 'string' && candidate.message.trim() ? candidate.message.trim() : 'Request failed'))

	if (statusCode === 401) {
		await clearUserSession(event)
		throw createError({ statusCode: 401, message: 'Unauthorized' })
	}

	if (typeof statusCode === 'number' && statusCode >= 400) {
		throw createError({
			statusCode,
			message: payloadMessage ?? fallbackMessage,
			data: payload && typeof payload === 'object' ? payload : undefined
		})
	}

	console.error('[handleBackendError] backend call failed with no usable status code', error)

	throw createError({
		statusCode: 502,
		message: 'Unable to reach the server. Please try again.',
		cause: error
	})
}
