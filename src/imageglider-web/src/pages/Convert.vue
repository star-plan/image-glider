<template>
  <div class="convert-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片格式转换</h1>
      <p class="text-gray-600 dark:text-gray-300">在不同图片格式间转换，支持 JPG、PNG、WEBP 等</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload and Settings Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange"/>
        </div>

        <!-- Conversion Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">转换设置</h2>

          <div class="space-y-6">
            <!-- Current Format Info -->
            <div class="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
              <h4 class="text-sm font-medium text-blue-800 dark:text-blue-200 mb-2">当前格式</h4>
              <div class="flex items-center space-x-4">
                <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">
                  {{ currentFormat.toUpperCase() }}
                </div>
                <div class="text-sm text-blue-600 dark:text-blue-400">
                  {{ formatFileSize(selectedFile.size) }}
                </div>
              </div>
            </div>

            <!-- Target Format Selection -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                目标格式
              </label>
              <div class="grid grid-cols-2 gap-3">
                <div
                    v-for="format in availableFormats"
                    :key="format.value"
                    :class="[
                    'p-4 border-2 rounded-lg cursor-pointer transition-all',
                    settings.targetFormat === format.value
                      ? 'border-blue-500 bg-blue-50 dark:bg-blue-900/20'
                      : 'border-gray-200 dark:border-gray-600 hover:border-gray-300 dark:hover:border-gray-500'
                  ]"
                    @click="settings.targetFormat = format.value"
                >
                  <div class="flex items-center space-x-3">
                    <el-icon class="text-xl" :class="format.color">
                      <component :is="format.icon"/>
                    </el-icon>
                    <div>
                      <div class="font-medium text-gray-900 dark:text-white">
                        {{ format.name }}
                      </div>
                      <div class="text-xs text-gray-500 dark:text-gray-400">
                        {{ format.description }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Quality Setting (for lossy formats) -->
            <div v-if="isLossyFormat(settings.targetFormat)">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                输出质量 ({{ settings.quality }})
              </label>
              <el-slider
                  v-model="settings.quality"
                  :min="1"
                  :max="100"
                  :step="1"
                  show-stops
                  show-input
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                仅适用于 JPEG格式 和 WEBP 格式的图片
              </p>
            </div>
          </div>

          <!-- Conversion Preview -->
          <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">转换预览</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始格式：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ currentFormat.toUpperCase() }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">目标格式：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{
                    settings.targetFormat.toUpperCase()
                  }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始大小：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{
                    formatFileSize(selectedFile.size)
                  }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">预估大小：</span>
                <span class="font-medium text-green-600 dark:text-green-400">{{ formatFileSize(estimatedSize) }}</span>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetSettings" :disabled="processing">
              <el-icon>
                <RefreshLeft/>
              </el-icon>
              重置
            </el-button>
            <el-button
                type="primary"
                @click="convertImage"
                :loading="processing"
                :disabled="settings.targetFormat === currentFormat"
            >
              <el-icon>
                <RefreshRight/>
              </el-icon>
              开始转换
            </el-button>
          </div>
        </div>
      </div>

      <!-- Preview and Result Section -->
      <div v-if="selectedFile" class="space-y-6">
        <!-- Format Information -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">格式说明</h2>

          <div class="space-y-4">
            <div v-for="format in formatInfo" :key="format.name" class="border-l-4 pl-4" :class="format.borderColor">
              <h3 class="font-semibold text-gray-900 dark:text-white">{{ format.name }}</h3>
              <p class="text-sm text-gray-600 dark:text-gray-300 mt-1">{{ format.description }}</p>
              <div class="flex flex-wrap gap-2 mt-2">
                <span
                    v-for="feature in format.features"
                    :key="feature"
                    class="px-2 py-1 text-xs rounded-full bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300"
                >
                  {{ feature }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Conversion Result -->
        <div v-if="convertedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">转换结果</h2>

          <div class="text-center mb-4">
            <img
                :src="convertedImageUrl"
                alt="转换结果"
                class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Conversion Stats -->
          <div class="grid grid-cols-2 gap-4 mb-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="text-center">
              <div class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ currentFormat.toUpperCase() }}
              </div>
              <div class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatFileSize(selectedFile.size) }}
              </div>
            </div>
            <div class="text-center">
              <div class="text-lg font-semibold text-blue-600 dark:text-blue-400">
                {{ settings.targetFormat.toUpperCase() }}
              </div>
              <div class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatFileSize(convertedSize) }}
              </div>
            </div>
          </div>

          <!-- Size Comparison -->
          <div class="text-center space-y-2 mb-4">
            <div class="text-lg font-medium text-gray-700 dark:text-gray-300">
              文件大小变化：
              <span :class="sizeChangeClass">
                {{ sizeChangeText }}
              </span>
            </div>
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadResult">
              <el-icon>
                <Download/>
              </el-icon>
              下载转换结果
            </el-button>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onUnmounted, ref } from 'vue'
