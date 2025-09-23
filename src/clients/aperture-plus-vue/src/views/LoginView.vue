<template>
  <div class="login-page">
    <div class="login-container">
      <h1>光圈+ 登录</h1>
      <input v-model="username" type="text" placeholder="用户名或邮箱" />
      <input v-model="password" type="password" placeholder="密码" />
      <button @click="handleLogin">登录</button>
      <p>还没有账号？ <RouterLink to="/register">立即注册</RouterLink></p>
      <p>已有账号？ <RouterLink to="/login">立即登录</RouterLink></p>
    </div>
  </div>
</template>

<script setup lang="ts">
// src/views/LoginView.vue

import { ref } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth' // 1. 导入我们的 auth store
import { useRouter } from 'vue-router'

const username = ref('')
const password = ref('')
const authStore = useAuthStore() // 2. 获取 store 的实例
const router = useRouter()

const handleLogin = async () => {
  try {
    const baseUrl = import.meta.env.VITE_API_IDENTITY_BASE_URL
    const url = `${baseUrl}/accounts/login`
    const response = await axios.post(url, {
      loginIdentifier: username.value,
      password: password.value,
    })

    // 3. 登录成功后，调用 store 的 action 来保存 token
    authStore.setToken(response.data.token)

    console.log('登录成功! Token 已保存。')
    alert('登录成功!')
    router.push({ name: 'home' })
  } catch (error) {
    console.error('登录失败:', error)
    alert('登录失败，请检查用户名和密码。')
  }
}
</script>

<style scoped>
/* 整个页面的容器，让登录框可以在垂直和水平方向上都居中 */
.login-page {
  width: 100%;
  display: flex;
  justify-content: center; /* 水平居中 */
  align-items: center; /* 垂直居中 */
  width: 100%;
  height: 100%; /* 高度占满父容器（main-content）的 100% */
  background-color: #f0f2f5; /* 一个柔和的背景色 */
}

/* 登录表单的容器 */
.login-container {
  display: flex;
  flex-direction: column; /* 让内部元素垂直排列 */
  align-items: center; /* 内部元素水平居中 */
  padding: 40px;
  background-color: white;
  border-radius: 8px; /* 圆角 */
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); /* 添加一点阴影，让它看起来有立体感 */
  width: 100%;
  max-width: 400px; /* 限制最大宽度 */
}

h1 {
  margin-top: 0;
  margin-bottom: 24px;
  font-size: 24px;
  color: #333;
}

/* 统一设置输入框的样式 */
input {
  width: 100%;
  padding: 12px 16px;
  margin-bottom: 16px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 16px;
  box-sizing: border-box; /* 让 padding 不会撑大盒子的宽度 */
  transition: border-color 0.2s; /* 添加一个过渡效果，让交互更平滑 */
}

/* 当用户点击输入框时（获得焦点），边框变色 */
input:focus {
  outline: none; /* 去掉默认的蓝色轮廓 */
  border-color: #409eff;
}

/* 统一设置按钮的样式 */
button {
  width: 100%;
  padding: 12px 16px;
  border: none;
  border-radius: 4px;
  background-color: #409eff; /* 主题蓝色 */
  color: white;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer; /* 鼠标悬停时显示小手图标 */
  transition: background-color 0.2s; /* 添加过渡效果 */
}

/* 鼠标悬停在按钮上时，颜色变深一点 */
button:hover {
  background-color: #337ecc;
}
</style>
