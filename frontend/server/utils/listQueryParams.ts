export function getListQueryParams(query: Record<string, unknown>) {
	const params = new URLSearchParams()

	if (query.pageSize) {
		params.set('per_page', String(query.pageSize))
	}

	if (query.page) {
		params.set('page', String(query.page))
	}

	if (query.sortBy) {
		let sortBy = String(query.sortBy)

		if (query.order) {
			sortBy = `${sortBy} ${query.order}`
		}

		params.set('order_by', sortBy)
	}

	if (query.q) {
		params.set('search', String(query.q))
	}

	return params
}
