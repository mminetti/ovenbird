<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { SecurityRolesResponse } from '~~/server/api/security/roles'
import type { SecurityRole } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

definePageMeta({
	permission: 'roles.read'
})

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedRoleId = ref<number | null>(null)
const selectedRoleName = ref<string | null>(null)

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

const { data, pending, refresh } = await useFetch<SecurityRolesResponse>('/api/security/roles', {
	query,
	lazy: true,
	server: false
})

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedRoleId.value = null
	selectedRoleName.value = null

	const displayName = name || `Role #${id}`

	toast.add({
		title: 'Role deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(name: string) {
	await refresh()

	toast.add({
		title: 'Role created',
		description: `${name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(role: SecurityRole) {
	await refresh()

	toast.add({
		title: 'Role updated',
		description: `${role.name} has been updated.`,
		color: 'success'
	})
}

function openRoleDetails(role: SecurityRole) {
	selectedRoleId.value = role.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<SecurityRole, SortColumn>[] = [
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
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (role: SecurityRole) => h(
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
								openRoleDetails(role)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedRoleId.value = role.id
								selectedRoleName.value = role.name
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
	title: 'Roles'
})
</script>

<template>
	<UDashboardPanel id="roles">
		<template #header>
			<UDashboardNavbar title="Roles">
				<template #leading>
					<UDashboardSidebarCollapse />
				</template>

				<template #right>
					<RolesAddModal @created="handleCreated" />
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
				@select="openRoleDetails"
			/>

			<DeleteModal
				:id="selectedRoleId"
				ref="deleteModal"
				:name="selectedRoleName"
				endpoint="/api/security/roles"
				@deleted="handleDeleted"
			/>
			<RolesEditModal
				ref="editModal"
				:role-id="selectedRoleId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
