<template>
  <div class="activity-card">
    <div class="card-header">
      <!-- <el-avatar :size="40" :src="activity.postedByUser.avatarUrl">
          {{ activity.postedByUser.userName.charAt(0) }}
        </el-avatar> -->
      <span class="username">{{ activity.postedByUser.userName }}</span>
    </div>
    <div class="card-content">
      <h3 class="title">{{ activity.activityTitle }}</h3>
      <p class="description">{{ activity.activityDescription }}</p>
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
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Activity } from '@/types/activity'

// 使用 defineProps 来声明这个组件需要接收一个名为 'activity' 的 prop
defineProps<{
  activity: Activity
}>()
</script>

<style scoped>
.activity-card {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  transition: box-shadow 0.3s;
}
.activity-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}
.card-header {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
}
.username {
  margin-left: 0.75rem;
  font-weight: bold;
}
.title {
  margin: 0 0 0.5rem 0;
}
.description {
  color: #606266;
  line-height: 1.5;
}
.card-footer {
  display: flex;
  gap: 1.5rem;
  color: #909399;
  margin-top: 1rem;
  border-top: 1px solid #f0f2f5;
  padding-top: 1rem;
}
.info-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
</style>
