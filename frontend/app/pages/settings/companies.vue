<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { CompaniesResponse } from '~~/server/api/companies'
import type { Company } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'companies.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'marketName' | 'timeZoneId'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedCompanyId = ref<number | null>(null)
const selectedCompanyName = ref<string | null>(null)

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

const { data, pending, refresh, error } = await useFetch<CompaniesResponse>('/api/companies', {
	query,
	lazy: true,
	server: false
})

useApiErrorToast(error)

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedCompanyId.value = null
	selectedCompanyName.value = null

	const displayName = name || `Company #${id}`

	toast.add({
		title: 'Company deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(company: Company) {
	await refresh()

	toast.add({
		title: 'Company created',
		description: `${company.name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(company: Company) {
	await refresh()

	toast.add({
		title: 'Company updated',
		description: `${company.name} has been updated.`,
		color: 'success'
	})
}

function openCompanyDetails(company: Company) {
	selectedCompanyId.value = company.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<Company, SortColumn>[] = [
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
		key: 'marketName',
		label: 'Market',
		type: 'string'
	},
	{
		key: 'timeZoneId',
		label: 'Time zone',
		type: 'string'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (company: Company) => h(
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
								openCompanyDetails(company)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedCompanyId.value = company.id
								selectedCompanyName.value = company.name
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
	title: 'Companies'
})
</script>

<template>
	<UDashboardPanel id="companies">
		<template #header>
			<UDashboardNavbar title="Companies">
				<template #leading>
					<UDashboardSidebarCollapse />
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
				<CompaniesAddModal @created="handleCreated" />
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
				@select="openCompanyDetails"
			/>

			<DeleteModal
				:id="selectedCompanyId"
				ref="deleteModal"
				:name="selectedCompanyName"
				endpoint="/api/companies"
				@deleted="handleDeleted"
			/>
			<CompaniesEditModal
				ref="editModal"
				:company-id="selectedCompanyId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
