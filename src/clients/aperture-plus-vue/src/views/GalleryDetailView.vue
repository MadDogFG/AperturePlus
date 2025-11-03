<template>
  <div class="gallery-detail-container" v-if="gallery">
    <div class="header">
      <router-link :to="backLink" class="back-link">
        <el-icon><ArrowLeftBold /></el-icon>
        <span>返回作品集</span>
      </router-link>

      <h1>{{ gallery.galleryName }}</h1>

      <div v-if="isOwner" class="actions">
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
      :class="{ 'selection-mode': isOwner && isSelectionMode }"
    >
      <div
        v-for="photo in gallery.photos"
        :key="photo.photoId"
        class="photo-card"
        :class="{ selected: selectedPhotos.includes(photo.photoId) }"
        @click="handlePhotoClick(photo)"
      >
        <img :src="photo.photoUrl" alt="照片" />
        <div v-if="isOwner" class="selection-overlay">
          <el-icon><Check /></el-icon>
        </div>
      </div>
    </div>
    <div v-else class="empty-state">
      <p>{{ isOwner ? '这个相册里还没有照片，快上传一些吧！' : '这个相册里还没有照片。' }}</p>
    </div>
  </div>
  <div v-else class="loading-state">
    <p>正在加载相册信息...</p>
  </div>

  <el-dialog
    v-if="isOwner"
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
import { ref, computed } from 'vue'
import type { UploadFile, UploadUserFile } from 'element-plus'
import { useRouter } from 'vue-router'
import { usePortfolioStore } from '@/stores/portfolio'
import type { Photo } from '@/types/portfolio'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowLeftBold, Upload, Check, Plus } from '@element-plus/icons-vue'

const props = defineProps({
  galleryId: {
    type: String,
    required: true,
  },
  userId: {
    type: String,
    default: null,
  },
  isOwner: {
    type: Boolean,
    default: false,
  },
})

const router = useRouter()
const store = usePortfolioStore()

const gallery = computed(() =>
  store.portfolio?.galleries.find((g) => g.galleryId === props.galleryId),
)

const backLink = computed(() => {
  if (props.isOwner) {
    return '/profile/portfolio'
  } else {
    return `/user/${props.userId}/portfolio`
  }
})

// --- 状态 ---
const isSelectionMode = ref(false)
const selectedPhotos = ref<string[]>([])
const isUploadDialogVisible = ref(false)
const fileList = ref<UploadUserFile[]>([])
const isPreviewDialogVisible = ref(false)
const previewImageUrl = ref('')

// --- 方法 ---

const toggleSelectionMode = () => {
  if (!props.isOwner) return
  isSelectionMode.value = !isSelectionMode.value
  selectedPhotos.value = []
}

//
// --- 【修复】handlePhotoClick 函数 ---
//
const handlePhotoClick = (photo: Photo) => {
  // 1. 如果是 "Owner" 且处于 "选择模式"
  if (props.isOwner && isSelectionMode.value) {
    // 执行选择逻辑
    const index = selectedPhotos.value.indexOf(photo.photoId)
    if (index > -1) {
      selectedPhotos.value.splice(index, 1)
    } else {
      selectedPhotos.value.push(photo.photoId)
    }
  } else {
    // 2. 否则 (包括 "非Owner" 或 "非选择模式")
    // 执行预览逻辑
    previewImageUrl.value = photo.photoUrl
    isPreviewDialogVisible.value = true
  }
}

const handleDeleteSelected = () => {
  if (!props.isOwner) return
  ElMessageBox.confirm(`确定要删除选中的 ${selectedPhotos.value.length} 张照片吗？`, '确认删除', {
    type: 'warning',
  })
    .then(async () => {
      await store.deletePhotos(props.galleryId, selectedPhotos.value)
      isSelectionMode.value = false
      selectedPhotos.value = []
    })
    .catch(() => {
      ElMessage.info('已取消删除')
    })
}

// --- 上传相关方法 (保持不变) ---
const handleDialogClose = () => {
  fileList.value = []
}

const handlePictureCardPreview = (file: UploadFile) => {
  previewImageUrl.value = file.url!
  isPreviewDialogVisible.value = true
}

const handleUpload = async () => {
  if (!props.isOwner) return

  const filesToUpload = fileList.value.map((file) => file.raw as File)
  if (filesToUpload.length === 0) {
    ElMessage.warning('请至少选择一个文件')
    return
  }

  const success = await store.uploadPhotos(props.galleryId, filesToUpload)

  if (success) {
    ElMessage.success('上传成功！')
    isUploadDialogVisible.value = false
    fileList.value = []
  } else {
    ElMessage.error('上传失败')
  }
}
</script>

<style scoped>
/* 样式保持不变 */
.gallery-detail-container {
  padding: 2rem;
  box-sizing: border-box;
  width: 100%;
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
  gap: 8px;
  font-size: 1rem;
  color: #606266;
  text-decoration: none;
  padding: 8px 12px;
  border-radius: 6px;
  transition: background-color 0.2s;
}

.back-link:hover {
  background-color: #f0f2f5;
}

h1 {
  font-size: 2rem;
  font-weight: 600;
  margin: 0;
  flex-grow: 1; /* 占据剩余空间 */
}

.actions {
  display: flex;
  gap: 1rem;
}

.photo-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.photo-card {
  position: relative;
  aspect-ratio: 1 / 1;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  border: 2px solid transparent;
  transition:
    border-color 0.2s,
    transform 0.2s;
}

.photo-card img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* 选择模式下的样式 */
.photo-grid.selection-mode .photo-card:hover {
  transform: scale(0.98);
}

.photo-card .selection-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  font-size: 40px;
  opacity: 0;
  transition: opacity 0.2s;
}

.photo-card.selected .selection-overlay {
  opacity: 1;
  background-color: rgba(64, 158, 255, 0.7); /* 蓝色选中 */
}

.photo-grid.selection-mode .photo-card:not(.selected):hover .selection-overlay {
  opacity: 1;
  background-color: rgba(0, 0, 0, 0.3); /* 灰色悬浮 */
}

.loading-state,
.empty-state {
  padding: 4rem;
  text-align: center;
  color: #909399;
  font-size: 1.1rem;
}

.upload-dialog-content {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
