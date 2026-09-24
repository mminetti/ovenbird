<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { SecurityRole } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const props = defineProps<{
	roleId: number | null
}>()

const emit = defineEmits<{
	updated: [role: SecurityRole]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	permissionIds: z.array(z.number())
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const allPermissions = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	permissionIds: []
})

const originalPermissionIds = ref<number[]>([])
const lastModifiedAtUtc = ref<string | null>(null)
const lastModifiedBy = ref<string | null>(null)

const { timezone } = useTimezone()

const slideoverTitle = computed(() => `Edit role ${state.name?.trim() || ''}`.trim())

const formattedLastModifiedAtUtc = computed(() => {
	if (!lastModifiedAtUtc.value) {
		return ''
	}

	return formatUtcToTimezone(lastModifiedAtUtc.value, timezone.value)
})

const permissionItems = computed(() => groupDataListItemsByParent(allPermissions.value))

function resetFormState() {
	state.name = ''
	state.permissionIds = []
	originalPermissionIds.value = []
	lastModifiedAtUtc.value = null
	lastModifiedBy.value = null
}

async function loadRole() {
	if (!props.roleId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const [role, permissionsResponse] = await Promise.all([
			$fetch<SecurityRole>('/api/security/roles', { query: { id: props.roleId } }),
			$fetch<DataListResponse>('/api/data-lists/Permissions')
		])

		allPermissions.value = permissionsResponse.items
		state.name = role.name
		state.permissionIds = role.permissions?.map(permission => permission.id) || []
		originalPermissionIds.value = [...state.permissionIds]
		lastModifiedAtUtc.value = role.lastModifiedAtUtc
		lastModifiedBy.value = role.lastModifiedBy
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	if (isOpen) {
		await loadRole()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.roleId || loading.value) {
		return
	}

	loading.value = true

	try {
		const addedPermissions = event.data.permissionIds
			.filter(id => !originalPermissionIds.value.includes(id))
			.map(permissionId => ({ permissionId, operation: 'add' as const }))
		const removedPermissions = originalPermissionIds.value
			.filter(id => !event.data.permissionIds.includes(id))
			.map(permissionId => ({ permissionId, operation: 'remove' as const }))

		await $fetch('/api/security/roles', {
			method: 'PATCH',
			query: { id: props.roleId },
			body: {
				name: event.data.name,
				permissions: [...addedPermissions, ...removedPermissions]
			}
		})

		const updated = await $fetch<SecurityRole>('/api/security/roles', {
			query: { id: props.roleId }
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
	<USlideover v-model:open="open" :title="slideoverTitle">
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

				<UFormField label="Permissions" name="permissionIds">
					<USelectMenu
						v-model="state.permissionIds"
						:items="permissionItems"
						value-key="value"
						label-key="label"
						multiple
						placeholder="Select permissions"
						class="w-full"
						:disabled="loading"
					/>
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
