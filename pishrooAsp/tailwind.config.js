/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        // تمام فایل‌های Razor/CSHTML
        './Pages/**/*.cshtml',
        './Pages/**/*.razor',
        './Views/**/*.cshtml',

        // فایل‌های JS اگر دارید
        './wwwroot/js/**/*.js',

        // کامپوننت‌های خاص
        './**/*.html',
    ],
    theme: {
        extend: {
            fontFamily: {
                vazirmatn: ['Vazirmatn', 'sans-serif'],
            },
        },
    },
    plugins: [],
}