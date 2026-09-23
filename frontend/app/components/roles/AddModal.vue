<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const emit = defineEmits<{
	created: [name: string]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	permissionIds: z.array(z.number())
})

type Schema = z.output<typeof schema>

const open = ref(false)
const submitting = ref(false)
const form = ref<{ submit: () => void }>()
const allPermissions = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	permissionIds: []
})

const slideoverTitle = computed(() => `New role ${state.name?.trim() || ''}`.trim())

const permissionItems = computed(() =>
	allPermissions.value.map(permission => ({ label: permission.name, value: Number(permission.id) }))
)

function resetForm() {
	state.name = ''
	state.permissionIds = []
}

watch(open, async (isOpen) => {
	if (!isOpen) {
		resetForm()
		modalError.value = null
		return
	}

	try {
		const permissionsResponse = await $fetch<DataListResponse>('/api/data-lists/Permissions')
		allPermissions.value = permissionsResponse.items
	} catch (error) {
		modalError.value = parseApiError(error)
	}
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (submitting.value) {
		return
	}

	submitting.value = true

	try {
		await $fetch('/api/security/roles', {
			method: 'POST',
			body: {
				name: event.data.name,
				permissionIds: event.data.permissionIds
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
		<UButton label="New role" icon="i-lucide-plus" class="ml-auto shrink-0" />

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
				<UFormField label="Name" placeholder="" name="name">
					<UInput v-model="state.name" class="w-full" autofocus />
				</UFormField>
				<UFormField label="Permissions" name="permissionIds">
					<USelectMenu
						v-model="state.permissionIds"
						:items="permissionItems"
						value-key="value"
						label-key="label"
						multiple
						placeholder="Select permissions"
						class="w-full"
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
