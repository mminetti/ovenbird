const FALLBACK_TIMEZONE = 'UTC'

function detectBrowserTimezone(): string {
	if (import.meta.server) {
		return FALLBACK_TIMEZONE
	}

	try {
		return Intl.DateTimeFormat().resolvedOptions().timeZone || FALLBACK_TIMEZONE
	} catch {
		return FALLBACK_TIMEZONE
	}
}

/**
 * Manages the viewer's timezone preference for displaying UTC dates.
 * Defaults to the browser's detected timezone and persists overrides in a cookie.
 */
export function useTimezone() {
	const timezoneCookie = useCookie<string | null>('display-timezone', {
		maxAge: 60 * 60 * 24 * 365
	})

	const timezone = computed({
		get: () => timezoneCookie.value || detectBrowserTimezone(),
		set: (tz: string) => {
			timezoneCookie.value = tz
		}
	})

	function resetToDetected() {
		timezoneCookie.value = null
	}

	return {
		timezone,
		resetToDetected
	}
}
