// 支持的图片格式
export const SUPPORTED_IMAGE_TYPES = [
    'image/jpeg',
    'image/jpg',
    'image/png',
    'image/gif',
    'image/webp',
    'image/bmp',
    'image/tiff'
]

// 格式化文件大小
export function formatFileSize(bytes) {
    if (bytes === 0) return '0 B'

    const sizes = ['B', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(1024))
    const size = (bytes / Math.pow(1024, i)).toFixed(i === 0 ? 0 : 2)

    return `${size} ${sizes[i]}`
}

// 验证文件类型
export function isValidImageType(file) {
    return SUPPORTED_IMAGE_TYPES.includes(file.type)
}

// 创建图片预览URL
export function createImagePreview(file) {
    return URL.createObjectURL(file)
}

// 释放图片预览URL
export function revokeImagePreview(url) {
    URL.revokeObjectURL(url)
}

// 下载文件
export function downloadFile(blob, filename) {
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
}

// 读取文件为DataURL
export function readFileAsDataURL(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = () => resolve(reader.result)
        reader.onerror = reject
        reader.readAsDataURL(file)
    })
}