<template>
  <div class="home-page">
    <!-- Hero Section -->
    <div class="text-center mb-12">
      <h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
        ImageGlider
      </h1>
      <p class="text-xl text-gray-600 dark:text-gray-300 mb-8">
        现代化的图片处理工具，让图片处理变得简单高效
      </p>
      <div class="flex justify-center">
        <el-button type="primary" size="large" @click="scrollToFeatures">
          开始使用
          <el-icon class="ml-2"><ArrowDown /></el-icon>
        </el-button>
      </div>
    </div>

    <!-- Features Grid -->
    <div ref="featuresSection" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <div
        v-for="feature in features"
        :key="feature.name"
        class="feature-card bg-white dark:bg-gray-800 rounded-xl shadow-md p-6 cursor-pointer border border-gray-200 dark:border-gray-700"
        @click="navigateTo(feature.path)"
      >
        <div class="flex items-center justify-center w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-lg mb-4">
          <component :is="feature.icon" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">
          {{ feature.name }}
        </h3>
        <p class="text-gray-600 dark:text-gray-300 text-sm mb-4">
          {{ feature.description }}
        </p>
        <div class="flex items-center text-blue-600 dark:text-blue-400 text-sm font-medium">
          立即使用
          <el-icon class="ml-1"><ArrowRight /></el-icon>
        </div>
      </div>
    </div>

    <!-- Statistics Section -->
    <div class="mt-16 bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl p-8 text-white">
      <div class="text-center mb-8">
        <h2 class="text-3xl font-bold mb-4">为什么选择 ImageGlider？</h2>
        <p class="text-blue-100 text-lg">专业的图片处理能力，简洁的用户体验</p>
      </div>
      
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="text-center">
          <div class="text-4xl font-bold mb-2">8+</div>
          <div class="text-blue-100">处理功能</div>
        </div>
        <div class="text-center">
          <div class="text-4xl font-bold mb-2">10MB</div>
          <div class="text-blue-100">最大文件支持</div>
        </div>
        <div class="text-center">
          <div class="text-4xl font-bold mb-2">100%</div>
          <div class="text-blue-100">客户端处理</div>
        </div>
      </div>
    </div>

    <!-- Quick Start Guide -->
    <div class="mt-16">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-8 text-center">
        快速开始
      </h2>
      
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="text-center">
          <div class="flex items-center justify-center w-16 h-16 bg-green-100 dark:bg-green-900/30 rounded-full mb-4 mx-auto">
            <span class="text-2xl font-bold text-green-600 dark:text-green-400">1</span>
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">选择功能</h3>
          <p class="text-gray-600 dark:text-gray-300">从上方功能卡片中选择您需要的图片处理功能</p>
        </div>
        
        <div class="text-center">
          <div class="flex items-center justify-center w-16 h-16 bg-blue-100 dark:bg-blue-900/30 rounded-full mb-4 mx-auto">
            <span class="text-2xl font-bold text-blue-600 dark:text-blue-400">2</span>
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">上传图片</h3>
          <p class="text-gray-600 dark:text-gray-300">拖拽或点击上传您要处理的图片文件</p>
        </div>
        
        <div class="text-center">
          <div class="flex items-center justify-center w-16 h-16 bg-purple-100 dark:bg-purple-900/30 rounded-full mb-4 mx-auto">
            <span class="text-2xl font-bold text-purple-600 dark:text-purple-400">3</span>
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">下载结果</h3>
          <p class="text-gray-600 dark:text-gray-300">调整参数后处理图片，然后下载处理结果</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ArrowDown, ArrowRight } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

const router = useRouter()
const featuresSection = ref(null)

const features = [
  {
    name: '颜色调整',
    path: '/color',
    icon: 'Brush',
    description: '调整图片的亮度、对比度、饱和度、色相和伽马值'
  },
  {
    name: '图片压缩',
    path: '/compress',
    icon: 'Files',
    description: '优化图片文件大小，支持多种压缩级别'
  },
  {
    name: '格式转换',
    path: '/convert',
    icon: 'RefreshRight',
    description: '在不同图片格式间转换，支持 JPG、PNG、WEBP 等'
  },
  {
    name: '图片裁剪',
    path: '/crop',
    icon: 'Crop',
    description: '精确裁剪、中心裁剪或按百分比裁剪图片'
  },
  {
    name: '尺寸调整',
    path: '/resize',
    icon: 'FullScreen',
    description: '调整图片尺寸或生成缩略图'
  },
  {
    name: '添加水印',
    path: '/watermark',
    icon: 'PictureRounded',
    description: '为图片添加文本或图片水印'
  },
  {
    name: '图片信息',
    path: '/info',
    icon: 'InfoFilled',
    description: '查看图片详细信息和清理元数据'
  }
]

const navigateTo = (path) => {
  if (path === '/color' || path === '/resize' || path === '/watermark') {
    ElMessage.warning('该功能尚未开放，敬请期待')
    return
  }
  router.push(path)
}

const scrollToFeatures = () => {
  featuresSection.value?.scrollIntoView({ behavior: 'smooth' })
}
</script>

<style scoped>
.home-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}

.feature-card:hover {
  @apply transform -translate-y-2 shadow-xl;
}
</style>