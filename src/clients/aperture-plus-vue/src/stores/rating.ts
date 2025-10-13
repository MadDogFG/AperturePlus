import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ReceivedRating } from '@/types/rating'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'

export const useRatingStore = defineStore('rating', () => {
  // --- State ---
  const receivedRatings = ref<ReceivedRating[]>([])
  const isLoading = ref(false)

  // --- Actions ---
  async function fetchReceivedRatings() {
    // 如果已经加载过了，就不重复加载，除非强制刷新
    if (receivedRatings.value.length > 0) return

    isLoading.value = true
    try {
      // **注意**: 后端 RatingsController.cs 中可能需要添加一个接口来获取当前用户收到的所有评价
      // 这里我们假设接口是 GET /api/ratings/my-received-ratings
      const baseUrl = import.meta.env.VITE_API_RATING_BASE_URL // 需要在 .env 文件中配置
      const response = await apiClient.get<ReceivedRating[]>(
        `${baseUrl}/ratings/my-received-ratings`,
      )
      receivedRatings.value = response.data
    } catch (error) {
      console.error('获取收到的评价列表失败:', error)
      ElMessage.error('加载评价信息失败')
    } finally {
      isLoading.value = false
    }
  }

  function clearRatings() {
    receivedRatings.value = []
  }

  return {
    receivedRatings,
    isLoading,
    fetchReceivedRatings,
    clearRatings,
  }
})
