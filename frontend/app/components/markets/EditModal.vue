<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Market } from '~/types'

const props = defineProps<{
	marketId: number | null
}>()

const emit = defineEmits<{
	updated: [market: Market]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(250, 'Name is too long'),
	identifier: z.string().min(1, 'Identifier is required').max(250, 'Identifier is too long')
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	identifier: ''
})

const slideoverTitle = computed(() => `Edit market ${state.name?.trim() || ''}`.trim())

function resetFormState() {
	state.name = ''
	state.identifier = ''
}

async function loadMarket() {
	if (!props.marketId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const market = await $fetch<Market>('/api/markets', { query: { id: props.marketId } })

		state.name = market.name
		state.identifier = market.identifier
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadMarket()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.marketId || loading.value) {
		return
	}

	loading.value = true

	try {
		const updated = await $fetch<Market>('/api/markets', {
			method: 'PATCH',
			query: { id: props.marketId },
			body: {
				name: event.data.name,
				identifier: event.data.identifier
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
	<USlideover v-model:open="open" :title="slideoverTitle" :ui="{ content: 'max-w-2xl' }">
		<template #body>
			<ModalLoadingBar :loading="loading" />

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
					<UInput v-model="state.name" class="w-full" :disabled="loading" />
				</UFormField>

				<UFormField label="Identifier" name="identifier">
					<UInput v-model="state.identifier" class="w-full" :disabled="loading" />
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
