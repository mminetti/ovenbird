import type { Company } from '~/types'
import { getListQueryParams } from '../utils/listQueryParams'
import { handleBackendError } from '../utils/handleBackendError'

export interface CompaniesResponse {
	items: Company[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, marketId?: unknown, timeZoneId?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const timeZoneId = typeof body?.timeZoneId === 'string' ? body.timeZoneId.trim() : ''

			return await callBackend(event, '/settings/companies', {
				method: 'POST',
				body: {
					name,
					marketId: Number(body?.marketId),
					timeZoneId
				}
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const companyId = Number(query.id)
			const body = await readBody<{ name?: unknown, marketId?: unknown, timeZoneId?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const timeZoneId = typeof body?.timeZoneId === 'string' ? body.timeZoneId.trim() : undefined

			return await callBackend(event, `/settings/companies/${companyId}`, {
				method: 'PUT',
				body: {
					id: companyId,
					name,
					marketId: Number(body?.marketId),
					timeZoneId
				}
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const companyId = Number(query.id)

			return await callBackend(event, `/settings/companies/${companyId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const companyId = Number(query.id)
			return await callBackend<Company>(event, `/settings/companies/${companyId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<CompaniesResponse>(event, `/settings/companies?${params.toString()}`)
	} catch (error) {
		return handleBackendError(error, event)
	}
})
