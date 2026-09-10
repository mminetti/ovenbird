<script setup lang="ts" generic="T, TSortColumn extends string = string">
import { h, resolveComponent } from 'vue'
import type { TableColumn } from '@nuxt/ui'
import { createTypedColumns } from '~/utils/paginatedTableColumns'
import type { SortState, TypedColumnConfig } from '~/utils/paginatedTableColumns'

const props = withDefaults(
	defineProps<{
		data: T[]
		columns?: TableColumn<T>[]
		columnConfig?: TypedColumnConfig<T, TSortColumn>[]
		total: number
		loading?: boolean
		page?: number
		pageSize?: number
		sort?: SortState<TSortColumn>
		rowCountOptions?: Array<{ label: string, value: number }>
	}>(),
	{
		loading: false,
		page: 1,
		pageSize: 20,
		rowCountOptions: () => [10, 20, 30, 50, 100].map(value => ({
			label: String(value),
			value
		}))
	}
)

const emit = defineEmits<{
	'update:page': [value: number]
	'update:pageSize': [value: number]
	'update:sort': [value: SortState<TSortColumn>]
	'select': [row: T]
}>()

const UButton = resolveComponent('UButton')

const currentPage = computed({
	get: () => props.page,
	set: value => emit('update:page', value)
})

const currentPageSize = computed({
	get: () => props.pageSize,
	set: (value) => {
		emit('update:pageSize', value)
		// Reset to first page when page size changes
		emit('update:page', 1)
	}
})

function toggleSort(column: TSortColumn) {
	if (!props.sort) {
		return
	}

	if (props.sort.column === column) {
		emit('update:sort', {
			column,
			direction: props.sort.direction === 'asc' ? 'desc' : 'asc'
		})
		emit('update:page', 1)
		return
	}

	emit('update:sort', {
		column,
		direction: 'asc'
	})
	emit('update:page', 1)
}

function getSortIcon(column: TSortColumn) {
	if (!props.sort || props.sort.column !== column) {
		return 'i-lucide-arrow-up-down'
	}

	return props.sort.direction === 'asc'
		? 'i-lucide-arrow-up-narrow-wide'
		: 'i-lucide-arrow-down-wide-narrow'
}

function sortableHeader(label: string, column: TSortColumn) {
	return () => h(UButton, {
		color: 'neutral',
		variant: 'ghost',
		label,
		icon: getSortIcon(column),
		class: '-mx-2.5',
		onClick: () => toggleSort(column)
	})
}

const resolvedColumns = computed<TableColumn<T>[]>(() => {
	if (props.columns?.length) {
		return props.columns
	}

	if (props.columnConfig?.length) {
		return createTypedColumns(props.columnConfig, props.sort ? sortableHeader : undefined)
	}

	return []
})

function handleRowSelect(_event: Event, row: { original: T }) {
	emit('select', row.original)
}
</script>

<template>
	<div>
		<UTable
			class="shrink-0"
			:data="data"
			:columns="resolvedColumns"
			:loading="loading"
			:on-select="handleRowSelect"
			:ui="{
				base: 'table-auto border-separate border-spacing-0',
				thead: '[&>tr]:after:content-none',
				tbody: '[&>tr]:last:[&>td]:border-b-0',
				tr: 'hover:bg-elevated/30 transition-colors duration-150 data-[selectable=true]:cursor-pointer',
				th: 'bg-elevated/50 px-3 py-1.5 first:rounded-l-lg last:rounded-r-lg border-y border-default first:border-l last:border-r last:w-12',
				td: 'px-3 py-2 border-b border-default last:w-12',
				separator: 'h-0'
			}"
		/>

		<div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 border-t border-default pt-3">
			<div class="hidden sm:flex items-center gap-2">
				<span class="text-sm text-muted">Rows</span>
				<USelect
					v-model="currentPageSize"
					:items="rowCountOptions"
					size="md"
					class="w-20"
				/>
			</div>
			<UPagination
				v-model:page="currentPage"
				:items-per-page="pageSize"
				:total="total"
				:max="5"
			/>
		</div>
	</div>
</template>
