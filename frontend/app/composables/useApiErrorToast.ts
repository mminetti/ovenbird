import type { Ref } from 'vue'

/**
 * Surfaces a `useFetch`/`useAsyncData` `error` ref as a toast. 401s are
 * skipped — the `api-error-handler` plugin already clears the session and
 * redirects to /login for those, so toasting here would just be a second,
 * shorter-lived message racing that redirect.
 */
export function useApiErrorToast(error: Ref<unknown>) {
	const toast = useToast()

	watch(error, (value) => {
		if (!value) {
			return
		}

		const statusCode = (value as { statusCode?: number }).statusCode
		if (statusCode === 401) {
			return
		}

		toast.add({
			title: 'Something went wrong',
			description: extractApiErrorMessage(value),
			color: 'error'
		})
	})
}
