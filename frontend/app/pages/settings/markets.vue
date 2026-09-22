<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { MarketsResponse } from '~~/server/api/markets'
import type { Market } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'markets.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'identifier'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedMarketId = ref<number | null>(null)
const selectedMarketName = ref<string | null>(null)

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

const { data, pending, refresh, error } = await useFetch<MarketsResponse>('/api/markets', {
	query,
	lazy: true,
	server: false
})

useApiErrorToast(error)

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedMarketId.value = null
	selectedMarketName.value = null

	const displayName = name || `Market #${id}`

	toast.add({
		title: 'Market deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(market: Market) {
	await refresh()

	toast.add({
		title: 'Market created',
		description: `${market.name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(market: Market) {
	await refresh()

	toast.add({
		title: 'Market updated',
		description: `${market.name} has been updated.`,
		color: 'success'
	})
}

function openMarketDetails(market: Market) {
	selectedMarketId.value = market.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<Market, SortColumn>[] = [
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
		key: 'identifier',
		label: 'Identifier',
		type: 'string'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (market: Market) => h(
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
								openMarketDetails(market)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedMarketId.value = market.id
								selectedMarketName.value = market.name
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
	title: 'Markets'
})
</script>

<template>
	<UDashboardPanel id="markets">
		<template #header>
			<UDashboardNavbar title="Markets">
				<template #leading>
					<UDashboardSidebarCollapse />
				</template>

				<template #right>
					<MarketsAddModal @created="handleCreated" />
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
				@select="openMarketDetails"
			/>

			<DeleteModal
				:id="selectedMarketId"
				ref="deleteModal"
				:name="selectedMarketName"
				endpoint="/api/markets"
				@deleted="handleDeleted"
			/>
			<MarketsEditModal
				ref="editModal"
				:market-id="selectedMarketId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
