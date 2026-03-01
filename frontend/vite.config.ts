import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Port must stay above 7000 and not conflict with the backend (7100).
    port: 7200,
    strictPort: true,
  },
})
