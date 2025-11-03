<template>
  <div class="profile-layout">
    <ProfileSidebar :user="userStore.profile" :is-owner="true" />

    <div class="content">
      <RouterView />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
// 1. 导入重构后的 Sidebar
import ProfileSidebar from '@/components/profile/ProfileSidebar.vue'

// 2. useUserStore 保持不变，它需要在 onMounted 时获取数据
const userStore = useUserStore()

// 3. onMounted 保持不变
onMounted(() => {
  // 这会触发 userStore.fetchProfile()
  // Pinia store 会被填充，并通过 :user="userStore.profile" 传给 Sidebar
  // 子路由 (Portfolio, Ratings) 也会在它们自己的 onMounted 中从 store 获取数据
})
</script>

<style scoped>
/* 样式保持不变 */
.profile-layout {
  display: flex;
  width: 100%;
  height: 100%;
  background-color: #f5f7fa; /* 给内容区一个背景色 */
}

.content {
  flex-grow: 1;
  overflow-y: auto; /* 让内容区可以滚动 */
  height: 100%;
}
</style>
