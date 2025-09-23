// src/api/axios.ts
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// 创建一个 axios 实例，我们可以对它进行自定义配置
const apiClient = axios.create({})

// 定义请求拦截器
apiClient.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore()
    const token = authStore.token

    if (token) {
      // 如果 token 存在，则添加到请求的 Authorization 头中
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

export default apiClient
