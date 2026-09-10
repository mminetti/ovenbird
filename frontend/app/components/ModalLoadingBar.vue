<script setup lang="ts">
defineProps<{
	loading: boolean
}>()
</script>

<template>
	<div class="-mt-6 -mr-6 -ml-6 mb-6 h-px relative" aria-hidden="true">
		<UProgress
			size="2xs"
			class="absolute inset-0 transition-opacity"
			:class="loading ? 'opacity-100' : 'opacity-0 pointer-events-none'"
		/>
	</div>
</template>
