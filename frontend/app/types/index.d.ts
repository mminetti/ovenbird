import type { AvatarProps } from '@nuxt/ui'

export type UserStatus = 'subscribed' | 'unsubscribed' | 'bounced'
export type SaleStatus = 'paid' | 'failed' | 'refunded'

export interface User {
	id: number
	name: string
	email: string
	avatar?: AvatarProps
	status: UserStatus
	location: string
}

export interface Mail {
	id: number
	unread?: boolean
	from: User
	subject: string
	body: string
	date: string
}

export interface Stat {
	title: string
	icon: string
	value: number | string
	variation: number
	formatter?: (value: number) => string
}

export interface Sale {
	id: string
	date: string
	status: SaleStatus
	email: string
	amount: number
}

export interface Notification {
	id: number
	unread?: boolean
	sender: User
	body: string
	date: string
}

export type Period = 'daily' | 'weekly' | 'monthly'

export interface Range {
	start: Date
	end: Date
}

export interface SecurityPermission {
	id: number
	name: string
	moduleId: number
	moduleName: string
	description: string
}

export interface SecurityRole {
	id: number
	name: string
	lastModifiedAtUtc: string
	lastModifiedBy: string | null
	permissions?: SecurityPermission[]
}

export interface SecurityUser {
	id: number
	name: string
	email: string
	externalIdentifier: string
	isActive: boolean
	lastModifiedAtUtc: string
	lastModifiedBy: string | null
	roles?: SecurityRole[]
}

export interface ConnectorField {
	id?: number
	name: string
	value?: string | null
	isSecret: boolean
}

export interface Connector {
	id: number
	name: string
	description?: string | null
	connectorTypeId: number
	connectorTypeName: string
	connectorImplementationId: number
	connectorImplementationName: string
	fields: ConnectorField[]
}

export interface Company {
	id: number
	name: string
	marketId: number
	marketName: string
	timeZoneId: string
}

export interface Market {
	id: number
	name: string
	identifier: string
}

export interface ConfigurationField {
	id?: number
	name: string
	value?: string | null
}

export interface ConfigurationConnector {
	id: number
	name: string
}

export interface Configuration {
	id: number
	name: string
	description?: string | null
	configurationTypeId: number
	configurationTypeName: string
	companyId?: number | null
	companyName?: string | null
	fields: ConfigurationField[]
	connectors: ConfigurationConnector[]
}