import { ElLoading, ElMessage } from 'element-plus'
import { Download, RefreshLeft, RefreshRight } from '@element-plus/icons-vue'
// 导入组件
import ImageUpload from '../components/common/ImageUpload.vue'
// 导入方法
import { imageApi } from '../http/modules/imageApi'
import { createImagePreview, formatFileSize, revokeImagePreview } from '../utils/file'

const selectedFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const convertedImageUrl = ref('')
const convertedSize = ref(0)

const settings = ref({
  targetFormat: 'jpg',
  quality: 90,
  preserveTransparency: true,
  webpMode: 'lossy'
})

const availableFormats = [
  {
    value: 'jpg',
    name: 'JPEG',
    description: '通用有损格式',
    icon: 'Picture',
    color: 'text-orange-500'
  },
  {
    value: 'png',
    name: 'PNG',
    description: '支持透明度',
    icon: 'PictureRounded',
    color: 'text-green-500'
  },
  {
    value: 'webp',
    name: 'WEBP',
    description: '现代高效格式',
    icon: 'Picture',
    color: 'text-blue-500'
  },
  {
    value: 'gif',
    name: 'GIF',
    description: '支持动画',
    icon: 'PictureRounded',
    color: 'text-purple-500'
  },
  {
    value: 'bmp',
    name: 'BMP',
    description: '不压缩，图像质量高',
    icon: 'PictureRounded',
    color: 'text-yellow-500'
  },
  {
    value: 'tiff',
    name: 'TIFF',
    description: '高质量图像格式',
    icon: 'PictureRounded',
    color: 'text-red-500'
  }
]

const formatInfo = [
  {
    name: 'JPEG',
    description: '最常用的图片格式，适合照片和复杂图像，文件小但有损压缩',
    features: ['有损压缩', '小文件', '广泛支持', '适合照片'],
    borderColor: 'border-orange-500'
  },
  {
    name: 'PNG',
    description: '无损压缩格式，支持透明度，适合图标和简单图像',
    features: ['无损压缩', '透明度支持', '适合图标', '文件较大'],
    borderColor: 'border-green-500'
  },
  {
    name: 'WEBP',
    description: 'Google开发的现代格式，同时支持有损和无损压缩，文件更小',
    features: ['有损/无损', '文件更小', '现代浏览器', '高质量'],
    borderColor: 'border-blue-500'
  },
  {
    name: 'GIF',
    description: '支持动画的格式，颜色有限，适合简单动画和图标',
    features: ['动画支持', '256色', '小文件', '广泛支持'],
    borderColor: 'border-purple-500'
  },
  {
    name: 'BMP',
    description: '不压缩的位图格式，文件体积大，适合简单位图、Windows环境',
    features: ['不压缩', '文件大', '图像质量高', '适合图标'],
    borderColor: 'border-yellow-500'
  },
  {
    name: 'TIFF',
    description: '高质量的图像格式，支持多种颜色，文件大但支持多种格式',
    features: ['多种颜色', '文件大', '支持多种格式', '高质量'],
    borderColor: 'border-red-500'
  }
]

