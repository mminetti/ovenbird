<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Market } from '~/types'

const emit = defineEmits<{
	created: [market: Market]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(250, 'Name is too long'),
	identifier: z.string().min(1, 'Identifier is required').max(250, 'Identifier is too long')
})

type Schema = z.output<typeof schema>

const open = ref(false)
const submitting = ref(false)
const form = ref<{ submit: () => void }>()
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	identifier: ''
})

function resetForm() {
	state.name = ''
	state.identifier = ''
}

watch(open, (isOpen) => {
	if (isOpen) {
		return
	}

	resetForm()
	modalError.value = null
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (submitting.value) {
		return
	}

	submitting.value = true

	try {
		const created = await $fetch<Market>('/api/markets', {
			method: 'POST',
			body: {
				name: event.data.name,
				identifier: event.data.identifier
			}
		})

		open.value = false
		emit('created', created)
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		submitting.value = false
	}
}
</script>

<template>
	<USlideover v-model:open="open" title="New market">
		<UButton label="New market" icon="i-lucide-plus" class="ml-auto shrink-0" />

		<template #body>
			<ModalLoadingBar :loading="submitting" />

			<ModalApiError :error="modalError" class="mb-4" />
			<UForm
				ref="form"
				:key="String(open)"
				:schema="schema"
				:validate-on="[]"
				:state="state"
				class="space-y-4"
				@submit="onSubmit"
			>
				<UFormField label="Name" name="name">
					<UInput
						v-model="state.name"
						class="w-full"
						:disabled="submitting"
						autofocus
					/>
				</UFormField>

				<UFormField label="Identifier" name="identifier">
					<UInput v-model="state.identifier" class="w-full" :disabled="submitting" />
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
