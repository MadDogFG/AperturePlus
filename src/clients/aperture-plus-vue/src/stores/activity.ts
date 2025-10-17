/* eslint-disable @typescript-eslint/no-explicit-any */
// src/stores/activity.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Activity } from '@/types/activity'
import { ElMessage } from 'element-plus' // 引入 ElMessage 用于提示
import apiClient from '@/api/axios' // 复用我们配置好的axios实例
import { useUserStore } from './user'

export const useActivityStore = defineStore('activity', () => {
  // --- State ---
  const activities = ref<Activity[]>([])
  const page = ref(1)
  const hasMore = ref(true) // 是否还有更多数据可以加载
  const isLoading = ref(false) // 是否正在加载中，防止重复请求
  const activityHistory = ref<Activity[]>([])
  const isHistoryLoading = ref(false)

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
          pagesize: 50, // 每次加载10条
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

  async function createActivity(activityData: any) {
    // [!code focus start]
    // 1. 前端验证：创建者必须选择一个角色
    if (!activityData.creatorRole) {
      ElMessage.error('您必须选择自己在活动中的角色')
      return false
    }

    // 2. 根据创建者的角色，计算最终需要的角色数量
    let requiredPhotographers = activityData.photographerCount
    let requiredModels = activityData.modelCount
    if (activityData.creatorRole === 'Photographer') {
      requiredPhotographers += 1 // 加上创建者本人
    } else {
      requiredModels += 1 // 加上创建者本人
    }

    // 3. 构建 RoleRequirements 数组
    const roleRequirements = []
    if (requiredPhotographers > 0) {
      roleRequirements.push({ role: 'Photographer', quantity: requiredPhotographers })
    }
    if (requiredModels > 0) {
      roleRequirements.push({ role: 'Model', quantity: requiredModels })
    }

    // 3. 构建将要发送给后端的最终数据
    const payload = {
      activityTitle: activityData.activityTitle,
      activityDescription: activityData.activityDescription,
      activityLocation: {
        latitude: 0,
        longitude: 0,
      },
      activityStartTime: activityData.activityStartTime,
      fee: activityData.fee,
      roleRequirements: roleRequirements,
      creatorRole: activityData.creatorRole,
    }

    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      // 4. 发送 POST 请求到后端API
      const response = await apiClient.post(`${baseUrl}/activity/CreateActivity`, payload)
      ElMessage.success('活动创建成功')
      // 5. 成功后重置活动列表并重新加载第一页
      reset()
      await fetchActivities()
      return response.data // 返回 true 表示成功
    } catch (error: any) {
      const errorMsg = error.response?.data?.message || '创建活动失败，请检查填写的内容。'
      ElMessage.error(errorMsg)
      console.error('创建活动失败:', error)
      return null
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

  async function fetchActivityHistory() {
    const userStore = useUserStore()
    if (!userStore.profile) {
      ElMessage.error('无法获取用户信息，请重新登录。')
      return
    }

    isHistoryLoading.value = true
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL

      // 第一步：发起API请求并等待它完成。注意括号在这里结束。
      const response = await apiClient.get<Activity[]>(
        `${baseUrl}/activity/GetActivitiesByUserId/${userStore.profile.userId}`,
      ) // [!code focus] <-- API 调用在这里结束

      // 第二步：在API调用成功后，使用返回的 response 来处理数据。
      activityHistory.value = response.data.filter(
        // [!code focus]
        (act) => act.status === 'Completed' || act.status === 'Cancelled',
      )
    } catch (error) {
      console.error('获取活动历史失败:', error)
      ElMessage.error('加载活动历史失败')
    } finally {
      isHistoryLoading.value = false
    }
  }

  return {
    // 已有的
    activities,
    hasMore,
    isLoading,
    fetchActivities,
    reset,
    updateActivityInList,
    createActivity,

    // 新增的
    activityHistory,
    isHistoryLoading,
    fetchActivityHistory,
  }
})
