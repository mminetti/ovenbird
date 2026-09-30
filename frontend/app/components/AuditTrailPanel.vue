<script setup lang="ts">
import type { AuditTrailResponse } from '~~/server/api/audit-trail'
import type { AuditTrailRecord } from '~/types'
import type { TypedColumnConfig } from '~/utils/paginatedTableColumns'

const props = defineProps<{
	entityType: string
	entityId: number | string | null
}>()

const { timezone } = useTimezone()

const page = ref(1)
const pageSize = ref(10)
const data = ref<AuditTrailResponse | null>(null)
const loading = ref(false)
const fetchError = ref<unknown>(null)
const selectedRecord = ref<AuditTrailRecord | null>(null)

async function load() {
	if (!props.entityId) {
		data.value = null
		return
	}

	loading.value = true
	fetchError.value = null

	try {
		data.value = await $fetch<AuditTrailResponse>('/api/audit-trail', {
			query: {
				entityType: props.entityType,
				entityId: String(props.entityId),
				page: page.value,
				pageSize: pageSize.value
			}
		})
	} catch (error) {
		fetchError.value = error
		data.value = null
	} finally {
		loading.value = false
	}
}

useApiErrorToast(fetchError)

watch(() => [props.entityType, props.entityId], () => {
	page.value = 1
	selectedRecord.value = null
	load()
})

watch([page, pageSize], load)

onMounted(load)

const rows = computed(() => data.value?.items ?? [])
const total = computed(() => data.value?.totalCount ?? 0)

function formatValue(value: unknown): string {
	if (value === null || value === undefined) {
		return '—'
	}

	if (typeof value === 'boolean') {
		return value ? 'true' : 'false'
	}

	if (typeof value === 'object') {
		return JSON.stringify(value)
	}

	return String(value)
}

function parseValues(json: string | null): Record<string, unknown> {
	if (!json) {
		return {}
	}

	try {
		const parsed = JSON.parse(json)
		return (parsed && typeof parsed === 'object') ? parsed as Record<string, unknown> : {}
	} catch {
		return {}
	}
}

const diffRows = computed(() => {
	if (!selectedRecord.value) {
		return []
	}

	const oldValues = parseValues(selectedRecord.value.oldValues)
	const newValues = parseValues(selectedRecord.value.newValues)
	const keys = Array.from(new Set([...Object.keys(oldValues), ...Object.keys(newValues)])).sort()

	return keys.map(key => ({
		key,
		oldValue: formatValue(oldValues[key]),
		newValue: formatValue(newValues[key])
	}))
})

const columnConfig = computed<TypedColumnConfig<AuditTrailRecord, keyof AuditTrailRecord & string>[]>(() => [
	{
		key: 'timestampUtc',
		label: 'Timestamp',
		type: 'date',
		timeZone: timezone.value
	},
	{
		key: 'userId',
		label: 'User',
		type: 'string'
	},
	{
		key: 'action',
		label: 'Action',
		type: 'badge',
		badgeColor: 'neutral',
		badgeVariant: 'subtle'
	},
	{
		key: 'entityType',
		label: 'Entity',
		type: 'string'
	}
])
</script>

<template>
	<div>
		<div v-if="selectedRecord">
			<UButton
				label="Back to history"
				icon="i-lucide-arrow-left"
				color="neutral"
				variant="ghost"
				size="sm"
				class="mb-3 -ml-2.5"
				@click="selectedRecord = null"
			/>

			<div class="mb-3 flex flex-wrap items-center gap-3 text-sm">
				<UBadge color="neutral" variant="subtle">
					{{ selectedRecord.action }}
				</UBadge>
				<span class="text-muted">{{ formatUtcToTimezone(selectedRecord.timestampUtc, timezone) }}</span>
				<span v-if="selectedRecord.userId" class="text-muted">by {{ selectedRecord.userId }}</span>
			</div>

			<div v-if="diffRows.length" class="overflow-hidden rounded-lg border border-default">
				<table class="w-full text-sm">
					<thead>
						<tr class="bg-elevated/50">
							<th class="px-3 py-1.5 text-left font-medium text-highlighted">
								Field
							</th>
							<th v-if="selectedRecord.action !== 'Create'" class="px-3 py-1.5 text-left font-medium text-highlighted">
								Old value
							</th>
							<th v-if="selectedRecord.action !== 'Delete'" class="px-3 py-1.5 text-left font-medium text-highlighted">
								New value
							</th>
						</tr>
					</thead>
					<tbody>
						<tr v-for="row in diffRows" :key="row.key" class="border-t border-default">
							<td class="px-3 py-2 align-top font-medium text-highlighted">
								{{ row.key }}
							</td>
							<td v-if="selectedRecord.action !== 'Create'" class="px-3 py-2 align-top break-all text-muted">
								{{ row.oldValue }}
							</td>
							<td v-if="selectedRecord.action !== 'Delete'" class="px-3 py-2 align-top break-all text-muted">
								{{ row.newValue }}
							</td>
						</tr>
					</tbody>
				</table>
			</div>
			<p v-else class="text-sm text-muted">
				No field changes recorded for this entry.
			</p>
		</div>

		<PaginatedTable
			v-else
			:data="rows"
			:column-config="columnConfig"
			:total="total"
			:loading="loading"
			:page="page"
			:page-size="pageSize"
			@update:page="page = $event"
			@update:page-size="pageSize = $event"
			@select="(record) => (selectedRecord = record)"
		/>
	</div>
</template>
