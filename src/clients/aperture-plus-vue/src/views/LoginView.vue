<template>
  <div class="login-page">
    <div class="login-container">
      <h1>光圈+ 登录</h1>

      <input v-model="username" type="text" placeholder="用户名或邮箱" />

      <input v-model="password" type="password" placeholder="密码" />

      <button @click="handleLogin">登录</button>
    </div>
  </div>
</template>

<script setup lang="ts">
// src/views/LoginView.vue

import { ref } from 'vue'
import axios from 'axios' // 1. 导入我们刚刚安装的 axios

const username = ref('')
const password = ref('')

// 2. 将 handleLogin 函数改造为异步函数 (async)
const handleLogin = async () => {
  console.log('正在尝试登录...')

  // 3. 使用 try...catch 来处理可能发生的网络错误
  try {
    // 4. 使用 axios 发送一个 POST 请求
    const response = await axios.post(
      // 我们的 IdentityService API 地址
      'https://localhost:7289/api/Accounts/login',
      {
        // 请求体 (payload)，字段名要和后端的 LoginRequest DTO 一致
        loginIdentifier: username.value,
        password: password.value,
      },
    )

    // 5. 如果请求成功，后端会返回数据，我们把它打印出来
    console.log('登录成功!', response.data)
    alert('登录成功！收到的 Token: ' + response.data.token)
  } catch (error) {
    // 6. 如果请求失败 (比如密码错误)，后端会返回错误信息
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
  min-height: 100vh; /* 高度至少占满整个视口 */
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
