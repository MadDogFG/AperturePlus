<template>
  <div class="profile-layout">
    <ProfileSidebar />

    <div class="content">
      <RouterView />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import ProfileSidebar from '@/components/profile/ProfileSidebar.vue'

const userStore = useUserStore()

// 在父组件挂载时，确保用户信息被获取
onMounted(() => {
  userStore.fetchProfile()
})
</script>

<style scoped>
.profile-layout {
  display: flex;
  height: 100%; /* 占满 main-content 的高度 */
  width: 100%;
}

.content {
  flex-grow: 1;
  padding: 2rem;
  overflow-y: auto;
}
</style>
