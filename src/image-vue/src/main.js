import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import './style.css'
import App from './App.vue'

// Import pages
import Home from './pages/Home.vue'
import ColorAdjust from './pages/ColorAdjust.vue'
import Compress from './pages/Compress.vue'
import Convert from './pages/Convert.vue'
import Crop from './pages/Crop.vue'
import Resize from './pages/Resize.vue'
import Watermark from './pages/Watermark.vue'
import InfoMetadata from './pages/InfoMetadata.vue'

// Router configuration
const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/color', name: 'ColorAdjust', component: ColorAdjust },
  { path: '/compress', name: 'Compress', component: Compress },
  { path: '/convert', name: 'Convert', component: Convert },
  { path: '/crop', name: 'Crop', component: Crop },
  { path: '/resize', name: 'Resize', component: Resize },
  { path: '/watermark', name: 'Watermark', component: Watermark },
  { path: '/info', name: 'InfoMetadata', component: InfoMetadata },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

const app = createApp(App)

// Register Element Plus icons
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(ElementPlus)
app.use(router)
app.mount('#app')
