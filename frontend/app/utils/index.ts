export function randomInt(min: number, max: number): number {
	return Math.floor(Math.random() * (max - min + 1)) + min
}

export function randomFrom<T>(array: T[]): T {
	return array[Math.floor(Math.random() * array.length)]!
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
