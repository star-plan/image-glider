<template>
  <div class="watermark-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">添加水印</h1>
      <p class="text-gray-600 dark:text-gray-300">为图片添加文本或图片水印</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Upload and Settings Section -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">上传图片</h2>
          <ImageUpload v-model="selectedFile" @change="handleFileChange" />
        </div>

        <!-- Watermark Settings -->
        <div v-if="selectedFile" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">水印设置</h2>
          
          <!-- Watermark Type -->
          <div class="mb-6">
            <el-radio-group v-model="watermarkType">
              <el-radio label="text">文本水印</el-radio>
              <el-radio label="image">图片水印</el-radio>
            </el-radio-group>
          </div>

          <!-- Text Watermark Settings -->
          <div v-if="watermarkType === 'text'" class="space-y-6">
            <!-- Text Content -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                水印文本
              </label>
              <el-input
                v-model="textSettings.text"
                placeholder="请输入水印文本"
                maxlength="50"
                show-word-limit
              />
            </div>

            <!-- Font Settings -->
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  字体大小 ({{ textSettings.fontSize }}px)
                </label>
                <el-slider
                  v-model="textSettings.fontSize"
                  :min="12"
                  :max="72"
                  :step="1"
                  show-stops
                  show-input
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  字体颜色
                </label>
                <el-color-picker v-model="textSettings.fontColor" />
              </div>
            </div>
          </div>

          <!-- Image Watermark Settings -->
          <div v-if="watermarkType === 'image'" class="space-y-6">
            <!-- Watermark Image Upload -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                水印图片
              </label>
              <ImageUpload 
                v-model="watermarkFile" 
                @change="handleWatermarkFileChange"
                :show-info="false"
              />
            </div>

            <!-- Scale Setting -->
            <div v-if="watermarkFile">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                缩放比例 ({{ imageSettings.scale }})
              </label>
              <el-slider
                v-model="imageSettings.scale"
                :min="0.1"
                :max="2.0"
                :step="0.1"
                show-stops
                show-input
              />
            </div>
          </div>

          <!-- Common Settings -->
          <div class="space-y-6 mt-6">
            <!-- Position -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">
                水印位置
              </label>
              <div class="grid grid-cols-3 gap-2">
                <el-button
                  v-for="pos in positions"
                  :key="pos.value"
                  :type="position === pos.value ? 'primary' : ''"
                  @click="position = pos.value"
                  size="small"
                >
                  {{ pos.name }}
                </el-button>
              </div>
            </div>

            <!-- Opacity -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                透明度 ({{ opacity }}%)
              </label>
              <el-slider
                v-model="opacity"
                :min="10"
                :max="100"
                :step="5"
                show-stops
                show-input
              />
            </div>

            <!-- Quality -->
            <div>
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
          </div>

          <!-- Preview Settings -->
          <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">水印预览</h4>
            <div class="text-sm space-y-1">
              <div v-if="watermarkType === 'text'">
                <span class="text-gray-500 dark:text-gray-400">文本：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ textSettings.text || '无' }}</span>
              </div>
              <div v-if="watermarkType === 'image'">
                <span class="text-gray-500 dark:text-gray-400">图片：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ watermarkFile?.name || '未选择' }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">位置：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{ positionName }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">透明度：</span>
                <span class="font-medium text-green-600 dark:text-green-400">{{ opacity }}%</span>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 mt-6">
            <el-button @click="resetSettings" :disabled="processing">
              <el-icon><RefreshLeft /></el-icon>
              重置
            </el-button>
            <el-button 
              type="primary" 
              @click="addWatermark" 
              :loading="processing"
              :disabled="!canProcess"
            >
              <el-icon><PictureRounded /></el-icon>
              添加水印
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
              {{ selectedFile.name }}
            </div>
          </div>
        </div>

        <!-- Watermark Preview -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">水印预览</h2>
          
          <div class="relative inline-block">
            <img
              ref="previewImage"
              :src="originalImageUrl"
              alt="预览"
              class="max-h-64 rounded-lg shadow-md"
            />
            
            <!-- Text Watermark Preview -->
            <div
              v-if="watermarkType === 'text' && textSettings.text"
              class="absolute pointer-events-none"
              :style="textWatermarkStyle"
            >
              {{ textSettings.text }}
            </div>

            <!-- Image Watermark Preview -->
            <img
              v-if="watermarkType === 'image' && watermarkImageUrl"
              :src="watermarkImageUrl"
              alt="水印"
              class="absolute pointer-events-none"
              :style="imageWatermarkStyle"
            />
          </div>
        </div>

        <!-- Result -->
        <div v-if="resultImageUrl" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">添加结果</h2>
          
          <div class="text-center mb-4">
            <img
              :src="resultImageUrl"
              alt="水印结果"
              class="max-h-64 mx-auto rounded-lg shadow-md"
            />
          </div>

          <!-- Download Button -->
          <div class="text-center">
            <el-button type="success" size="large" @click="downloadResult">
              <el-icon><Download /></el-icon>
              下载水印图片
            </el-button>
          </div>
        </div>

        <!-- Watermark Templates -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">水印模板</h2>
          
          <div class="grid grid-cols-1 gap-3">
            <el-button
              v-for="template in watermarkTemplates"
              :key="template.name"
              @click="applyTemplate(template)"
              size="small"
              class="text-left"
            >
              <div>
                <div class="font-medium">{{ template.name }}</div>
                <div class="text-xs text-gray-500">{{ template.description }}</div>
              </div>
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
import { RefreshLeft, PictureRounded, Download } from '@element-plus/icons-vue'
import ImageUpload from '../components/common/ImageUpload.vue'

