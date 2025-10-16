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
      <el-form-item label="请选择您在此活动中的角色（必选）">
        <el-radio-group v-model="activityForm.creatorRole">
          <el-radio-button label="Photographer">我是摄影师</el-radio-button>
          <el-radio-button label="Model">我是模特</el-radio-button>
        </el-radio-group>
      </el-form-item>
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
        <el-button type="primary" @click="handleCreateActivity" :loading="isEnrolling">
          确认创建
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 文件: src/views/HomeView.vue -> <script>

import { ref, onMounted } from 'vue'
import { useActivityStore } from '@/stores/activity'
import { useActivityDetailStore } from '@/stores/activityDetail' // [!code ++]
import { useUserStore } from '@/stores/user' // [!code ++]
import ActivityCard from '@/components/activity/ActivityCard.vue'
import { ElMessage } from 'element-plus'

const activityStore = useActivityStore()
onMounted(() => {
  // 如果列表是空的，就去获取第一页数据
  if (activityStore.activities.length === 0) {
    activityStore.fetchActivities()
  }
})
const activityDetailStore = useActivityDetailStore() // [!code ++]
const userStore = useUserStore() // [!code ++]

const isDialogVisible = ref(false)
const isEnrolling = ref(false) // [!code ++] 新增加载状态

const activityForm = ref({
  activityTitle: '',
  activityDescription: '',
  activityStartTime: '',
  fee: 0,
  photographerCount: 1,
  modelCount: 1,
  location: '',
  creatorRole: '',
})

// [!code focus start]
const handleCreateActivity = async () => {
  isEnrolling.value = true

  // --- 第1步: 创建活动 ---
  const creationResult = await activityStore.createActivity(activityForm.value)

  if (!creationResult || !creationResult.successed) {
    ElMessage.error('活动创建失败，请稍后再试。')
    isEnrolling.value = false
    return
  }

  const activityId = creationResult.activityId
  const creatorRole = activityForm.value.creatorRole as 'Photographer' | 'Model'
  const creatorId = userStore.profile!.userId

  try {
    // --- 第2步: 申请加入 ---
    await activityDetailStore.requestJoinActivity(activityId, creatorRole)

    // --- 第3步: 批准自己 ---
    // 注意: approveParticipant 需要 activityId, applicantId, role
    await activityDetailStore.approveParticipant(activityId, creatorId, creatorRole)

    ElMessage.success('您已成功作为发起人加入活动！')
    isDialogVisible.value = false // 全部成功后关闭弹窗
  } catch (error) {
    console.error('自动报名或批准失败:', error)
    ElMessage.error('自动报名失败，请稍后在活动详情页手动加入。')
  } finally {
    isEnrolling.value = false
  }
}
// [!code focus end]
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
