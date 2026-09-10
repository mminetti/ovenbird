<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import type { SecurityUser } from '~/types'

const props = defineProps<{
	userId: number | null
}>()

const emit = defineEmits<{
	updated: [user: SecurityUser]
}>()

const schema = z.object({
	name: z.string().min(1, 'Name is required').max(200, 'Name is too long'),
	email: z.string().min(1, 'Email is required').email('A valid email address is required').max(200, 'Email is too long'),
	isActive: z.boolean()
})

type Schema = z.output<typeof schema>

const open = ref(false)
const loading = ref(true)
const form = ref<{ submit: () => void }>()
const user = ref<SecurityUser | null>(null)
const modalError = ref<ReturnType<typeof parseApiError> | null>(null)

const state = reactive<Partial<Schema>>({
	name: '',
	email: '',
	isActive: true
})

const slideoverTitle = computed(() => `Edit user ${state.name?.trim() || ''}`.trim())

function resetFormState() {
	user.value = null
	state.name = ''
	state.email = ''
	state.isActive = true
}

async function loadUser() {
	if (!props.userId) {
		resetFormState()
		return
	}

	resetFormState()
	loading.value = true

	try {
		const loadedUser = await $fetch<SecurityUser>('/api/security/users', {
			query: { id: props.userId }
		})

		user.value = loadedUser
		state.name = loadedUser.name
		state.email = loadedUser.email
		state.isActive = loadedUser.isActive
	} finally {
		loading.value = false
	}
}

watch(open, async (isOpen) => {
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
		const updated = await $fetch<SecurityUser>('/api/security/users', {
			method: 'PATCH',
			query: { id: props.userId },
			body: {
				name: event.data.name,
				email: event.data.email,
				isActive: event.data.isActive
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
	<USlideover v-model:open="open" :title="slideoverTitle">
		<template #body>
			<ModalLoadingBar :loading="loading" />

			<ModalApiError :error="modalError" class="mb-4" />
			<UForm
				ref="form"
				:schema="schema"
				:state="state"
				class="space-y-4"
				@submit="onSubmit"
			>
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
				<UFormField label="Active" name="isActive">
					<USwitch v-model="state.isActive" :disabled="loading" />
				</UFormField>

				<div v-if="user?.roles?.length" class="space-y-1">
					<span class="text-sm font-medium text-highlighted">Roles</span>
					<div class="flex flex-wrap gap-1.5">
						<UBadge
							v-for="role in user.roles"
							:key="role.id"
							variant="subtle"
							color="neutral"
							:label="role.name"
						/>
					</div>
				</div>
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
