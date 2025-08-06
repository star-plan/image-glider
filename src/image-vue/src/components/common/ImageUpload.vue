<template>
  <div class="image-upload-container">
    <!-- Upload Area -->
    <div
      :class="[
        'image-preview',
        { 'dragover': isDragover },
        { 'has-image': previewUrl }
      ]"
      @drop="handleDrop"
      @dragover="handleDragover"
      @dragleave="handleDragleave"
      @click="triggerFileInput"
    >
      <input
        ref="fileInput"
        type="file"
        accept="image/*"
        @change="handleFileChange"
        class="hidden"
      />
      
      <!-- Upload Content -->
      <div v-if="!previewUrl" class="upload-content">
        <el-icon class="text-4xl text-gray-400 dark:text-gray-500 mb-4">
          <Plus />
        </el-icon>
        <p class="text-lg font-medium text-gray-700 dark:text-gray-300 mb-2">
          点击或拖拽图片到此处
        </p>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          支持 JPG、PNG、GIF、WEBP 等格式
        </p>
        <p class="text-xs text-gray-400 dark:text-gray-500 mt-2">
          最大文件大小：{{ maxSizeMB }}MB
        </p>
      </div>

      <!-- Image Preview -->
      <div v-else class="image-preview-content">
        <div class="relative group">
          <img
            :src="previewUrl"
            :alt="fileName"
            class="max-h-64 max-w-full object-contain rounded-lg shadow-lg"
          />
          <div class="absolute inset-0 bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity rounded-lg flex items-center justify-center">
            <el-button @click.stop="removeImage" type="danger" circle>
              <el-icon><Delete /></el-icon>
            </el-button>
          </div>
        </div>
        <div class="mt-4 text-center">
          <p class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ fileName }}</p>
          <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatFileSize(fileSize) }}</p>
        </div>
      </div>
    </div>

    <!-- File Info -->
    <div v-if="file && showInfo" class="mt-4 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
      <h4 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">文件信息</h4>
      <div class="grid grid-cols-2 gap-2 text-sm">
        <div>
          <span class="text-gray-500 dark:text-gray-400">文件名：</span>
          <span class="text-gray-700 dark:text-gray-300">{{ fileName }}</span>
        </div>
        <div>
          <span class="text-gray-500 dark:text-gray-400">大小：</span>
          <span class="text-gray-700 dark:text-gray-300">{{ formatFileSize(fileSize) }}</span>
        </div>
        <div>
          <span class="text-gray-500 dark:text-gray-400">类型：</span>
          <span class="text-gray-700 dark:text-gray-300">{{ file.type }}</span>
        </div>
        <div v-if="imageDimensions">
          <span class="text-gray-500 dark:text-gray-400">尺寸：</span>
          <span class="text-gray-700 dark:text-gray-300">{{ imageDimensions }}</span>
        </div>
      </div>
    </div>

    <!-- Error Message -->
    <div v-if="error" class="mt-4">
      <el-alert :title="error" type="error" show-icon />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { Plus, Delete } from '@element-plus/icons-vue'

const props = defineProps({
  modelValue: {
    type: File,
    default: null
  },
  maxSizeMB: {
    type: Number,
    default: 10
  },
  showInfo: {
    type: Boolean,
    default: true
  },
  acceptTypes: {
    type: Array,
    default: () => ['image/jpeg', 'image/png', 'image/gif', 'image/webp']
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'error'])

const fileInput = ref(null)
const isDragover = ref(false)
const previewUrl = ref('')
const error = ref('')
const imageDimensions = ref('')

const file = computed(() => props.modelValue)
const fileName = computed(() => file.value?.name || '')
const fileSize = computed(() => file.value?.size || 0)

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileChange = (event) => {
  const selectedFile = event.target.files[0]
  if (selectedFile) {
    processFile(selectedFile)
  }
}

const handleDrop = (event) => {
  event.preventDefault()
  isDragover.value = false
  
  const files = event.dataTransfer.files
  if (files.length > 0) {
    processFile(files[0])
  }
}

const handleDragover = (event) => {
  event.preventDefault()
  isDragover.value = true
}

const handleDragleave = (event) => {
  event.preventDefault()
  isDragover.value = false
}

const processFile = (selectedFile) => {
  error.value = ''
  
  // Validate file type
  if (!props.acceptTypes.includes(selectedFile.type)) {
    error.value = '不支持的文件格式'
    emit('error', error.value)
    return
  }
  
  // Validate file size
  const maxSizeBytes = props.maxSizeMB * 1024 * 1024
  if (selectedFile.size > maxSizeBytes) {
    error.value = `文件大小不能超过 ${props.maxSizeMB}MB`
    emit('error', error.value)
    return
  }
  
  // Create preview URL
  previewUrl.value = URL.createObjectURL(selectedFile)
  
  // Get image dimensions
  const img = new Image()
  img.onload = () => {
    imageDimensions.value = `${img.width} × ${img.height}`
  }
  img.src = previewUrl.value
  
  // Emit file
  emit('update:modelValue', selectedFile)
  emit('change', selectedFile)
}

const removeImage = () => {
  if (previewUrl.value) {
    URL.revokeObjectURL(previewUrl.value)
  }
  previewUrl.value = ''
  imageDimensions.value = ''
  error.value = ''
  
  if (fileInput.value) {
    fileInput.value.value = ''
  }
  
  emit('update:modelValue', null)
  emit('change', null)
}

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// Watch for external file changes
watch(() => props.modelValue, (newFile) => {
  if (!newFile && previewUrl.value) {
    removeImage()
  }
})

// Cleanup on unmount
import { onUnmounted } from 'vue'
onUnmounted(() => {
  if (previewUrl.value) {
    URL.revokeObjectURL(previewUrl.value)
  }
})
</script>

<style scoped>
.image-upload-container {
  @apply w-full;
}

.image-preview {
  @apply min-h-[200px] cursor-pointer transition-all duration-300;
}

.image-preview.dragover {
  @apply border-blue-500 bg-blue-50 dark:bg-blue-900/20 transform scale-105;
}

.image-preview.has-image {
  @apply border-solid border-gray-300 dark:border-gray-600;
}

.upload-content {
  @apply flex flex-col items-center justify-center h-full py-8;
}

.image-preview-content {
  @apply flex flex-col items-center justify-center p-6;
}
</style>