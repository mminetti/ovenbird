import { formatInTimeZone, toZonedTime } from 'date-fns-tz'

/**
 * Convert a UTC date string or Date object to the given timezone.
 * @param utcDate - Date string in ISO format or Date object (assumed to be UTC)
 * @param timezone - Timezone string (e.g., 'America/Chicago')
 */
export function convertUtcToTimezone(utcDate: string | Date, timezone: string): Date {
	const date = typeof utcDate === 'string' ? new Date(utcDate) : utcDate
	return toZonedTime(date, timezone)
}

/**
 * Convert a UTC date to a formatted string in the given timezone.
 * @param utcDate - Date string in ISO format or Date object (assumed to be UTC)
 * @param timezone - Timezone string (e.g., 'America/Chicago')
 * @param formatString - date-fns format string (default: 'MMM d, yyyy h:mm a')
 */
export function formatUtcToTimezone(
	utcDate: string | Date,
	timezone: string,
	formatString = 'MMM d, yyyy h:mm a'
): string {
	const date = typeof utcDate === 'string' ? new Date(utcDate) : utcDate
	return formatInTimeZone(date, timezone, formatString)
}

/**
 * Get a timezone's abbreviation for display (e.g., "CST" or "GMT+2").
 * @param timezone - Timezone string (e.g., 'America/Chicago')
 * @param date - Reference date (default: now)
 */
export function getTimezoneAbbr(timezone: string, date: Date = new Date()): string {
	const formatter = new Intl.DateTimeFormat('en-US', {
		timeZone: timezone,
		timeZoneName: 'short'
	})
	const parts = formatter.formatToParts(date)
	const timeZoneName = parts.find(part => part.type === 'timeZoneName')
	return timeZoneName?.value ?? timezone
}
