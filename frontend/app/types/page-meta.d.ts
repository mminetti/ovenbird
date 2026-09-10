// Augments Nuxt's page meta so `definePageMeta({ permission: '...' })` type-checks.
// Read by app/middleware/permission.global.ts.
declare module '#app' {
	interface PageMeta {
		permission?: string
	}
}

export {}
