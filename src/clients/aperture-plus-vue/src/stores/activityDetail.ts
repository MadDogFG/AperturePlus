// src/stores/activityDetail.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type {
  ActivityDetail,
  Participant,
  ParticipantStatus,
  RoleType,
  Activity,
} from '@/types/activity'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from './user'
import { useActivityStore } from './activity'

interface UserProfile {
  userId: string
  userName: string
  avatarUrl: string
}

export const useActivityDetailStore = defineStore('activityDetail', () => {
  // --- State ---
  const activity = ref<ActivityDetail | null>(null)
  const isLoading = ref(false)

  // --- Actions ---

  async function fetchActivity(id: string) {
    isLoading.value = true
    activity.value = null // 先清空旧数据
    try {
      // 1. 获取活动基本信息
      const activityBaseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      const response = await apiClient.get<ActivityDetail>(
        `${activityBaseUrl}/activity/GetActivityById/${id}`,
      )
      const fetchedActivity = response.data

      // 2. 准备一个数组来存放所有获取用户信息的请求
      const userProfileBaseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL
      const userProfilePromises = fetchedActivity.participants.map((p) =>
        apiClient.get<UserProfile>(
          `${userProfileBaseUrl}/userprofile/GetUserProfileById/${p.userId}`,
        ),
      )

      // 3. 并发执行所有请求
      const userProfileResponses = await Promise.all(userProfilePromises)

      // 4. 创建一个用户ID到用户名的映射表，方便查找
      const userMap = new Map<string, UserProfile>()
      userProfileResponses.forEach((res) => {
        userMap.set(res.data.userId, res.data)
      })

      // 5. 遍历参与者列表，用映射表的数据补全信息
      fetchedActivity.participants.forEach((p) => {
        const profile = userMap.get(p.userId)
        if (profile) {
          p.userName = profile.userName
          p.avatarUrl = profile.avatarUrl
        } else {
          p.userName = '未知用户' // 作为降级处理
        }
      })

      // 6. 最后，将完整的数据赋值给 state
      activity.value = fetchedActivity
    } catch (error) {
      console.error('获取活动详情或用户信息失败:', error)
      ElMessage.error('加载活动信息失败')
    } finally {
      isLoading.value = false
    }
  }

  function _updateParticipantStatus(applicantId: string, newStatus: ParticipantStatus) {
    if (activity.value) {
      const participant = activity.value.participants.find((p) => p.userId === applicantId)
      if (participant) {
        participant.status = newStatus
      }
    }
  }

  async function approveParticipant(activityId: string, applicantId: string, role: RoleType) {
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      await apiClient.post(
        `${baseUrl}/activity/ApproveParticipant/${activityId}/${applicantId}`,
        null,
        {
          params: {
            roleString: role,
          },
        },
      )
      _updateParticipantStatus(applicantId, 'Approved')
      ElMessage.success('已批准该用户')
    } catch (error) {
      console.error('批准失败:', error)
      ElMessage.error('操作失败，请重试')
    }
  }

  async function rejectParticipant(activityId: string, applicantId: string) {
    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      await apiClient.post(`${baseUrl}/activity/RejectParticipant/${activityId}/${applicantId}`)
      _updateParticipantStatus(applicantId, 'Rejected')
      ElMessage.info('已拒绝/移除该用户')
    } catch (error) {
      console.error('拒绝失败:', error)
      ElMessage.error('操作失败，请重试')
    }
  }

  // --- 新增 Action ---
  async function requestJoinActivity(activityId: string, role: RoleType) {
    const userStore = useUserStore()
    const activityStore = useActivityStore()

    if (!userStore.profile) {
      ElMessage.error('无法获取当前用户信息，请重新登录。')
      return false
    }

    try {
      const baseUrl = import.meta.env.VITE_API_ACTIVITY_BASE_URL
      await apiClient.post(`${baseUrl}/activity/RequestJoinActivity/${activityId}`, null, {
        params: { roleString: role },
      })

      if (activity.value) {
        // 1. 乐观更新当前详情页的数据
        const newParticipant: Participant = {
          userId: userStore.profile.userId,
          userName: userStore.profile.userName,
          avatarUrl: userStore.profile.avatarUrl,
          role: role,
          status: 'Pending',
        }
        activity.value.participants.push(newParticipant)

        // 2. 【完整代码】手动构建一个符合首页 Activity 类型的对象
        const totalRequired = activity.value.roleRequirements.reduce(
          (sum, req) => sum + req.quantity,
          0,
        )

        const updatedActivityForList: Activity = {
          activityId: activity.value.activityId,
          activityTitle: activity.value.activityTitle,
          activityDescription: activity.value.activityDescription,
          activityLocation: activity.value.activityLocation,
          activityStartTime: activity.value.activityStartTime,
          postedByUser: activity.value.postedByUser,
          status: activity.value.status,
          fee: activity.value.fee,
          roleRequirements: activity.value.roleRequirements,
          totalRequiredCount: totalRequired, // 重新计算总需求人数
          approvedParticipantsCount: activity.value.participants.filter(
            (p) => p.status === 'Approved',
          ).length,
          pendingParticipantsCount: activity.value.participants.filter(
            (p) => p.status === 'Pending',
          ).length,
        }

        // 3. 调用首页 store 的方法，同步状态
        activityStore.updateActivityInList(updatedActivityForList)
      }

      ElMessage.success('申请已发送！')
      return true
    } catch (error: any) {
      console.error('申请加入失败:', error)
      const errorMsg = error.response?.data?.message || '申请失败，请稍后再试。'
      ElMessage.error(errorMsg)
      return false
    }
  }

  return {
    activity,
    isLoading,
    fetchActivity,
    approveParticipant,
    rejectParticipant,
    requestJoinActivity, // 导出新 action
  }
})
