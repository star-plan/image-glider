<template>
  <div class="crop-page">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">图片裁剪</h1>
      <p class="text-gray-600 dark:text-gray-300">精确裁剪、中心裁剪或按百分比裁剪图片 — 支持手动拖动与缩放裁剪区域</p>
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
            <el-radio-group v-model="cropMode" @change="onModeChange">
              <el-radio label="precise">精确裁剪</el-radio>
              <el-radio label="center">中心裁剪</el-radio>
              <el-radio label="percent">百分比裁剪</el-radio>
              <el-radio label="manual">自定义 (拖动/缩放)</el-radio>
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

          <!-- Manual controls hint -->
          <div v-if="cropMode === 'manual'" class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
            <div>在原始图片上拖动裁剪区域以移动。拖动角点或边缘以缩放。使用下面的 +/- 按钮等比例放大/缩小。</div>
            <div class="flex items-center space-x-2 mt-2">
              <el-button size="small" @click="scaleCrop(0.9)">- 缩小</el-button>
              <el-button size="small" @click="scaleCrop(1.1)">+ 放大</el-button>
              <el-switch v-model="lockAspect" active-text="锁定比例" inactive-text="自由比例" size="small" />
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
          <div v-if="interactiveCrop && imageWidth && imageHeight" class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">裁剪预览</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-500 dark:text-gray-400">原始尺寸：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">{{ imageWidth }} × {{ imageHeight }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">裁剪尺寸：</span>
                <span class="font-medium text-blue-600 dark:text-blue-400">{{ interactiveCrop.width }} × {{ interactiveCrop.height }}</span>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">裁剪位置：</span>
                <span class="font-medium text-gray-700 dark:text-gray-300">({{ interactiveCrop.x }}, {{ interactiveCrop.y }})</span>
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

            <!-- Crop Overlay (interactive) -->
            <div
                v-if="interactiveCrop && showOverlay"
                class="absolute border-2 border-red-500 bg-red-500 bg-opacity-20 cursor-move"
                :style="overlayStyle"
                @mousedown.prevent.stop="onOverlayMouseDown"
                @touchstart.prevent.stop="onOverlayTouchStart"
            >
              <!-- resize handles (8 handles) -->
              <div v-for="h in handles" :key="h" :class="['absolute w-3 h-3 bg-white border rounded', handleClass(h)]"
                   @mousedown.prevent.stop="(e) => onHandleMouseDown(e, h)"
                   @touchstart.prevent.stop="(e) => onHandleTouchStart(e, h)"
              />
            </div>
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
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ interactiveCrop.width }} × {{ interactiveCrop.height }}</div>
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
import {ref, computed, watch, onMounted, onBeforeUnmount, onUnmounted} from 'vue'
import { ElMessage, ElLoading } from 'element-plus'
import { RefreshLeft, Crop, Download } from '@element-plus/icons-vue'
// 导入组件
import ImageUpload from '../components/common/ImageUpload.vue'
// 导入方法
import { imageApi } from '../http/modules/imageApi'
import {createImagePreview, revokeImagePreview} from "../utils/file.js";

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

const preciseSettings = ref({ x: 0, y: 0, width: 100, height: 100 })
const centerSettings = ref({ width: 100, height: 100 })
const percentSettings = ref({ xPercent: 10, yPercent: 10, widthPercent: 80, heightPercent: 80 })

const cropPresets = [
  { name: '1:1 正方形', ratio: 1, mode: 'center' },
  { name: '16:9 宽屏', ratio: 16/9, mode: 'center' },
  { name: '4:3 标准', ratio: 4/3, mode: 'center' },
  { name: '3:2 照片', ratio: 3/2, mode: 'center' }
]

// interactive crop state (used for manual dragging/resizing)
const interactiveCrop = ref(null)
const lockAspect = ref(false)

// internal drag/resize state
let isDragging = false
let isResizing = false
let activeHandle = null
let startMouse = { x: 0, y: 0 }
let startCrop = { x: 0, y: 0, width: 0, height: 0 }

const handles = ['nw','n','ne','e','se','s','sw','w']

let lastObjectUrl = null

const cropRatio = computed(() => {
  if (!interactiveCrop.value || !imageWidth.value || !imageHeight.value) return 0
  const originalArea = imageWidth.value * imageHeight.value
  const cropArea = interactiveCrop.value.width * interactiveCrop.value.height
  return Math.round((cropArea / originalArea) * 100)
})

