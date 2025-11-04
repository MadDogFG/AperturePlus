<template>
  <aside class="sidebar">
    <div v-if="user" class="user-info">
      <el-avatar
        :size="80"
        :src="user.avatarUrl"
        class="avatar"
        :class="{ clickable: isOwner }"
        @click="isOwner ? (isAvatarDialogVisible = true) : null"
      />
      <h2 class="username">{{ user.userName }}</h2>

      <div class="bio-container">
        <p
          v-if="!isEditingBio"
          @click="isOwner ? startEditingBio() : null"
          class="bio-text"
          :class="{ clickable: isOwner }"
        >
          {{ user.bio || (isOwner ? '点击添加个人简介...' : '暂无简介') }}
        </p>
        <div v-else-if="isOwner" class="edit-mode-bio">
          <el-input v-model="newBio" type="textarea" :rows="3" />
          <div class="edit-buttons">
            <el-button size="small" type="success" @click="saveBio">保存</el-button>
            <el-button size="small" @click="cancelEditingBio">取消</el-button>
          </div>
        </div>
      </div>

      <div class="roles-container">
        <el-tag
          v-for="role in user.roles"
          :key="role"
          class="role-tag"
          type="success"
          effect="light"
          :closable="isOwner && role !== 'User'"
          @close="handleRemoveRole(role)"
        >
          {{ role }}
        </el-tag>
        <el-button
          v-if="isOwner"
          @click="isAddRoleDialogVisible = true"
          class="add-role-btn"
          circle
        >
          <el-icon><i-ep-plus /></el-icon>
        </el-button>
      </div>

      <el-button
        v-if="!isOwner"
        type="primary"
        size="large"
        class="start-chat-btn"
        @click="handleStartChat"
        :loading="isChatLoading"
      >
        发起私聊
      </el-button>
    </div>

    <div v-else>Loading...</div>

    <el-menu class="profile-menu">
      <RouterLink
        :to="{
          name: isOwner ? 'profile-portfolio' : 'public-portfolio',
          params: { userId: user?.userId },
        }"
        v-slot="{ navigate, isActive }"
      >
        <el-menu-item :class="{ 'is-active': isActive }" @click="navigate">
          <el-icon><i-ep-picture-rounded /></el-icon>
          <span>作品集</span>
        </el-menu-item>
      </RouterLink>

      <RouterLink
        :to="{
          name: isOwner ? 'profile-ratings' : 'public-ratings',
          params: { userId: user?.userId },
        }"
        vV-slot="{ navigate, isActive }"
      >
        <el-menu-item :class="{ 'is-active': isActive }" @click="navigate">
          <el-icon><i-ep-star-filled /></el-icon>
          <span>评价</span>
        </el-menu-item>
      </RouterLink>

      <RouterLink v-if="isOwner" to="/profile/history" v-slot="{ navigate, isActive }">
        <el-menu-item :class="{ 'is-active': isActive }" @click="navigate">
          <el-icon><i-ep-clock /></el-icon>
          <span>活动历史</span>
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
import { ref, computed, watch, type PropType } from 'vue'
import { useUserStore, type UserProfile } from '@/stores/user'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useChatStore } from '@/stores/chat'

const props = defineProps({
  user: {
    type: Object as PropType<UserProfile | null>,
    required: true,
  },
  isOwner: {
    type: Boolean,
    default: false,
  },
})

const userStore = useUserStore()
const chatStore = useChatStore()
const isChatLoading = ref(false)
const isEditingBio = ref(false)
const newBio = ref('')
const isAvatarDialogVisible = ref(false)
const newAvatarUrl = ref('')
const isAddRoleDialogVisible = ref(false)
const selectedRoles = ref<string[]>([])

const handleStartChat = async () => {
  if (props.user) {
    isChatLoading.value = true
    try {
      // 4. 【修改】await 调用并添加 try/catch
      await chatStore.openChatWithUser(props.user.userId)
    } catch (error) {
      console.error(error)
      ElMessage.error('发起聊天失败，请稍后再试。')
    } finally {
      isChatLoading.value = false
    }
  }
}

