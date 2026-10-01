<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { SecurityUser } from '~/types'
import type { DataListItem, DataListResponse } from '~~/server/api/data-lists/[type]'

const props = defineProps<{
	userId: number | null
}>()

const emit = defineEmits<{
	updated: [user: SecurityUser]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	email: z.string().min(1, 'Email is required').email('A valid email address is required').max(200, 'Email is too long'),
	isActive: z.boolean(),
	roleIds: z.array(z.number())
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const viewMode = ref<'form' | 'audit'>('form')
const form = ref<{ submit: () => void }>()
const allRoles = ref<DataListItem[]>([])
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	email: '',
	isActive: true,
	roleIds: []
})

const originalRoleIds = ref<number[]>([])
const externalIdentifier = ref('')
const lastModifiedAtUtc = ref<string | null>(null)
const lastModifiedBy = ref<string | null>(null)

const { timezone } = useTimezone()

const slideoverTitle = computed(() => `Edit user ${state.name?.trim() || ''}`.trim())

const formattedLastModifiedAtUtc = computed(() => {
	if (!lastModifiedAtUtc.value) {
		return ''
	}

	return formatUtcToTimezone(lastModifiedAtUtc.value, timezone.value)
})

const roleItems = computed(() =>
	allRoles.value.map(role => ({ label: role.name, value: Number(role.id) }))
)

function resetFormState() {
	state.name = ''
	state.email = ''
	state.isActive = true
	state.roleIds = []
	originalRoleIds.value = []
	externalIdentifier.value = ''
	lastModifiedAtUtc.value = null
	lastModifiedBy.value = null
}

async function loadUser() {
	if (!props.userId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const [loadedUser, rolesResponse] = await Promise.all([
			$fetch<SecurityUser>('/api/security/users', { query: { id: props.userId } }),
			$fetch<DataListResponse>('/api/data-lists/Roles')
		])

		allRoles.value = rolesResponse.items
		state.name = loadedUser.name
		state.email = loadedUser.email
		state.isActive = loadedUser.isActive
		state.roleIds = loadedUser.roles?.map(role => role.id) || []
		originalRoleIds.value = [...state.roleIds]
		externalIdentifier.value = loadedUser.externalIdentifier
		lastModifiedAtUtc.value = loadedUser.lastModifiedAtUtc
		lastModifiedBy.value = loadedUser.lastModifiedBy
	} catch (error) {
		modalError.value = parseApiError(error)
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
	viewMode.value = 'form'

	if (isOpen) {
		await loadUser()
		return
	}

	modalError.value = null
	resetFormState()
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
	if (!props.userId || loading.value) {
		return
	}

	loading.value = true

	try {
		const addedRoles = event.data.roleIds
			.filter(id => !originalRoleIds.value.includes(id))
			.map(roleId => ({ roleId, operation: 'add' as const }))
		const removedRoles = originalRoleIds.value
			.filter(id => !event.data.roleIds.includes(id))
			.map(roleId => ({ roleId, operation: 'remove' as const }))

		await $fetch('/api/security/users', {
			method: 'PATCH',
			query: { id: props.userId },
			body: {
				name: event.data.name,
				email: event.data.email,
				isActive: event.data.isActive,
				roles: [...addedRoles, ...removedRoles]
			}
		})

		const updated = await $fetch<SecurityUser>('/api/security/users', {
			query: { id: props.userId }
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
					entity-type="User"
					:entity-id="userId"
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
							<UInput v-model="state.name" class="w-full" :disabled="loading" />
						</UFormField>
						<UFormField label="Email" name="email">
							<UInput
								v-model="state.email"
								type="email"
								class="w-full"
								:disabled="loading"
							/>
						</UFormField>
					</div>

					<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
						<UFormField label="External Identifier">
							<UInput
								:model-value="externalIdentifier"
								class="w-full"
								:ui="{ base: 'bg-elevated text-muted disabled:opacity-100' }"
								disabled
							/>
						</UFormField>
						<UFormField label="Active" name="isActive">
							<USwitch v-model="state.isActive" :disabled="loading" />
						</UFormField>
					</div>

					<UFormField label="Roles" name="roleIds">
						<USelectMenu
							v-model="state.roleIds"
							:items="roleItems"
							value-key="value"
							label-key="label"
							multiple
							placeholder="Select roles"
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
