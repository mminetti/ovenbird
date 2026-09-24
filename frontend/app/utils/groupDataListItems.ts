import type { DataListItem } from '~~/server/api/data-lists/[type]'

export interface GroupedSelectItem {
	label: string
	value: number
}

/** Groups data-list items by their parentId (e.g. a permission's Module) into USelectMenu's nested-array group format. */
export function groupDataListItemsByParent(items: DataListItem[]) {
	const groups = new Map<string, GroupedSelectItem[]>()

	for (const item of items) {
		const group = item.parentId || 'Other'

		if (!groups.has(group)) {
			groups.set(group, [])
		}

		groups.get(group)!.push({ label: item.name, value: Number(item.id) })
	}

	return Array.from(groups.entries())
		.sort(([a], [b]) => a.localeCompare(b))
		.map(([group, groupItems]) => [
			{ type: 'label' as const, label: group },
			...groupItems
		])
}
