<template>
  <div class="color-adjust-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片颜色调整</h1>
      <p class="text-gray-600 dark:text-gray-300">调整图片的亮度、对比度、饱和度、色相和伽马值</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Controls Section -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">调整参数</h2>
          
          <div class="space-y-6">
            <!-- Brightness -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                亮度 ({{ adjustments.brightness }})
              </label>
              <el-slider
                v-model="adjustments.brightness"
                :min="-100"
                :max="100"
                :step="1"
                show-stops
                show-input
                @change="previewAdjustment"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                -100 到 100，0 为不调整
              </p>
            </div>

            <!-- Contrast -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                对比度 ({{ adjustments.contrast }})
              </label>
              <el-slider
                v-model="adjustments.contrast"
                :min="-100"
                :max="100"
                :step="1"
                show-stops
                show-input
                @change="previewAdjustment"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                -100 到 100，0 为不调整
              </p>
            </div>

            <!-- Saturation -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                饱和度 ({{ adjustments.saturation }})
              </label>
              <el-slider
                v-model="adjustments.saturation"
                :min="-100"
                :max="100"
                :step="1"
                show-stops
                show-input
                @change="previewAdjustment"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                -100 到 100，0 为不调整
              </p>
            </div>

            <!-- Hue -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                色相 ({{ adjustments.hue }})
              </label>
              <el-slider
                v-model="adjustments.hue"
                :min="-180"
                :max="180"
                :step="1"
                show-stops
                show-input
                @change="previewAdjustment"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                -180 到 180，0 为不调整
              </p>
            </div>

            <!-- Gamma -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                伽马值 ({{ adjustments.gamma }})
              </label>
              <el-slider
                v-model="adjustments.gamma"
                :min="0.1"
                :max="3.0"
                :step="0.1"
                show-stops
                show-input
                @change="previewAdjustment"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                0.1 到 3.0，1.0 为不调整
              </p>
            </div>

            <!-- Quality -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                输出质量 ({{ adjustments.quality }})
              </label>
              <el-slider
                v-model="adjustments.quality"
                :min="1"
                :max="100"
                :step="1"
                show-stops
                show-input
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                1 到 100，数值越高质量越好
              </p>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetAdjustments" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button type="primary" @click="processImage" :loading="processing">
              <el-icon><Check /></el-icon>
              应用调整
            </el-button>
          </div>
        </div>
      </div>

      <!-- Preview Section -->
      <div v-if="selectedFile" class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">预览效果</h2>
          
          <!-- Preview Toggle -->
          <div class="mb-4">
            <el-radio-group v-model="previewMode" @change="updatePreview">
              <el-radio label="original">原图</el-radio>
              <el-radio label="adjusted">调整后</el-radio>
              <el-radio label="split">对比</el-radio>
            </el-radio-group>
          </div>

          <!-- Image Preview -->
          <div class="relative bg-gray-100 dark:bg-gray-700 rounded-lg overflow-hidden" style="min-height: 300px;">
            <canvas
              ref="previewCanvas"
              class="max-w-full max-h-96 mx-auto block"
              :style="canvasStyle"
            />
            
            <!-- Split View Divider -->
            <div
              v-if="previewMode === 'split'"
              class="absolute top-0 left-1/2 w-0.5 h-full bg-white shadow-lg transform -translate-x-0.5"
            />
          </div>

          <!-- Preview Info -->
          <div class="mt-4 text-sm text-gray-500 dark:text-gray-400 text-center">
            <p v-if="previewMode === 'split'">左侧：原图 | 右侧：调整后</p>
            <p v-else-if="previewMode === 'original'">当前显示：原图</p>
            <p v-else>当前显示：调整后效果</p>
          </div>
        </div>

        <!-- Result Section -->
        <div v-if="processedImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">处理结果</h2>
          
          <div class="text-center">
            <img
              :src="processedImageUrl"
              alt="处理结果"
              class="max-h-64 mx-auto rounded-lg shadow-md mb-4"
            />
            <el-button type="success" @click="downloadResult">
              <el-icon><Download /></el-icon>
              下载结果
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { RefreshLeft, Check, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const processing = ref(false)
const previewCanvas = ref(null)
const previewMode = ref('original')
const processedImageUrl = ref('')

const adjustments = ref({
  brightness: 0,
  contrast: 0,
  saturation: 0,
  hue: 0,
  gamma: 1.0,
  quality: 90
})

const canvasStyle = ref({})

let originalImageData = null
let adjustedImageData = null

const handleFileChange = (file) => {
  if (file) {
    loadImageToCanvas(file)
    processedImageUrl.value = ''
  }
}

const loadImageToCanvas = (file) => {
  const reader = new FileReader()
  reader.onload = (e) => {
    const img = new Image()
    img.onload = () => {
      nextTick(() => {
        if (previewCanvas.value) {
          const canvas = previewCanvas.value
          const ctx = canvas.getContext('2d')
          
          // Set canvas size
          const maxWidth = 600
          const maxHeight = 400
          let { width, height } = img
          
          if (width > maxWidth || height > maxHeight) {
            const ratio = Math.min(maxWidth / width, maxHeight / height)
            width *= ratio
            height *= ratio
          }
          
          canvas.width = width
          canvas.height = height
          canvasStyle.value = { width: width + 'px', height: height + 'px' }
          
          // Draw original image
          ctx.drawImage(img, 0, 0, width, height)
          
          // Store original image data
          originalImageData = ctx.getImageData(0, 0, width, height)
          adjustedImageData = ctx.getImageData(0, 0, width, height)
          
          updatePreview()
        }
      })
    }
    img.src = e.target.result
  }
  reader.readAsDataURL(file)
}

const previewAdjustment = () => {
  if (!originalImageData) return
  
  // Apply adjustments to image data (simplified preview)
  const imageData = new ImageData(
    new Uint8ClampedArray(originalImageData.data),
    originalImageData.width,
    originalImageData.height
  )
  
  // Apply color adjustments (simplified)
  const data = imageData.data
  for (let i = 0; i < data.length; i += 4) {
    let r = data[i]
    let g = data[i + 1]
    let b = data[i + 2]
    
    // Brightness
    r += adjustments.value.brightness * 2.55
    g += adjustments.value.brightness * 2.55
    b += adjustments.value.brightness * 2.55
    
    // Contrast
    const contrast = (adjustments.value.contrast + 100) / 100
    r = ((r / 255 - 0.5) * contrast + 0.5) * 255
    g = ((g / 255 - 0.5) * contrast + 0.5) * 255
    b = ((b / 255 - 0.5) * contrast + 0.5) * 255
    
    // Gamma
    const gamma = adjustments.value.gamma
    r = Math.pow(r / 255, 1 / gamma) * 255
    g = Math.pow(g / 255, 1 / gamma) * 255
    b = Math.pow(b / 255, 1 / gamma) * 255
    
    data[i] = Math.max(0, Math.min(255, r))
    data[i + 1] = Math.max(0, Math.min(255, g))
    data[i + 2] = Math.max(0, Math.min(255, b))
  }
  
  adjustedImageData = imageData
  updatePreview()
}

const updatePreview = () => {
  if (!previewCanvas.value || !originalImageData) return
  
  const canvas = previewCanvas.value
  const ctx = canvas.getContext('2d')
  
  if (previewMode.value === 'original') {
    ctx.putImageData(originalImageData, 0, 0)
  } else if (previewMode.value === 'adjusted') {
    ctx.putImageData(adjustedImageData, 0, 0)
  } else if (previewMode.value === 'split') {
    // Left half: original
    const leftHalf = ctx.createImageData(canvas.width / 2, canvas.height)
    const originalData = originalImageData.data
    const leftData = leftHalf.data
    
    for (let y = 0; y < canvas.height; y++) {
      for (let x = 0; x < canvas.width / 2; x++) {
        const sourceIndex = (y * canvas.width + x) * 4
        const targetIndex = (y * (canvas.width / 2) + x) * 4
        
        leftData[targetIndex] = originalData[sourceIndex]
        leftData[targetIndex + 1] = originalData[sourceIndex + 1]
        leftData[targetIndex + 2] = originalData[sourceIndex + 2]
        leftData[targetIndex + 3] = originalData[sourceIndex + 3]
      }
    }
    
    // Right half: adjusted
    const rightHalf = ctx.createImageData(canvas.width / 2, canvas.height)
    const adjustedData = adjustedImageData.data
    const rightData = rightHalf.data
    
    for (let y = 0; y < canvas.height; y++) {
      for (let x = 0; x < canvas.width / 2; x++) {
        const sourceIndex = (y * canvas.width + (x + canvas.width / 2)) * 4
        const targetIndex = (y * (canvas.width / 2) + x) * 4
        
        rightData[targetIndex] = adjustedData[sourceIndex]
        rightData[targetIndex + 1] = adjustedData[sourceIndex + 1]
        rightData[targetIndex + 2] = adjustedData[sourceIndex + 2]
        rightData[targetIndex + 3] = adjustedData[sourceIndex + 3]
      }
    }
    
    ctx.putImageData(leftHalf, 0, 0)
    ctx.putImageData(rightHalf, canvas.width / 2, 0)
  }
}

const resetAdjustments = () => {
  adjustments.value = {
    brightness: 0,
    contrast: 0,
    saturation: 0,
    hue: 0,
    gamma: 1.0,
    quality: 90
  }
  previewAdjustment()
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
    formData.append('brightness', adjustments.value.brightness)
    formData.append('contrast', adjustments.value.contrast)
    formData.append('saturation', adjustments.value.saturation)
    formData.append('hue', adjustments.value.hue)
    formData.append('gamma', adjustments.value.gamma)
    formData.append('quality', adjustments.value.quality)
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, use the adjusted preview as result
    const canvas = previewCanvas.value
    canvas.toBlob((blob) => {
      processedImageUrl.value = URL.createObjectURL(blob)
      ElMessage.success('颜色调整完成')
    }, 'image/jpeg', adjustments.value.quality / 100)
    
  } catch (error) {
    ElMessage.error('处理失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const downloadResult = () => {
  if (processedImageUrl.value) {
    const link = document.createElement('a')
    link.href = processedImageUrl.value
    link.download = `color_adjusted_${selectedFile.value.name}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}
</script>

<style scoped>
.color-adjust-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>