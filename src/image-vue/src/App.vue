<script setup>
import { ref, onMounted } from 'vue'

const isDark = ref(false)
const mobileMenuOpen = ref(false)

const navigation = [
  { name: '首页', href: '/', icon: 'House' },
  { name: '颜色调整', href: '/color', icon: 'Brush' },
  { name: '图片压缩', href: '/compress', icon: 'Files' },
  { name: '格式转换', href: '/convert', icon: 'RefreshRight' },
  { name: '图片裁剪', href: '/crop', icon: 'Crop' },
  { name: '尺寸调整', href: '/resize', icon: 'FullScreen' },
  { name: '添加水印', href: '/watermark', icon: 'PictureRounded' },
  { name: '图片信息', href: '/info', icon: 'InfoFilled' },
]

const toggleTheme = () => {
  isDark.value = !isDark.value
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
  updateTheme()
}

const updateTheme = () => {
  if (isDark.value) {
    document.documentElement.classList.add('dark')
  } else {
    document.documentElement.classList.remove('dark')
  }
}

onMounted(() => {
  // Check for saved theme preference or default to light mode
  const savedTheme = localStorage.getItem('theme')
  if (savedTheme) {
    isDark.value = savedTheme === 'dark'
  } else {
    // Check system preference
    isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches
  }
  updateTheme()
})
</script>

<template>
  <div :class="{ 'dark': isDark }" class="min-h-screen bg-gray-50 dark:bg-gray-900 transition-colors duration-300">
    <!-- Header -->
    <header class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo and Title -->
          <div class="flex items-center space-x-4">
            <div class="flex-shrink-0">
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">ImageGlider</h1>
            </div>
          </div>

          <!-- Navigation -->
          <nav class="hidden md:flex space-x-8">
            <router-link
              v-for="item in navigation"
              :key="item.name"
              :to="item.href"
              class="text-gray-500 hover:text-gray-900 dark:text-gray-300 dark:hover:text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
              :class="{ 'text-blue-600 dark:text-blue-400': $route.path === item.href }"
            >
              <!-- <component :is="item.icon" class="w-4 h-4 inline mr-2" />-->
              {{ item.name }}
            </router-link>
          </nav>

          <!-- Theme Toggle and Mobile Menu -->
          <div class="flex items-center space-x-4">
            <el-button @click="toggleTheme" circle size="small" :icon="isDark ? 'Sunny' : 'Moon'" />
            
            <!-- Mobile menu button -->
            <el-button 
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="md:hidden"
              :icon="mobileMenuOpen ? 'Close' : 'Menu'"
              circle
              size="small"
            />
          </div>
        </div>
      </div>

      <!-- Mobile Navigation -->
      <div v-show="mobileMenuOpen" class="md:hidden">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700">
          <router-link
            v-for="item in navigation"
            :key="item.name"
            :to="item.href"
            @click="mobileMenuOpen = false"
            class="text-gray-500 hover:text-gray-900 dark:text-gray-300 dark:hover:text-white block px-3 py-2 rounded-md text-base font-medium transition-colors"
            :class="{ 'text-blue-600 dark:text-blue-400': $route.path === item.href }"
          >
            <component :is="item.icon" class="w-4 h-4 inline mr-2" />
            {{ item.name }}
          </router-link>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <div class="px-4 py-6 sm:px-0">
        <router-view />
      </div>
    </main>

    <!-- Footer -->
    <footer class="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 mt-auto">
      <div class="max-w-7xl mx-auto py-4 px-4 sm:px-6 lg:px-8">
        <div class="text-center text-sm text-gray-500 dark:text-gray-400">
          © 2024 ImageGlider. 现代化的图片处理工具
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