watch(
  () => props.user,
  (newUser) => {
    if (newUser) {
      newBio.value = newUser.bio || ''
      newAvatarUrl.value = newUser.avatarUrl || ''
      selectedRoles.value = newUser.roles || []
    }
  },
  { immediate: true }, // 立即执行一次
)

// --- 所有 methods 保持不变 ---
// 它们仅在 isOwner=true 时被调用，
// 此时 props.user === userStore.profile，
// 调用 userStore.updateProfile() 会同时更新两者，逻辑正确。

const startEditingBio = () => {
  newBio.value = props.user?.bio || ''
  isEditingBio.value = true
}

const cancelEditingBio = () => {
  isEditingBio.value = false
}

const saveBio = async () => {
  const success = await userStore.updateProfile({ bio: newBio.value })
  if (success) {
    ElMessage.success('简介已更新')
    isEditingBio.value = false
  } else {
    ElMessage.error('更新失败')
  }
}

const handleUpdateAvatar = async () => {
  const success = await userStore.updateProfile({ avatarUrl: newAvatarUrl.value })
  if (success) {
    ElMessage.success('头像已更新')
    isAvatarDialogVisible.value = false
  } else {
    ElMessage.error('更新失败')
  }
}

const rolesToShowInDialog = computed(() => {
  const allRoles = ['Photographer', 'Model']
  if (!props.user) return []
  return allRoles.filter((role) => !props.user!.roles.includes(role))
})

const handleRemoveRole = (roleToRemove: string) => {
  if (roleToRemove === 'User') return
  ElMessageBox.confirm(`确定要移除 "${roleToRemove}" 角色吗?`, '确认', {
    type: 'warning',
  })
    .then(async () => {
      if (!props.user) return
      const newRoles = props.user.roles.filter((r) => r !== roleToRemove)
      const success = await userStore.updateRoles(newRoles)
      if (success) {
        ElMessage.success('角色已移除')
      } else {
        ElMessage.error('操作失败')
      }
    })
    .catch(() => {})
}

const handleSaveRoles = async () => {
  if (!props.user) return
  const newRoles = Array.from(new Set([...props.user.roles, ...selectedRoles.value]))
  const success = await userStore.updateRoles(newRoles)
  if (success) {
    ElMessage.success('角色已更新')
    isAddRoleDialogVisible.value = false
  } else {
    ElMessage.error('更新失败')
  }
}
</script>

<style scoped>
/* 【新增】为新按钮添加样式 */
.start-chat-btn {
  width: 100%;
  margin-top: 1rem;
}

/* 保持所有原有样式不变 */
.sidebar {
  width: 280px;
  background: #ffffff;
  border-right: 1px solid #e6e6e6;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
  box-sizing: border-box;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  margin-bottom: 2rem;
}

.avatar {
  margin-bottom: 1rem;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.avatar.clickable {
  cursor: pointer;
  transition: transform 0.2s ease-out;
}

.avatar.clickable:hover {
  transform: scale(1.05);
}

.username {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0 0 0.5rem 0;
}

.bio-container {
  width: 100%;
}

.bio-text {
  font-size: 0.9rem;
  color: #606266;
  min-height: 40px; /* 至少给点高度 */
  line-height: 1.4;
  white-space: pre-wrap; /* 保持换行 */
}

.bio-text.clickable {
  cursor: pointer;
  border-radius: 4px;
  padding: 5px;
  transition: background-color 0.2s;
}

.bio-text.clickable:hover {
  background: #f5f7fa;
}

.edit-mode-bio {
  width: 100%;
}

.edit-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 8px;
}

.roles-container {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 8px;
  margin-top: 1rem;
  width: 100%;
}

.role-tag {
  font-size: 0.85rem;
}

.add-role-btn {
  width: 24px;
  height: 24px;
}

.profile-menu {
  border-right: none;
}

.profile-menu .el-menu-item {
  font-size: 1rem;
  color: #303133;
}

.profile-menu .el-menu-item:hover {
  background-color: #f5f7fa;
}

.profile-menu .el-menu-item.is-active {
  color: var(--el-color-primary);
  background-color: var(--el-color-primary-light-9);
  border-right: 3px solid var(--el-color-primary);
}

.profile-menu a {
  text-decoration: none;
  color: inherit;
}
</style>
