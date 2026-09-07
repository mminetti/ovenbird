// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
	modules: [
		'nuxt-auth-utils',
		'@nuxt/eslint',
		'@nuxt/ui',
		'@vueuse/nuxt'
	],

	devtools: {
		enabled: true
	},

	css: ['~/assets/css/main.css'],

	runtimeConfig: {
		public: {
			backendUrl: 'https://localhost:57679'
		}
	},

	routeRules: {
		'/api/**': {
			cors: true
		}
	},

	compatibilityDate: '2026-06-30',

	eslint: {
		config: {
			stylistic: {
				indent: 'tab',
				commaDangle: 'never',
				braceStyle: '1tbs'
			}
		}
	}
})
