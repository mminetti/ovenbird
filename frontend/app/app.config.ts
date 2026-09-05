export default defineAppConfig({
	ui: {
		colors: {
			primary: 'zinc',
			secondary: 'zinc',
			success: 'emerald',
			info: 'sky',
			warning: 'amber',
			error: 'red',
			neutral: 'zinc'
		}
	}
})

// Dark mode and shade overrides — add the CSS variables from the CSS export to your main.css
// Dark mode uses different palettes:
// primary: zinc-950 (shifted from default 500)
// secondary: zinc-600 (shifted from default 500)
// success: emerald-600 (shifted from default 500)
// info: sky-600 (shifted from default 500)
// warning: amber-600 (shifted from default 500)
// error: red-600 (shifted from default 500)
