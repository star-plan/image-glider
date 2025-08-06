import axios from '../axios'

export const imageApi = {
    // 格式转换
    convertImageApi(formData) {
        return axios({
            method: 'post',
            url: '/convert',
            data: formData,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    },
    // 下载图片
    downloadFileApi(fileName) {
        return axios({
            method: 'get',
            url: `/downloads/${fileName}`,
            responseType: 'blob'
        })
    }
}