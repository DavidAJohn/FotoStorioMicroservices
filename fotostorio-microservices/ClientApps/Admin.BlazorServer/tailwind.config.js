module.exports = {
    content: [
        "./Pages/*.razor",
        "./Pages/**/*.razor",
        "./Shared/*.razor",
        "./Components/*.razor",
        "./App.razor"
    ],
    theme: {
        container: {
            center: true,
            padding: '2rem'
        },
        extend: {
            spacing: {
                '112': '28rem',
                '128': '32rem',
                '144': '36rem',
            },
            borderRadius: {
                '4xl': '2rem',
            },
            colors: {
                fotoblue: '#111750',
                fotofuchsia: '#FF2A57'
            },
            fontFamily: {
                body: ['Roboto']
            }
        }
    },
    plugins: [],
}