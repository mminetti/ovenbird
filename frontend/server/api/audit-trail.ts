import type { AuditTrailRecord } from '~/types'
import { handleBackendError } from '../utils/handleBackendError'

export interface AuditTrailResponse {
	items: AuditTrailRecord[]
	totalCount: number
}

export default defineEventHandler(async (event) => {
	try {
		const query = getQuery(event)
		const entityType = typeof query.entityType === 'string' ? query.entityType : ''
		const entityId = typeof query.entityId === 'string' ? query.entityId : ''

		const params = new URLSearchParams()
		params.set('entity_type', entityType)
		params.set('entity_id', entityId)

		if (query.page) {
			params.set('page', String(query.page))
		}

		if (query.pageSize) {
			params.set('per_page', String(query.pageSize))
		}

		return await callBackend<AuditTrailResponse>(event, `/audit-trail?${params.toString()}`)
	} catch (error) {
		return handleBackendError(error, event)
	}
})
