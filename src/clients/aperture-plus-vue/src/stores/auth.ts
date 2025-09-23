// src/stores/auth.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

// 'auth' 是这个 store 的唯一 ID，Pinia 用它来连接到 DevTools
export const useAuthStore = defineStore('auth', () => {
  // 1. State (状态): 就像组件里的 ref，这是我们的核心数据
  const token = ref<string | null>(localStorage.getItem('token')) // 从 localStorage 初始化

  // 2. Getters (计算属性): 像组件里的 computed，根据 state 派生出新状态
  const isAuthenticated = computed(() => !!token.value) // 如果 token 存在，则为 true

  // 3. Actions (动作): 修改 state 的函数
  function setToken(newToken: string) {
    token.value = newToken
    localStorage.setItem('token', newToken) // 同步到 localStorage
  }

  function logout() {
    token.value = null
    localStorage.removeItem('token') // 从 localStorage 移除
    // 这里未来还可以加上跳转到登录页的逻辑
    alert('您已退出登录')
  }

  // 4. 返回 store 的公共 API
  return {
    token,
    isAuthenticated,
    setToken,
    logout,
  }
})
