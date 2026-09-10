<script setup lang="ts">
import { h, resolveComponent } from 'vue'
import type { SecurityUsersResponse } from '~~/server/api/security/users'
import type { SecurityUser } from '~/types'
import type { SortDirection, TypedColumnConfig } from '~/utils/paginatedTableColumns'

const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const deleteModal = useTemplateRef('deleteModal')
const editModal = useTemplateRef('editModal')
const toast = useToast()

type SortColumn = 'id' | 'name' | 'email' | 'isActive'

const page = ref(1)
const pageSize = ref(10)
const sort = ref<{ column: SortColumn, direction: SortDirection }>({ column: 'id', direction: 'asc' })
const search = ref('')
const debouncedSearch = ref('')
const selectedUserId = ref<number | null>(null)
const selectedUserName = ref<string | null>(null)

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

const { data, pending, refresh } = await useFetch<SecurityUsersResponse>('/api/security/users', {
	query,
	lazy: true,
	server: false
})

async function handleDeleted(id: number, name?: string | null) {
	await refresh()

	selectedUserId.value = null
	selectedUserName.value = null

	const displayName = name || `User #${id}`

	toast.add({
		title: 'User deleted',
		description: `${displayName} has been deleted.`,
		color: 'success'
	})
}

async function handleCreated(name: string) {
	await refresh()

	toast.add({
		title: 'User created',
		description: `${name} has been added.`,
		color: 'success'
	})
}

async function handleUpdated(user: SecurityUser) {
	await refresh()

	toast.add({
		title: 'User updated',
		description: `${user.name} has been updated.`,
		color: 'success'
	})
}

function openUserDetails(user: SecurityUser) {
	selectedUserId.value = user.id
	editModal.value?.openModal()
}

const isFetching = ref(true)
watch(pending, (newVal) => {
	isFetching.value = newVal
})

const rows = computed(() => (data.value?.items) || [])
const total = computed(() => data.value?.totalCount || 0)
const isLoading = computed(() => isFetching.value)

const columnConfig: TypedColumnConfig<SecurityUser, SortColumn>[] = [
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
		key: 'email',
		label: 'Email',
		type: 'string'
	},
	{
		key: 'isActive',
		label: 'Active',
		type: 'bool'
	},
	{
		key: 'actions',
		label: 'Actions',
		type: 'action',
		render: (user: SecurityUser) => h(
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
								openUserDetails(user)
							}
						},
						{
							label: 'Delete',
							icon: 'i-lucide-trash',
							color: 'error',
							onSelect() {
								selectedUserId.value = user.id
								selectedUserName.value = user.name
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
	title: 'Users'
})
</script>

<template>
	<UDashboardPanel id="users">
		<template #header>
			<UDashboardNavbar title="Users">
				<template #leading>
					<UDashboardSidebarCollapse />
				</template>

				<template #right>
					<UsersAddModal @created="handleCreated" />
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
				@select="openUserDetails"
			/>

			<DeleteModal
				:id="selectedUserId"
				ref="deleteModal"
				:name="selectedUserName"
				endpoint="/api/security/users"
				@deleted="handleDeleted"
			/>
			<UsersEditModal
				ref="editModal"
				:user-id="selectedUserId"
				@updated="handleUpdated"
			/>
		</template>
	</UDashboardPanel>
</template>
