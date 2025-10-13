<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<template>
  <div class="home-container">
    <div class="actions-header">
      <el-button type="primary" size="large" @click="isDialogVisible = true">
        <el-icon><i-ep-plus /></el-icon>
        <span>发起新活动</span>
      </el-button>
    </div>
    <div
      v-if="activityStore.isLoading && activityStore.activities.length === 0"
      class="loading-state"
    >
      <p>正在加载活动...</p>
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else-if="activityStore.activities.length > 0" class="activity-list">
      <RouterLink
        v-for="activity in activityStore.activities"
        :key="activity.activityId"
        :to="{ name: 'activity-detail', params: { id: activity.activityId } }"
        class="activity-link"
      >
        <ActivityCard :activity="activity" />
      </RouterLink>

      <div class="load-more-sentinel">
        <p v-if="activityStore.isLoading">正在加载更多...</p>
        <p v-if="!activityStore.hasMore && !activityStore.isLoading">没有更多活动了</p>
      </div>
    </div>

    <div v-else class="empty-state">
      <p>暂时还没有任何活动，快去发起一个吧！</p>
    </div>
  </div>
  <el-dialog v-model="isDialogVisible" title="发起新活动" width="60%" :close-on-click-modal="false">
    <el-form :model="activityForm" label-width="120px" label-position="top">
      <el-form-item label="活动标题">
        <el-input v-model="activityForm.activityTitle" placeholder="给活动起一个吸引人的名字吧" />
      </el-form-item>
      <el-form-item label="活动详情描述">
        <el-input
          v-model="activityForm.activityDescription"
          type="textarea"
          :rows="4"
          placeholder="详细介绍一下活动内容、主题、风格、对参与者的要求等"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="活动开始时间">
            <el-date-picker
              v-model="activityForm.activityStartTime"
              type="datetime"
              placeholder="选择日期和时间"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="费用 (元)">
            <el-input-number v-model="activityForm.fee" :min="0" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-divider>角色需求</el-divider>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="需要几位摄影师？">
            <el-input-number
              v-model="activityForm.photographerCount"
              :min="0"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="需要几位模特？">
            <el-input-number v-model="activityForm.modelCount" :min="0" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="活动地点 (详细地址)">
        <el-input v-model="activityForm.location" placeholder="输入详细的活动地点" />
      </el-form-item>
    </el-form>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="isDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleCreateActivity"> 确认创建 </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { useActivityStore } from '@/stores/activity'
import ActivityCard from '@/components/activity/ActivityCard.vue'
// 代码块 1.3: 添加弹窗控制和表单数据的 ref
const isDialogVisible = ref(false)

const activityForm = ref({
  activityTitle: '',
  activityDescription: '',
  activityStartTime: '',
  fee: 0,
  photographerCount: 1,
  modelCount: 1,
  location: '', // 暂时简化地点
})
const activityStore = useActivityStore()

// --- 下拉加载逻辑 ---
const observer = ref<IntersectionObserver | null>(null)

const handleCreateActivity = async () => {
  // 调用 store里的 action，并把表单数据传过去
  const success = await activityStore.createActivity(activityForm.value)

  // 如果 store action 返回 true (表示成功)
  if (success) {
    isDialogVisible.value = false // 关闭弹窗
    // 重置表单，方便下次填写
    activityForm.value = {
      activityTitle: '',
      activityDescription: '',
      activityStartTime: '',
      fee: 0,
      photographerCount: 1,
      modelCount: 1,
      location: '',
    }
  }
}

onMounted(() => {
  // 如果是首次加载，获取数据
  if (activityStore.activities.length === 0) {
    activityStore.fetchActivities()
  }

  // 设置 IntersectionObserver 来实现无限滚动加载
  const sentinelEl = document.querySelector('.load-more-sentinel')
  if (sentinelEl) {
    observer.value = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && activityStore.hasMore && !activityStore.isLoading) {
          activityStore.fetchActivities()
        }
      },
      { threshold: 1.0 },
    )
    observer.value.observe(sentinelEl)
  }
})

onUnmounted(() => {
  // 组件卸载时停止监听
  if (observer.value) {
    observer.value.disconnect()
  }
})
</script>

<style scoped>
.home-container {
  width: 100%;
  max-width: 800px;
  margin: 0 auto;
  padding: 24px;
  box-sizing: border-box;
}

.loading-state,
.empty-state {
  text-align: center;
  color: #909399;
  padding: 40px 0;
}

.activity-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

/* ↓↓↓ 关键样式：重置 RouterLink 默认样式 ↓↓↓
  这会移除链接的下划线和默认颜色，使其外观和行为完全由内部的 ActivityCard 决定
*/
.activity-link {
  text-decoration: none;
  color: inherit;
  display: block; /* 让链接占据整行，可点击区域更大 */
}

.load-more-sentinel {
  height: 50px;
  text-align: center;
  color: #909399;
}

.actions-header {
  padding: 1rem;
  border-bottom: 1px solid #ebeef5;
  text-align: center;
}
</style>
