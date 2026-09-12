import { handleBackendError } from '../../utils/handleBackendError'

export interface DataListItem {
	id: string
	name: string
}

export interface DataListResponse {
	type: string
	items: DataListItem[]
}

export default defineEventHandler(async (event) => {
	const type = getRouterParam(event, 'type')

	try {
		return await callBackend<DataListResponse>(event, `/data-lists/${type}`)
	} catch (error) {
		await handleBackendError(error, event)
	}
})
