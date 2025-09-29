<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import TheHeader from '@/components/TheHeader.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

onMounted(() => {
  authStore.init()
})
</script>

<template>
  <div class="app-container">
    <TheHeader />
    <main class="main-content">
      <RouterView />
    </main>
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
