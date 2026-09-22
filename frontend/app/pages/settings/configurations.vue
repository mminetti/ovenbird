<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { ConfigurationsResponse } from '~~/server/api/configurations'
import type { Configuration } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'configurations.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'configurationTypeName' | 'companyName'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedConfigurationId = ref<number | null>(null)
const selectedConfigurationName = ref<string | null>(null)

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

const { data, pending, refresh, error } = await useFetch<ConfigurationsResponse>('/api/configurations', {
	query,
	lazy: true,
	server: false
})

useApiErrorToast(error)

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedConfigurationId.value = null
	selectedConfigurationName.value = null

	const displayName = name || `Configuration #${id}`

	toast.add({
		title: 'Configuration deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(configuration: Configuration) {
	await refresh()

	toast.add({
		title: 'Configuration created',
		description: `${configuration.name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(configuration: Configuration) {
	await refresh()

	toast.add({
		title: 'Configuration updated',
		description: `${configuration.name} has been updated.`,
		color: 'success'
	})
}

function openConfigurationDetails(configuration: Configuration) {
	selectedConfigurationId.value = configuration.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<Configuration, SortColumn>[] = [
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
		key: 'configurationTypeName',
		label: 'Type',
		type: 'string'
	},
	{
		key: 'companyName',
		label: 'Company',
		type: 'string'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (configuration: Configuration) => h(
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
								openConfigurationDetails(configuration)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedConfigurationId.value = configuration.id
								selectedConfigurationName.value = configuration.name
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
	title: 'Configurations'
})
</script>

<template>
	<UDashboardPanel id="configurations">
		<template #header>
			<UDashboardNavbar title="Configurations">
				<template #leading>
					<UDashboardSidebarCollapse />
				</template>

				<template #right>
					<ConfigurationsAddModal @created="handleCreated" />
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
				@select="openConfigurationDetails"
			/>

			<DeleteModal
				:id="selectedConfigurationId"
				ref="deleteModal"
				:name="selectedConfigurationName"
				endpoint="/api/configurations"
				@deleted="handleDeleted"
			/>
			<ConfigurationsEditModal
				ref="editModal"
				:configuration-id="selectedConfigurationId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
