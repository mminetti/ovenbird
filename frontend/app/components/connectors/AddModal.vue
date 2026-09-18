<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { Connector } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const emit = defineEmits<{
	created: [connector: Connector]
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
const submitting = ref(false)
const loadingLists = ref(false)
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

function resetForm() {
	state.name = ''
	state.description = ''
	state.connectorTypeId = undefined
	state.connectorImplementationId = undefined
	state.fields = []
}

async function loadLookups() {
	loadingLists.value = true

	try {
		const [typesResponse, implementationsResponse] = await Promise.all([
			$fetch<DataListResponse>('/api/data-lists/ConnectorTypes'),
			$fetch<DataListResponse>('/api/data-lists/ConnectorImplementations')
		])

		connectorTypes.value = typesResponse.items
		connectorImplementations.value = implementationsResponse.items
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
		const created = await $fetch<Connector>('/api/connectors', {
			method: 'POST',
			body: {
				name: event.data.name,
				description: event.data.description,
				connectorImplementationId: event.data.connectorImplementationId,
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
	<USlideover v-model:open="open" title="New connector" :ui="{ content: 'max-w-2xl' }">
		<UButton label="New connector" icon="i-lucide-plus" class="ml-auto shrink-0" />

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
					<UFormField label="Type" name="connectorTypeId">
						<USelectMenu
							v-model="state.connectorTypeId"
							:items="connectorTypeItems"
							value-key="value"
							label-key="label"
							placeholder="Select a type"
							class="w-full"
							:disabled="submitting || loadingLists"
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
							:disabled="submitting || loadingLists || state.connectorTypeId === undefined"
						/>
					</UFormField>
				</div>

				<UFormField label="Fields" name="fields">
					<ConnectorsFieldsEditor v-model="state.fields" :disabled="submitting" />
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
