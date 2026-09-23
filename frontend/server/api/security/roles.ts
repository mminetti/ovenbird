import type { SecurityRole } from '~/types'
import { getListQueryParams } from '../../utils/listQueryParams'
import { handleBackendError } from '../../utils/handleBackendError'

export interface SecurityRolesResponse {
	items: SecurityRole[]
	totalCount: number
}

interface RolePermissionOperation {
	permissionId: number
	operation: 'add' | 'remove'
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, permissionIds?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const permissionIds = Array.isArray(body?.permissionIds)
				? body.permissionIds.filter((id): id is number => typeof id === 'number')
				: []

			return await callBackend(event, '/security/roles', {
				method: 'POST',
				body: { name, permissionIds }
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const roleId = Number(query.id)
			const body = await readBody<{ name?: unknown, permissions?: RolePermissionOperation[] }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const permissions = Array.isArray(body?.permissions)
				? body.permissions
						.filter(p => typeof p?.permissionId === 'number' && (p.operation === 'add' || p.operation === 'remove'))
						.map(p => ({ permissionId: p.permissionId, operation: p.operation }))
				: []

			return await callBackend(event, `/security/roles/${roleId}`, {
				method: 'PUT',
				body: { id: roleId, name, permissions }
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
		return handleBackendError(error, event)
	}
})
