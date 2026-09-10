import type { SecurityUser } from '~/types'
import { getListQueryParams } from '../../utils/listQueryParams'
import { handleBackendError } from '../../utils/handleBackendError'

export interface SecurityUsersResponse {
	items: SecurityUser[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	const method = event.method

	try {
		if (method === 'POST') {
			const body = await readBody<{ name?: unknown, email?: unknown, externalIdentifier?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : ''
			const email = typeof body?.email === 'string' ? body.email.trim() : ''
			const externalIdentifier = typeof body?.externalIdentifier === 'string' ? body.externalIdentifier.trim() : ''

			return await callBackend(event, '/security/users', {
				method: 'POST',
				body: { name, email, externalIdentifier }
			})
		}

		if (method === 'PATCH') {
			const query = getQuery(event)
			const userId = Number(query.id)
			const body = await readBody<{ name?: unknown, email?: unknown, isActive?: unknown }>(event)
			const name = typeof body?.name === 'string' ? body.name.trim() : undefined
			const email = typeof body?.email === 'string' ? body.email.trim() : undefined
			const isActive = typeof body?.isActive === 'boolean' ? body.isActive : undefined

			return await callBackend(event, `/security/users/${userId}`, {
				method: 'PUT',
				body: { id: userId, name, email, isActive }
			})
		}

		if (method === 'DELETE') {
			const query = getQuery(event)
			const userId = Number(query.id)

			return await callBackend(event, `/security/users/${userId}`, {
				method: 'DELETE'
			})
		}

		const query = getQuery(event)

		if (query.id) {
			const userId = Number(query.id)
			return await callBackend<SecurityUser>(event, `/security/users/${userId}`)
		}

		const params = getListQueryParams(query)

		return await callBackend<SecurityUsersResponse>(event, `/security/users?${params.toString()}`)
	} catch (error) {
		await handleBackendError(error, event)
	}
})