const overlayStyle = computed(() => {
  if (!interactiveCrop.value || !originalImage.value) return {}
  const img = originalImage.value
  const imgRect = img.getBoundingClientRect()
  const scaleX = img.offsetWidth / imageWidth.value
  const scaleY = img.offsetHeight / imageHeight.value
  return {
    left: interactiveCrop.value.x * scaleX + 'px',
    top: interactiveCrop.value.y * scaleY + 'px',
    width: interactiveCrop.value.width * scaleX + 'px',
    height: interactiveCrop.value.height * scaleY + 'px'
  }
})

const handleClass = (h) => {
  // position handles roughly (tailwind classes not used here for brevity)
  const base = 'transform -translate-1/2 '
  switch (h) {
    case 'nw': return base + 'left-0 top-0 cursor-nwse-resize'
    case 'n': return base + 'left-1/2 top-0 cursor-ns-resize'
    case 'ne': return base + 'right-0 top-0 cursor-nesw-resize'
    case 'e': return base + 'right-0 top-1/2 cursor-ew-resize'
    case 'se': return base + 'right-0 bottom-0 cursor-nwse-resize'
    case 's': return base + 'left-1/2 bottom-0 cursor-ns-resize'
    case 'sw': return base + 'left-0 bottom-0 cursor-nesw-resize'
    case 'w': return base + 'left-0 top-1/2 cursor-ew-resize'
  }
}

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
    preciseSettings.value = { x: 0, y: 0, width: Math.min(imageWidth.value, 200), height: Math.min(imageHeight.value, 200) }
    centerSettings.value = { width: Math.min(imageWidth.value, 200), height: Math.min(imageHeight.value, 200) }
    percentSettings.value = { xPercent: 10, yPercent: 10, widthPercent: 80, heightPercent: 80 }

    // initialize interactive crop to a sensible default (center)
    const w = Math.min(imageWidth.value, 200)
    const h = Math.min(imageHeight.value, 200)
    interactiveCrop.value = { x: Math.max(0, Math.round((imageWidth.value - w)/2)), y: Math.max(0, Math.round((imageHeight.value - h)/2)), width: w, height: h }
  }
}

const onModeChange = () => {
  // when switching modes, populate interactiveCrop from computed cropPreview-like values
  if (!imageWidth.value || !imageHeight.value) return

  if (cropMode.value === 'precise') {
    interactiveCrop.value = { ...preciseSettings.value }
  } else if (cropMode.value === 'center') {
    const width = centerSettings.value.width
    const height = centerSettings.value.height
    interactiveCrop.value = { x: Math.max(0, Math.round((imageWidth.value - width)/2)), y: Math.max(0, Math.round((imageHeight.value - height)/2)), width, height }
  } else if (cropMode.value === 'percent') {
    const x = Math.round((percentSettings.value.xPercent / 100) * imageWidth.value)
    const y = Math.round((percentSettings.value.yPercent / 100) * imageHeight.value)
    const width = Math.round((percentSettings.value.widthPercent / 100) * imageWidth.value)
    const height = Math.round((percentSettings.value.heightPercent / 100) * imageHeight.value)
    interactiveCrop.value = { x, y, width, height }
  } else if (cropMode.value === 'manual') {
    // keep current interactiveCrop
    if (!interactiveCrop.value) resetCropSettings()
  }
}

// utilities to convert client coords to image coords
const clientToImage = (clientX, clientY) => {
  const img = originalImage.value
  if (!img) return { x: 0, y: 0 }
  const rect = img.getBoundingClientRect()
  const scaleX = img.offsetWidth / imageWidth.value
  const scaleY = img.offsetHeight / imageHeight.value
  const x = Math.round((clientX - rect.left) / scaleX)
  const y = Math.round((clientY - rect.top) / scaleY)
  return { x, y }
}

// drag handlers
const onOverlayMouseDown = (e) => {
  if (cropMode.value !== 'manual') return
  isDragging = true
  startMouse = { x: e.clientX, y: e.clientY }
  startCrop = { ...interactiveCrop.value }
  window.addEventListener('mousemove', onWindowMouseMove)
  window.addEventListener('mouseup', onWindowMouseUp)
}
const onOverlayTouchStart = (e) => {
  if (cropMode.value !== 'manual') return
  const t = e.touches[0]
  isDragging = true
  startMouse = { x: t.clientX, y: t.clientY }
  startCrop = { ...interactiveCrop.value }
  window.addEventListener('touchmove', onWindowTouchMove, { passive: false })
  window.addEventListener('touchend', onWindowTouchEnd)
}

