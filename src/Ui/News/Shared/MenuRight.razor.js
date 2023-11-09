export function isDarkModeDefault() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
}

export function toggleDarkMode() {
    if (document.documentElement.classList.contains('dark'))
        document.documentElement.classList.remove('dark');
    else
        document.documentElement.classList.add('dark');
}

