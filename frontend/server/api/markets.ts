import type { Market } from '~/types'
import { getListQueryParams } from '../utils/listQueryParams'
import { handleBackendError } from '../utils/handleBackendError'

export interface MarketsResponse {
	items: Market[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, identifier?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const identifier = typeof body?.identifier === 'string' ? body.identifier.trim() : ''

			return await callBackend(event, '/markets', {
				method: 'POST',
				body: {
					name,
					identifier
				}
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const marketId = Number(query.id)
			const body = await readBody<{ name?: unknown, identifier?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const identifier = typeof body?.identifier === 'string' ? body.identifier.trim() : undefined

			return await callBackend(event, `/markets/${marketId}`, {
				method: 'PUT',
				body: {
					id: marketId,
					name,
					identifier
				}
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const marketId = Number(query.id)

			return await callBackend(event, `/markets/${marketId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const marketId = Number(query.id)
			return await callBackend<Market>(event, `/markets/${marketId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<MarketsResponse>(event, `/markets?${params.toString()}`)
	} catch (error) {
		return handleBackendError(error, event)
	}
})
