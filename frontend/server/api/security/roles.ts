import type { SecurityRole } from '~/types'
import { getListQueryParams } from '../../utils/listQueryParams'
import { handleBackendError } from '../../utils/handleBackendError'

export interface SecurityRolesResponse {
	items: SecurityRole[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''

			return await callBackend(event, '/security/roles', {
				method: 'POST',
				body: { name }
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const roleId = Number(query.id)
			const body = await readBody<{ name?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined

			return await callBackend(event, `/security/roles/${roleId}`, {
				method: 'PUT',
				body: { id: roleId, name }
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const roleId = Number(query.id)

			return await callBackend(event, `/security/roles/${roleId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const roleId = Number(query.id)
			return await callBackend<SecurityRole>(event, `/security/roles/${roleId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<SecurityRolesResponse>(event, `/security/roles?${params.toString()}`)
	} catch (error) {
		await handleBackendError(error, event)
	}
})
