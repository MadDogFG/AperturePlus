<template>
  <div>
    <div class="header">
      <h1>{{ isOwner ? '我的作品集' : 'TA 的作品集' }}</h1>
      <el-button v-if="isOwner" type="primary" @click="isCreateGalleryDialogVisible = true">
        <el-icon><i-ep-plus /></el-icon>
        <span>创建新相册</span>
      </el-button>
    </div>

    <div v-if="portfolioStore.isLoading" class="loading-state">
      <p>正在加载作品集...</p>
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else-if="portfolioStore.portfolio && portfolioStore.portfolio.galleries.length > 0">
      <div class="gallery-grid">
        <div
          v-for="gallery in portfolioStore.portfolio.galleries"
          :key="gallery.galleryId"
          class="gallery-card"
        >
          <router-link :to="`/profile/portfolio/${gallery.galleryId}`" class="gallery-link">
            <div class="gallery-cover">
              <img v-if="gallery.coverPhotoUrl" :src="gallery.coverPhotoUrl" alt="相册封面" />
              <div v-else class="empty-cover">
                <el-icon><i-ep-picture-rounded /></el-icon>
              </div>
            </div>
            <div class="gallery-info">
              <span class="gallery-name">{{ gallery.galleryName }}</span>
              <span class="photo-count">{{ gallery.photos.length }} 张</span>
            </div>
          </router-link>
          <el-button
            v-if="isOwner"
            class="delete-btn"
            type="danger"
            :icon="Delete"
            circle
            @click.stop="handleDeleteGallery(gallery.galleryId)"
          />
        </div>
      </div>
    </div>
    <div v-else class="empty-state">
      <p>
        {{ isOwner ? '你还没有创建任何相册，点击右上角按钮开始吧！' : 'TA 还没有创建任何相册。' }}
      </p>
    </div>

    <el-dialog v-model="isCreateGalleryDialogVisible" title="创建新相册" width="500">
      <el-input v-model="newGalleryName" placeholder="请输入相册名称" />
      <template #footer>
        <el-button @click="isCreateGalleryDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleCreateGallery">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
// 1. 【修复】恢复 store 的导入
import { usePortfolioStore } from '@/stores/portfolio'
import { Delete } from '@element-plus/icons-vue'

// 2. 【修复】恢复 isOwner prop (由 router 提供)
defineProps({
  isOwner: {
    type: Boolean,
    default: false,
  },
})

// 3. 【修复】恢复 store 实例
const portfolioStore = usePortfolioStore()

// 4. 【修复】恢复 onMounted 数据获取
onMounted(() => {
  // force: true 确保每次进入都刷新，或者您可以删除此参数
  portfolioStore.fetchPortfolio(true)
})

// 5. 【修复】恢复弹窗和创建逻辑
const isCreateGalleryDialogVisible = ref(false)
const newGalleryName = ref('')

const handleCreateGallery = async () => {
  // 直接调用 store 的 action
  const success = await portfolioStore.createGallery(newGalleryName.value)
  if (success) {
    isCreateGalleryDialogVisible.value = false
    newGalleryName.value = ''
    // store.createGallery 内部会乐观更新，无需手动 fetch
  }
}

// 6. 【修复】恢复删除逻辑
const handleDeleteGallery = (galleryId: string) => {
  // 直接调用 store 的 action
  portfolioStore.deleteGallery(galleryId)
}
</script>

<style scoped>
/* 样式保持不变 */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 0 2rem;
}

h1 {
  font-size: 1.8rem;
  font-weight: 600;
}

.gallery-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
  padding: 0 2rem;
}

.gallery-card {
  position: relative;
  border: 1px solid #dcdfe6;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.gallery-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
}

.gallery-link {
  text-decoration: none;
  color: inherit;
}

.gallery-cover {
  width: 100%;
  aspect-ratio: 16 / 10;
  background-color: #f5f7fa;
}

.gallery-cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.empty-cover {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  color: #c0c4cc;
}

.empty-cover .el-icon {
  font-size: 60px;
}

.gallery-info {
  padding: 1rem;
}

.gallery-name {
  font-size: 1.1rem;
  font-weight: 500;
  display: block;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.photo-count {
  font-size: 0.9rem;
  color: #909399;
  margin-top: 0.25rem;
}

.delete-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  background-color: rgba(245, 108, 108, 0.8);
  border-color: transparent;
  backdrop-filter: blur(2px);
}

.delete-btn:hover {
  background-color: rgba(245, 108, 108, 1);
  border-color: transparent;
}

.loading-state,
.empty-state {
  padding: 2rem;
  text-align: center;
  color: #909399;
}
</style>
