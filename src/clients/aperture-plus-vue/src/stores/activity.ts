// src/stores/activity.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Activity } from '@/types/activity'
import apiClient from '@/api/axios' // 复用我们配置好的axios实例

export const useActivityStore = defineStore('activity', () => {
  // --- State ---
  const activities = ref<Activity[]>([])
  const page = ref(1)
  const hasMore = ref(true) // 是否还有更多数据可以加载
  const isLoading = ref(false) // 是否正在加载中，防止重复请求

  // --- Actions ---
  async function fetchActivities() {
    // 如果正在加载，或者已经没有更多数据了，就直接返回
    if (isLoading.value || !hasMore.value) return

    isLoading.value = true
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL // 我们需要在 .env 文件里加上这个
      console.log(baseUrl)
      const response = await apiClient.get(`${baseUrl}/activity/GetAllActivity`, {
        params: {
          page: page.value,
          pagesize: 10, // 每次加载10条
        },
      })
      console.log(response)
      // 如果是第一页，就直接替换列表；否则，追加到现有列表后面
      if (page.value === 1) {
        activities.value = response.data.items || []
      } else {
        activities.value.push(...response.data.items)
      }

      hasMore.value = response.data.hasMore
      // 如果还有更多数据，页码+1，为下次加载做准备
      if (hasMore.value) {
        page.value++
      }
    } catch (error) {
      console.error('获取活动列表失败:', error)
    } finally {
      // 无论成功还是失败，最后都要把加载状态设为 false
      isLoading.value = false
    }
  }

  // 重置状态，用于下拉刷新等场景
  function reset() {
    activities.value = []
    page.value = 1
    hasMore.value = true
    isLoading.value = false
  }

  function updateActivityInList(updatedActivity: Activity) {
    const index = activities.value.findIndex((a) => a.activityId === updatedActivity.activityId)
    if (index !== -1) {
      // 找到了就直接用新的对象替换旧的，Vue 的响应式系统会更新UI
      activities.value[index] = updatedActivity
    }
  }

  return {
    activities,
    hasMore,
    isLoading,
    fetchActivities,
    reset,
    updateActivityInList,
  }
})
