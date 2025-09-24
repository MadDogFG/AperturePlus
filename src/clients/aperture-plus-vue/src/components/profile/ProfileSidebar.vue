<template>
  <aside class="sidebar">
    <div v-if="userStore.profile" class="user-info">
      <el-avatar
        :size="80"
        :src="userStore.profile.avatarUrl"
        class="avatar clickable"
        @click="isAvatarDialogVisible = true"
      />
      <h2 class="username">{{ userStore.profile.userName }}</h2>

      <div class="bio-container">
        <p v-if="!isEditingBio" @click="startEditingBio" class="bio-text clickable">
          {{ userStore.profile.bio || '点击添加个人简介...' }}
        </p>
        <div v-else class="edit-mode-bio">
          <el-input v-model="newBio" type="textarea" :rows="3" />
          <div class="edit-buttons">
            <el-button size="small" type="success" @click="saveBio">保存</el-button>
            <el-button size="small" @click="cancelEditingBio">取消</el-button>
          </div>
        </div>
      </div>

      <div class="roles-container">
        <el-tag v-for="role in userStore.profile.roles" :key="role" type="success" effect="light">
          {{ role }}
        </el-tag>
        <el-button @click="isAddRoleDialogVisible = true" class="add-role-btn" circle>
          <el-icon><i-ep-plus /></el-icon>
        </el-button>
      </div>
    </div>
    <div v-else>Loading...</div>

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

    <el-dialog v-model="isAvatarDialogVisible" title="更换头像" width="500">
      <el-input v-model="newAvatarUrl" placeholder="请粘贴新的头像图片URL" />
      <template #footer>
        <el-button @click="isAvatarDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleUpdateAvatar">保存</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="isAddRoleDialogVisible" title="添加新角色" width="500">
      <el-checkbox-group v-model="selectedRoles">
        <el-checkbox v-for="role in rolesToShowInDialog" :key="role" :label="role" :value="role" />
      </el-checkbox-group>
      <template #footer>
        <el-button @click="isAddRoleDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveRoles">保存</el-button>
      </template>
    </el-dialog>
  </aside>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useUserStore } from '@/stores/user'
import { ElMessage } from 'element-plus'

const userStore = useUserStore()

// --- 编辑状态 ---
const isEditingBio = ref(false)
const newBio = ref('')
const isAvatarDialogVisible = ref(false)
const newAvatarUrl = ref('')
const isAddRoleDialogVisible = ref(false)
const selectedRoles = ref<string[]>([])

// --- 可用角色列表 ---
const availableRoles = ['Photographer', 'Model', 'Requester']
const rolesToShowInDialog = computed(() => {
  return availableRoles.filter((role) => !userStore.profile?.roles.includes(role))
})

// --- 事件处理 ---
const startEditingBio = () => {
  if (userStore.profile) {
    newBio.value = userStore.profile.bio
    isEditingBio.value = true
  }
}

const cancelEditingBio = () => {
  isEditingBio.value = false
}

const saveBio = async () => {
  const success = await userStore.updateProfile({ bio: newBio.value })
  if (success) {
    isEditingBio.value = false
    ElMessage.success('简介更新成功！')
  } else {
    ElMessage.error('更新失败，请稍后再试。')
  }
}

const handleUpdateAvatar = async () => {
  if (!newAvatarUrl.value.trim()) return ElMessage.warning('URL不能为空')
  const success = await userStore.updateProfile({ avatarUrl: newAvatarUrl.value })
  if (success) {
    isAvatarDialogVisible.value = false
    ElMessage.success('头像更新成功！')
  } else {
    ElMessage.error('更新失败，请稍后再试。')
  }
}

const handleSaveRoles = async () => {
  if (selectedRoles.value.length === 0) return ElMessage.warning('请选择角色')

  // 构造完整的角色列表
  const newRoleList = [...new Set([...(userStore.profile?.roles || []), ...selectedRoles.value])]

  const success = await userStore.updateRoles(newRoleList)
  if (success) {
    isAddRoleDialogVisible.value = false
    ElMessage.success('角色更新成功！部分更改可能需要重新登录生效。')
  } else {
    ElMessage.error('更新失败，请稍后再试。')
  }
}
</script>

<style scoped>
.sidebar {
  width: 280px;
  flex-shrink: 0;
  border-right: 1px solid #e0e0e0;
  background-color: #fafafa;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}
.user-info {
  text-align: center;
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
.clickable {
  cursor: pointer;
}
.bio-container {
  margin-top: 1rem;
  width: 100%;
}
.bio-text {
  color: #606266;
  font-size: 0.9rem;
  line-height: 1.5;
  white-space: pre-wrap;
  border: 1px dashed transparent;
  padding: 5px;
}
.bio-text:hover {
  border-color: #dcdfe6;
}
.edit-mode-bio .edit-buttons {
  margin-top: 8px;
  display: flex;
  justify-content: flex-end;
}
.roles-container {
  margin-top: 1.5rem;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  align-items: center;
  justify-content: center;
}
.add-role-btn {
  width: 24px;
  height: 24px;
}
.profile-menu {
  border-right: none;
}
.sidebar a {
  text-decoration: none;
}
</style>
