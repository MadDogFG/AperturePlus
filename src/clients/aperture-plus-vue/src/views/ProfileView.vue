<script setup lang="ts">
import { onMounted } from 'vue'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()

// 组件挂载时，尝试获取用户信息
// 这可以防止用户直接访问 /profile 页面时没有数据
onMounted(() => {
  userStore.fetchProfile()
})
</script>

<template>
  <div class="profile-layout">
    <aside class="sidebar">
      <div class="user-info">
        <el-avatar :size="80" :src="userStore.profile?.avatarUrl" class="avatar" />
        <h2 class="username">{{ userStore.profile?.userName }}</h2>
      </div>

      <el-menu class="profile-menu">
        <RouterLink to="/profile/portfolio" v-slot="{ navigate, isActive }">
          <el-menu-item :class="{ 'is-active': isActive }" @click="navigate">
            <el-icon><i-ep-picture-rounded /></el-icon>
            <span>作品集</span>
          </el-menu-item>
        </RouterLink>
        <RouterLink to="/profile/ratings" v-slot="{ navigate, isActive }">
          <el-menu-item :class="{ 'is-active': isActive }" @click="navigate">
            <el-icon><i-ep-star-filled /></el-icon>
            <span>评价</span>
          </el-menu-item>
        </RouterLink>
      </el-menu>
    </aside>

    <div class="content">
      <RouterView />
    </div>
  </div>
</template>

<style scoped>
.profile-layout {
  display: flex;
  height: 100%;
}

.sidebar {
  width: 280px;
  flex-shrink: 0; /* 防止侧边栏被压缩 */
  border-right: 1px solid #e0e0e0;
  background-color: #fafafa;
  padding: 2rem;
}

.user-info {
  text-align: center;
  margin-bottom: 2rem;
}

.avatar {
  margin-bottom: 1rem;
  border: 3px solid #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.username {
  font-size: 1.5rem;
  margin: 0;
}

.profile-menu {
  border-right: none; /* Element Plus 菜单默认有右边框，我们去掉它 */
}

/* 去掉 RouterLink 的下划线 */
.sidebar a {
  text-decoration: none;
}

.content {
  flex-grow: 1; /* 占据所有剩余空间 */
  padding: 2rem;
  overflow-y: auto; /* 如果内容超长，让内容区自己滚动 */
}
</style>
