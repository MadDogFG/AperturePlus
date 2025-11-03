<template>
  <div class="activity-card">
    <div class="card-header">
      <div class="user-info">
        <UserPopover
          :user-id="activity.postedByUser.userId"
          :user-name="activity.postedByUser.userName"
        />
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
          {{ role.role === 'Photographer' ? '摄影师' : '模特' }} x
          {{ role.quantity }}
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
          {{ activity.approvedParticipantsCount }} /
          {{ activity.totalRequiredCount }}
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
// 【新增】导入新组件
import UserPopover from '@/components/common/UserPopover.vue'
import { User } from '@element-plus/icons-vue' // 确保 User 图标已导入

defineProps({
  activity: {
    type: Object as () => Activity,
    required: true,
  },
})
</script>

<style scoped>
/* 样式保持不变 */
.activity-card {
  background-color: #fff;
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  overflow: hidden;
  transition: box-shadow 0.2s ease-in-out;
  cursor: pointer;
}

.activity-card:hover {
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.8rem 1.25rem;
  border-bottom: 1px solid #f0f2f5;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* 我们不再需要 .username 样式
  因为 UserPopover 有自己的 .user-link 样式
*/

.card-content {
  padding: 1.25rem;
}

.title {
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0 0 0.75rem 0;
  color: #303133;
  /* 标题截断 */
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.description {
  font-size: 0.9rem;
  color: #606266;
  margin: 0 0 1rem 0;
  line-height: 1.5;
  /* 描述截断 */
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
  min-height: 40px; /* 保持 2 行的高度 */
}

.roles-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: #606266;
}

.role-req-tag {
  font-weight: 500;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.8rem 1.25rem;
  background-color: #fcfcfc;
  border-top: 1px solid #f0f2f5;
  font-size: 0.85rem;
  color: #909399;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 6px;
}

.participants-info .participants-text {
  color: #606266;
  font-weight: 500;
}

.participants-info .pending-text {
  color: #e6a23c;
  font-size: 0.9em;
  margin-left: 4px;
}
</style>
