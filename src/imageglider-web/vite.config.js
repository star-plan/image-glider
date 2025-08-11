import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    host: '0.0.0.0',
    port: 3000, 
    proxy: {
      '/api': {
        target: 'http://localhost:5229',
        changeOrigin: true
      },
    },
  }
})
