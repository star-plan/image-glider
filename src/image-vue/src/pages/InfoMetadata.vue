<template>
  <div class="info-metadata-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片信息</h1>
      <p class="text-gray-600 dark:text-gray-300">查看图片详细信息和清理元数据</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Metadata Cleanup Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">元数据清理</h2>
          
          <div class="space-y-4">
            <!-- Strip All -->
            <div class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <div>
                <div class="font-medium text-gray-900 dark:text-white">清理所有元数据</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">移除所有EXIF、ICC、XMP等数据</div>
              </div>
              <el-switch v-model="cleanupSettings.stripAll" />
            </div>

            <!-- Individual Options -->
            <div v-if="!cleanupSettings.stripAll" class="space-y-3 ml-4">
              <div class="flex items-center justify-between">
                <div>
                  <div class="font-medium text-gray-900 dark:text-white">EXIF 数据</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">相机设置、拍摄信息等</div>
                </div>
                <el-switch v-model="cleanupSettings.stripExif" />
              </div>

              <div class="flex items-center justify-between">
                <div>
                  <div class="font-medium text-gray-900 dark:text-white">ICC 配置文件</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">颜色配置文件</div>
                </div>
                <el-switch v-model="cleanupSettings.stripIcc" />
              </div>

              <div class="flex items-center justify-between">
                <div>
                  <div class="font-medium text-gray-900 dark:text-white">XMP 数据</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">扩展元数据</div>
                </div>
                <el-switch v-model="cleanupSettings.stripXmp" />
              </div>
            </div>

            <!-- Quality Setting -->
            <div class="mt-6">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                输出质量 ({{ cleanupSettings.quality }})
              </label>
              <el-slider
                v-model="cleanupSettings.quality"
                :min="1"
                :max="100"
                :step="1"
                show-stops
                show-input
              />
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetCleanupSettings" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button type="primary" @click="cleanMetadata" :loading="processing">
              <el-icon><Delete /></el-icon>
              清理元数据
            </el-button>
          </div>
        </div>
      </div>

      <!-- Info and Result Section -->
      <div v-if="selectedFile" class="space-y-6">
        <!-- Image Preview -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">图片预览</h2>
          <div class="text-center">
            <img
              :src="originalImageUrl"
              alt="图片预览"
              class="max-h-64 mx-auto rounded-lg shadow-md"
              @load="analyzeImage"
            />
          </div>
        </div>

        <!-- Basic Info -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">基本信息</h2>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="space-y-3">
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">文件名：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ selectedFile.name }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">文件大小：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ formatFileSize(selectedFile.size) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">文件类型：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ selectedFile.type }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">最后修改：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ formatDate(selectedFile.lastModified) }}</span>
              </div>
            </div>
            
            <div class="space-y-3">
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">图片尺寸：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ imageInfo.width }} × {{ imageInfo.height }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">宽高比：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ aspectRatio }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">像素总数：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ totalPixels }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">颜色深度：</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ colorDepth }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Detailed Analysis -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">详细分析</h2>
          
          <!-- Color Analysis -->
          <div class="mb-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-3">颜色分析</h3>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="text-2xl font-bold text-red-500">{{ colorAnalysis.red }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">红色均值</div>
              </div>
              <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="text-2xl font-bold text-green-500">{{ colorAnalysis.green }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">绿色均值</div>
              </div>
              <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="text-2xl font-bold text-blue-500">{{ colorAnalysis.blue }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">蓝色均值</div>
              </div>
              <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="text-2xl font-bold text-gray-600 dark:text-gray-400">{{ colorAnalysis.brightness }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">亮度</div>
              </div>
            </div>
          </div>

          <!-- Format Analysis -->
          <div class="mb-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-3">格式特性</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="font-medium text-gray-900 dark:text-white mb-2">压缩类型</div>
                <div class="text-sm text-gray-600 dark:text-gray-300">{{ compressionType }}</div>
              </div>
              <div class="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="font-medium text-gray-900 dark:text-white mb-2">透明度支持</div>
                <div class="text-sm text-gray-600 dark:text-gray-300">{{ hasTransparency ? '支持' : '不支持' }}</div>
              </div>
            </div>
          </div>

          <!-- Metadata Preview -->
          <div>
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-3">元数据预览</h3>
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4 max-h-40 overflow-y-auto">
              <div v-if="hasMetadata" class="space-y-2 text-sm">
                <div class="flex justify-between">
                  <span class="text-gray-500 dark:text-gray-400">EXIF数据：</span>
                  <span class="text-green-600 dark:text-green-400">检测到</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-500 dark:text-gray-400">ICC配置：</span>
                  <span class="text-green-600 dark:text-green-400">检测到</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-500 dark:text-gray-400">XMP数据：</span>
                  <span class="text-yellow-600 dark:text-yellow-400">可能存在</span>
                </div>
              </div>
              <div v-else class="text-gray-500 dark:text-gray-400 text-sm">
                未检测到明显的元数据信息
              </div>
            </div>
          </div>
        </div>

        <!-- Cleanup Result -->
        <div v-if="cleanedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">清理结果</h2>
          
          <div class="text-center mb-4">
            <img
              :src="cleanedImageUrl"
              alt="清理后的图片"
              class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Comparison Stats -->
          <div class="grid grid-cols-2 gap-4 mb-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="text-center">
              <div class="text-lg font-semibold text-gray-900 dark:text-white">原始文件</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ formatFileSize(selectedFile.size) }}</div>
            </div>
            <div class="text-center">
              <div class="text-lg font-semibold text-green-600 dark:text-green-400">清理后</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ formatFileSize(cleanedSize) }}</div>
            </div>
          </div>

          <!-- Size Reduction -->
          <div class="text-center mb-4">
            <div class="text-lg font-medium text-gray-700 dark:text-gray-300">
              文件大小减少：
              <span class="text-green-600 dark:text-green-400 font-bold">
                {{ sizeReduction }}
              </span>
            </div>
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadCleaned">
              <el-icon><Download /></el-icon>
              下载清理后的图片
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { RefreshLeft, Delete, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const cleanedImageUrl = ref('')
const cleanedSize = ref(0)

const imageInfo = ref({
  width: 0,
  height: 0
})

const colorAnalysis = ref({
  red: 0,
  green: 0,
  blue: 0,
  brightness: 0
})

const cleanupSettings = ref({
  stripAll: true,
  stripExif: true,
  stripIcc: false,
  stripXmp: true,
  quality: 90
})

const aspectRatio = computed(() => {
  if (!imageInfo.value.width || !imageInfo.value.height) return '未知'
  const gcd = (a, b) => b === 0 ? a : gcd(b, a % b)
  const divisor = gcd(imageInfo.value.width, imageInfo.value.height)
  const ratioW = imageInfo.value.width / divisor
  const ratioH = imageInfo.value.height / divisor
  return `${ratioW}:${ratioH}`
})

const totalPixels = computed(() => {
  const total = imageInfo.value.width * imageInfo.value.height
  if (total > 1000000) {
    return `${(total / 1000000).toFixed(1)}M`
  }
  return total.toLocaleString()
})

const colorDepth = computed(() => {
  // Simplified color depth detection
  return '24位 (RGB)'
})

const compressionType = computed(() => {
  if (!selectedFile.value) return '未知'
  const type = selectedFile.value.type.toLowerCase()
  if (type.includes('jpeg')) return '有损压缩 (JPEG)'
  if (type.includes('png')) return '无损压缩 (PNG)'
  if (type.includes('webp')) return '现代压缩 (WebP)'
  if (type.includes('gif')) return 'LZW压缩 (GIF)'
  return '未知压缩'
})

const hasTransparency = computed(() => {
  if (!selectedFile.value) return false
  const type = selectedFile.value.type.toLowerCase()
  return type.includes('png') || type.includes('gif') || type.includes('webp')
})

const hasMetadata = computed(() => {
  // Simplified metadata detection based on file type and size
  if (!selectedFile.value) return false
  const type = selectedFile.value.type.toLowerCase()
  return type.includes('jpeg') && selectedFile.value.size > 100000
})

const sizeReduction = computed(() => {
  if (!cleanedSize.value || !selectedFile.value) return '0%'
  const reduction = selectedFile.value.size - cleanedSize.value
  const percentage = Math.round((reduction / selectedFile.value.size) * 100)
  if (percentage > 0) {
    return `${percentage}% (${formatFileSize(reduction)})`
  }
  return '无变化'
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    cleanedImageUrl.value = ''
    cleanedSize.value = 0
  }
}

const analyzeImage = () => {
  const img = new Image()
  img.onload = () => {
    imageInfo.value.width = img.width
    imageInfo.value.height = img.height
    
    // Perform color analysis using canvas
    analyzeColors(img)
  }
  img.src = originalImageUrl.value
}

const analyzeColors = (img) => {
  const canvas = document.createElement('canvas')
  const ctx = canvas.getContext('2d')
  
  // Use a smaller sample for performance
  const sampleWidth = Math.min(img.width, 100)
  const sampleHeight = Math.min(img.height, 100)
  
  canvas.width = sampleWidth
  canvas.height = sampleHeight
  
  ctx.drawImage(img, 0, 0, sampleWidth, sampleHeight)
  
  const imageData = ctx.getImageData(0, 0, sampleWidth, sampleHeight)
  const data = imageData.data
  
  let totalRed = 0, totalGreen = 0, totalBlue = 0, totalBrightness = 0
  const pixelCount = sampleWidth * sampleHeight
  
  for (let i = 0; i < data.length; i += 4) {
    const r = data[i]
    const g = data[i + 1]
    const b = data[i + 2]
    
    totalRed += r
    totalGreen += g
    totalBlue += b
    totalBrightness += (r + g + b) / 3
  }
  
  colorAnalysis.value = {
    red: Math.round(totalRed / pixelCount),
    green: Math.round(totalGreen / pixelCount),
    blue: Math.round(totalBlue / pixelCount),
    brightness: Math.round(totalBrightness / pixelCount)
  }
}

const resetCleanupSettings = () => {
  cleanupSettings.value = {
    stripAll: true,
    stripExif: true,
    stripIcc: false,
    stripXmp: true,
    quality: 90
  }
}

const cleanMetadata = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('请先上传图片')
    return
  }
  
  processing.value = true
  
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('stripAll', cleanupSettings.value.stripAll)
    formData.append('stripExif', cleanupSettings.value.stripExif)
    formData.append('stripIcc', cleanupSettings.value.stripIcc)
    formData.append('stripXmp', cleanupSettings.value.stripXmp)
    formData.append('quality', cleanupSettings.value.quality)
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, create cleaned version using canvas
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const img = new Image()
    
    img.onload = () => {
      canvas.width = img.width
      canvas.height = img.height
      ctx.drawImage(img, 0, 0)
      
      canvas.toBlob((blob) => {
        cleanedImageUrl.value = URL.createObjectURL(blob)
        cleanedSize.value = blob.size
        ElMessage.success('元数据清理完成')
      }, 'image/jpeg', cleanupSettings.value.quality / 100)
    }
    
    img.src = originalImageUrl.value
    
  } catch (error) {
    ElMessage.error('清理失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const downloadCleaned = () => {
  if (cleanedImageUrl.value) {
    const link = document.createElement('a')
    link.href = cleanedImageUrl.value
    link.download = `cleaned_${selectedFile.value.name}`
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

const formatDate = (timestamp) => {
  return new Date(timestamp).toLocaleString('zh-CN')
}
</script>

<style scoped>
.info-metadata-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>