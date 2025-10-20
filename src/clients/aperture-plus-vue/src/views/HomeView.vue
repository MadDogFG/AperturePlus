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

      <div ref="loadMoreSentinel" class="load-more-sentinel">
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
import { ref, onMounted, onUnmounted, nextTick } from 'vue' // 确保导入了 nextTick
import { useActivityStore } from '@/stores/activity'
import ActivityCard from '@/components/activity/ActivityCard.vue'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router'
const router = useRouter()

const activityStore = useActivityStore()

const isDialogVisible = ref(false)
const isEnrolling = ref(false)

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

const handleCreateActivity = async () => {
  isEnrolling.value = true
  const creationResult = await activityStore.createActivity(activityForm.value)
  if (!creationResult || !creationResult.activityId) {
    ElMessage.error('活动创建失败，请稍后再试。')
    isEnrolling.value = false
    return
  }
  // ... 如果成功，你可能还想关闭弹窗等
  isDialogVisible.value = false
  isEnrolling.value = false
  activityStore.reset() // 清空旧的列表数据
  await router.push('/home') // 跳转回首页
  window.location.reload() // 强制刷新页面以加载新活动
}

// --- 分页逻辑开始 ---

const loadMoreSentinel = ref<HTMLDivElement | null>(null)
let observer: IntersectionObserver | null = null
/**
 * 提取出一个设置观察者的函数
 * 这样可以在不同时机（挂载后、加载后）复用
 */
const setupObserver = () => {
  // 确保在附加新观察者之前断开旧的
  if (observer) {
    observer.disconnect()
  }

  observer = new IntersectionObserver(
    (entries) => {
      const firstEntry = entries[0]
      // 当哨兵元素进入视口、不在加载中、且还有更多数据时，加载下一页
      if (firstEntry.isIntersecting && !activityStore.isLoading && activityStore.hasMore) {
        activityStore.fetchActivities()
      }
    },
    { threshold: 0.1 },
  )

  // 使用 nextTick 确保 DOM 元素 (loadMoreSentinel) 已经渲染完毕
  nextTick(() => {
    console.log(
      '%c[关键日志 2] nextTick 执行。此时 哨兵元素 (loadMoreSentinel) 是:',
      'color: orange',
      loadMoreSentinel.value,
    )

    if (loadMoreSentinel.value) {
      console.log('%c[关键日志 3] 成功：观察者已附加', 'color: green')
      observer.observe(loadMoreSentinel.value)
    } else {
      console.error('%c[关键日志 3] 失败：哨兵元素是 null！观察者未附加', 'color: red')
      // 加上这个日志，看看列表里是不是已经有数据了
      console.log(
        `%c[关键日志 4] 失败时的状态: activities.length = ${activityStore.activities.length}`,
        'color: red',
      )
    }
  })
}

onMounted(() => {
  if (activityStore.activities.length === 0) {
    // 关键修复：
    // 仅在首次加载数据 *完成* 后才设置观察者。
    // 无论 fetchActivities 成功还是失败，DOM 都会更新
    // (显示列表或显示“无数据”），此时 loadMoreSentinel ref 才可能被正确设置。
    activityStore.fetchActivities().finally(() => {
      setupObserver()
    })
  } else {
    // 如果 store 中已有数据（例如，从其他页面导航回来）
    // DOM 元素会立即渲染，所以可以直接设置观察者。
    setupObserver()
  }
})

onUnmounted(() => {
  // 组件卸载时，断开观察者连接，防止内存泄漏
  if (observer) {
    observer.disconnect()
  }
})

// --- 分页逻辑结束 ---
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
