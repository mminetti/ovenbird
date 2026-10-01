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
const viewMode = ref<'form' | 'audit'>('form')
const form = ref<{ submit: () => void }>()
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	description: ''
})

const moduleId = ref<number | null>(null)
const moduleName = ref<string | null>(null)

const slideoverTitle = computed(() => `Edit permission ${state.name?.trim() || ''}`.trim())

function resetFormState() {
	state.name = ''
	state.description = ''
	moduleId.value = null
	moduleName.value = null
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
		moduleId.value = permission.moduleId
		moduleName.value = permission.moduleName
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	viewMode.value = 'form'

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
				moduleId: moduleId.value,
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
	<USlideover v-model:open="open" :title="slideoverTitle" :ui="{ content: 'max-w-2xl' }">
		<template #body>
			<ModalLoadingBar :loading="loading" />

			<ModalApiError :error="modalError" class="mb-4" />
			<Transition
				enter-active-class="transition duration-150 ease-out"
				enter-from-class="opacity-0"
				enter-to-class="opacity-100"
				leave-active-class="transition duration-100 ease-in"
				leave-from-class="opacity-100"
				leave-to-class="opacity-0"
				mode="out-in"
			>
				<AuditTrailPanel
					v-if="viewMode === 'audit'"
					entity-type="Permission"
					:entity-id="permissionId"
				/>
				<UForm
					v-else
					ref="form"
					:key="String(open)"
					:schema="schema"
					:validate-on="[]"
					:state="state"
					class="space-y-4"
					@submit="onSubmit"
				>
					<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
						<UFormField label="Name" name="name">
							<UInput
								v-model="state.name"
								class="w-full"
								:ui="{ base: 'bg-elevated text-muted disabled:opacity-100' }"
								disabled
							/>
						</UFormField>
						<UFormField label="Module">
							<UInput
								:model-value="moduleName ?? undefined"
								class="w-full"
								:ui="{ base: 'bg-elevated text-muted disabled:opacity-100' }"
								disabled
							/>
						</UFormField>
					</div>
					<UFormField label="Description" name="description">
						<UTextarea v-model="state.description" class="w-full" :disabled="loading" />
					</UFormField>
				</UForm>
			</Transition>
		</template>

		<template #footer>
			<div class="flex justify-end gap-2 w-full">
				<UButton
					v-if="viewMode === 'form'"
					label="Audit"
					icon="i-lucide-history"
					color="neutral"
					variant="ghost"
					:disabled="loading"
					@click="viewMode = 'audit'"
				/>
				<UButton
					v-if="viewMode === 'form'"
					label="Cancel"
					color="neutral"
					variant="subtle"
					:disabled="loading"
					@click="open = false"
				/>
				<UButton
					v-if="viewMode === 'form'"
					label="Save"
					color="primary"
					variant="solid"
					:loading="loading"
					:disabled="loading"
					@click="form?.submit()"
				/>
				<UButton
					v-if="viewMode === 'audit'"
					label="Back to edit"
					icon="i-lucide-arrow-left"
					color="neutral"
					variant="subtle"
					@click="viewMode = 'form'"
				/>
			</div>
		</template>
	</USlideover>
</template>
