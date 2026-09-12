import { handleBackendError } from '../../../utils/handleBackendError'

export default defineEventHandler(async (event) => {
	try {
		const query = getQuery(event)
		const roleId = Number(query.id)
		const body = await readBody<{ permissionIds?: unknown }>(event)
		const permissionIds = Array.isArray(body?.permissionIds)
			? body.permissionIds.filter((id): id is number => typeof id === 'number')
			: []

		return await callBackend(event, `/security/roles/${roleId}/permissions`, {
			method: 'POST',
			body: { roleId, permissionIds }
		})
	} catch (error) {
		await handleBackendError(error, event)
	}
})
