/**
 * Shared between server routes (normalizing backend errors) and the Vue app
 * (parsing errors back out of `$fetch` calls to our own `/api/**` routes) so
 * both sides agree on where a human-readable message and field-level
 * validation errors live in a JSON error payload.
 */

export function getMessageFromPayload(payload: unknown): string | null {
	if (!payload || typeof payload !== 'object') {
		return null
	}

	const candidate = payload as Record<string, unknown>
	const message = candidate.detail ?? candidate.title ?? candidate.message ?? candidate.statusMessage ?? candidate.error

	return typeof message === 'string' && message.trim()
		? message.trim()
		: null
}

export function getValidationErrorsFromPayload(payload: unknown): Record<string, string[]> | null {
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
