<template>
  <div class="compress-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片压缩</h1>
      <p class="text-gray-600 dark:text-gray-300">优化图片文件大小，支持多种压缩级别</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload and Settings Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Compression Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">压缩设置</h2>
          
          <div class="space-y-6">
            <!-- Compression Level -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                压缩级别 ({{ settings.compressionLevel }})
              </label>
              <el-slider
                v-model="settings.compressionLevel"
                :min="1"
                :max="100"
                :step="1"
                show-stops
                show-input
                @change="updateEstimation"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                数值越小压缩越强，文件越小
              </p>
            </div>

            <!-- Quality Preset -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                质量预设
              </label>
              <el-radio-group v-model="qualityPreset" @change="applyPreset">
                <el-radio label="high">高质量 (85-95)</el-radio>
                <el-radio label="medium">中等质量 (65-80)</el-radio>
                <el-radio label="low">低质量 (30-60)</el-radio>
                <el-radio label="custom">自定义</el-radio>
              </el-radio-group>
            </div>

            <!-- Preserve Metadata -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                元数据选项
              </label>
              <el-switch
                v-model="settings.preserveMetadata"
                active-text="保留元数据"
                inactive-text="移除元数据"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                移除元数据可以进一步减小文件大小
              </p>
            </div>
          </div>

          <!-- Compression Preview -->
          <div v-if="originalSize" class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">压缩预估</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始大小：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ formatFileSize(originalSize) }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">预估大小：</span>
                <span class="font-medium text-green-600 dark:text-green-400">{{ formatFileSize(estimatedSize) }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">压缩率：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{ compressionRatio }}%</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">节省空间：</span>
                <span class="font-medium text-purple-600 dark:text-purple-400">{{ formatFileSize(originalSize - estimatedSize) }}</span>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetSettings" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button type="primary" @click="compressImage" :loading="processing">
              <el-icon><RefreshLeft /></el-icon>
              开始压缩
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
            />
            <div class="mt-4 text-sm text-gray-500 dark:text-gray-400">
              <p>{{ selectedFile.name }}</p>
              <p>{{ formatFileSize(selectedFile.size) }}</p>
            </div>
          </div>
        </div>

        <!-- Compressed Result -->
        <div v-if="compressedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">压缩结果</h2>
          
          <div class="text-center mb-4">
            <img
              :src="compressedImageUrl"
              alt="压缩结果"
              class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Comparison Stats -->
          <div class="grid grid-cols-2 gap-4 mb-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="text-center">
              <div class="text-lg font-semibold text-gray-900 dark:text-white">原始</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ formatFileSize(originalSize) }}</div>
            </div>
            <div class="text-center">
              <div class="text-lg font-semibold text-green-600 dark:text-green-400">压缩后</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ formatFileSize(compressedSize) }}</div>
            </div>
          </div>

          <!-- Success Stats -->
          <div class="text-center space-y-2 mb-4">
            <div class="text-2xl font-bold text-green-600 dark:text-green-400">
              减少了 {{ actualCompressionRatio }}%
            </div>
            <div class="text-sm text-gray-500 dark:text-gray-400">
              节省了 {{ formatFileSize(originalSize - compressedSize) }} 空间
            </div>
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadResult">
              <el-icon><Download /></el-icon>
              下载压缩图片
            </el-button>
          </div>
        </div>

        <!-- Quality Comparison -->
        <div v-if="compressedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">质量对比</h2>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="text-center">
              <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">原始图片</h3>
              <img
                :src="originalImageUrl"
                alt="原始"
                class="w-full max-h-48 object-contain rounded border"
              />
            </div>
            <div class="text-center">
              <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">压缩后</h3>
              <img
                :src="compressedImageUrl"
                alt="压缩后"
                class="w-full max-h-48 object-contain rounded border"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { RefreshLeft, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const compressedImageUrl = ref('')
const qualityPreset = ref('custom')

const settings = ref({
  compressionLevel: 75,
  preserveMetadata: false
})

const originalSize = computed(() => selectedFile.value?.size || 0)
const estimatedSize = computed(() => {
  if (!originalSize.value) return 0
  // Simple estimation based on compression level
  const ratio = settings.value.compressionLevel / 100
  return Math.round(originalSize.value * ratio)
})

const compressionRatio = computed(() => {
  if (!originalSize.value) return 0
  return Math.round(((originalSize.value - estimatedSize.value) / originalSize.value) * 100)
})

const compressedSize = ref(0)
const actualCompressionRatio = computed(() => {
  if (!originalSize.value || !compressedSize.value) return 0
  return Math.round(((originalSize.value - compressedSize.value) / originalSize.value) * 100)
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    compressedImageUrl.value = ''
    compressedSize.value = 0
    updateEstimation()
  }
}

const updateEstimation = () => {
  // Trigger reactivity for computed properties
}

const applyPreset = (preset) => {
  switch (preset) {
    case 'high':
      settings.value.compressionLevel = 90
      break
    case 'medium':
      settings.value.compressionLevel = 75
      break
    case 'low':
      settings.value.compressionLevel = 45
      break
    case 'custom':
      // Keep current value
      break
  }
  updateEstimation()
}

const resetSettings = () => {
  settings.value = {
    compressionLevel: 75,
    preserveMetadata: false
  }
  qualityPreset.value = 'custom'
  updateEstimation()
}

const compressImage = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('请先上传图片')
    return
  }
  
  processing.value = true
  
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('compressionLevel', settings.value.compressionLevel)
    formData.append('preserveMetadata', settings.value.preserveMetadata)
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, create a compressed version using canvas
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const img = new Image()
    
    img.onload = () => {
      canvas.width = img.width
      canvas.height = img.height
      ctx.drawImage(img, 0, 0)
      
      // Convert to blob with compression
      canvas.toBlob((blob) => {
        compressedImageUrl.value = URL.createObjectURL(blob)
        compressedSize.value = blob.size
        ElMessage.success('图片压缩完成')
      }, 'image/jpeg', settings.value.compressionLevel / 100)
    }
    
    img.src = originalImageUrl.value
    
  } catch (error) {
    ElMessage.error('压缩失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const downloadResult = () => {
  if (compressedImageUrl.value) {
    const link = document.createElement('a')
    link.href = compressedImageUrl.value
    link.download = `compressed_${selectedFile.value.name}`
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

// Watch for compression level changes to update preset
watch(() => settings.value.compressionLevel, (newLevel) => {
  if (newLevel >= 85) {
    qualityPreset.value = 'high'
  } else if (newLevel >= 65) {
    qualityPreset.value = 'medium'
  } else if (newLevel >= 30) {
    qualityPreset.value = 'low'
  } else {
    qualityPreset.value = 'custom'
  }
})
</script>

<style scoped>
.compress-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>