const onHandleMouseDown = (e, handle) => {
  if (cropMode.value !== 'manual') return
  isResizing = true
  activeHandle = handle
  startMouse = { x: e.clientX, y: e.clientY }
  startCrop = { ...interactiveCrop.value }
  window.addEventListener('mousemove', onWindowMouseMove)
  window.addEventListener('mouseup', onWindowMouseUp)
}
const onHandleTouchStart = (e, handle) => {
  if (cropMode.value !== 'manual') return
  const t = e.touches[0]
  isResizing = true
  activeHandle = handle
  startMouse = { x: t.clientX, y: t.clientY }
  startCrop = { ...interactiveCrop.value }
  window.addEventListener('touchmove', onWindowTouchMove, { passive: false })
  window.addEventListener('touchend', onWindowTouchEnd)
}

const onWindowMouseMove = (e) => {
  e.preventDefault()
  const deltaClientX = e.clientX - startMouse.x
  const deltaClientY = e.clientY - startMouse.y
  const img = originalImage.value
  if (!img) return
  const scaleX = img.offsetWidth / imageWidth.value
  const scaleY = img.offsetHeight / imageHeight.value
  const dx = Math.round(deltaClientX / scaleX)
  const dy = Math.round(deltaClientY / scaleY)

  if (isDragging) {
    let nx = startCrop.x + dx
    let ny = startCrop.y + dy
    nx = Math.max(0, Math.min(nx, imageWidth.value - interactiveCrop.value.width))
    ny = Math.max(0, Math.min(ny, imageHeight.value - interactiveCrop.value.height))
    interactiveCrop.value = { ...interactiveCrop.value, x: nx, y: ny }
  } else if (isResizing && activeHandle) {
    const c = { ...startCrop }
    // depending on handle, adjust x/y/width/height
    let newX = c.x
    let newY = c.y
    let newW = c.width
    let newH = c.height
    if (activeHandle.includes('e')) {
      newW = Math.max(1, c.width + dx)
    }
    if (activeHandle.includes('s')) {
      newH = Math.max(1, c.height + dy)
    }
    if (activeHandle.includes('w')) {
      newW = Math.max(1, c.width - dx)
      newX = c.x + dx
    }
    if (activeHandle.includes('n')) {
      newH = Math.max(1, c.height - dy)
      newY = c.y + dy
    }

    // lock aspect if needed
    if (lockAspect.value) {
      const aspect = c.width / c.height || 1
      if (newW / newH > aspect) {
        newW = Math.round(newH * aspect)
      } else {
        newH = Math.round(newW / aspect)
      }
    }

    // clamp
    newX = Math.max(0, Math.min(newX, imageWidth.value - 1))
    newY = Math.max(0, Math.min(newY, imageHeight.value - 1))
    newW = Math.max(1, Math.min(newW, imageWidth.value - newX))
    newH = Math.max(1, Math.min(newH, imageHeight.value - newY))

    interactiveCrop.value = { x: newX, y: newY, width: newW, height: newH }
  }
}

const onWindowMouseUp = () => {
  isDragging = false
  isResizing = false
  activeHandle = null
  window.removeEventListener('mousemove', onWindowMouseMove)
  window.removeEventListener('mouseup', onWindowMouseUp)
}

const onWindowTouchMove = (e) => {
  e.preventDefault()
  const t = e.touches[0]
  onWindowMouseMove({ clientX: t.clientX, clientY: t.clientY, preventDefault: () => {} })
}
const onWindowTouchEnd = () => {
  isDragging = false
  isResizing = false
  activeHandle = null
  window.removeEventListener('touchmove', onWindowTouchMove)
  window.removeEventListener('touchend', onWindowTouchEnd)
}

onBeforeUnmount(() => {
  window.removeEventListener('mousemove', onWindowMouseMove)
  window.removeEventListener('mouseup', onWindowMouseUp)
  window.removeEventListener('touchmove', onWindowTouchMove)
  window.removeEventListener('touchend', onWindowTouchEnd)
})

const applyCropPreset = (preset) => {
  if (!imageWidth.value || !imageHeight.value) return
  cropMode.value = 'center'
  let width, height
  if (preset.ratio > 1) {
    width = Math.min(imageWidth.value, 400)
    height = Math.round(width / preset.ratio)
  } else {
    height = Math.min(imageHeight.value, 400)
    width = Math.round(height * preset.ratio)
  }
  centerSettings.value = { width, height }
  interactiveCrop.value = { x: Math.max(0, Math.round((imageWidth.value - width)/2)), y: Math.max(0, Math.round((imageHeight.value - height)/2)), width, height }
}

