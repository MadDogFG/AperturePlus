import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ReceivedRating, PendingRatingDto } from '@/types/rating'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'

export const useRatingStore = defineStore('rating', () => {
  // --- State ---
  const receivedRatings = ref<ReceivedRating[]>([])
  const isLoading = ref(false)
  const pendingRatings = ref<PendingRatingDto[]>([])
  const isPendingLoading = ref(false)
  const isSubmitting = ref(false)

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

  async function fetchPendingRatings() {
    isPendingLoading.value = true
    try {
      const baseUrl = import.meta.env.VITE_API_RATING_BASE_URL
      const response = await apiClient.get<any[]>(`${baseUrl}/ratings/pending`)
      // [!code focus start]
      // 关键修复：为每个从后端获取的任务，手动补充前端v-model需要的字段
      pendingRatings.value = response.data.map((p) => ({
        ...p,
        ratingId: p.pendingRatingId,
        score: 0,
        comments: '',
      }))
      // [!code focus end]
    } catch (error) {
      console.error('获取待评价列表失败:', error)
      ElMessage.error('加载待评价任务失败')
    } finally {
      isPendingLoading.value = false
    }
  }

  async function submitRating(payload: { ratingId: string; score: number; comments?: string }) {
    isSubmitting.value = true
    try {
      const baseUrl = import.meta.env.VITE_API_RATING_BASE_URL
      await apiClient.post(`${baseUrl}/Ratings/submit/${payload.ratingId}`, {
        score: payload.score,
        comments: payload.comments,
      })

      // [!code focus start]
      // 关键修复：不再重新请求API，而是直接从本地数组中移除已完成的项
      pendingRatings.value = pendingRatings.value.filter((p) => p.ratingId !== payload.ratingId)
      // [!code focus end]
      return true // 返回成功状态
    } catch (error) {
      console.error('提交评价失败:', error)
      ElMessage.error('提交评价失败')
      return false // 返回失败状态
    } finally {
      isSubmitting.value = false
    }
  }

  return {
    receivedRatings,
    isLoading,
    fetchReceivedRatings,
    clearRatings,
    pendingRatings,
    isPendingLoading,
    isSubmitting,
    fetchPendingRatings,
    submitRating,
  }
})
