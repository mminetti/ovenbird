<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Connector } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const props = defineProps<{
	connectorId: number | null
}>()

const emit = defineEmits<{
	updated: [connector: Connector]
}>()

const fieldSchema = z.object({
	name: z.string().min(1, 'Field name is required').max(200, 'Field name is too long'),
	value: z.string().optional(),
	isSecret: z.boolean()
})

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	description: z.string().max(1000, 'Description is too long').optional(),
	connectorTypeId: z.number({ error: 'Type is required' }),
	connectorImplementationId: z.number({ error: 'Implementation is required' }),
	fields: z.array(fieldSchema)
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const connectorTypes = ref<DataListItem[]>([])
const connectorImplementations = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	description: '',
	connectorTypeId: undefined,
	connectorImplementationId: undefined,
	fields: []
})

const lastModifiedAtUtc = ref<string | null>(null)
const lastModifiedBy = ref<string | null>(null)

const { timezone } = useTimezone()

const slideoverTitle = computed(() => `Edit connector ${state.name?.trim() || ''}`.trim())

const formattedLastModifiedAtUtc = computed(() => {
	if (!lastModifiedAtUtc.value) {
		return ''
	}

	return formatUtcToTimezone(lastModifiedAtUtc.value, timezone.value)
})

const connectorTypeItems = computed(() =>
	connectorTypes.value.map(type => ({ label: type.name, value: Number(type.id) }))
)

const connectorImplementationItems = computed(() =>
	connectorImplementations.value
		.filter(implementation => implementation.parentId === String(state.connectorTypeId))
		.map(implementation => ({ label: implementation.name, value: Number(implementation.id) }))
)

watch(() => state.connectorTypeId, () => {
	const validIds = new Set(connectorImplementationItems.value.map(item => item.value))
	if (state.connectorImplementationId !== undefined && !validIds.has(state.connectorImplementationId)) {
		state.connectorImplementationId = undefined
	}
})

function resetFormState() {
	state.name = ''
	state.description = ''
	state.connectorTypeId = undefined
	state.connectorImplementationId = undefined
	state.fields = []
	lastModifiedAtUtc.value = null
	lastModifiedBy.value = null
}

async function loadConnector() {
	if (!props.connectorId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const [connector, typesResponse, implementationsResponse] = await Promise.all([
			$fetch<Connector>('/api/connectors', { query: { id: props.connectorId } }),
			$fetch<DataListResponse>('/api/data-lists/ConnectorTypes'),
			$fetch<DataListResponse>('/api/data-lists/ConnectorImplementations')
		])

		connectorTypes.value = typesResponse.items
		connectorImplementations.value = implementationsResponse.items

		state.name = connector.name
		state.description = connector.description ?? ''
		state.connectorTypeId = connector.connectorTypeId
		state.connectorImplementationId = connector.connectorImplementationId
		state.fields = connector.fields.map(field => ({
			name: field.name,
			value: field.value ?? undefined,
			isSecret: field.isSecret
		}))
		lastModifiedAtUtc.value = connector.lastModifiedAtUtc
		lastModifiedBy.value = connector.lastModifiedBy
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadConnector()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.connectorId || loading.value) {
		return
	}

	loading.value = true

	try {
		const updated = await $fetch<Connector>('/api/connectors', {
			method: 'PATCH',
			query: { id: props.connectorId },
			body: {
				name: event.data.name,
				description: event.data.description,
				connectorImplementationId: event.data.connectorImplementationId,
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
					<UFormField label="Type" name="connectorTypeId">
						<USelectMenu
							v-model="state.connectorTypeId"
							:items="connectorTypeItems"
							value-key="value"
							label-key="label"
							placeholder="Select a type"
							class="w-full"
							:disabled="loading"
						/>
					</UFormField>

					<UFormField label="Implementation" name="connectorImplementationId">
						<USelectMenu
							v-model="state.connectorImplementationId"
							:items="connectorImplementationItems"
							value-key="value"
							label-key="label"
							placeholder="Select an implementation"
							class="w-full"
							:disabled="loading || state.connectorTypeId === undefined"
						/>
					</UFormField>
				</div>

				<UFormField label="Fields" name="fields">
					<ConnectorsFieldsEditor v-model="state.fields" :disabled="loading" />
				</UFormField>

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
