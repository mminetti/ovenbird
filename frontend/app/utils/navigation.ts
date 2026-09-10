import type { NavigationMenuItem } from '@nuxt/ui'

export interface NavEntry {
	label: string
	icon: string
	to: string
	permission?: string
}

export interface NavSection {
	label: string
	icon: string
	items: NavEntry[]
}

// Each `permission` string mirrors a backend Constants.Permissions entry
// (see backend/src/UseCases/Common/Constants.Permissions.cs). Add one entry
// per section as its page ships — no other wiring needed.
export const NAV_SECTIONS: NavSection[] = [
	{
		label: 'Security',
		icon: 'i-lucide-shield',
		items: [
			{ label: 'Users', icon: 'i-lucide-user-cog', to: '/security/users', permission: 'users.read' }
			// { label: 'Roles', icon: 'i-lucide-shield-check', to: '/security/roles', permission: 'roles.read' },
			// { label: 'Permissions', icon: 'i-lucide-key', to: '/security/permissions', permission: 'permissions.read' },
			// { label: 'Modules', icon: 'i-lucide-blocks', to: '/security/modules', permission: 'modules.read' }
		]
	}
]

export function buildSecuredNavItems(
	hasPermission: (permission: string) => boolean,
	onSelect?: () => void
): NavigationMenuItem[] {
	return NAV_SECTIONS
		.map((section) => {
			const items = section.items.filter(item => !item.permission || hasPermission(item.permission))

			if (items.length === 0) {
				return null
			}

			return {
				label: section.label,
				icon: section.icon,
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
		.filter((section): section is NonNullable<typeof section> => section !== null)
}
