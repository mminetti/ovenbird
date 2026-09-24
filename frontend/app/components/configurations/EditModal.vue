<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Configuration } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const props = defineProps<{
	configurationId: number | null
}>()

const emit = defineEmits<{
	updated: [configuration: Configuration]
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
const loading = ref(true)
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

const lastModifiedAtUtc = ref<string | null>(null)
const lastModifiedBy = ref<string | null>(null)

const { timezone } = useTimezone()

const slideoverTitle = computed(() => `Edit configuration ${state.name?.trim() || ''}`.trim())

const formattedLastModifiedAtUtc = computed(() => {
	if (!lastModifiedAtUtc.value) {
		return ''
	}

	return formatUtcToTimezone(lastModifiedAtUtc.value, timezone.value)
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

function resetFormState() {
	state.name = ''
	state.description = ''
	state.configurationTypeId = undefined
	state.companyId = undefined
	state.connectorIds = []
	state.fields = []
	lastModifiedAtUtc.value = null
	lastModifiedBy.value = null
}

async function loadConfiguration() {
	if (!props.configurationId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const [configuration, typesResponse, companiesResponse, connectorsResponse] = await Promise.all([
			$fetch<Configuration>('/api/configurations', { query: { id: props.configurationId } }),
			$fetch<DataListResponse>('/api/data-lists/ConfigurationTypes'),
			$fetch<DataListResponse>('/api/data-lists/Companies'),
			$fetch<DataListResponse>('/api/data-lists/Connectors')
		])

		configurationTypes.value = typesResponse.items
		companies.value = companiesResponse.items
		connectors.value = connectorsResponse.items

		state.name = configuration.name
		state.description = configuration.description ?? ''
		state.configurationTypeId = configuration.configurationTypeId
		state.companyId = configuration.companyId ?? undefined
		state.connectorIds = configuration.connectors.map(connector => connector.id)
		state.fields = configuration.fields.map(field => ({
			name: field.name,
			value: field.value ?? undefined
		}))
		lastModifiedAtUtc.value = configuration.lastModifiedAtUtc
		lastModifiedBy.value = configuration.lastModifiedBy
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadConfiguration()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.configurationId || loading.value) {
		return
	}

	loading.value = true

	try {
		const updated = await $fetch<Configuration>('/api/configurations', {
			method: 'PATCH',
			query: { id: props.configurationId },
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

				<UFormField label="Description" name="description">
					<UTextarea v-model="state.description" class="w-full" :disabled="loading" />
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
							:disabled="loading"
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
							:disabled="loading"
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
						:disabled="loading"
					/>
				</UFormField>

				<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
					<UFormField label="Updated By">
						<UInput
							:model-value="lastModifiedBy ?? undefined"
							class="w-full"
							:ui="{ base: 'bg-elevated text-muted disabled:opacity-100' }"
							disabled
						/>
					</UFormField>
					<UFormField label="Updated At">
						<UInput
							:model-value="formattedLastModifiedAtUtc"
							class="w-full"
							:ui="{ base: 'bg-elevated text-muted disabled:opacity-100' }"
							disabled
						/>
					</UFormField>
				</div>

				<UFormField label="Fields" name="fields">
					<ConfigurationsFieldsEditor v-model="state.fields" :disabled="loading" />
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
