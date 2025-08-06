<template>
  <div class="resize-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片尺寸调整</h1>
      <p class="text-gray-600 dark:text-gray-300">调整图片尺寸或生成缩略图</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload and Settings Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Resize Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">调整设置</h2>
          
          <!-- Mode Selection -->
          <div class="mb-6">
            <el-radio-group v-model="mode">
              <el-radio label="resize">尺寸调整</el-radio>
              <el-radio label="thumbnail">生成缩略图</el-radio>
            </el-radio-group>
          </div>

          <!-- Resize Mode Settings -->
          <div v-if="mode === 'resize'" class="space-y-6">
            <!-- Resize Mode -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                调整模式
              </label>
              <el-select v-model="resizeSettings.resizeMode" class="w-full">
                <el-option label="保持宽高比" value="KeepAspectRatio" />
                <el-option label="拉伸模式" value="Stretch" />
                <el-option label="裁剪模式" value="Crop" />
              </el-select>
            </div>

            <!-- Dimensions -->
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  宽度 (px)
                </label>
                <el-input-number
                  v-model="resizeSettings.width"
                  :min="1"
                  :max="5000"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  高度 (px)
                </label>
                <el-input-number
                  v-model="resizeSettings.height"
                  :min="1"
                  :max="5000"
                  size="small"
                  class="w-full"
                />
              </div>
            </div>

            <!-- Quick Size Presets -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                快速尺寸
              </label>
              <div class="grid grid-cols-2 gap-2">
                <el-button
                  v-for="preset in sizePresets"
                  :key="preset.name"
                  @click="applySizePreset(preset)"
                  size="small"
                >
                  {{ preset.name }}
                </el-button>
              </div>
            </div>
          </div>

          <!-- Thumbnail Mode Settings -->
          <div v-if="mode === 'thumbnail'" class="space-y-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                最大边长 ({{ thumbnailSettings.maxSize }}px)
              </label>
              <el-slider
                v-model="thumbnailSettings.maxSize"
                :min="50"
                :max="500"
                :step="10"
                show-stops
                show-input
              />
            </div>
          </div>

          <!-- Quality Setting -->
          <div class="mt-6">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              输出质量 ({{ quality }})
            </label>
            <el-slider
              v-model="quality"
              :min="1"
              :max="100"
              :step="1"
              show-stops
              show-input
            />
          </div>

          <!-- Preview Info -->
          <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">预览信息</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始尺寸：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ imageWidth }} × {{ imageHeight }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">目标尺寸：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{ targetWidth }} × {{ targetHeight }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">缩放比例：</span>
                <span class="font-medium text-green-600 dark:text-green-400">{{ scaleRatio }}%</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">文件大小：</span>
                <span class="font-medium text-purple-600 dark:text-purple-400">{{ estimatedSize }}</span>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetSettings" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button type="primary" @click="processImage" :loading="processing">
              <el-icon><FullScreen /></el-icon>
              开始处理
            </el-button>
          </div>
        </div>
      </div>

      <!-- Preview and Result Section -->
      <div v-if="selectedFile" class="space-y-6">
        <!-- Original Image Preview -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">原始图片</h2>
          <div class="text-center">
            <img
              :src="originalImageUrl"
              alt="原始图片"
              class="max-h-64 mx-auto rounded-lg shadow-md"
              @load="handleImageLoad"
            />
            <div class="mt-4 text-sm text-gray-500 dark:text-gray-400">
              {{ selectedFile.name }} • {{ imageWidth }} × {{ imageHeight }}
            </div>
          </div>
        </div>

        <!-- Processed Result -->
        <div v-if="processedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            {{ mode === 'thumbnail' ? '缩略图结果' : '调整结果' }}
          </h2>
          
          <div class="text-center mb-4">
            <img
              :src="processedImageUrl"
              alt="处理结果"
              class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Comparison Stats -->
          <div class="grid grid-cols-2 gap-4 mb-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="text-center">
              <div class="text-lg font-semibold text-gray-900 dark:text-white">原始</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ imageWidth }} × {{ imageHeight }}</div>
            </div>
            <div class="text-center">
              <div class="text-lg font-semibold text-blue-600 dark:text-blue-400">处理后</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ actualWidth }} × {{ actualHeight }}</div>
            </div>
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadResult">
              <el-icon><Download /></el-icon>
              下载结果
            </el-button>
          </div>
        </div>

        <!-- Size Comparison Chart -->
        <div v-if="processedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">尺寸对比</h2>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="text-center">
              <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">原始图片</h3>
              <div class="border border-gray-300 dark:border-gray-600 rounded p-4">
                <img
                  :src="originalImageUrl"
                  alt="原始"
                  class="w-full max-h-32 object-contain"
                />
                <div class="text-xs text-gray-500 dark:text-gray-400 mt-2">
                  {{ imageWidth }} × {{ imageHeight }}
                </div>
              </div>
            </div>
            <div class="text-center">
              <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">处理后</h3>
              <div class="border border-gray-300 dark:border-gray-600 rounded p-4">
                <img
                  :src="processedImageUrl"
                  alt="处理后"
                  class="w-full max-h-32 object-contain"
                />
                <div class="text-xs text-gray-500 dark:text-gray-400 mt-2">
                  {{ actualWidth }} × {{ actualHeight }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { RefreshLeft, FullScreen, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const processedImageUrl = ref('')

const mode = ref('resize')
const quality = ref(90)

const imageWidth = ref(0)
const imageHeight = ref(0)
const actualWidth = ref(0)
const actualHeight = ref(0)

const resizeSettings = ref({
  width: 800,
  height: 600,
  resizeMode: 'KeepAspectRatio'
})

const thumbnailSettings = ref({
  maxSize: 150
})

const sizePresets = [
  { name: '1920×1080', width: 1920, height: 1080 },
  { name: '1280×720', width: 1280, height: 720 },
  { name: '800×600', width: 800, height: 600 },
  { name: '640×480', width: 640, height: 480 }
]

const targetWidth = computed(() => {
  if (mode.value === 'thumbnail') {
    if (!imageWidth.value || !imageHeight.value) return 0
    
    const maxSize = thumbnailSettings.value.maxSize
    if (imageWidth.value > imageHeight.value) {
      return maxSize
    } else {
      return Math.round((maxSize * imageWidth.value) / imageHeight.value)
    }
  } else {
    if (resizeSettings.value.resizeMode === 'KeepAspectRatio') {
      // Calculate based on aspect ratio
      const aspectRatio = imageWidth.value / imageHeight.value
      const targetWidth = resizeSettings.value.width
      const targetHeight = resizeSettings.value.height
      
      if (targetWidth / targetHeight > aspectRatio) {
        return Math.round(targetHeight * aspectRatio)
      } else {
        return targetWidth
      }
    }
    return resizeSettings.value.width
  }
})

const targetHeight = computed(() => {
  if (mode.value === 'thumbnail') {
    if (!imageWidth.value || !imageHeight.value) return 0
    
    const maxSize = thumbnailSettings.value.maxSize
    if (imageHeight.value > imageWidth.value) {
      return maxSize
    } else {
      return Math.round((maxSize * imageHeight.value) / imageWidth.value)
    }
  } else {
    if (resizeSettings.value.resizeMode === 'KeepAspectRatio') {
      // Calculate based on aspect ratio
      const aspectRatio = imageWidth.value / imageHeight.value
      const targetWidth = resizeSettings.value.width
      const targetHeight = resizeSettings.value.height
      
      if (targetWidth / targetHeight > aspectRatio) {
        return targetHeight
      } else {
        return Math.round(targetWidth / aspectRatio)
      }
    }
    return resizeSettings.value.height
  }
})

const scaleRatio = computed(() => {
  if (!imageWidth.value || !imageHeight.value) return 0
  
  const originalArea = imageWidth.value * imageHeight.value
  const targetArea = targetWidth.value * targetHeight.value
  
  return Math.round((targetArea / originalArea) * 100)
})

const estimatedSize = computed(() => {
  if (!selectedFile.value) return '0 KB'
  
  const ratio = scaleRatio.value / 100
  const estimatedBytes = selectedFile.value.size * ratio * (quality.value / 100)
  
  return formatFileSize(estimatedBytes)
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    processedImageUrl.value = ''
  }
}

const handleImageLoad = () => {
  const img = new Image()
  img.onload = () => {
    imageWidth.value = img.width
    imageHeight.value = img.height
    
    // Set default resize dimensions
    resizeSettings.value.width = Math.min(img.width, 800)
    resizeSettings.value.height = Math.min(img.height, 600)
  }
  img.src = originalImageUrl.value
}

const applySizePreset = (preset) => {
  resizeSettings.value.width = preset.width
  resizeSettings.value.height = preset.height
}

const resetSettings = () => {
  resizeSettings.value = {
    width: 800,
    height: 600,
    resizeMode: 'KeepAspectRatio'
  }
  
  thumbnailSettings.value = {
    maxSize: 150
  }
  
  quality.value = 90
}

const processImage = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('请先上传图片')
    return
  }
  
  processing.value = true
  
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('quality', quality.value)
    
    if (mode.value === 'resize') {
      formData.append('width', resizeSettings.value.width)
      formData.append('height', resizeSettings.value.height)
      formData.append('resizeMode', resizeSettings.value.resizeMode)
    } else {
      formData.append('maxSize', thumbnailSettings.value.maxSize)
    }
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, create resized version using canvas
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const img = new Image()
    
    img.onload = () => {
      canvas.width = targetWidth.value
      canvas.height = targetHeight.value
      
      ctx.drawImage(img, 0, 0, targetWidth.value, targetHeight.value)
      
      canvas.toBlob((blob) => {
        processedImageUrl.value = URL.createObjectURL(blob)
        actualWidth.value = targetWidth.value
        actualHeight.value = targetHeight.value
        ElMessage.success(`${mode.value === 'thumbnail' ? '缩略图生成' : '尺寸调整'}完成`)
      }, 'image/jpeg', quality.value / 100)
    }
    
    img.src = originalImageUrl.value
    
  } catch (error) {
    ElMessage.error('处理失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const downloadResult = () => {
  if (processedImageUrl.value) {
    const prefix = mode.value === 'thumbnail' ? 'thumbnail' : 'resized'
    const link = document.createElement('a')
    link.href = processedImageUrl.value
    link.download = `${prefix}_${selectedFile.value.name}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}
</script>

<style scoped>
.resize-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>