import type { NavigationMenuItem } from '@nuxt/ui'

export interface NavEntry {
	label: string
	icon: string
	to: string
	permission?: string
}

export interface NavGroup {
	label: string
	icon: string
	items: NavEntry[]
}

export interface NavSection {
	label: string
	icon: string
	groups: NavGroup[]
}

// Each `permission` string mirrors a backend Constants.Permissions entry
// (see backend/src/UseCases/Common/Constants.Permissions.cs). Add one entry
// per group as its page ships — no other wiring needed.
export const NAV_SECTIONS: NavSection[] = [
	{
		label: 'Settings',
		icon: 'i-lucide-settings',
		groups: [
			{
				label: 'Configurations',
				icon: 'i-lucide-settings-2',
				items: [
					{ label: 'Connectors', icon: 'i-lucide-plug', to: '/settings/configurations/connectors', permission: 'connectors.read' },
					{ label: 'Companies', icon: 'i-lucide-building-2', to: '/settings/configurations/companies', permission: 'companies.read' }
				]
			},
			{
				label: 'Security',
				icon: 'i-lucide-shield',
				items: [
					{ label: 'Users', icon: 'i-lucide-user-cog', to: '/settings/security/users', permission: 'users.read' },
					{ label: 'Roles', icon: 'i-lucide-shield-check', to: '/settings/security/roles', permission: 'roles.read' },
					{ label: 'Permissions', icon: 'i-lucide-key', to: '/settings/security/permissions', permission: 'permissions.read' }
				]
			}
		]
	}
]

export function buildSecuredNavItems(
	hasPermission: (permission: string) => boolean,
	onSelect?: () => void
): NavigationMenuItem[] {
	return NAV_SECTIONS
		.map((section) => {
			const groups = section.groups
				.map((group) => {
					const items = group.items.filter(item => !item.permission || hasPermission(item.permission))

					if (items.length === 0) {
						return null
					}

					return {
						label: group.label,
						icon: group.icon,
						defaultOpen: true,
						type: 'trigger' as const,
						children: items.map(item => ({
							label: item.label,
							icon: item.icon,
							to: item.to,
							onSelect
						}))
					}
				})
				.filter((group): group is NonNullable<typeof group> => group !== null)

			if (groups.length === 0) {
				return null
			}

			return {
				label: section.label,
				icon: section.icon,
				defaultOpen: true,
				type: 'trigger' as const,
				children: groups
			}
		})
		.filter((section): section is NonNullable<typeof section> => section !== null)
}
