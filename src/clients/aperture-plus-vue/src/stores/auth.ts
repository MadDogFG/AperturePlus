// src/stores/auth.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useUserStore } from './user'

export const useAuthStore = defineStore('auth', () => {
  // --- State ---
  const token = ref<string | null>(localStorage.getItem('token'))

  // --- Getters ---
  const isAuthenticated = computed(() => !!token.value)

  // --- Actions ---
  async function setToken(newToken: string) {
    token.value = newToken
    localStorage.setItem('token', newToken)

    // 登录成功后，让 userStore 去获取用户信息
    const userStore = useUserStore()
    await userStore.fetchProfile()
  }

  function logout() {
    // 退出时，不仅要清空自己的 token，也要清空 userStore 的数据
    const userStore = useUserStore()
    userStore.clearProfile()

    token.value = null
    localStorage.removeItem('token')
  }

  async function init() {
    if (isAuthenticated.value) {
      const userStore = useUserStore()
      // 如果页面刷新了，userStore是空的，就去重新获取
      if (!userStore.profile) {
        await userStore.fetchProfile()
      }
    }
  }
  return {
    token,
    isAuthenticated,
    setToken,
    logout,
    init,
  }
})
