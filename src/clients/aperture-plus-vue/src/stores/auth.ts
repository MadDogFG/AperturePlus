// src/stores/auth.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useUserStore } from './user' //导入 user store

// 'auth' 是这个 store 的唯一 ID，Pinia 用它来连接到 DevTools
export const useAuthStore = defineStore('auth', () => {
  // State (状态): 就像组件里的 ref，这是我们的核心数据
  const token = ref<string | null>(localStorage.getItem('token')) // 从 localStorage 初始化

  // Getters (计算属性): 像组件里的 computed，根据 state 派生出新状态
  const isAuthenticated = computed(() => !!token.value) // 如果 token 存在，则为 true

  // Actions (动作): 修改 state 的函数
  async function setToken(newToken: string) {
    token.value = newToken
    localStorage.setItem('token', newToken)

    // 登录成功后，获取用户信息
    const userStore = useUserStore()
    await userStore.fetchProfile()
  }

  function logout() {
    const userStore = useUserStore()
    userStore.clearProfile() // 退出时清空用户信息

    token.value = null
    localStorage.removeItem('token')
    alert('您已退出登录')
  }

  // 返回 store 的公共 API
  return {
    token,
    isAuthenticated,
    setToken,
    logout,
  }
})
