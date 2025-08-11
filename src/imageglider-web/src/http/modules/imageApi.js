import $axios from '../axios';
export const imageApi = {
    // 格式转换
    convertImageApi(formData) {
        return $axios({
            method: 'post',
            url: '/convert',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 元数据清理
    cleanMetadataApi(formData) {
        return $axios({
            method: 'post',
            url: '/metadata/strip',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 压缩图片
    compressImageApi(formData) {
        return $axios({
            method: 'post',
            url: '/compress',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 精确裁剪图片
    cropImageApi(formData) {
        return $axios({
            method: 'post',
            url: '/crop',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 中心裁剪图片
    centerCropImageApi(formData) {
        return $axios({
            method: 'post',
            url: '/crop/center',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 按百分比裁剪图片
    percentCropImageApi(formData) {
        return $axios({
            method: 'post',
            url: '/crop/percent',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            },
            timeout: 50000
        })
    },
    // 下载图片
    downloadFileApi(fileName) {
        return $axios({
            method: 'get',
            url: `/download/${fileName}`,
            responseType: 'blob'
        })
    }
}