// src/stores/activityDetail.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ActivityDetail, ParticipantStatus, RoleType } from '@/types/activity'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'

export const useActivityDetailStore = defineStore('activityDetail', () => {
  // --- State ---
  const activity = ref<ActivityDetail | null>(null)
  const isLoading = ref(false)

  // --- Actions ---

  // 获取活动详情
  async function fetchActivity(id: string) {
    isLoading.value = true
    activity.value = null
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      const response = await apiClient.get<ActivityDetail>(
        `${baseUrl}/activity/GetActivityById/${id}`,
      )
      activity.value = response.data
    } catch (error) {
      console.error('获取活动详情失败:', error)
      ElMessage.error('加载活动信息失败')
    } finally {
      isLoading.value = false
    }
  }

  // 内部更新参与者状态的辅助函数，用于乐观更新UI
  function _updateParticipantStatus(applicantId: string, newStatus: ParticipantStatus) {
    if (activity.value) {
      const participant = activity.value.participants.find((p) => p.userId === applicantId)
      if (participant) {
        participant.status = newStatus
      }
    }
  }

  // 批准参与者
  async function approveParticipant(activityId: string, applicantId: string, role: RoleType) {
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      // 假设后端需要role信息来批准
      await apiClient.post(
        `${baseUrl}/activity/${activityId}/participants/${applicantId}/approve`,
        { role: role.toString() },
      )
      _updateParticipantStatus(applicantId, 'Approved')
      ElMessage.success('已批准该用户')
    } catch (error) {
      console.error('批准失败:', error)
      ElMessage.error('操作失败，请重试')
    }
  }

  // 拒绝/移除参与者
  async function rejectParticipant(activityId: string, applicantId: string) {
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      await apiClient.post(`${baseUrl}/activity/${activityId}/participants/${applicantId}/reject`)
      _updateParticipantStatus(applicantId, 'Rejected')
      ElMessage.info('已拒绝/移除该用户')
    } catch (error) {
      console.error('拒绝失败:', error)
      ElMessage.error('操作失败，请重试')
    }
  }

  return {
    activity,
    isLoading,
    fetchActivity,
    approveParticipant,
    rejectParticipant,
  }
})
