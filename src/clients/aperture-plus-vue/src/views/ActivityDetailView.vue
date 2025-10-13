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
          <div><strong>状态:</strong> {{ activitystatusText(store.activity.status) }}</div>
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
        <template #header><strong>活动管理</strong></template>
        <div class="management-actions">
          <template
            v-if="
              store.activity &&
              (store.activity.status === 'Open' || store.activity.status === 'Progress')
            "
          >
            <el-button type="success" @click="handleCompleteActivity">完成活动</el-button>
            <el-button type="danger" @click="handleCancelActivity">取消活动</el-button>
          </template>
          <el-text v-else-if="store.activity" class="status-tip">
            活动已 {{ activitystatusText(store.activity.status) }}，无需其他操作。
          </el-text>
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

        <div class="visitor-actions">
          <el-divider v-if="canApply || currentUserStatus" />
          <el-button v-if="canApply" type="primary" @click="openApplyDialog" size="large">
            申请加入
          </el-button>
          <el-tag
            v-if="!canApply && currentUserStatus"
            :type="statusTagType(currentUserStatus)"
            size="large"
            effect="light"
          >
            您已申请: {{ statusText(currentUserStatus) }}
          </el-tag>
        </div>
      </el-card>
    </div>
    <el-empty v-else-if="!store.isLoading" description="活动不存在或加载失败" />

    <el-dialog v-model="isApplyDialogVisible" title="申请加入活动" width="500px">
      <div v-if="availableRolesForApplication.length > 0">
        <p>请选择您要申请的角色：</p>
        <el-radio-group v-model="selectedRole">
          <el-radio
            v-for="role in availableRolesForApplication"
            :key="role.role"
            :label="role.role"
            border
            size="large"
          >
            {{ role.role === 'Photographer' ? '摄影师' : '模特' }} (空缺
            {{ role.quantity - getApprovedCountForRole(role.role) }} 人)
          </el-radio>
        </el-radio-group>
      </div>
      <div v-else>
        <el-empty description="抱歉，所有角色都已满员啦" />
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="isApplyDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleConfirmApply" :disabled="!selectedRole">
            <!-- <el-button type="primary" @click="handleConfirmApply"> -->
            确认申请
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useActivityDetailStore } from '@/stores/activityDetail'
import { useUserStore } from '@/stores/user'
import type { RoleType, ParticipantStatus } from '@/types/activity'
import { ElMessage, ElMessageBox } from 'element-plus'

const route = useRoute()
const store = useActivityDetailStore()
const userStore = useUserStore()
const activityId = route.params.id as string

// --- 申请对话框的状态 ---
const isApplyDialogVisible = ref(false)
const selectedRole = ref<RoleType | null>(null)

// --- 计算属性 ---
const isOwner = computed(() => userStore.profile?.userId === store.activity?.postedByUser.userId)
const approvedParticipants = computed(
  () => store.activity?.participants.filter((p) => p.status === 'Approved') || [],
)

const currentUserStatus = computed(() => {
  if (!userStore.profile || !store.activity) return null
  const participant = store.activity.participants.find(
    (p) => p.userId === userStore.profile!.userId,
  )
  return participant ? participant.status : null
})

const availableRolesForApplication = computed(() => {
  return store.activity?.roleRequirements.filter((role) => !isRoleFull(role.role)) || []
})

const canApply = computed(() => {
  // 必须不是活动所有者，且自己还未申请过，并且至少有一个角色还没满员
  return !isOwner.value && !currentUserStatus.value && availableRolesForApplication.value.length > 0
})

// 添加按钮点击处理函数
const handleCompleteActivity = () => {
  ElMessageBox.confirm('确定要将此活动标记为完成吗？完成后将无法再管理参与者。', '确认完成', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning',
  })
    .then(() => {
      store.completeActivity(activityId)
    })
    .catch(() => {
      ElMessage.info('操作已取消')
    })
}

const handleCancelActivity = () => {
  ElMessageBox.confirm('确定要取消此活动吗？此操作不可撤销。', '确认取消', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'danger',
  })
    .then(() => {
      store.cancelActivity(activityId)
    })
    .catch(() => {
      ElMessage.info('操作已取消')
    })
}

// --- 方法 ---
onMounted(() => {
  store.fetchActivity(activityId)
})

const getApprovedCountForRole = (role: RoleType | number) => {
  // 允许接收数字或字符串
  // 将传入的参数统一转换为字符串形式
  const targetRoleString = role
  return (
    store.activity?.participants.filter(
      (p) => p.role === targetRoleString && p.status === 'Approved',
    ).length || 0
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

const isRoleFull = (role: RoleType | number) => {
  const approved = getApprovedCountForRole(role)
  const requirement = store.activity?.roleRequirements.find((r) => r.role === role)?.quantity || 0
  return approved >= requirement
}

// --- 所有者操作 ---
const handleApprove = (applicantId: string, role: RoleType) => {
  if (!store.activity) return
  store.approveParticipant(store.activity.activityId, applicantId, role)
}

const handleReject = (applicantId: string) => {
  if (!store.activity) return
  store.rejectParticipant(store.activity.activityId, applicantId)
}

// --- 访客操作 ---
const openApplyDialog = () => {
  selectedRole.value = null // 重置选项
  isApplyDialogVisible.value = true
}

const handleConfirmApply = async () => {
  console.log(selectedRole.value)
  console.log(store.activity)
  console.log(userStore.profile)
  if (!selectedRole.value || !store.activity || !userStore.profile) {
    ElMessage.warning('请选择一个角色')
    return
  }
  const success = await store.requestJoinActivity(store.activity.activityId, selectedRole.value)
  if (success) {
    isApplyDialogVisible.value = false
  }
}

// --- UI 辅助方法 ---
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
const activitystatusText = (status: string) => {
  if (status === 'Cancelled') return '已取消'
  if (status === 'Completed') return '已完成'
  if (status === 'Progress') return '执行中'
  return '已开放'
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
.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
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
.visitor-actions {
  text-align: center;
  margin-top: 20px;
}
.el-radio-group {
  display: flex;
  flex-direction: column;
  gap: 15px;
  align-items: flex-start;
}
.management-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}
.status-tip {
  color: #909399;
}
</style>
