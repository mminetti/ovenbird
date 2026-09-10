import { h } from 'vue'
import type { TableColumn } from '@nuxt/ui'
import { UBadge, UIcon } from '#components'

export type SortDirection = 'asc' | 'desc'

export interface SortState<TSortColumn extends string> {
	column: TSortColumn
	direction: SortDirection
}

type ValueType = 'string' | 'number' | 'date' | 'bool' | 'badge' | 'action'

interface BaseColumnConfig<T, TSortColumn extends string> {
	key: keyof T & TSortColumn
	label: string
	type: Exclude<ValueType, 'action'>
}

interface ActionColumnConfig<T> {
	key?: string
	label: string
	type: 'action'
	render: (row: T) => unknown
}

interface NumberColumnConfig {
	numberLocale?: string
}

interface DateColumnConfig {
	dateLocale?: string
	dateOptions?: Intl.DateTimeFormatOptions
}

interface BadgeColumnConfig {
	badgeColor?: 'primary' | 'secondary' | 'success' | 'info' | 'warning' | 'error' | 'neutral'
	badgeVariant?: 'solid' | 'outline' | 'soft' | 'subtle'
}

export type TypedColumnConfig<T, TSortColumn extends string>
	= (BaseColumnConfig<T, TSortColumn> & NumberColumnConfig & DateColumnConfig & BadgeColumnConfig) | ActionColumnConfig<T>

export function createTypedColumns<T, TSortColumn extends string>(
	configs: TypedColumnConfig<T, TSortColumn>[],
	sortableHeader?: (label: string, column: TSortColumn) => () => unknown
): TableColumn<T>[] {
	return configs.map((config) => {
		if (config.type === 'action') {
			return {
				id: config.key || 'actions',
				header: () => h('span', { class: 'sr-only' }, config.label),
				cell: ({ row }) => config.render(row.original),
				size: 40
			} as TableColumn<T>
		}

		const column: TableColumn<T> = {
			accessorKey: config.key,
			header: sortableHeader
				? sortableHeader(config.label, config.key)
				: () => h('span', { class: 'text-sm font-medium text-highlighted' }, config.label)
		}

		if (config.type === 'number') {
			column.cell = ({ row }) => {
				const value = Number(row.getValue(config.key))
				return new Intl.NumberFormat(config.numberLocale || 'en-US').format(value)
			}
		}

		if (config.type === 'date') {
			column.cell = ({ row }) => {
				const raw = row.getValue(config.key)
				const date = raw instanceof Date ? raw : new Date(String(raw))

				if (Number.isNaN(date.getTime())) {
					return '-'
				}

				return new Intl.DateTimeFormat(config.dateLocale || 'en-US', config.dateOptions).format(date)
			}
		}

		if (config.type === 'bool') {
			column.cell = ({ row }) => h(UIcon, {
				name: row.getValue(config.key) ? 'i-lucide-check-circle-2' : 'i-lucide-circle',
				class: 'size-4'
			})
		}

		if (config.type === 'badge') {
			column.cell = ({ row }) => h(
				UBadge,
				{
					variant: config.badgeVariant || 'subtle',
					color: config.badgeColor || 'neutral'
				},
				() => String(row.original[config.key] ?? '')
			)
		}

		return column
	})
}
