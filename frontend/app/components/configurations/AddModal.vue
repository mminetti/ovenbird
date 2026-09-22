<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Configuration } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const emit = defineEmits<{
	created: [configuration: Configuration]
}>()

const fieldSchema = z.object({
	name: z.string().min(1, 'Field name is required').max(200, 'Field name is too long'),
	value: z.string().optional()
})

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	description: z.string().max(1000, 'Description is too long').optional(),
	configurationTypeId: z.number({ error: 'Type is required' }),
	companyId: z.number().optional(),
	connectorIds: z.array(z.number()),
	fields: z.array(fieldSchema)
})

type Schema = z.output<typeof schema>

const open = ref(false)
const submitting = ref(false)
const loadingLists = ref(false)
const form = ref<{ submit: () => void }>()
const configurationTypes = ref<DataListItem[]>([])
const companies = ref<DataListItem[]>([])
const connectors = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	description: '',
	configurationTypeId: undefined,
	companyId: undefined,
	connectorIds: [],
	fields: []
})

const configurationTypeItems = computed(() =>
	configurationTypes.value.map(type => ({ label: type.name, value: Number(type.id) }))
)

const companyItems = computed(() =>
	companies.value.map(company => ({ label: company.name, value: Number(company.id) }))
)

const connectorItems = computed(() =>
	connectors.value.map(connector => ({ label: connector.name, value: Number(connector.id) }))
)

function resetForm() {
	state.name = ''
	state.description = ''
	state.configurationTypeId = undefined
	state.companyId = undefined
	state.connectorIds = []
	state.fields = []
}

async function loadLookups() {
	loadingLists.value = true

	try {
		const [typesResponse, companiesResponse, connectorsResponse] = await Promise.all([
			$fetch<DataListResponse>('/api/data-lists/ConfigurationTypes'),
			$fetch<DataListResponse>('/api/data-lists/Companies'),
			$fetch<DataListResponse>('/api/data-lists/Connectors')
		])

		configurationTypes.value = typesResponse.items
		companies.value = companiesResponse.items
		connectors.value = connectorsResponse.items
	} catch (error) {
		modalError.value = parseApiError(error)
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
		const created = await $fetch<Configuration>('/api/configurations', {
			method: 'POST',
			body: {
				name: event.data.name,
				description: event.data.description,
				configurationTypeId: event.data.configurationTypeId,
				companyId: event.data.companyId,
				connectorIds: event.data.connectorIds,
				fields: event.data.fields
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
	<USlideover v-model:open="open" title="New configuration" :ui="{ content: 'max-w-2xl' }">
		<UButton label="New configuration" icon="i-lucide-plus" class="ml-auto shrink-0" />

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

				<UFormField label="Description" name="description">
					<UTextarea v-model="state.description" class="w-full" :disabled="submitting" />
				</UFormField>

				<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
					<UFormField label="Type" name="configurationTypeId">
						<USelectMenu
							v-model="state.configurationTypeId"
							:items="configurationTypeItems"
							value-key="value"
							label-key="label"
							placeholder="Select a type"
							class="w-full"
							:disabled="submitting || loadingLists"
						/>
					</UFormField>

					<UFormField label="Company" name="companyId">
						<USelectMenu
							v-model="state.companyId"
							:items="companyItems"
							value-key="value"
							label-key="label"
							placeholder="Select a company"
							class="w-full"
							:disabled="submitting || loadingLists"
						/>
					</UFormField>
				</div>

				<UFormField label="Connectors" name="connectorIds">
					<USelectMenu
						v-model="state.connectorIds"
						:items="connectorItems"
						value-key="value"
						label-key="label"
						multiple
						placeholder="Select connectors"
						class="w-full"
						:disabled="submitting || loadingLists"
					/>
				</UFormField>

				<UFormField label="Fields" name="fields">
					<ConfigurationsFieldsEditor v-model="state.fields" :disabled="submitting" />
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