const currentFormat = computed(() => {
  if (!selectedFile.value) return ''
  const ext = selectedFile.value.name.split('.').pop().toLowerCase()
  return ext === 'jpeg' ? 'jpg' : ext
})

const estimatedSize = computed(() => {
  if (!selectedFile.value) return 0

  const originalSize = selectedFile.value.size
  const fromFormat = currentFormat.value
  const toFormat = settings.value.targetFormat

  // Simple estimation logic
  if (fromFormat === toFormat) return originalSize

  if (toFormat === 'jpg') {
    return Math.round(originalSize * (settings.value.quality / 100) * 0.7)
  } else if (toFormat === 'png') {
    return Math.round(originalSize * 1.2)
  } else if (toFormat === 'webp') {
    return Math.round(originalSize * 0.6)
  } else if (toFormat === 'gif') {
    return Math.round(originalSize * 0.8)
  } else if (toFormat === 'bmp') {
    return Math.round(originalSize * 3)
  } else if (toFormat === 'tiff') {
    return Math.round(originalSize * 5)
  }

  return originalSize
})

const sizeChangeClass = computed(() => {
  if (!convertedSize.value || !selectedFile.value) return ''

  const change = convertedSize.value - selectedFile.value.size
  if (change > 0) return 'text-red-600 dark:text-red-400'
  if (change < 0) return 'text-green-600 dark:text-green-400'
  return 'text-gray-600 dark:text-gray-400'
})

const sizeChangeText = computed(() => {
  if (!convertedSize.value || !selectedFile.value) return ''

  const change = convertedSize.value - selectedFile.value.size
  const percent = Math.abs(Math.round((change / selectedFile.value.size) * 100))

  if (change > 0) return `增加 ${percent}%`
  if (change < 0) return `减少 ${percent}%`
  return '无变化'
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    convertedImageUrl.value = ''
    convertedSize.value = 0

    // Set default target format (different from current)
    const current = currentFormat.value
    if (current === 'jpg') {
      settings.value.targetFormat = 'png'
    } else {
      settings.value.targetFormat = 'jpg'
    }
  }
}

const isLossyFormat = (format) => {
  return ['jpg','webp'].includes(format)
}

const resetSettings = () => {
  settings.value = {
    targetFormat: 'jpg',
    quality: 90,
    preserveTransparency: true,
    webpMode: 'lossy'
  }
  selectedFile.value = null
  originalImageUrl.value = ''
  convertedImageUrl.value = ''
  convertedSize.value = 0
}

const convertImage = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('请先上传图片')
    return
  }

  if (settings.value.targetFormat === currentFormat.value) {
    ElMessage.warning('目标格式与当前格式相同')
    return
  }

  processing.value = true
  const loading = ElLoading.service({
    lock: true,
    text: 'Loading',
    background: 'rgba(0, 0, 0, 0.7)',
  })
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('fileExt', `.${settings.value.targetFormat}`)
    formData.append('quality', settings.value.quality)
    
    // Simulate processing delay
    const res = await imageApi.convertImageApi(formData)
    if (res.statusCode === 200) {
      const blob = await imageApi.downloadFileApi(res.data);

      if (settings.value.targetFormat === 'tiff') {
        ElMessage.success(res.message);
        return;
      }

      // 设置结果
      convertedImageUrl.value = createImagePreview(blob.data);
      // 从响应头中获取文件大小
      convertedSize.value = parseInt(blob.headers['content-length'])
      loading.close()
      // 显示成功消息
      ElMessage.success(res.message);
    }

  } catch (error) {
    ElMessage.error('转换失败：' + error.message)
  } finally {
    processing.value = false
    loading.close()
  }
}

const downloadResult = () => {
  if (convertedImageUrl.value) {
    const originalName = selectedFile.value.name.split('.')[0]
    const link = document.createElement('a')
    link.href = convertedImageUrl.value
    link.download = `${originalName}.${settings.value.targetFormat}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}


onUnmounted(() => {
  if (convertedImageUrl.value) {
    revokeImagePreview(convertedImageUrl.value);
  }
});
</script>

<style scoped>
.convert-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>