const selectedFile = ref(null)
const watermarkFile = ref(null)
const processing = ref(false)
const originalImageUrl = ref('')
const watermarkImageUrl = ref('')
const resultImageUrl = ref('')
const previewImage = ref(null)

const watermarkType = ref('text')
const position = ref('BottomRight')
const opacity = ref(50)
const quality = ref(90)

const textSettings = ref({
  text: 'Sample Watermark',
  fontSize: 24,
  fontColor: '#FFFFFF'
})

const imageSettings = ref({
  scale: 1.0
})

const positions = [
  { name: '左上', value: 'TopLeft' },
  { name: '上中', value: 'TopCenter' },
  { name: '右上', value: 'TopRight' },
  { name: '左中', value: 'MiddleLeft' },
  { name: '中心', value: 'Center' },
  { name: '右中', value: 'MiddleRight' },
  { name: '左下', value: 'BottomLeft' },
  { name: '下中', value: 'BottomCenter' },
  { name: '右下', value: 'BottomRight' }
]

const watermarkTemplates = [
  {
    name: '版权声明',
    description: '白色文字，右下角，50%透明度',
    type: 'text',
    text: '© 2024 版权所有',
    fontSize: 20,
    fontColor: '#FFFFFF',
    position: 'BottomRight',
    opacity: 50
  },
  {
    name: '品牌标识',
    description: '大字体，中心位置，30%透明度',
    type: 'text',
    text: 'BRAND',
    fontSize: 48,
    fontColor: '#000000',
    position: 'Center',
    opacity: 30
  },
  {
    name: '网站标识',
    description: '小字体，右下角，70%透明度',
    type: 'text',
    text: 'www.example.com',
    fontSize: 16,
    fontColor: '#666666',
    position: 'BottomRight',
    opacity: 70
  }
]

const positionName = computed(() => {
  const pos = positions.find(p => p.value === position.value)
  return pos ? pos.name : '未知'
})

const canProcess = computed(() => {
  if (watermarkType.value === 'text') {
    return textSettings.value.text.trim().length > 0
  } else {
    return watermarkFile.value !== null
  }
})

