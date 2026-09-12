import type { SecurityPermission } from '~/types'
import { getListQueryParams } from '../../utils/listQueryParams'
import { handleBackendError } from '../../utils/handleBackendError'

export interface SecurityPermissionsResponse {
	items: SecurityPermission[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, description?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const description = typeof body?.description === 'string' ? body.description.trim() : ''

			return await callBackend(event, '/security/permissions', {
				method: 'POST',
				body: { name, description }
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const permissionId = Number(query.id)
			const body = await readBody<{ name?: unknown, description?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const description = typeof body?.description === 'string' ? body.description.trim() : undefined

			return await callBackend(event, `/security/permissions/${permissionId}`, {
				method: 'PUT',
				body: { id: permissionId, name, description }
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const permissionId = Number(query.id)

			return await callBackend(event, `/security/permissions/${permissionId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const permissionId = Number(query.id)
			return await callBackend<SecurityPermission>(event, `/security/permissions/${permissionId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<SecurityPermissionsResponse>(event, `/security/permissions?${params.toString()}`)
	} catch (error) {
		await handleBackendError(error, event)
	}
})
