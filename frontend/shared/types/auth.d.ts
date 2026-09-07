declare module '#auth-utils' {
	interface User {
		id: string
		name: string
		email: string
		permissions: string[]
	}

	// accessToken is server-only — never sent to the browser
	interface SecureSessionData {
		accessToken: string
	}
}

export {}
