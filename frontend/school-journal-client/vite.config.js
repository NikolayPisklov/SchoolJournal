import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from `@tailwindcss/vite`
import mkcert from `vite-plugin-mkcert`

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
    mkcert()
  ]
})
