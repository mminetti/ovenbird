import { handleBackendError } from '../../../utils/handleBackendError'

export default defineEventHandler(async (event) => {
	try {
		const query = getQuery(event)
		const userId = Number(query.id)
		const body = await readBody<{ roleIds?: unknown }>(event)
		const roleIds = Array.isArray(body?.roleIds)
			? body.roleIds.filter((id): id is number => typeof id === 'number')
			: []

		return await callBackend(event, `/security/users/${userId}/roles`, {
			method: 'POST',
			body: { userId, roleIds }
		})
	} catch (error) {
		return handleBackendError(error, event)
	}
})