const textWatermarkStyle = computed(() => {
  if (!previewImage.value) return {}
  
  const img = previewImage.value
  const rect = img.getBoundingClientRect()
  
  let left = '10px'
  let top = '10px'
  let transform = ''
  
  switch (position.value) {
    case 'TopLeft':
      left = '10px'
      top = '10px'
      break
    case 'TopCenter':
      left = '50%'
      top = '10px'
      transform = 'translateX(-50%)'
      break
    case 'TopRight':
      left = 'auto'
      right = '10px'
      top = '10px'
      break
    case 'MiddleLeft':
      left = '10px'
      top = '50%'
      transform = 'translateY(-50%)'
      break
    case 'Center':
      left = '50%'
      top = '50%'
      transform = 'translate(-50%, -50%)'
      break
    case 'MiddleRight':
      left = 'auto'
      right = '10px'
      top = '50%'
      transform = 'translateY(-50%)'
      break
    case 'BottomLeft':
      left = '10px'
      top = 'auto'
      bottom = '10px'
      break
    case 'BottomCenter':
      left = '50%'
      top = 'auto'
      bottom = '10px'
      transform = 'translateX(-50%)'
      break
    case 'BottomRight':
      left = 'auto'
      right = '10px'
      top = 'auto'
      bottom = '10px'
      break
  }
  
  return {
    left,
    top,
    right,
    bottom,
    transform,
    fontSize: textSettings.value.fontSize + 'px',
    color: textSettings.value.fontColor,
    opacity: opacity.value / 100,
    fontWeight: 'bold',
    textShadow: '1px 1px 2px rgba(0,0,0,0.5)'
  }
})

const imageWatermarkStyle = computed(() => {
  if (!previewImage.value || !watermarkImageUrl.value) return {}
  
  const baseSize = 50 // Base size in pixels
  const scaledSize = baseSize * imageSettings.value.scale
  
  let left = '10px'
  let top = '10px'
  let transform = ''
  
  switch (position.value) {
    case 'TopLeft':
      left = '10px'
      top = '10px'
      break
    case 'TopCenter':
      left = '50%'
      top = '10px'
      transform = 'translateX(-50%)'
      break
    case 'TopRight':
      left = 'auto'
      right = '10px'
      top = '10px'
      break
    case 'MiddleLeft':
      left = '10px'
      top = '50%'
      transform = 'translateY(-50%)'
      break
    case 'Center':
      left = '50%'
      top = '50%'
      transform = 'translate(-50%, -50%)'
      break
    case 'MiddleRight':
      left = 'auto'
      right = '10px'
      top = '50%'
      transform = 'translateY(-50%)'
      break
    case 'BottomLeft':
      left = '10px'
      top = 'auto'
      bottom = '10px'
      break
    case 'BottomCenter':
      left = '50%'
      top = 'auto'
      bottom = '10px'
      transform = 'translateX(-50%)'
      break
    case 'BottomRight':
      left = 'auto'
      right = '10px'
      top = 'auto'
      bottom = '10px'
      break
  }
  
  return {
    left,
    top,
    right,
    bottom,
    transform,
    width: scaledSize + 'px',
    height: scaledSize + 'px',
    opacity: opacity.value / 100,
    objectFit: 'contain'
  }
})

const handleFileChange = (file) => {
  if (file) {
    originalImageUrl.value = URL.createObjectURL(file)
    resultImageUrl.value = ''
  }
}

const handleWatermarkFileChange = (file) => {
  if (file) {
    watermarkImageUrl.value = URL.createObjectURL(file)
  } else {
    watermarkImageUrl.value = ''
  }
}

const applyTemplate = (template) => {
  watermarkType.value = template.type
  if (template.type === 'text') {
    textSettings.value.text = template.text
    textSettings.value.fontSize = template.fontSize
    textSettings.value.fontColor = template.fontColor
  }
  position.value = template.position
  opacity.value = template.opacity
}

const resetSettings = () => {
  textSettings.value = {
    text: 'Sample Watermark',
    fontSize: 24,
    fontColor: '#FFFFFF'
  }
  
  imageSettings.value = {
    scale: 1.0
  }
  
  position.value = 'BottomRight'
  opacity.value = 50
  quality.value = 90
  watermarkType.value = 'text'
}

