// src/stores/rating.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ReceivedRating, PendingRatingDto, SentRating, RatingStats } from '@/types/rating'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'

export const useRatingStore = defineStore('rating', () => {
  // --- State ---
  // "当前查看的" 用户的评价
  const receivedRatings = ref<ReceivedRating[]>([])
  const stats = ref<RatingStats | null>(null)
  const isLoading = ref(false) // for received
  const isStatsLoading = ref(false)

  // "我自己的" 待处理和已发送评价
  const pendingRatings = ref<PendingRatingDto[]>([])
  const sentRatings = ref<SentRating[]>([])
  const isPendingLoading = ref(false)
  const isSentLoading = ref(false)

  const isSubmitting = ref(false)
  const baseUrl = import.meta.env.VITE_API_RATING_BASE_URL

  // --- 【统一】 Action 1：获取评价数据 ---
  async function fetchRatings(userId: string | null = null) {
    // 1. 清空 "当前查看" 的数据
    isLoading.value = true
    isStatsLoading.value = true
    receivedRatings.value = []
    stats.value = null

    // 2. 根据 userId 分别调用
    if (userId) {
      // --- 获取 "他人" 数据 ---
      try {
        // 2a. 获取统计
        const statsResponse = await apiClient.get<RatingStats>(
          `${baseUrl}/ratings/statistics/${userId}`,
        )
        stats.value = statsResponse.data
      } catch (e) {
        console.error('Failed to fetch user stats', e)
      } finally {
        isStatsLoading.value = false
      }

      try {
        // 2b. 获取收到的评价
        const ratingsResponse = await apiClient.get<ReceivedRating[]>(
          `${baseUrl}/ratings/my-received-ratings/${userId}`,
        )
        receivedRatings.value = ratingsResponse.data
      } catch (e) {
        console.error('Failed to fetch user ratings', e)
      } finally {
        isLoading.value = false
      }
    } else {
      // --- 获取 "我的" 数据 ---
      // 2a. 获取统计 (我的)
      isStatsLoading.value = true
      try {
        const statsResponse = await apiClient.get<RatingStats>(`${baseUrl}/ratings/statistics`)
        stats.value = statsResponse.data
      } catch (e) {
        console.error('Failed to fetch my stats', e)
      } finally {
        isStatsLoading.value = false
      }

      // 2b. 获取收到的评价 (我的)
      isLoading.value = true
      try {
        const ratingsResponse = await apiClient.get<ReceivedRating[]>(
          `${baseUrl}/ratings/my-received-ratings`,
        )
        receivedRatings.value = ratingsResponse.data
      } catch (e) {
        console.error('Failed to fetch my ratings', e)
      } finally {
        isLoading.value = false
      }

      // 2c. 【仅 "我的"】获取已发送的评价
      isSentLoading.value = true
      try {
        const sentResponse = await apiClient.get<SentRating[]>(`${baseUrl}/ratings/sent`)
        sentRatings.value = sentResponse.data
      } catch (e) {
        console.error('Failed to fetch sent ratings', e)
      } finally {
        isSentLoading.value = false
      }
    }
  }

  // --- 其他 Action 保持不变 ---

  function clearRatings() {
    receivedRatings.value = []
    stats.value = null
    pendingRatings.value = []
    sentRatings.value = []
  }

  // fetchPendingRatings 只获取 "我的" 待办，所以保持不变
  async function fetchPendingRatings() {
    isPendingLoading.value = true
    try {
      const response = await apiClient.get<any[]>(`${baseUrl}/ratings/pending`)
      pendingRatings.value = response.data.map((p) => ({
        ...p,
        ratingId: p.pendingRatingId,
        score: 0,
        comments: '',
      }))
    } catch (error) {
      console.error('获取待评价列表失败:', error)
      ElMessage.error('加载待评价任务失败')
    } finally {
      isPendingLoading.value = false
    }
  }

  // submitRating 只提交 "我的" 评价，所以保持不变
  async function submitRating(payload: { ratingId: string; score: number; comments?: string }) {
    isSubmitting.value = true
    try {
      await apiClient.post(`${baseUrl}/Ratings/submit/${payload.ratingId}`, {
        score: payload.score,
        comments: payload.comments,
      })
      pendingRatings.value = pendingRatings.value.filter((p) => p.ratingId !== payload.ratingId)
      return true
    } catch (error) {
      console.error('提交评价失败:', error)
      ElMessage.error('提交评价失败')
      return false
    } finally {
      isSubmitting.value = false
    }
  }

  return {
    receivedRatings,
    isLoading,
    clearRatings,
    pendingRatings,
    isPendingLoading,
    isSubmitting,
    fetchPendingRatings,
    submitRating,
    sentRatings,
    isSentLoading,
    stats,
    isStatsLoading,
    // 【统一】只导出一个 fetchRatings
    fetchRatings,
  }
})
