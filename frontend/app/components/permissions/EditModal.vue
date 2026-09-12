<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { SecurityPermission } from '~/types'

const props = defineProps<{
	permissionId: number | null
}>()

const emit = defineEmits<{
	updated: [permission: SecurityPermission]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	description: z.string().max(500, 'Description is too long').optional()
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	description: ''
})

const slideoverTitle = computed(() => `Edit permission ${state.name?.trim() || ''}`.trim())

function resetFormState() {
	state.name = ''
	state.description = ''
}

async function loadPermission() {
	if (!props.permissionId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const permission = await $fetch<SecurityPermission>('/api/security/permissions', {
			query: { id: props.permissionId }
		})

		state.name = permission.name
		state.description = permission.description
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadPermission()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.permissionId || loading.value) {
		return
	}

	loading.value = true

	try {
		const updated = await $fetch<SecurityPermission>('/api/security/permissions', {
			method: 'PATCH',
			query: { id: props.permissionId },
			body: {
				name: event.data.name,
				description: event.data.description || ''
			}
		})

		open.value = false
		emit('updated', updated)
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

defineExpose({
	openModal: () => {
		open.value = true
	}
})
</script>

<template>
	<USlideover v-model:open="open" :title="slideoverTitle">
		<template #body>
			<ModalLoadingBar :loading="loading" />

			<ModalApiError :error="modalError" class="mb-4" />
			<UForm
				ref="form"
				:schema="schema"
				:state="state"
				class="space-y-4"
				@submit="onSubmit"
			>
				<UFormField label="Name" name="name">
					<UInput v-model="state.name" class="w-full" :disabled="loading" />
				</UFormField>
				<UFormField label="Description" name="description">
					<UTextarea v-model="state.description" class="w-full" :disabled="loading" />
				</UFormField>
			</UForm>
		</template>

		<template #footer>
			<div class="flex justify-end gap-2 w-full">
				<UButton
					label="Cancel"
					color="neutral"
					variant="subtle"
					:disabled="loading"
					@click="open = false"
				/>
				<UButton
					label="Save"
					color="primary"
					variant="solid"
					:loading="loading"
					:disabled="loading"
					@click="form?.submit()"
				/>
			</div>
		</template>
	</USlideover>
</template>
