import type { Connector, ConnectorField } from '~/types'
import { getListQueryParams } from '../utils/listQueryParams'
import { handleBackendError } from '../utils/handleBackendError'

export interface ConnectorsResponse {
	items: Connector[]
	totalCount: number
}

interface ConnectorFieldPayload {
	name?: unknown
	value?: unknown
	isSecret?: unknown
}

function toFieldPayload(fields: unknown): Pick<ConnectorField, 'name' | 'value' | 'isSecret'>[] {
	if (!Array.isArray(fields)) {
		return []
	}

	return (fields as ConnectorFieldPayload[])
		.map(field => ({
			name: typeof field?.name === 'string' ? field.name.trim() : '',
			value: typeof field?.value === 'string' ? field.value : undefined,
			isSecret: Boolean(field?.isSecret)
		}))
		.filter(field => field.name.length > 0)
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, description?: unknown, connectorTypeId?: unknown, connectorImplementationId?: unknown, fields?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const description = typeof body?.description === 'string' ? body.description.trim() : undefined

			return await callBackend(event, '/connectors', {
				method: 'POST',
				body: {
					name,
					description,
					connectorTypeId: Number(body?.connectorTypeId),
					connectorImplementationId: Number(body?.connectorImplementationId),
					fields: toFieldPayload(body?.fields)
				}
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const connectorId = Number(query.id)
			const body = await readBody<{ name?: unknown, description?: unknown, connectorTypeId?: unknown, connectorImplementationId?: unknown, fields?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const description = typeof body?.description === 'string' ? body.description.trim() : undefined

			return await callBackend(event, `/connectors/${connectorId}`, {
				method: 'PUT',
				body: {
					id: connectorId,
					name,
					description,
					connectorTypeId: Number(body?.connectorTypeId),
					connectorImplementationId: Number(body?.connectorImplementationId),
					fields: toFieldPayload(body?.fields)
				}
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const connectorId = Number(query.id)

			return await callBackend(event, `/connectors/${connectorId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const connectorId = Number(query.id)
			return await callBackend<Connector>(event, `/connectors/${connectorId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<ConnectorsResponse>(event, `/connectors?${params.toString()}`)
	} catch (error) {
		return handleBackendError(error, event)
	}
})
