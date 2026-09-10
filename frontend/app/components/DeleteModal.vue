<script setup lang="ts">
const emit = defineEmits<{
	deleted: [id: number, name?: string | null]
}>()

const open = ref(false)
const submitting = ref(false)

const props = withDefaults(defineProps<{
	endpoint: string
	id?: number | null
	name?: string | null
	title?: string
	description?: string
}>(), {
	id: null,
	name: null,
	title: undefined,
	description: undefined
})

const modalTitle = computed(() => props.title ?? `Delete ${props.name}`)
const modalDescription = computed(() => props.description ?? `Are you sure you want to delete "${props.name}"?`)

async function onSubmit() {
	if (!props.id || submitting.value) {
		return
	}

	submitting.value = true

	try {
		await $fetch(props.endpoint, {
			method: 'DELETE',
			query: {
				id: props.id
			}
		})

		open.value = false
		emit('deleted', props.id, props.name)
	} catch {
		// Error is shown via ApiErrorBanner. Close modal so error is visible.
		open.value = false
	} finally {
		submitting.value = false
	}
}

defineExpose({
	openModal: () => {
		open.value = true
	}
})
</script>

<template>
	<UModal
		v-model:open="open"
		:title="modalTitle"
		:description="modalDescription"
	>
		<slot />

		<template #body>
			<div class="flex justify-end gap-2">
				<UButton
					label="Cancel"
					color="neutral"
					variant="subtle"
					@click="open = false"
				/>
				<UButton
					label="Delete"
					color="error"
					variant="solid"
					:loading="submitting"
					:disabled="!id"
					@click="onSubmit"
				/>
			</div>
		</template>
	</UModal>
</template>
