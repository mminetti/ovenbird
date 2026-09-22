<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { ConnectorsResponse } from '~~/server/api/connectors'
import type { Connector } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'connectors.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'connectorTypeName' | 'connectorImplementationName'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedConnectorId = ref<number | null>(null)
const selectedConnectorName = ref<string | null>(null)

watch(pageSize, () => {
	page.value = 1
})

watch(debouncedSearch, () => {
	page.value = 1
})

const query = computed(() => {
	return {
		pageSize: pageSize.value,
		page: page.value,
		sortBy: sort.value.column,
		order: sort.value.direction,
		q: debouncedSearch.value || undefined
	}
})

const { data, pending, refresh, error } = await useFetch<ConnectorsResponse>('/api/connectors', {
	query,
	lazy: true,
	server: false
})

useApiErrorToast(error)

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedConnectorId.value = null
	selectedConnectorName.value = null

	const displayName = name || `Connector #${id}`

	toast.add({
		title: 'Connector deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(connector: Connector) {
	await refresh()

	toast.add({
		title: 'Connector created',
		description: `${connector.name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(connector: Connector) {
	await refresh()

	toast.add({
		title: 'Connector updated',
		description: `${connector.name} has been updated.`,
		color: 'success'
	})
}

function openConnectorDetails(connector: Connector) {
	selectedConnectorId.value = connector.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<Connector, SortColumn>[] = [
	{
		key: 'id',
		label: 'Id',
		type: 'number'
	},
	{
		key: 'name',
		label: 'Name',
		type: 'string'
	},
	{
		key: 'connectorTypeName',
		label: 'Type',
		type: 'string'
	},
	{
		key: 'connectorImplementationName',
		label: 'Implementation',
		type: 'string'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (connector: Connector) => h(
			'div',
			{ class: 'text-right' },
			h(UDropdownMenu, {
				content: {
					align: 'end'
				},
				items: [
					[
						{
							type: 'label',
							label: 'Actions'
						}
					],
					[
						{
							label: 'View details',
							icon: 'i-lucide-list',
							onSelect() {
								openConnectorDetails(connector)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedConnectorId.value = connector.id
								selectedConnectorName.value = connector.name
								deleteModal.value?.openModal()
							}
						}
					]
				]
			}, {
				default: () => h(UButton, {
					icon: 'i-lucide-ellipsis-vertical',
					color: 'neutral',
					variant: 'ghost',
					size: 'sm',
					class: 'ml-auto'
				})
			})
		)
	}
]

useHead({
	title: 'Connectors'
})
</script>

<template>
	<UDashboardPanel id="connectors">
		<template #header>
			<UDashboardNavbar title="Connectors">
				<template #leading>
					<UDashboardSidebarCollapse />
				</template>

				<template #right>
					<ConnectorsAddModal @created="handleCreated" />
				</template>
			</UDashboardNavbar>
		</template>

		<template #body>
			<div class="flex w-full items-center gap-1.5">
				<SearchFilter
					v-model="search"
					placeholder="Search..."
					@update:debounced-value="debouncedSearch = $event"
				/>
			</div>

			<PaginatedTable
				:data="rows"
				:column-config="columnConfig"
				:total="total"
				:loading="isLoading"
				:page="page"
				:page-size="pageSize"
				:sort="sort"
				@update:page="page = $event"
				@update:page-size="pageSize = $event"
				@update:sort="sort = $event"
				@select="openConnectorDetails"
			/>

			<DeleteModal
				:id="selectedConnectorId"
				ref="deleteModal"
				:name="selectedConnectorName"
				endpoint="/api/connectors"
				@deleted="handleDeleted"
			/>
			<ConnectorsEditModal
				ref="editModal"
				:connector-id="selectedConnectorId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
