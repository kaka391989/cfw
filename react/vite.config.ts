import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc"; // Use the SWC plugin

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": "/src", // Adjust the alias based on your project structure
    },
  },
});
