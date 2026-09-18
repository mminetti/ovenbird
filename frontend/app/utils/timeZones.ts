export function timeZoneOptions(): string[] {
	if (typeof Intl.supportedValuesOf === 'function') {
		return Intl.supportedValuesOf('timeZone')
	}

	return ['UTC']
}
