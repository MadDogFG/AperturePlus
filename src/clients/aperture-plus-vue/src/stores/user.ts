// src/stores/user.ts

import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/api/axios'

// 1. 更新 UserProfile 接口，明确包含 roles 字段
export interface UserProfile {
  userId: string
  userName: string
  bio: string
  avatarUrl: string
  roles: string[] // 角色列表由 API 提供
}

export const useUserStore = defineStore('user', () => {
  // --- State ---
  const profile = ref<UserProfile | null>(null)

  // --- Actions ---
  function setProfile(userProfile: UserProfile) {
    profile.value = userProfile
  }

  function clearProfile() {
    profile.value = null
  }

  async function fetchProfile() {
    if (profile.value) return // 如果已有数据，则不重复获取
    try {
      const baseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL
      const response = await apiClient.get<UserProfile>(`${baseUrl}/userprofile/GetMyProfile`)
      setProfile(response.data)
    } catch (error) {
      console.error('获取用户信息失败:', error)
    }
  }

  async function updateProfile(payload: Partial<Omit<UserProfile, 'userId' | 'userName'>>) {
    try {
      const baseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL
      await apiClient.patch(`${baseUrl}/userprofile/UpdateMyProfile`, payload)

      // 更新成功后，直接在前端修改 state，UI 会自动响应，无需重新 fetch
      if (profile.value) {
        // 使用 Object.assign 优雅地合并更新
        Object.assign(profile.value, payload)
      }
      return true
    } catch (error) {
      console.error('更新用户信息失败:', error)
      return false
    }
  }

  async function updateRoles(roles: string[]) {
    try {
      const baseUrl = import.meta.env.VITE_API_IDENTITY_BASE_URL
      await apiClient.put(`${baseUrl}/accounts/UpdateRoles`, { roles })

      // 更新成功后，同样直接修改本地 state
      if (profile.value) {
        profile.value.roles = roles
      }
      return true
    } catch (error) {
      console.error('更新角色失败:', error)
      return false
    }
  }

  return {
    profile,
    setProfile,
    clearProfile,
    fetchProfile,
    updateProfile,
    updateRoles,
  }
})
