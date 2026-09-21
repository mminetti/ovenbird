import type { Configuration, ConfigurationField } from '~/types'
import { getListQueryParams } from '../utils/listQueryParams'
import { handleBackendError } from '../utils/handleBackendError'

export interface ConfigurationsResponse {
	items: Configuration[]
	totalCount: number
}

interface ConfigurationFieldPayload {
	name?: unknown
	value?: unknown
}

function toFieldPayload(fields: unknown): Pick<ConfigurationField, 'name' | 'value'>[] {
	if (!Array.isArray(fields)) {
		return []
	}

	return (fields as ConfigurationFieldPayload[])
		.map(field => ({
			name: typeof field?.name === 'string' ? field.name.trim() : '',
			value: typeof field?.value === 'string' ? field.value : undefined
		}))
		.filter(field => field.name.length > 0)
}

function toConnectorIds(connectorIds: unknown): number[] {
	if (!Array.isArray(connectorIds)) {
		return []
	}

	return connectorIds.filter((id): id is number => typeof id === 'number')
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, description?: unknown, configurationTypeId?: unknown, companyId?: unknown, connectorIds?: unknown, fields?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const description = typeof body?.description === 'string' ? body.description.trim() : undefined

			return await callBackend(event, '/settings/configurations', {
				method: 'POST',
				body: {
					name,
					description,
					configurationTypeId: Number(body?.configurationTypeId),
					companyId: body?.companyId ? Number(body.companyId) : undefined,
					connectorIds: toConnectorIds(body?.connectorIds),
					fields: toFieldPayload(body?.fields)
				}
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const configurationId = Number(query.id)
			const body = await readBody<{ name?: unknown, description?: unknown, configurationTypeId?: unknown, companyId?: unknown, connectorIds?: unknown, fields?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const description = typeof body?.description === 'string' ? body.description.trim() : undefined

			return await callBackend(event, `/settings/configurations/${configurationId}`, {
				method: 'PUT',
				body: {
					id: configurationId,
					name,
					description,
					configurationTypeId: Number(body?.configurationTypeId),
					companyId: body?.companyId ? Number(body.companyId) : undefined,
					connectorIds: toConnectorIds(body?.connectorIds),
					fields: toFieldPayload(body?.fields)
				}
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const configurationId = Number(query.id)

			return await callBackend(event, `/settings/configurations/${configurationId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const configurationId = Number(query.id)
			return await callBackend<Configuration>(event, `/settings/configurations/${configurationId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<ConfigurationsResponse>(event, `/settings/configurations?${params.toString()}`)
	} catch (error) {
		return handleBackendError(error, event)
	}
})
