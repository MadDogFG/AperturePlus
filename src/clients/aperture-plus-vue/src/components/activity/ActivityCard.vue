<template>
  <div class="activity-card">
    <div class="card-header">
      <div class="user-info">
        <span class="username">{{ activity.postedByUser.userName }}</span>
      </div>
      <el-tag :type="activity.fee > 0 ? 'warning' : 'success'" class="fee-tag" effect="dark">
        {{ activity.fee > 0 ? `¥ ${activity.fee}` : '免费' }}
      </el-tag>
    </div>

    <div class="card-content">
      <h3 class="title">{{ activity.activityTitle }}</h3>
      <p class="description">{{ activity.activityDescription }}</p>
      <div class="roles-info">
        <span>需求：</span>
        <el-tag
          v-for="role in activity.roleRequirements"
          :key="role.role"
          type="info"
          size="small"
          class="role-req-tag"
        >
          {{ role.role === 'Photographer' ? '摄影师' : '模特' }} x {{ role.quantity }}
        </el-tag>
      </div>
    </div>

    <div class="card-footer">
      <div class="info-item">
        <el-icon><i-ep-calendar /></el-icon>
        <span>{{ new Date(activity.activityStartTime).toLocaleDateString() }}</span>
      </div>
      <div class="info-item">
        <el-icon><i-ep-location /></el-icon>
        <span>地点占位符</span>
      </div>
      <div class="info-item participants-info">
        <el-icon><User /></el-icon>
        <span class="participants-text">
          {{ activity.approvedParticipantsCount }} / {{ activity.totalRequiredCount }}
          <span v-if="activity.pendingParticipantsCount > 0" class="pending-text">
            ({{ activity.pendingParticipantsCount }}人待审)
          </span>
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Activity } from '@/types/activity'
// 导入 User 图标，用于显示报名人数
import { User } from '@element-plus/icons-vue'

defineProps<{
  activity: Activity
}>()
</script>

<style scoped>
.activity-card {
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 16px;
  background-color: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  transition: box-shadow 0.3s ease;
  display: flex;
  flex-direction: column;
}

.activity-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.user-info {
  display: flex;
  align-items: center;
}

.username {
  font-weight: bold;
  margin-left: 8px; /* 如果有头像，这里可以调整 */
}

.fee-tag {
  font-weight: bold;
}

.card-content .title {
  margin: 0 0 8px 0;
  font-size: 1.2rem;
}

.card-content .description {
  color: #606266;
  font-size: 0.9rem;
  line-height: 1.5;
  margin-bottom: 12px;
}

.roles-info {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 10px;
  font-size: 14px;
  color: #606266;
}

.card-footer {
  margin-top: auto; /* 让 footer 总是贴近底部 */
  padding-top: 12px;
  border-top: 1px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: #909399;
  font-size: 0.85rem;
}

.info-item {
  display: flex;
  align-items: center;
}

.info-item .el-icon {
  margin-right: 6px;
}

.participants-text {
  display: flex;
  align-items: center;
}

.participants-info .pending-text {
  color: #e6a23c;
  margin-left: 4px;
  font-size: 12px;
}
</style>