const scaleCrop = (factor) => {
  if (!interactiveCrop.value) return
  const cx = interactiveCrop.value.x + interactiveCrop.value.width / 2
  const cy = interactiveCrop.value.y + interactiveCrop.value.height / 2
  let newW = Math.round(interactiveCrop.value.width * factor)
  let newH = Math.round(interactiveCrop.value.height * factor)
  if (lockAspect.value) {
    const aspect = interactiveCrop.value.width / interactiveCrop.value.height || 1
    if (newW / newH > aspect) newW = Math.round(newH * aspect)
    else newH = Math.round(newW / aspect)
  }
  newW = Math.max(1, Math.min(newW, imageWidth.value))
  newH = Math.max(1, Math.min(newH, imageHeight.value))
  const nx = Math.round(Math.max(0, Math.min(imageWidth.value - newW, cx - newW/2)))
  const ny = Math.round(Math.max(0, Math.min(imageHeight.value - newH, cy - newH/2)))
  interactiveCrop.value = { x: nx, y: ny, width: newW, height: newH }
}

const cropImage = async () => {
  if (!selectedFile.value || !interactiveCrop.value) {
    ElMessage.warning('请先上传图片并设置裁剪区域')
    return
  }

  processing.value = true
  const loading = ElLoading.service({
    lock: true,
    text: 'Loading',
    background: 'rgba(0, 0, 0, 0.7)',
  })
  try {
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('quality', quality.value)
    let res
    if (cropMode.value === 'precise') {
      formData.append('x', preciseSettings.value.x)
      formData.append('y', preciseSettings.value.y)
      formData.append('width', preciseSettings.value.width)
      formData.append('height', preciseSettings.value.height)
      res = await imageApi.cropImageApi(formData)
    } else if (cropMode.value === 'center') {
      formData.append('width', centerSettings.value.width)
      formData.append('height', centerSettings.value.height)
      res = await imageApi.centerCropImageApi(formData)
    } else if (cropMode.value === 'percent') {
      formData.append('xPercent', percentSettings.value.xPercent)
      formData.append('yPercent', percentSettings.value.yPercent)
      formData.append('widthPercent', percentSettings.value.widthPercent)
      formData.append('heightPercent', percentSettings.value.heightPercent)
      res = await imageApi.percentCropImageApi(formData)
    } else if (cropMode.value === 'manual') {
      formData.append('x', interactiveCrop.value.x)
      formData.append('y', interactiveCrop.value.y)
      formData.append('width', interactiveCrop.value.width)
      formData.append('height', interactiveCrop.value.height)
      res = await imageApi.cropImageApi(formData)
    }

    if (res.statusCode === 200) {
      const blob = await imageApi.downloadFileApi(res.data);
      console.log(blob)
      // 设置结果
      updateCroppedImage(blob.data);
      loading.close()
      // 显示成功消息
      ElMessage.success(res.message);
    }
  } catch (error) {
    ElMessage.error('裁剪失败：' + (error.message || error))
  } finally {
    processing.value = false
    loading.close()
  }
}

const downloadResult = () => {
  if (croppedImageUrl.value) {
    const link = document.createElement('a')
    link.href = croppedImageUrl.value
    link.download = `${selectedFile.value.name}`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    ElMessage.success('下载已开始')
  }
}

watch(
    () => [preciseSettings.value, centerSettings.value, percentSettings.value], // 监听四个参数
    ()=> {
      onModeChange()
    },
    { deep: true } // 深度监听
)


function updateCroppedImage(blob) {
  // 如果之前有旧 URL，先释放
  if (lastObjectUrl) {
    revokeImagePreview(lastObjectUrl)
  }

  const newUrl = createImagePreview(blob)
  lastObjectUrl = newUrl
  croppedImageUrl.value = newUrl
}

onUnmounted(() => {
  if (croppedImageUrl.value) {
    revokeImagePreview(croppedImageUrl.value);
  }
});
</script>

<style scoped>
.crop-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}

/* position helpers for handles */
.absolute > .left-0 { left: 0 }
.absolute > .right-0 { right: 0 }
.absolute > .top-0 { top: 0 }
.absolute > .bottom-0 { bottom: 0 }
.absolute > .top-1\/2 { top: 50% }
.absolute > .left-1\/2 { left: 50% }
.absolute > .bottom-0 { bottom: 0 }

/* small handle styling (overrides) */
.crop-page .absolute > .w-3.h-3 {
  width: 5px;
  height: 5px;
}
</style>
