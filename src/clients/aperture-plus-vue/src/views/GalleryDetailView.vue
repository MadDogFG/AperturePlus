<template>
  <div class="gallery-detail-container" v-if="gallery">
    <div class="header">
      <router-link to="/profile/portfolio" class="back-link">
        <el-icon><ArrowLeftBold /></el-icon>
        <span>返回作品集</span>
      </router-link>

      <h1>{{ gallery.galleryName }}</h1>

      <div class="actions">
        <el-button type="primary" @click="isUploadDialogVisible = true">
          <el-icon><Upload /></el-icon>
          <span>上传照片</span>
        </el-button>

        <el-button :type="isSelectionMode ? 'warning' : 'default'" @click="toggleSelectionMode">
          {{ isSelectionMode ? '取消选择' : '选择' }}
        </el-button>

        <el-button
          v-if="isSelectionMode"
          type="danger"
          @click="handleDeleteSelected"
          :disabled="selectedPhotos.length === 0"
        >
          删除选中 ({{ selectedPhotos.length }})
        </el-button>
      </div>
    </div>

    <div
      v-if="gallery.photos.length > 0"
      class="photo-grid"
      :class="{ 'selection-mode': isSelectionMode }"
    >
      <div
        v-for="photo in gallery.photos"
        :key="photo.photoId"
        class="photo-card"
        :class="{ selected: selectedPhotos.includes(photo.photoId) }"
        @click="handlePhotoClick(photo)"
      >
        <img :src="photo.photoUrl" alt="照片" />
        <div class="selection-overlay">
          <el-icon><Check /></el-icon>
        </div>
      </div>
    </div>

    <div v-else class="empty-state">
      <p>这个相册里还没有照片，快上传一些吧！</p>
    </div>
  </div>

  <div v-else class="loading-state">
    <p>正在加载相册信息...</p>
  </div>

  <el-dialog
    v-model="isUploadDialogVisible"
    title="上传照片"
    width="60%"
    @close="handleDialogClose"
  >
    <div class="upload-dialog-content">
      <el-upload
        v-model:file-list="fileList"
        list-type="picture-card"
        multiple
        :auto-upload="false"
        action="#"
        :on-preview="handlePictureCardPreview"
      >
        <el-icon><Plus /></el-icon>
      </el-upload>
    </div>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="isUploadDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="handleUpload" :disabled="fileList.length === 0">
          确认上传 {{ fileList.length > 0 ? fileList.length + ' 张' : '' }}
        </el-button>
      </span>
    </template>
  </el-dialog>

  <el-dialog v-model="isPreviewDialogVisible" title="图片预览" width="50%" center>
    <img :src="previewImageUrl" alt="Preview Image" style="width: 100%" />
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { UploadFile } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import { usePortfolioStore } from '@/stores/portfolio'
import type { Gallery, Photo } from '@/types/portfolio'
import { ElMessage } from 'element-plus'
import { ArrowLeftBold, Upload, Check, Plus } from '@element-plus/icons-vue'

// --- 状态定义 ---

const route = useRoute()
const router = useRouter()
const portfolioStore = usePortfolioStore()

const gallery = ref<Gallery | null>(null)
const selectedPhotos = ref<string[]>([])
const fileList = ref<UploadFile[]>([])
const isUploadDialogVisible = ref(false)
const isPreviewDialogVisible = ref(false)
const previewImageUrl = ref('')

// 新增：用于管理选择模式的状态
const isSelectionMode = ref(false)

// --- 计算属性 ---
const galleryId = computed(() => route.params.galleryId as string)

// --- 业务逻辑 ---

/**
 * 加载并设置当前相册的数据。
 */
async function loadGallery() {
  if (!portfolioStore.portfolio) {
    await portfolioStore.fetchPortfolio()
  }
  const foundGallery = portfolioStore.portfolio?.galleries.find(
    (g) => g.galleryId === galleryId.value,
  )
  if (foundGallery) {
    gallery.value = foundGallery
  } else {
    ElMessage.error('找不到指定的相册')
    router.push('/profile/portfolio')
  }
}

/**
 * 切换“选择模式”
 */
function toggleSelectionMode() {
  isSelectionMode.value = !isSelectionMode.value
  // 退出选择模式时，清空所有已选项
  if (!isSelectionMode.value) {
    selectedPhotos.value = []
  }
}

