<script setup lang="ts">
interface Props {
	modelValue: string
	placeholder?: string
	debounceDelay?: number
	minSearchLength?: number
}

const props = withDefaults(defineProps<Props>(), {
	debounceDelay: 350,
	minSearchLength: 3
})

const emit = defineEmits<{
	'update:modelValue': [value: string]
	'update:debouncedValue': [value: string]
}>()

let debounceTimeout: ReturnType<typeof setTimeout> | undefined

const handleInput = (value: string) => {
	emit('update:modelValue', value)

	if (debounceTimeout) {
		clearTimeout(debounceTimeout)
	}

	debounceTimeout = setTimeout(() => {
		if (!value || value.length >= props.minSearchLength) {
			emit('update:debouncedValue', value)
		}
	}, props.debounceDelay)
}

onBeforeUnmount(() => {
	if (debounceTimeout) {
		clearTimeout(debounceTimeout)
	}
})
</script>

<template>
	<UInput
		:model-value="modelValue"
		size="lg"
		class="w-full sm:w-96"
		:placeholder="placeholder || 'Search...'"
		@update:model-value="handleInput"
	>
		<template #leading>
			<UIcon name="i-lucide-search" class="size-4 text-muted" />
		</template>
	</UInput>
</template>
