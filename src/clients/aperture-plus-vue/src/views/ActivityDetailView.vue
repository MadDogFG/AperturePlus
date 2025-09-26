<template>
  <div class="activity-detail-container" v-loading="store.isLoading">
    <div v-if="store.activity" class="content">
      <el-card class="box-card info-card">
        <template #header>
          <div class="card-header">
            <h1>{{ store.activity.activityTitle }}</h1>
            <el-tag :type="store.activity.fee > 0 ? 'warning' : 'success'" effect="dark">
              {{ store.activity.fee > 0 ? `¥ ${store.activity.fee}` : '免费' }}
            </el-tag>
          </div>
        </template>
        <p class="description">{{ store.activity.activityDescription }}</p>
        <el-divider />
        <div class="info-grid">
          <div><strong>发布者:</strong> {{ store.activity.postedByUser.userName }}</div>
          <div><strong>状态:</strong> {{ store.activity.status }}</div>
          <div>
            <strong>时间:</strong> {{ new Date(store.activity.activityStartTime).toLocaleString() }}
          </div>
          <div><strong>地点:</strong> (地点占位符)</div>
        </div>
      </el-card>

      <el-card class="box-card roles-card">
        <template #header><strong>角色需求</strong></template>
        <div class="role-requirements">
          <div v-for="role in store.activity.roleRequirements" :key="role.role" class="role-item">
            <span>{{ role.role === 'Photographer' ? '摄影师' : '模特' }}:</span>
            <el-progress
              :percentage="getRoleProgress(role.role)"
              :stroke-width="20"
              text-inside
              :format="() => `${getApprovedCountForRole(role.role)}/${role.quantity}`"
            />
          </div>
        </div>
      </el-card>

      <el-card v-if="isOwner" class="box-card management-card">
        <template #header><strong>参与者管理</strong></template>
        <el-table :data="store.activity.participants" stripe style="width: 100%">
          <el-table-column label="用户">
            <template #default="{ row }">
              <span>{{ row.userName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="申请角色" width="120">
            <template #default="{ row }">
              {{ row.role === 'Photographer' ? '摄影师' : '模特' }}
            </template>
          </el-table-column>
          <el-table-column label="状态" width="120">
            <template #default="{ row }">
              <el-tag :type="statusTagType(row.status)">{{ statusText(row.status) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="180" align="center">
            <template #default="{ row }">
              <div v-if="row.status === 'Pending'">
                <el-button
                  size="small"
                  type="success"
                  @click="handleApprove(row.userId, row.role)"
                  :disabled="isRoleFull(row.role)"
                  >批准</el-button
                >
                <el-button size="small" type="danger" @click="handleReject(row.userId)"
                  >拒绝</el-button
                >
              </div>
              <div v-if="row.status === 'Approved'">
                <el-button size="small" type="danger" @click="handleReject(row.userId)"
                  >移除</el-button
                >
              </div>
              <div v-if="row.status === 'Rejected'">
                <el-button
                  size="small"
                  type="success"
                  @click="handleApprove(row.userId, row.role)"
                  :disabled="isRoleFull(row.role)"
                  >批准</el-button
                >
              </div>
            </template>
          </el-table-column>
        </el-table>
      </el-card>

      <el-card v-else class="box-card">
        <template #header><strong>已加入成员</strong></template>
        <div v-if="approvedParticipants.length > 0" class="participant-list">
          <el-tag v-for="p in approvedParticipants" :key="p.userId" size="large">{{
            p.userName
          }}</el-tag>
        </div>
        <el-empty v-else description="暂无成员加入" />
      </el-card>
    </div>
    <el-empty v-else-if="!store.isLoading" description="活动不存在或加载失败" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useActivityDetailStore } from '@/stores/activityDetail'
import { useUserStore } from '@/stores/user'
import type { RoleType, ParticipantStatus } from '@/types/activity'

const route = useRoute()
const store = useActivityDetailStore()
const userStore = useUserStore()

const activityId = route.params.id as string

// --- 计算属性 ---
const isOwner = computed(() => userStore.profile?.userId === store.activity?.postedByUser.userId)
const approvedParticipants = computed(
  () => store.activity?.participants.filter((p) => p.status === 'Approved') || [],
)

// --- 方法 ---
onMounted(() => {
  store.fetchActivity(activityId)
})

const getApprovedCountForRole = (role: RoleType) => {
  return (
    store.activity?.participants.filter((p) => p.role === role && p.status === 'Approved').length ||
    0
  )
}

const getRoleRequirement = (role: RoleType) => {
  return store.activity?.roleRequirements.find((r) => r.role === role)?.quantity || 0
}

const getRoleProgress = (role: RoleType) => {
  const approved = getApprovedCountForRole(role)
  const required = getRoleRequirement(role)
  return required > 0 ? (approved / required) * 100 : 0
}

const isRoleFull = (role: RoleType) => {
  const approved = getApprovedCountForRole(role)
  const required = getRoleRequirement(role)
  return approved >= required
}

const handleApprove = (applicantId: string, role: RoleType) => {
  if (!store.activity) return
  store.approveParticipant(store.activity.activityId, applicantId, role)
}

const handleReject = (applicantId: string) => {
  if (!store.activity) return
  store.rejectParticipant(store.activity.activityId, applicantId)
}

const statusTagType = (status: ParticipantStatus) => {
  if (status === 'Approved') return 'success'
  if (status === 'Pending') return 'warning'
  return 'info'
}
const statusText = (status: ParticipantStatus) => {
  if (status === 'Approved') return '已通过'
  if (status === 'Pending') return '待审核'
  return '已拒绝'
}
</script>

<style scoped>
.activity-detail-container {
  width: 100%;
  padding: 24px;
  background-color: #f5f7fa;
  box-sizing: border-box;
  overflow-y: auto;
}
.content {
  max-width: 960px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 20px;
}
.box-card {
  border-radius: 8px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.info-card h1 {
  margin: 0;
  font-size: 1.8rem;
}
.description {
  color: #606266;
  line-height: 1.6;
}
.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 12px;
  color: #303133;
}
.role-requirements {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.role-item {
  display: flex;
  align-items: center;
  gap: 16px;
}
.role-item span {
  width: 80px;
  flex-shrink: 0;
  font-weight: 500;
}
.participant-list {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