/**
 * 统一的照片点击处理器
 * @param photo - 被点击的照片对象
 */
function handlePhotoClick(photo: Photo) {
  if (isSelectionMode.value) {
    // 如果在选择模式下，执行选中/取消选中逻辑
    toggleSelection(photo.photoId)
  } else {
    // 如果在默认模式下，执行预览逻辑
    handlePhotoPreview(photo)
  }
}

/**
 * 处理照片的选中/取消选中逻辑
 * @param photoId - 照片ID
 */
function toggleSelection(photoId: string) {
  const index = selectedPhotos.value.indexOf(photoId)
  if (index > -1) {
    selectedPhotos.value.splice(index, 1)
  } else {
    selectedPhotos.value.push(photoId)
  }
}

/**
 * 处理从相册网格点击照片时的预览
 * @param photo - 照片对象
 */
function handlePhotoPreview(photo: Photo) {
  previewImageUrl.value = photo.photoUrl
  isPreviewDialogVisible.value = true
}

/**
 * 处理 el-upload 组件中的预览事件
 * @param uploadFile - el-upload 传入的文件对象
 */
function handlePictureCardPreview(uploadFile: UploadFile) {
  previewImageUrl.value = uploadFile.url!
  isPreviewDialogVisible.value = true
}

/**
 * 处理"确认上传"按钮的点击事件
 */
async function handleUpload() {
  const filesToUpload = fileList.value.map((uploadFile) => uploadFile.raw as File)
  if (filesToUpload.length > 0) {
    const success = await portfolioStore.uploadPhotos(galleryId.value, filesToUpload)
    if (success) {
      isUploadDialogVisible.value = false
      await portfolioStore.fetchPortfolio(true)
      await loadGallery()
    }
  }
}

/**
 * 处理删除选中照片的逻辑
 */
async function handleDeleteSelected() {
  if (selectedPhotos.value.length === 0) return
  await portfolioStore.deletePhotos(galleryId.value, selectedPhotos.value)
  selectedPhotos.value = []
  // 删除后自动退出选择模式
  isSelectionMode.value = false
}

/**
 * 当上传弹窗关闭时，清空已选择的文件列表
 */
function handleDialogClose() {
  fileList.value = []
}

// --- 生命周期钩子 ---
onMounted(async () => {
  await loadGallery()
})
</script>

<style scoped>
.gallery-detail-container {
  padding: 1rem;
}
.header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}
.back-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: #409eff;
  font-size: 1rem;
}
.header h1 {
  margin: 0;
  flex-grow: 1; /* 让标题占据多余空间，将右侧按钮推到最右边 */
}
.actions {
  display: flex;
  gap: 1rem;
}
.photo-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
}
.photo-card {
  position: relative;
  cursor: zoom-in; /* 默认显示放大光标 */
  border-radius: 8px;
  overflow: hidden;
  padding-top: 100%; /* 关键技巧：创建1:1的宽高比容器 */
  background-color: #f5f7fa;
}
.photo-card img {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover; /* 保证图片不变形地填满容器 */
  transition: transform 0.2s ease;
}
.photo-card:hover img {
  transform: scale(1.05); /* 悬停时轻微放大 */
}

/* 当进入选择模式时，改变光标样式以提示用户 */
.photo-grid.selection-mode .photo-card {
  cursor: pointer; /* 在选择模式下，光标是'pointer'表示可点击选择 */
}

.selection-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(64, 158, 255, 0.5); /* Element Plus 主题蓝色的半透明遮罩 */
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 28px;
  opacity: 0;
  transition: opacity 0.2s ease;
  border: 3px solid #409eff; /* 边框加粗更明显 */
  box-sizing: border-box;
}
.photo-card.selected .selection-overlay {
  opacity: 1; /* 仅在有 'selected' 类时显示遮罩 */
}
.empty-state,
.loading-state {
  text-align: center;
  padding: 3rem;
  color: #909399;
}

/* 为弹窗内容区添加滚动条 */
.upload-dialog-content {
  max-height: 60vh; /* 设置一个最大高度，例如视窗高度的60% */
  overflow-y: auto; /* 当内容超出最大高度时，显示垂直滚动条 */
  padding: 10px; /* 增加一点内边距，防止滚动条过于贴近预览图 */
}
</style>
