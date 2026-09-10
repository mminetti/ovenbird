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

function getMessageFromPayload(payload: unknown) {
	if (!payload || typeof payload !== 'object') {
		return null
	}

	const candidate = payload as Record<string, unknown>
	const message = candidate.detail ?? candidate.title ?? candidate.message ?? candidate.error

	return typeof message === 'string' && message.trim()
		? message.trim()
		: null
}

/**
 * Handles errors from backend API calls, specifically checking for 401 responses
 * and clearing the user session in that case.
 */
export async function handleBackendError(error: unknown, event: H3Event) {
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
			statusMessage: payloadMessage ?? fallbackMessage,
			data: payload && typeof payload === 'object' ? payload : undefined
		})
	}

	throw error
}
