<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Company } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const props = defineProps<{
	companyId: number | null
}>()

const emit = defineEmits<{
	updated: [company: Company]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(250, 'Name is too long'),
	marketId: z.number({ error: 'Market is required' }),
	timeZoneId: z.string().min(1, 'Time zone is required')
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const markets = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	marketId: undefined,
	timeZoneId: undefined
})

const slideoverTitle = computed(() => `Edit company ${state.name?.trim() || ''}`.trim())

const marketItems = computed(() =>
	markets.value.map(market => ({ label: market.name, value: Number(market.id) }))
)

const timeZoneItems = timeZoneOptions()

function resetFormState() {
	state.name = ''
	state.marketId = undefined
	state.timeZoneId = undefined
}

async function loadCompany() {
	if (!props.companyId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const [company, marketsResponse] = await Promise.all([
			$fetch<Company>('/api/companies', { query: { id: props.companyId } }),
			$fetch<DataListResponse>('/api/data-lists/Markets')
		])

		markets.value = marketsResponse.items

		state.name = company.name
		state.marketId = company.marketId
		state.timeZoneId = company.timeZoneId
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadCompany()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.companyId || loading.value) {
		return
	}

	loading.value = true

	try {
		const updated = await $fetch<Company>('/api/companies', {
			method: 'PATCH',
			query: { id: props.companyId },
			body: {
				name: event.data.name,
				marketId: event.data.marketId,
				timeZoneId: event.data.timeZoneId
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

				<UFormField label="Market" name="marketId">
					<USelectMenu
						v-model="state.marketId"
						:items="marketItems"
						value-key="value"
						label-key="label"
						placeholder="Select a market"
						class="w-full"
						:disabled="loading"
					/>
				</UFormField>

				<UFormField label="Time zone" name="timeZoneId">
					<USelectMenu
						v-model="state.timeZoneId"
						:items="timeZoneItems"
						value-key="value"
						label-key="label"
						placeholder="Select a time zone"
						class="w-full"
						:disabled="loading"
					/>
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
