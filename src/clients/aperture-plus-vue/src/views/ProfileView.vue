<script setup lang="ts">
// src/views/ProfileView.vue

import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()

// 1. 新增状态，控制“修改头像”对话框的显示
const isAvatarDialogVisible = ref(false)
// 2. 新增状态，用于绑定到对话框里的 input 输入框
const newAvatarUrl = ref('')

onMounted(() => {
  userStore.fetchProfile()
})

// 3. 定义打开对话框的函数
const openAvatarDialog = () => {
  if (userStore.profile) {
    // 打开时，可以把当前的头像URL填充到输入框
    newAvatarUrl.value = userStore.profile.avatarUrl
    isAvatarDialogVisible.value = true
  }
}

// 4. 定义保存新头像的函数
const handleUpdateAvatar = async () => {
  if (newAvatarUrl.value.trim() === '') {
    alert('头像URL不能为空！')
    return
  }

  const success = await userStore.updateProfile({ avatarUrl: newAvatarUrl.value })

  if (success) {
    isAvatarDialogVisible.value = false // 关闭对话框
    alert('头像更新成功！')
  } else {
    alert('更新失败，请稍后再试。')
  }
}
</script>

<template>
  <div class="profile-layout">
    <aside class="sidebar">
      <div class="user-info">
        <el-avatar
          :size="80"
          :src="userStore.profile?.avatarUrl"
          class="avatar clickable"
          @click="openAvatarDialog"
        />
        <h2 class="username">{{ userStore.profile?.userName }}</h2>
      </div>

      <el-dialog v-model="isAvatarDialogVisible" title="更换头像" width="500">
        <el-input v-model="newAvatarUrl" placeholder="请粘贴新的头像图片URL" />
        <template #footer>
          <div class="dialog-footer">
            <el-button @click="isAvatarDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="handleUpdateAvatar"> 保存 </el-button>
          </div>
        </template>
      </el-dialog>

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

.clickable {
  cursor: pointer;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}

.clickable:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
</style>
