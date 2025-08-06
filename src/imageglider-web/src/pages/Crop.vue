<template>
  <div class="crop-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片裁剪</h1>
      <p class="text-gray-600 dark:text-gray-300">精确裁剪、中心裁剪或按百分比裁剪图片</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload and Settings Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Crop Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">裁剪设置</h2>
          
          <!-- Crop Mode Selection -->
          <div class="mb-6">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
              裁剪模式
            </label>
            <el-radio-group v-model="cropMode" @change="resetCropSettings">
              <el-radio label="precise">精确裁剪</el-radio>
              <el-radio label="center">中心裁剪</el-radio>
              <el-radio label="percent">百分比裁剪</el-radio>
            </el-radio-group>
          </div>

          <!-- Precise Crop Settings -->
          <div v-if="cropMode === 'precise'" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  X 坐标 (px)
                </label>
                <el-input-number
                  v-model="preciseSettings.x"
                  :min="0"
                  :max="Math.max(0, imageWidth - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Y 坐标 (px)
                </label>
                <el-input-number
                  v-model="preciseSettings.y"
                  :min="0"
                  :max="Math.max(0, imageHeight - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  宽度 (px)
                </label>
                <el-input-number
                  v-model="preciseSettings.width"
                  :min="0"
                  :max="Math.max(0, imageWidth - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  高度 (px)
                </label>
                <el-input-number
                  v-model="preciseSettings.height"
                  :min="0"
                  :max="Math.max(0, imageHeight - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
            </div>
          </div>

          <!-- Center Crop Settings -->
          <div v-if="cropMode === 'center'" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  宽度 (px)
                </label>
                <el-input-number
                  v-model="centerSettings.width"
                  :min="0"
                  :max="Math.max(0, imageWidth - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  高度 (px)
                </label>
                <el-input-number
                  v-model="centerSettings.height"
                  :min="0"
                  :max="Math.max(0, imageHeight - 1)"
                  size="small"
                  class="w-full"
                />
              </div>
            </div>
            <div class="text-sm text-gray-500 dark:text-gray-400">
              从图片中心开始裁剪指定尺寸
            </div>
          </div>

          <!-- Percent Crop Settings -->
          <div v-if="cropMode === 'percent'" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  X 位置 (%)
                </label>
                <el-input-number
                  v-model="percentSettings.xPercent"
                  :min="0"
                  :max="100"
                  :precision="1"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Y 位置 (%)
                </label>
                <el-input-number
                  v-model="percentSettings.yPercent"
                  :min="0"
                  :max="100"
                  :precision="1"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  宽度 (%)
                </label>
                <el-input-number
                  v-model="percentSettings.widthPercent"
                  :min="1"
                  :max="100"
                  :precision="1"
                  size="small"
                  class="w-full"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  高度 (%)
                </label>
                <el-input-number
                  v-model="percentSettings.heightPercent"
                  :min="1"
                  :max="100"
                  :precision="1"
                  size="small"
                  class="w-full"
                />
              </div>
            </div>
          </div>

          <!-- Common Settings -->
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

          <!-- Crop Preview Info -->
          <div v-if="cropPreview" class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">裁剪预览</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始尺寸：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ imageWidth }} × {{ imageHeight }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">裁剪尺寸：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{ cropPreview.width }} × {{ cropPreview.height }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">裁剪位置：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">({{ cropPreview.x }}, {{ cropPreview.y }})</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">保留比例：</span>
                <span class="font-medium text-green-600 dark:text-green-400">{{ cropRatio }}%</span>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetCropSettings" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button type="primary" @click="cropImage" :loading="processing">
              <el-icon><Crop /></el-icon>
              开始裁剪
            </el-button>
          </div>
        </div>
      </div>

      <!-- Preview and Result Section -->
      <div v-if="selectedFile" class="space-y-6">
        <!-- Original Image Preview with Crop Overlay -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">原始图片</h2>
          <div class="relative inline-block">
            <img
              ref="originalImage"
              :src="originalImageUrl"
              alt="原始图片"
              class="max-h-96 rounded-lg shadow-md"
              @load="handleImageLoad"
            />
            <!-- Crop Overlay -->
            <div
              v-if="cropPreview && showOverlay"
              class="absolute border-2 border-red-500 bg-red-500 bg-opacity-20"
              :style="overlayStyle"
            />
          </div>
          <div class="mt-4 flex items-center justify-between">
            <div class="text-sm text-gray-500 dark:text-gray-400">
              {{ selectedFile.name }} • {{ imageWidth }} × {{ imageHeight }}
            </div>
            <el-switch
              v-model="showOverlay"
              active-text="显示裁剪区域"
              inactive-text="隐藏裁剪区域"
              size="small"
            />
          </div>
        </div>

        <!-- Cropped Result -->
        <div v-if="croppedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">裁剪结果</h2>
          
          <div class="text-center mb-4">
            <img
              :src="croppedImageUrl"
              alt="裁剪结果"
              class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Result Stats -->
          <div class="grid grid-cols-2 gap-4 mb-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="text-center">
              <div class="text-lg font-semibold text-gray-900 dark:text-white">原始</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ imageWidth }} × {{ imageHeight }}</div>
            </div>
            <div class="text-center">
              <div class="text-lg font-semibold text-blue-600 dark:text-blue-400">裁剪后</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ cropPreview?.width }} × {{ cropPreview?.height }}</div>
            </div>
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadResult">
              <el-icon><Download /></el-icon>
              下载裁剪结果
            </el-button>
          </div>
        </div>

        <!-- Quick Crop Presets -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">快速裁剪</h2>
          
          <div class="grid grid-cols-2 gap-3">
            <el-button
              v-for="preset in cropPresets"
              :key="preset.name"
              @click="applyCropPreset(preset)"
              size="small"
            >
              {{ preset.name }}
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { RefreshLeft, Crop, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const croppedImageUrl = ref('')
const originalImage = ref(null)
const showOverlay = ref(true)

const cropMode = ref('precise')
const quality = ref(90)

const imageWidth = ref(0)
const imageHeight = ref(0)

const preciseSettings = ref({
  x: 0,
  y: 0,
  width: 100,
  height: 100
})

const centerSettings = ref({
  width: 100,
  height: 100
})

const percentSettings = ref({
  xPercent: 10,
  yPercent: 10,
  widthPercent: 80,
  heightPercent: 80
})

const cropPresets = [
  { name: '1:1 正方形', ratio: 1, mode: 'center' },
  { name: '16:9 宽屏', ratio: 16/9, mode: 'center' },
  { name: '4:3 标准', ratio: 4/3, mode: 'center' },
  { name: '3:2 照片', ratio: 3/2, mode: 'center' }
]

const cropPreview = computed(() => {
  if (!imageWidth.value || !imageHeight.value) return null
  
  let x, y, width, height
  
  if (cropMode.value === 'precise') {
    x = preciseSettings.value.x
    y = preciseSettings.value.y
    width = preciseSettings.value.width
    height = preciseSettings.value.height
  } else if (cropMode.value === 'center') {
    width = centerSettings.value.width
    height = centerSettings.value.height
    x = Math.max(0, (imageWidth.value - width) / 2)
    y = Math.max(0, (imageHeight.value - height) / 2)
  } else if (cropMode.value === 'percent') {
    x = Math.round((percentSettings.value.xPercent / 100) * imageWidth.value)
    y = Math.round((percentSettings.value.yPercent / 100) * imageHeight.value)
    width = Math.round((percentSettings.value.widthPercent / 100) * imageWidth.value)
    height = Math.round((percentSettings.value.heightPercent / 100) * imageHeight.value)
  }
  
  // Ensure crop area is within image bounds
  x = Math.max(0, Math.min(x, imageWidth.value - 1))
  y = Math.max(0, Math.min(y, imageHeight.value - 1))
  width = Math.max(1, Math.min(width, imageWidth.value - x))
  height = Math.max(1, Math.min(height, imageHeight.value - y))
  
  return { x, y, width, height }
})

const cropRatio = computed(() => {
  if (!cropPreview.value || !imageWidth.value || !imageHeight.value) return 0
  const originalArea = imageWidth.value * imageHeight.value
  const cropArea = cropPreview.value.width * cropPreview.value.height
  return Math.round((cropArea / originalArea) * 100)
})

const overlayStyle = computed(() => {
  if (!cropPreview.value || !originalImage.value) return {}
  
  const img = originalImage.value
  const imgRect = img.getBoundingClientRect()
  const scaleX = img.offsetWidth / imageWidth.value
  const scaleY = img.offsetHeight / imageHeight.value
  
  return {
    left: cropPreview.value.x * scaleX + 'px',
    top: cropPreview.value.y * scaleY + 'px',
    width: cropPreview.value.width * scaleX + 'px',
    height: cropPreview.value.height * scaleY + 'px'
  }
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    croppedImageUrl.value = ''
  }
}

const handleImageLoad = () => {
  if (originalImage.value) {
    const img = new Image()
    img.onload = () => {
      imageWidth.value = img.width
      imageHeight.value = img.height
      resetCropSettings()
    }
    img.src = originalImageUrl.value
  }
}

const resetCropSettings = () => {
  if (imageWidth.value && imageHeight.value) {
    preciseSettings.value = {
      x: 0,
      y: 0,
      width: Math.min(imageWidth.value, 200),
      height: Math.min(imageHeight.value, 200)
    }
    
    centerSettings.value = {
      width: Math.min(imageWidth.value, 200),
      height: Math.min(imageHeight.value, 200)
    }
    
    percentSettings.value = {
      xPercent: 10,
      yPercent: 10,
      widthPercent: 80,
      heightPercent: 80
    }
  }
}

const applyCropPreset = (preset) => {
  if (!imageWidth.value || !imageHeight.value) return
  
  cropMode.value = 'center'
  
  let width, height
  if (preset.ratio > 1) {
    // Landscape
    width = Math.min(imageWidth.value, 400)
    height = Math.round(width / preset.ratio)
  } else {
    // Portrait or square
    height = Math.min(imageHeight.value, 400)
    width = Math.round(height * preset.ratio)
  }
  
  centerSettings.value = { width, height }
}

const cropImage = async () => {
  if (!selectedFile.value || !cropPreview.value) {
    ElMessage.warning('请先上传图片并设置裁剪区域')
    return
  }
  
  processing.value = true
  
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    
    if (cropMode.value === 'precise') {
      formData.append('x', cropPreview.value.x)
      formData.append('y', cropPreview.value.y)
      formData.append('width', cropPreview.value.width)
      formData.append('height', cropPreview.value.height)
    } else if (cropMode.value === 'center') {
      formData.append('width', cropPreview.value.width)
      formData.append('height', cropPreview.value.height)
    } else if (cropMode.value === 'percent') {
      formData.append('xPercent', percentSettings.value.xPercent)
      formData.append('yPercent', percentSettings.value.yPercent)
      formData.append('widthPercent', percentSettings.value.widthPercent)
      formData.append('heightPercent', percentSettings.value.heightPercent)
    }
    
    formData.append('quality', quality.value)
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, create cropped version using canvas
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const img = new Image()
    
    img.onload = () => {
      canvas.width = cropPreview.value.width
      canvas.height = cropPreview.value.height
      
      ctx.drawImage(
        img,
        cropPreview.value.x,
        cropPreview.value.y,
        cropPreview.value.width,
        cropPreview.value.height,
        0,
        0,
        cropPreview.value.width,
        cropPreview.value.height
      )
      
      canvas.toBlob((blob) => {
        croppedImageUrl.value = URL.createObjectURL(blob)
        ElMessage.success('图片裁剪完成')
      }, 'image/jpeg', quality.value / 100)
    }
    
    img.src = originalImageUrl.value
    
  } catch (error) {
    ElMessage.error('裁剪失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const downloadResult = () => {
  if (croppedImageUrl.value) {
    const link = document.createElement('a')
    link.href = croppedImageUrl.value
    link.download = `cropped_${selectedFile.value.name}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}
</script>

<style scoped>
.crop-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>