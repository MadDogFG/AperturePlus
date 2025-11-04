<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { RouterView } from 'vue-router'
import TheHeader from '@/components/TheHeader.vue'
import { useAuthStore } from '@/stores/auth'
import { useChatStore } from '@/stores/chat'
import ChatWindow from '@/components/chat/ChatWindow.vue'

const authStore = useAuthStore()
const chatStore = useChatStore()

onMounted(() => {
  authStore.init()
})

watch(
  () => authStore.isAuthenticated,
  (isAuth) => {
    if (isAuth) {
      // 如果用户已登录 (isAuthenticated 变为 true)
      // 立即开始连接 SignalR
      chatStore.startConnection()
    } else {
      // (可选) 在 chatStore 中添加 stopConnection() 并在登出时调用
    }
  },
  { immediate: true },
) // 立即执行一次，确保页面加载时如果已登录就连接
</script>

<template>
  <div class="app-container">
    <TheHeader />
    <main class="main-content">
      <RouterView />
    </main>
    <ChatWindow />
  </div>
</template>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column; /* 让子元素（header, main）垂直排列 */
  height: 100vh; /* 容器总高度撑满视口 */
}

.main-content {
  flex-grow: 1; /* 让 main 区域占据所有剩余的垂直空间 */
  /* 为了让 login-page 的居中继续生效，我们让 main 也变成一个 flex 容器 */
  display: flex;
}
</style>
