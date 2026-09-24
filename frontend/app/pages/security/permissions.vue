<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { SecurityPermissionsResponse } from '~~/server/api/security/permissions'
import type { SecurityPermission } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'permissions.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'moduleName' | 'description'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedPermissionId = ref<number | null>(null)

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

const { data, pending, refresh, error } = await useFetch<SecurityPermissionsResponse>('/api/security/permissions', {
	query,
	lazy: true,
	server: false
})

useApiErrorToast(error)

async function handleUpdated(permission: SecurityPermission) {
	await refresh()

	toast.add({
		title: 'Permission updated',
		description: `${permission.name} has been updated.`,
		color: 'success'
	})
}

function openPermissionDetails(permission: SecurityPermission) {
	selectedPermissionId.value = permission.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<SecurityPermission, SortColumn>[] = [
	{
		key: 'id',
		label: 'Id',
		type: 'number'
	},
	{
		key: 'moduleName',
		label: 'Module',
		type: 'string'
	},
	{
		key: 'name',
		label: 'Name',
		type: 'string'
	},
	{
		key: 'description',
		label: 'Description',
		type: 'string'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (permission: SecurityPermission) => h(
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
								openPermissionDetails(permission)
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
	title: 'Permissions'
})
</script>

<template>
	<UDashboardPanel id="permissions">
		<template #header>
			<UDashboardNavbar title="Permissions">
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
				@select="openPermissionDetails"
			/>

			<PermissionsEditModal
				ref="editModal"
				:permission-id="selectedPermissionId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
