<script setup lang="ts">
defineProps<{
	error: { message: string, errors?: Record<string, string[]> } | null
}>()
</script>

<template>
	<div
		v-if="error"
		class="flex gap-3 rounded-lg border border-error/30 bg-error/10 p-3"
	>
		<UIcon name="i-lucide-circle-alert" class="mt-0.5 size-4 shrink-0 text-error" />
		<div class="min-w-0 text-sm">
			<p class="font-medium text-highlighted">
				{{ error.message }}
			</p>
			<ul v-if="error.errors" class="mt-1 space-y-0.5 text-default">
				<li v-for="(messages, field) in error.errors" :key="field">
					<span class="font-medium capitalize">{{ field }}:</span>
					{{ messages.join(' ') }}
				</li>
			</ul>
		</div>
	</div>
</template>
