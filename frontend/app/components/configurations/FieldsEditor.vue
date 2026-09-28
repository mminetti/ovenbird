<script setup lang="ts">
export interface ConfigurationFieldItem {
	id?: number
	name: string
	value?: string
	operation?: 'create' | 'update' | 'delete'
}

const model = defineModel<ConfigurationFieldItem[]>({ default: () => [] })

const props = defineProps<{
	disabled?: boolean
}>()

const selectedEntityTab = ref('fields')
const entityTabs = [{
	label: 'Fields',
	value: 'fields'
}]

let nextTempId = -1

const visibleFields = computed(() =>
	model.value
		.map((field, index) => ({ field, index }))
		.filter(({ field }) => field.operation !== 'delete')
)

function addField() {
	model.value = [...model.value, { id: nextTempId--, name: '', value: '', operation: 'create' }]
}

function removeField(index: number) {
	const field = model.value[index]
	if (!field) {
		return
	}

	if (field.operation === 'create') {
		model.value = model.value.filter((_, i) => i !== index)
		return
	}

	model.value = model.value.map((f, i) => i === index ? { ...f, operation: 'delete' } : f)
}

function onFieldChange(index: number, patch: Partial<ConfigurationFieldItem>) {
	model.value = model.value.map((f, i) => {
		if (i !== index) {
			return f
		}

		const updated = { ...f, ...patch }
		if (!updated.operation) {
			updated.operation = 'update'
		}
		return updated
	})
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
				<div v-if="!visibleFields.length" class="rounded-lg border border-dashed border-default px-3 py-6 text-center text-sm text-muted">
					No fields
				</div>

				<div
					v-for="{ field, index } in visibleFields"
					:key="field.id"
					class="rounded-lg border border-default p-3 space-y-2"
				>
					<div class="flex items-center gap-2">
						<UFormField :name="`fields.${index}.name`" class="mb-0 flex-1">
							<UInput
								:model-value="field.name"
								class="w-full"
								placeholder="Field name"
								:disabled="props.disabled"
								@update:model-value="(value) => onFieldChange(index, { name: String(value) })"
							/>
						</UFormField>

						<UFormField :name="`fields.${index}.value`" class="mb-0 flex-1">
							<UInput
								:model-value="field.value"
								class="w-full"
								placeholder="Value"
								:disabled="props.disabled"
								@update:model-value="(value) => onFieldChange(index, { value: String(value) })"
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
