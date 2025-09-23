<template>
  <header class="app-header">
    <RouterLink to="/" class="logo">光圈+</RouterLink>
    <nav>
      <template v-if="!authStore.isAuthenticated">
        <RouterLink to="/login">登录</RouterLink>
        <RouterLink to="/register">注册</RouterLink>
      </template>
      <template v-else>
        <RouterLink to="/home">首页</RouterLink>
        <button @click="handleLogout" class="logout-btn">退出登录</button>
      </template>
    </nav>
  </header>
</template>

<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()

const handleLogout = () => {
  authStore.logout()
  // 退出后跳转到登录页
  router.push({ name: 'login' })
}
</script>

<style scoped>
.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background-color: #ffffff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.logo {
  font-size: 1.5rem;
  font-weight: bold;
  color: #333;
  text-decoration: none;
}

nav {
  display: flex;
  align-items: center;
  gap: 1.5rem; /* 链接之间的间距 */
}

nav a {
  text-decoration: none;
  color: #555;
  font-weight: 500;
  transition: color 0.3s;
}

nav a:hover,
nav a.router-link-exact-active {
  color: #409eff;
}

.logout-btn {
  border: none;
  background-color: transparent;
  color: #555;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  padding: 0;
}

.logout-btn:hover {
  color: #f56c6c; /* 红色以示危险操作 */
}
</style>
