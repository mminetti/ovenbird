<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Company } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const emit = defineEmits<{
	created: [company: Company]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(250, 'Name is too long'),
	marketId: z.number({ error: 'Market is required' }),
	timeZoneId: z.string().min(1, 'Time zone is required')
})

type Schema = z.output<typeof schema>

const open = ref(false)
const submitting = ref(false)
const loadingLists = ref(false)
const form = ref<{ submit: () => void }>()
const markets = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	marketId: undefined,
	timeZoneId: undefined
})

const marketItems = computed(() =>
	markets.value.map(market => ({ label: market.name, value: Number(market.id) }))
)

const timeZoneItems = timeZoneOptions()

function resetForm() {
	state.name = ''
	state.marketId = undefined
	state.timeZoneId = undefined
}

async function loadLookups() {
	loadingLists.value = true

	try {
		const marketsResponse = await $fetch<DataListResponse>('/api/data-lists/Markets')
		markets.value = marketsResponse.items
	} finally {
		loadingLists.value = false
	}
}

watch(open, (isOpen) => {
	if (isOpen) {
		loadLookups()
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
		const created = await $fetch<Company>('/api/companies', {
			method: 'POST',
			body: {
				name: event.data.name,
				marketId: event.data.marketId,
				timeZoneId: event.data.timeZoneId
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
	<USlideover v-model:open="open" title="New company" :ui="{ content: 'max-w-2xl' }">
		<UButton label="New company" icon="i-lucide-plus" class="ml-auto shrink-0" />

		<template #body>
			<ModalLoadingBar :loading="submitting || loadingLists" />

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

				<UFormField label="Market" name="marketId">
					<USelectMenu
						v-model="state.marketId"
						:items="marketItems"
						value-key="value"
						label-key="label"
						placeholder="Select a market"
						class="w-full"
						:disabled="submitting || loadingLists"
					/>
				</UFormField>

				<UFormField label="Time zone" name="timeZoneId">
					<USelectMenu
						v-model="state.timeZoneId"
						:items="timeZoneItems"
						placeholder="Select a time zone"
						class="w-full"
						:disabled="submitting"
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
