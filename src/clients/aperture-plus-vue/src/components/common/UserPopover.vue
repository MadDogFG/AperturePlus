<template>
  <el-popover
    placement="top-start"
    :width="300"
    trigger="hover"
    popper-class="user-popover"
    :hide-after="100"
    @show="loadData"
  >
    <template #default>
      <div v-loading="isLoading" class="popover-content">
        <div v-if="profile" class="profile-details">
          <el-avatar :size="60" :src="profile.avatarUrl" />
          <div class="info">
            <h3 class="username">{{ profile.userName }}</h3>
            <p class="bio">{{ profile.bio || '暂无简介' }}</p>
          </div>
        </div>
        <div v-else-if="!isLoading" class="error-text">加载失败</div>
      </div>
    </template>

    <template #reference>
      <router-link :to="destinationLink" class="user-link" @click.stop>
        {{ userName }}
      </router-link>
    </template>
  </el-popover>
</template>

<script setup lang="ts">
import { ref, computed, type PropType } from 'vue' // 【新增】导入 computed
import apiClient from '@/api/axios'
import { useUserStore, type UserProfile } from '@/stores/user' // 【新增】导入 useUserStore

const props = defineProps({
  userId: {
    type: String,
    required: true,
  },
  userName: {
    type: String,
    required: true,
  },
})

// --- 【新增】获取当前登录的用户信息 ---
const userStore = useUserStore()

// --- 【新增】计算目标链接 ---
const destinationLink = computed(() => {
  // 检查 store 是否有 profile，并且 ID 是否与当前 popover 的 ID 匹配
  if (userStore.profile && userStore.profile.userId === props.userId) {
    return '/profile' // 是我！跳转到我自己的主页
  }
  return `/user/${props.userId}` // 是别人！跳转到 TA 的公共主页
})
// ---------------------------------

const profile = ref<UserProfile | null>(null)
const isLoading = ref(false)
const hasLoaded = ref(false)

const userProfileBaseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL

const loadData = async () => {
  if (isLoading.value || hasLoaded.value) {
    return
  }

  isLoading.value = true
  try {
    const response = await apiClient.get<UserProfile>(
      `${userProfileBaseUrl}/userprofile/GetUserProfileById/${props.userId}`,
    )
    profile.value = response.data
    hasLoaded.value = true
  } catch (error) {
    console.error('Failed to fetch user popover data:', error)
    hasLoaded.value = false
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* 样式保持不变 */
.user-link {
  color: var(--el-color-primary);
  text-decoration: none;
  font-weight: 500;
}
.user-link:hover {
  text-decoration: underline;
}

.popover-content {
  min-height: 80px;
}

.profile-details {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.profile-details .info {
  display: flex;
  flex-direction: column;
}

.profile-details .username {
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0;
}

.profile-details .bio {
  font-size: 0.9rem;
  color: #606266;
  margin: 4px 0 0 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.4;
}

.error-text {
  color: #f56c6c;
  text-align: center;
  padding-top: 20px;
}
</style>
