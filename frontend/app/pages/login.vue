<script setup lang="ts">
definePageMeta({ layout: false })

const route = useRoute()
const errorParam = route.query.error

const errorMessage = computed(() => {
	if (!errorParam) return null
	if (errorParam === 'inactive') return 'Your account is pending activation by an administrator.'
	if (errorParam === 'oauth') return 'Sign-in failed. Please try again.'
	if (errorParam === 'backend') return 'We couldn\'t reach the server. Please try again in a moment.'
	return 'An error occurred. Please try again.'
})
</script>

<template>
	<div class="min-h-screen flex items-center justify-center bg-default">
		<UCard class="w-full max-w-sm">
			<template #header>
				<div class="flex flex-col items-center gap-3 py-2">
					<div class="text-center">
						<h1 class="text-xl font-semibold text-highlighted">
							Sign in to Ovenbird
						</h1>
						<p class="text-sm text-muted mt-1">
							Use your organization account to continue
						</p>
					</div>
				</div>
			</template>

			<div class="flex flex-col gap-4">
				<UAlert
					v-if="errorMessage"
					color="error"
					variant="soft"
					:title="errorMessage"
					icon="i-lucide-circle-alert"
				/>

				<UButton
					to="/auth/auth0"
					external
					block
					size="lg"
					color="neutral"
					variant="solid"
					leading-icon="i-lucide-log-in"
					label="Sign in with Auth0"
				/>
			</div>
		</UCard>
	</div>
</template>
