export interface TimeZoneOption {
	label: string
	value: string
}

const US_TIME_ZONES: TimeZoneOption[] = [
	{ value: 'America/New_York', label: 'Eastern Time (New York)' },
	{ value: 'America/Chicago', label: 'Central Time (Chicago)' },
	{ value: 'America/Denver', label: 'Mountain Time (Denver)' },
	{ value: 'America/Los_Angeles', label: 'Pacific Time (Los Angeles)' },
	{ value: 'America/Anchorage', label: 'Alaska Time (Anchorage)' },
	{ value: 'Pacific/Honolulu', label: 'Hawaii Time (Honolulu)' }
]

export function timeZoneOptions(): TimeZoneOption[] {
	return US_TIME_ZONES
}
