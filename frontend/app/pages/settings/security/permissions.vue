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
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'description'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedPermissionId = ref<number | null>(null)
const selectedPermissionName = ref<string | null>(null)

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

const { data, pending, refresh } = await useFetch<SecurityPermissionsResponse>('/api/security/permissions', {
	query,
	lazy: true,
	server: false
})

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedPermissionId.value = null
	selectedPermissionName.value = null

	const displayName = name || `Permission #${id}`

	toast.add({
		title: 'Permission deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(name: string) {
	await refresh()

	toast.add({
		title: 'Permission created',
		description: `${name} has been added.`,
		color: 'success'
	})
}

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
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedPermissionId.value = permission.id
								selectedPermissionName.value = permission.name
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

				<template #right>
					<PermissionsAddModal @created="handleCreated" />
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

			<DeleteModal
				:id="selectedPermissionId"
				ref="deleteModal"
				:name="selectedPermissionName"
				endpoint="/api/security/permissions"
				@deleted="handleDeleted"
			/>
			<PermissionsEditModal
				ref="editModal"
				:permission-id="selectedPermissionId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
