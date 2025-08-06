import axios from 'axios';


export default function $axios(options) {
    return new Promise((resolve, reject) => {
        const instance = axios.create({
            baseURL: '/api',
            responseType: options.responseType || 'json'
        })
        // response 响应拦截器
        instance.interceptors.response.use(
            response => {
                // 如果是文件下载，直接返回整个response
                if (options.responseType === 'blob') {
                    return response
                }
                return response.data
            },
            err => {
                let reason = err
                if (err && err.response) {
                    // 包装一下默认的错误提示
                    switch (err.response.status) {
                        case 400:
                            err.message = '请求参数错误'
                            break
                        case 401:
                            err.message = '请先登录再访问'
                            break
                        case 403:
                            err.message = '拒绝访问，没有访问权限'
                            break
                        case 404:
                            err.message = `请求地址出错: ${err.response.config.url}`
                            break
                        case 408:
                            err.message = '请求超时'
                            break
                        case 500:
                            err.message = '服务器内部错误'
                            break
                        case 501:
                            err.message = '服务未实现'
                            break
                        case 502:
                            err.message = '网关错误'
                            break
                        case 503:
                            err.message = '服务不可用'
                            break
                        case 504:
                            err.message = '网关超时'
                            break
                        case 505:
                            err.message = 'HTTP版本不受支持'
                            break
                        default:
                    }

                    // 如果有错误响应的话就处理
                    if (err.response.data) {
                        reason = err.response.data
                        // 如果没有返回错误信息的话就用上面的默认信息
                        if (!reason.message) reason.message = err.message
                    }
                }
                console.error(err)
                return Promise.reject(reason) // 返回接口返回的错误信息
            }
        )
        // 请求处理
        instance(options).then(res => {
            resolve(res)
            return false
        }).catch(error => {
            reject(error)
        })
    })
}
  