<template>
  <div>
    <div class="header">
      <h1>我的作品集</h1>
      <el-button type="primary" @click="isCreateGalleryDialogVisible = true">
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
            class="delete-btn"
            type="danger"
            :icon="Delete"
            circle
            @click.stop="portfolioStore.deleteGallery(gallery.galleryId)"
          />
        </div>
      </div>
    </div>

    <div v-else class="empty-state">
      <p>你还没有创建任何相册，点击右上角按钮开始吧！</p>
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
import { usePortfolioStore } from '@/stores/portfolio'
import { Delete } from '@element-plus/icons-vue'

const portfolioStore = usePortfolioStore()

const isCreateGalleryDialogVisible = ref(false)
const newGalleryName = ref('')

onMounted(() => {
  portfolioStore.fetchPortfolio()
})

const handleCreateGallery = async () => {
  const success = await portfolioStore.createGallery(newGalleryName.value)
  if (success) {
    isCreateGalleryDialogVisible.value = false
    newGalleryName.value = '' // 清空输入框
  }
}
</script>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.loading-state,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #909399;
}

.gallery-grid {
  display: grid;
  /* auto-fill: 自动填充尽可能多的列 */
  /* minmax(220px, 1fr): 每列最小宽度220px，最大平分剩余空间 */
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 1.5rem;
}

.gallery-card {
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transition:
    transform 0.2s,
    box-shadow 0.2s;
  position: relative; /* 为删除按钮的绝对定位提供基准 */
}

.gallery-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

.gallery-cover {
  width: 100%;
  padding-top: 100%; /* 1:1 Aspect Ratio */
  position: relative;
  background-color: #f5f7fa;
}

/* 新增：让链接充满整个卡片 */
.gallery-link {
  display: block;
  text-decoration: none;
  color: inherit;
}

/* 新增：删除按钮样式 */
.delete-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  opacity: 0; /* 默认隐藏 */
  transition: opacity 0.2s;
}

.gallery-card:hover .delete-btn {
  opacity: 1; /* 悬停时显示 */
}

.gallery-cover img {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.empty-cover {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 48px;
  color: #c0c4cc;
}

.gallery-info {
  padding: 1rem;
  background-color: #fff;
}

.gallery-name {
  font-weight: bold;
  display: block;
}

.photo-count {
  font-size: 0.85rem;
  color: #909399;
}
</style>
