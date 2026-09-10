export function randomInt(min: number, max: number): number {
	return Math.floor(Math.random() * (max - min + 1)) + min
}

export function randomFrom<T>(array: T[]): T {
	return array[Math.floor(Math.random() * array.length)]!
}

function getMessageFromPayload(payload: unknown) {
	if (!payload || typeof payload !== 'object') {
		return null
	}

	const candidate = payload as Record<string, unknown>
	const message = candidate.detail ?? candidate.title ?? candidate.message ?? candidate.statusMessage ?? candidate.error

	return typeof message === 'string' && message.trim()
		? message.trim()
		: null
}

function getValidationErrorsFromPayload(payload: unknown): Record<string, string[]> | null {
	if (!payload || typeof payload !== 'object') {
		return null
	}

	const candidate = payload as Record<string, unknown>
	const rawErrors = (
		(candidate.errors && typeof candidate.errors === 'object' && !Array.isArray(candidate.errors))
			? candidate.errors
			: (
					candidate.data
					&& typeof candidate.data === 'object'
					&& !Array.isArray(candidate.data)
					&& (candidate.data as Record<string, unknown>).errors
					&& typeof (candidate.data as Record<string, unknown>).errors === 'object'
					&& !Array.isArray((candidate.data as Record<string, unknown>).errors)
						? (candidate.data as Record<string, unknown>).errors
						: null
				)
	)

	if (!rawErrors) {
		return null
	}

	const errors = rawErrors as Record<string, unknown>
	const result: Record<string, string[]> = {}

	for (const [key, value] of Object.entries(errors)) {
		if (typeof value === 'string' && value.trim()) {
			result[key] = [value.trim()]
			continue
		}

		if (Array.isArray(value)) {
			const messages = value
				.filter(v => typeof v === 'string' && v.trim())
				.map(v => v.trim())

			if (messages.length > 0) {
				result[key] = messages
			}
		}
	}

	return Object.keys(result).length > 0 ? result : null
}

export function extractApiErrorMessage(error: unknown, fallback = 'Something went wrong while contacting the API.') {
	if (typeof error === 'string' && error.trim()) {
		return error.trim()
	}

	if (!error || typeof error !== 'object') {
		return fallback
	}

	const candidate = error as {
		data?: unknown
		message?: unknown
		statusMessage?: unknown
		response?: {
			_data?: unknown
			statusText?: unknown
		}
	}

	return getMessageFromPayload(candidate.data)
		?? getMessageFromPayload(candidate.response?._data)
		?? (typeof candidate.statusMessage === 'string' && candidate.statusMessage.trim() ? candidate.statusMessage.trim() : null)
		?? (typeof candidate.message === 'string' && candidate.message.trim() ? candidate.message.trim() : null)
		?? (typeof candidate.response?.statusText === 'string' && candidate.response.statusText.trim() ? candidate.response.statusText.trim() : null)
		?? fallback
}

export function parseApiError(error: unknown): { message: string, errors?: Record<string, string[]> } {
	const candidate = (error && typeof error === 'object') ? error as { data?: unknown, response?: { _data?: unknown } } : null
	const payload = candidate?.data ?? candidate?.response?._data
	const errors = getValidationErrorsFromPayload(payload)

	return {
		message: extractApiErrorMessage(error),
		...(errors ? { errors } : {})
	}
}
