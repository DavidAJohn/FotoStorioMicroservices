/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
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
    },
  },
  plugins: [],
}
