# UI-Components
- TableTemplate
	- Planned
- Icon
	- Took [bootstrap icons](https://github.com/twbs/icons) and made a component that creates the svg element directly
	- So basically all icons are written directly to responses which is a very
      stupid idea as you can see if you list them all the pagesize is huge and
	  client can't cache it, but you can control the drawing better and was fun
	  thing to make. Sizes are small still but you don't want to write those into
	  each line for example.
	- Will make use of the small font size and load css so font is cached on client side.
	- This version I wanna keep the fun mess I made

# Howto tailwind blazor without node?

Install [Tailwind-CLI](https://github.com/tailwindlabs/tailwindcss)
	- Just saves the CLI as d:\tailwind-cli\tailwind-cli.exe
	- Added path to environment variables
	- Same as `arduino-cli`

Create Tailwind configuration (in the new blazorserver app folder)
```sh
tailwind-cli init
Created Tailwind CSS config file: tailwind.config.js
```

Adjust the configuration to pickup all our components/pages
```json
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./**/*.{html,razor}]'],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

Create our tailwind CSS (tailwind.css in the source root, not wwwroot)
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Run Tailwind
```sh
tailwind-cli -i tailwind.css -m -o wwwroot/css/app.css

Rebuilding...

warn - No utility classes were detected in your source files. If this is unexpected, double-check the `content` option in your Tailwind CSS configuration.
warn - https://tailwindcss.com/docs/content-configuration

Done in 306ms.
```

With watch option you can leave it running and rebuilding when changes occur.

Configuring post-build step can be done in the project file, ofc this will depends on the cli tool so
that needs to be installed on the build host.


