<template>
  <div class="register-page">
    <div class="register-container">
      <h1>光圈+ 注册</h1>

      <input v-model="username" type="text" placeholder="用户名" />
      <input v-model="email" type="text" placeholder="邮箱" />
      <input v-model="password" type="password" placeholder="密码" />

      <button @click="handleRegister">注册</button>
      <p>还没有账号？ <RouterLink to="/register">立即注册</RouterLink></p>
      <p>已有账号？ <RouterLink to="/login">立即登录</RouterLink></p>
    </div>
  </div>
</template>

<script setup lang="ts">
// src/views/LoginView.vue

import { ref } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router' // 1. 从 vue-router 导入 useRouter

const username = ref('')
const email = ref('')
const password = ref('')

const router = useRouter() // 2. 获取 router 实例

const handleRegister = async () => {
  try {
    const response = await axios.post('https://localhost:7289/api/accounts/register', {
      username: username.value,
      password: password.value,
      email: email.value,
    })

    console.log('注册成功:', response)
    alert('注册成功! 现在将跳转到登录页面。')
    router.push({ name: 'login' })
  } catch (error) {
    console.error('注册失败:', error)
    alert('注册失败:' + error)
  }
}
</script>

<style scoped>
/* 整个页面的容器，让登录框可以在垂直和水平方向上都居中 */
.register-page {
  width: 100%;
  display: flex;
  justify-content: center; /* 水平居中 */
  align-items: center; /* 垂直居中 */
  min-height: 100vh; /* 高度至少占满整个视口 */
  background-color: #f0f2f5; /* 一个柔和的背景色 */
}

/* 登录表单的容器 */
.register-container {
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
