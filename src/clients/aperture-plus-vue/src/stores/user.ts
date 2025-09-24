// src/stores/user.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/api/axios' // 导入配置好的 axios 实例

// 定义 UserProfile 的数据结构，要和后端的 UserProfileDto.cs 一致
export interface UserProfile {
  userId: string
  userName: string
  bio: string
  avatarUrl: string
}

export const useUserStore = defineStore('user', () => {
  const profile = ref<UserProfile | null>(null)

  function setProfile(userProfile: UserProfile) {
    profile.value = userProfile
  }

  function clearProfile() {
    profile.value = null
  }

  async function fetchProfile() {
    // 如果已经有用户信息，就不重复获取
    if (profile.value) {
      return
    }
    try {
      const baseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL
      const url = `${baseUrl}/userprofile/GetMyProfile`
      const response = await apiClient.get<UserProfile>(url)
      setProfile(response.data)
    } catch (error) {
      console.error('获取用户信息失败:', error)
      // 这里可以触发登出逻辑，因为token可能已失效
    }
  }

  async function updateProfile(payload: { bio?: string; avatarUrl?: string }) {
    try {
      const baseUrl = import.meta.env.VITE_API_IDENTITY_BASE_URL
      const url = `${baseUrl}/userprofile/UpdateMyProfile`

      await apiClient.patch(url, payload)

      // 更新成功后，直接在前端修改 profile 的值，避免重新请求
      if (profile.value && payload.bio) {
        profile.value.bio = payload.bio
      }
      if (profile.value && payload.avatarUrl) {
        profile.value.avatarUrl = payload.avatarUrl
      }

      return true // 返回成功状态
    } catch (error) {
      console.error('更新用户信息失败:', error)
      return false // 返回失败状态
    }
  }

  return {
    profile,
    setProfile,
    clearProfile,
    fetchProfile,
    updateProfile,
  }
})