const addWatermark = async () => {
  if (!selectedFile.value || !canProcess.value) {
    ElMessage.warning('请检查设置是否完整')
    return
  }
  
  processing.value = true
  
  try {
    // Simulate API call with FormData
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('position', position.value)
    formData.append('opacity', opacity.value)
    formData.append('quality', quality.value)
    
    if (watermarkType.value === 'text') {
      formData.append('text', textSettings.value.text)
      formData.append('fontSize', textSettings.value.fontSize)
      formData.append('fontColor', textSettings.value.fontColor)
    } else {
      formData.append('watermarkFile', watermarkFile.value)
      formData.append('scale', imageSettings.value.scale)
    }
    
    // Simulate processing delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // For demo purposes, create watermarked version using canvas
    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')
    const img = new Image()
    
    img.onload = () => {
      canvas.width = img.width
      canvas.height = img.height
      
      // Draw original image
      ctx.drawImage(img, 0, 0)
      
      // Add watermark
      if (watermarkType.value === 'text') {
        addTextWatermarkToCanvas(ctx, canvas.width, canvas.height)
      } else if (watermarkImageUrl.value) {
        addImageWatermarkToCanvas(ctx, canvas.width, canvas.height)
      }
      
      canvas.toBlob((blob) => {
        resultImageUrl.value = URL.createObjectURL(blob)
        ElMessage.success('水印添加完成')
      }, 'image/jpeg', quality.value / 100)
    }
    
    img.src = originalImageUrl.value
    
  } catch (error) {
    ElMessage.error('添加水印失败：' + error.message)
  } finally {
    processing.value = false
  }
}

const addTextWatermarkToCanvas = (ctx, width, height) => {
  ctx.save()
  
  // Set text properties
  ctx.font = `${textSettings.value.fontSize}px Arial`
  ctx.fillStyle = textSettings.value.fontColor
  ctx.globalAlpha = opacity.value / 100
  ctx.textBaseline = 'top'
  
  // Calculate position
  const textMetrics = ctx.measureText(textSettings.value.text)
  const textWidth = textMetrics.width
  const textHeight = textSettings.value.fontSize
  
  let x, y
  
  switch (position.value) {
    case 'TopLeft':
      x = 10
      y = 10
      break
    case 'TopCenter':
      x = (width - textWidth) / 2
      y = 10
      break
    case 'TopRight':
      x = width - textWidth - 10
      y = 10
      break
    case 'MiddleLeft':
      x = 10
      y = (height - textHeight) / 2
      break
    case 'Center':
      x = (width - textWidth) / 2
      y = (height - textHeight) / 2
      break
    case 'MiddleRight':
      x = width - textWidth - 10
      y = (height - textHeight) / 2
      break
    case 'BottomLeft':
      x = 10
      y = height - textHeight - 10
      break
    case 'BottomCenter':
      x = (width - textWidth) / 2
      y = height - textHeight - 10
      break
    case 'BottomRight':
      x = width - textWidth - 10
      y = height - textHeight - 10
      break
  }
  
  ctx.fillText(textSettings.value.text, x, y)
  ctx.restore()
}

const addImageWatermarkToCanvas = (ctx, width, height) => {
  // This would require loading the watermark image
  // For demo purposes, we'll just add a placeholder
  ctx.save()
  ctx.globalAlpha = opacity.value / 100
  ctx.fillStyle = 'rgba(255, 255, 255, 0.8)'
  
  const watermarkSize = 50 * imageSettings.value.scale
  let x, y
  
  switch (position.value) {
    case 'TopLeft':
      x = 10
      y = 10
      break
    case 'TopRight':
      x = width - watermarkSize - 10
      y = 10
      break
    case 'BottomLeft':
      x = 10
      y = height - watermarkSize - 10
      break
    case 'BottomRight':
      x = width - watermarkSize - 10
      y = height - watermarkSize - 10
      break
    case 'Center':
      x = (width - watermarkSize) / 2
      y = (height - watermarkSize) / 2
      break
    default:
      x = width - watermarkSize - 10
      y = height - watermarkSize - 10
  }
  
  ctx.fillRect(x, y, watermarkSize, watermarkSize)
  ctx.restore()
}

const downloadResult = () => {
  if (resultImageUrl.value) {
    const link = document.createElement('a')
    link.href = resultImageUrl.value
    link.download = `watermarked_${selectedFile.value.name}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}
</script>

<style scoped>
.watermark-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}
</style>