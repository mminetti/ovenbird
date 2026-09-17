<script setup lang="ts">
export interface ConnectorFieldItem {
	name: string
	value?: string
	isSecret: boolean
}

const model = defineModel<ConnectorFieldItem[]>({ default: () => [] })

const props = defineProps<{
	disabled?: boolean
}>()

function addField() {
	model.value = [...model.value, { name: '', value: '', isSecret: false }]
}

function removeField(index: number) {
	model.value = model.value.filter((_, i) => i !== index)
}
</script>

<template>
	<div class="space-y-3">
		<div
			v-for="(field, index) in model"
			:key="index"
			class="flex flex-col sm:flex-row gap-2 sm:items-start rounded-md border border-default p-3"
		>
			<UFormField :name="`fields.${index}.name`" label="Field name" class="flex-1">
				<UInput
					v-model="field.name"
					placeholder="e.g. host"
					class="w-full"
					:disabled="props.disabled"
				/>
			</UFormField>

			<UFormField
				:name="`fields.${index}.value`"
				:label="field.isSecret ? 'Secret reference name' : 'Value'"
				class="flex-1"
			>
				<UInput
					v-model="field.value"
					:placeholder="field.isSecret ? 'e.g. connector-ftp-password' : 'Value'"
					class="w-full"
					:disabled="props.disabled"
				/>
				<template v-if="field.isSecret" #help>
					Looked up in Key Vault or an environment variable at runtime — not the actual secret.
				</template>
			</UFormField>

			<UFormField label="Secret" class="shrink-0">
				<div class="flex items-center gap-2 h-8">
					<USwitch v-model="field.isSecret" :disabled="props.disabled" />
				</div>
			</UFormField>

			<div class="flex items-end sm:items-start sm:pt-6">
				<UButton
					icon="i-lucide-trash"
					color="error"
					variant="ghost"
					:disabled="props.disabled"
					aria-label="Remove field"
					@click="removeField(index)"
				/>
			</div>
		</div>

		<UButton
			label="Add field"
			icon="i-lucide-plus"
			color="neutral"
			variant="subtle"
			:disabled="props.disabled"
			@click="addField"
		/>
	</div>
</template>
