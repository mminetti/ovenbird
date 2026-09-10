<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'

const emit = defineEmits<{
	created: [name: string]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	email: z.string().min(1, 'Email is required').email('A valid email address is required').max(200, 'Email is too long'),
	externalIdentifier: z.string().min(1, 'External identifier is required').max(200, 'External identifier is too long')
})

type Schema = z.output<typeof schema>

const open = ref(false)
const submitting = ref(false)
const form = ref<{ submit: () => void }>()
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	email: '',
	externalIdentifier: ''
})

const slideoverTitle = computed(() => `New user ${state.name?.trim() || ''}`.trim())

function resetForm() {
	state.name = ''
	state.email = ''
	state.externalIdentifier = ''
}

watch(open, (isOpen) => {
	if (!isOpen) {
		resetForm()
		modalError.value = null
	}
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (submitting.value) {
		return
	}

	submitting.value = true

	try {
		await $fetch('/api/security/users', {
			method: 'POST',
			body: {
				name: event.data.name,
				email: event.data.email,
				externalIdentifier: event.data.externalIdentifier
			}
		})

		open.value = false
		emit('created', event.data.name)
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		submitting.value = false
	}
}
</script>

<template>
	<USlideover v-model:open="open" :title="slideoverTitle">
		<UButton label="New user" icon="i-lucide-plus" class="ml-auto shrink-0" />

		<template #body>
			<ModalLoadingBar :loading="submitting" />

			<ModalApiError :error="modalError" class="mb-4" />
			<UForm
				ref="form"
				:schema="schema"
				:state="state"
				class="space-y-4"
				@submit="onSubmit"
			>
				<UFormField label="Name" placeholder="" name="name">
					<UInput v-model="state.name" class="w-full" autofocus />
				</UFormField>
				<UFormField label="Email" placeholder="" name="email">
					<UInput v-model="state.email" type="email" class="w-full" />
				</UFormField>
				<UFormField label="External Identifier" placeholder="" name="externalIdentifier">
					<UInput v-model="state.externalIdentifier" class="w-full" />
				</UFormField>
			</UForm>
		</template>

		<template #footer>
			<div class="flex justify-end gap-2 w-full">
				<UButton
					label="Cancel"
					color="neutral"
					variant="subtle"
					:disabled="submitting"
					@click="open = false"
				/>
				<UButton
					label="Create"
					color="primary"
					variant="solid"
					:loading="submitting"
					:disabled="submitting"
					@click="form?.submit()"
				/>
			</div>
		</template>
	</USlideover>
</template>
