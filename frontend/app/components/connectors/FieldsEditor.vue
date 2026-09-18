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

const selectedEntityTab = ref('fields')
const entityTabs = [{
	label: 'Fields',
	value: 'fields'
}]

function addField() {
	model.value = [...model.value, { name: '', value: '', isSecret: false }]
}

function removeField(index: number) {
	model.value = model.value.filter((_, i) => i !== index)
}
</script>

<template>
	<UTabs
		v-model="selectedEntityTab"
		:items="entityTabs"
		variant="link"
		class="pt-2"
		:ui="{
			trigger: 'data-[state=active]:bg-elevated data-[state=active]:text-highlighted data-[state=active]:font-semibold rounded-md px-3'
		}"
	>
		<template #list-trailing>
			<div class="ms-auto">
				<UButton
					label="Add field"
					icon="i-lucide-plus"
					color="neutral"
					variant="soft"
					size="xs"
					:disabled="props.disabled"
					@click="addField"
				/>
			</div>
		</template>

		<template #content="{ item }">
			<div v-if="item.value === 'fields'" class="mt-3 space-y-3">
				<div v-if="!model.length" class="rounded-lg border border-dashed border-default px-3 py-6 text-center text-sm text-muted">
					No fields
				</div>

				<div
					v-for="(field, index) in model"
					:key="index"
					class="rounded-lg border border-default p-3 space-y-2"
				>
					<div class="flex items-center gap-2">
						<UFormField :name="`fields.${index}.name`" class="mb-0 w-3/4">
							<UInput
								v-model="field.name"
								class="w-full"
								placeholder="Field name"
								:disabled="props.disabled"
							/>
						</UFormField>

						<UFormField label="Secret" orientation="horizontal" class="mb-0 ms-auto">
							<USwitch v-model="field.isSecret" :disabled="props.disabled" />
						</UFormField>
					</div>

					<div class="flex items-center gap-2">
						<UFormField :name="`fields.${index}.value`" class="mb-0 flex-1">
							<UInput
								v-model="field.value"
								class="w-full"
								:placeholder="field.isSecret ? 'Secret reference name (e.g. connector-ftp-password)' : 'Value'"
								:disabled="props.disabled"
							/>
						</UFormField>

						<UButton
							icon="i-lucide-trash-2"
							color="error"
							variant="ghost"
							size="xs"
							:disabled="props.disabled"
							class="shrink-0"
							@click="removeField(index)"
						/>
					</div>
				</div>
			</div>
		</template>
	</UTabs>
</template>
