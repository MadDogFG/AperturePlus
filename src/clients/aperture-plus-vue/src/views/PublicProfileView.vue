<template>
  <div class="profile-layout">
    <ProfileSidebar :user="user" :is-owner="false" />

    <div class="content">
      <RouterView />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import ProfileSidebar from '@/components/profile/ProfileSidebar.vue'
import apiClient from '@/api/axios' // 【新增】直接使用 apiClient
import type { UserProfile } from '@/stores/user' // 【新增】导入类型

const route = useRoute()

// 1. 【修复】创建本地 user ref，不再依赖 userStore
const user = ref<UserProfile | null>(null)
const userProfileBaseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL

// 2. 【修复】创建本地 fetchUser 逻辑
async function fetchUser(userId: string) {
  user.value = null // 切换时先清空
  try {
    const response = await apiClient.get<UserProfile>(
      `${userProfileBaseUrl}/userprofile/GetUserProfileById/${userId}`,
    )
    user.value = response.data
  } catch (error) {
    console.error('获取他人用户信息失败:', error)
  }
}

// 3. onMounted 时获取
onMounted(() => {
  fetchUser(route.params.userId as string)
})

// 4. 监听路由，以便在不同用户主页间切换
watch(
  () => route.params.userId,
  (newUserId) => {
    if (newUserId) {
      fetchUser(newUserId as string)
    }
  },
)
</script>

<style scoped>
/* 样式与 ProfileView 完全一致 */
.profile-layout {
  display: flex;
  width: 100%;
  height: 100%;
  background-color: #f5f7fa;
}

.content {
  flex-grow: 1;
  overflow-y: auto;
  height: 100%;
}
</style>
