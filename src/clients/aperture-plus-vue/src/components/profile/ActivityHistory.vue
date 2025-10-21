<template>
  <div class="activity-history-container">
    <h1>我的活动历史</h1>

    <div v-if="isLoading" class="loading-state">
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else-if="activityStore.activityHistory.length > 0" class="history-list">
      <el-card shadow="never">
        <div
          v-for="(activity, index) in activityStore.activityHistory"
          :key="activity.activityId"
          class="history-item-wrapper"
        >
          <div class="history-item">
            <RouterLink
              :to="{ name: 'activity-detail', params: { id: activity.activityId } }"
              class="item-content-link"
            >
              <div class="item-content">
                <div class="item-header">
                  <h3 class="activity-title">{{ activity.activityTitle }}</h3>
                  <el-tag
                    :type="activity.status === 'Completed' ? 'success' : 'info'"
                    effect="light"
                  >
                    {{ activity.status === 'Completed' ? '已完成' : '已取消' }}
                  </el-tag>
                </div>
                <p class="activity-date">
                  活动时间：{{ new Date(activity.activityStartTime).toLocaleString() }}
                </p>
              </div>
            </RouterLink>
            <div class="item-actions">
              <el-button
                v-if="activity.status === 'Completed' && pendingCounts.get(activity.activityId)"
                type="primary"
                @click="handleOpenRatingDialog(activity)"
              >
                去评价 ({{ pendingCounts.get(activity.activityId) }})
              </el-button>
            </div>
          </div>
          <el-divider v-if="index < activityStore.activityHistory.length - 1" />
        </div>
      </el-card>
    </div>

    <el-empty v-else description="您还没有已完成的活动记录" />
  </div>

  <el-dialog
    v-model="isDialogVisible"
    :title="`评价活动：${selectedActivity?.activityTitle}`"
    width="60%"
  >
    <div class="rating-dialog-content">
      <p class="dialog-tip">请为以下参与者打分并留下您的评价 (1-10分)：</p>
      <div v-if="pendingRatingsForActivity.length > 0" class="ratings-to-submit-list">
        <div v-for="task in pendingRatingsForActivity" :key="task.ratingId" class="rating-task">
          <div class="user-info">
            <div class="name-role">
              <span class="name">{{ task.rateToUserName }}</span>
              <span class="role">{{ task.ratedUserRole }}</span>
            </div>
          </div>
          <div class="rating-inputs">
            <el-rate
              v-model="task.score"
              :max="10"
              :colors="['#99A9BF', '#F7BA2A', '#FF9900']"
              show-text
            />
            <el-input
              v-model="task.comments"
              type="textarea"
              :rows="2"
              placeholder="可以说说你们的合作感受..."
              class="comment-input"
            />
          </div>
          <div class="submit-action">
            <el-button
              type="primary"
              plain
              @click="handleSubmitRating(task)"
              :loading="ratingStore.isSubmitting"
            >
              提交
            </el-button>
          </div>
        </div>
      </div>
      <el-empty v-else description="该活动的所有成员都已评价完毕！" />
    </div>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="isDialogVisible = false">关闭</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { Check } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import { useActivityStore } from '@/stores/activity'
import { useRatingStore } from '@/stores/rating'
import type { Activity } from '@/types/activity'
import type { PendingRatingDto } from '@/types/rating'

const activityStore = useActivityStore()
const ratingStore = useRatingStore()

const isDialogVisible = ref(false)
const selectedActivity = ref<Activity | null>(null)

const isLoading = computed(() => activityStore.isHistoryLoading || ratingStore.isPendingLoading)

const pendingCounts = computed(() => {
  const counts = new Map<string, number>()
  for (const rating of ratingStore.pendingRatings) {
    counts.set(rating.activityId, (counts.get(rating.activityId) || 0) + 1)
  }
  return counts
})

const pendingRatingsForActivity = computed(() => {
  if (!selectedActivity.value) return []
  return ratingStore.pendingRatings.filter(
    (p) => p.activityId === selectedActivity.value?.activityId,
  )
})

onMounted(() => {
  activityStore.fetchActivityHistory()
  ratingStore.fetchPendingRatings()
})

const handleOpenRatingDialog = (activity: Activity) => {
  selectedActivity.value = activity
  isDialogVisible.value = true
}

const handleSubmitRating = async (ratingTask: PendingRatingDto) => {
  console.log('ratingTask', ratingTask)
  if (!ratingTask.score || ratingTask.score === 0) {
    ElMessage.warning('请至少给出一星评价哦')
    return
  }

  const success = await ratingStore.submitRating({
    ratingId: ratingTask.ratingId,
    score: ratingTask.score,
    comments: ratingTask.comments,
  })

  if (success) {
    ElMessage.success(`对 ${ratingTask.rateToUserName} 的评价已提交！`)
    // 检查是否所有评价都已完成，如果是，则关闭对话框
    // 注意：这里的检查要基于 store 的最新状态
    const remainingTasks = ratingStore.pendingRatings.filter(
      (p) => p.activityId === selectedActivity.value?.activityId,
    )
    if (remainingTasks.length === 0) {
      isDialogVisible.value = false
    }
  }
}
</script>

<style scoped>
.activity-history-container {
  padding: 1rem 2rem;
  width: 100%;
  box-sizing: border-box;
}

.history-item-wrapper {
  padding: 1rem 0;
}
.history-item-wrapper:first-child {
  padding-top: 0;
}

.item-content-link {
  text-decoration: none;
  color: inherit;
  display: block;
}

.history-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.item-content {
  flex-grow: 1;
}

.item-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.activity-title {
  margin: 0;
  font-size: 1.1rem;
}

.activity-date {
  color: #909399;
  font-size: 0.9rem;
  margin: 0;
}

.item-actions {
  flex-shrink: 0;
  margin-left: 1rem;
}

.dialog-tip {
  color: #606266;
  margin-bottom: 1.5rem;
}

.ratings-to-submit-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.rating-task {
  display: grid;
  grid-template-columns: 150px 1fr 100px;
  align-items: center;
  gap: 1rem;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.name-role {
  display: flex;
  flex-direction: column;
}

.name {
  font-weight: bold;
}

.role {
  font-size: 0.8rem;
  color: #909399;
}

.rating-inputs {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.submit-action {
  text-align: right;
}
</style>
