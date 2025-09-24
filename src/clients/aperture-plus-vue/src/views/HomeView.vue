<template>
  <div class="home-container">
    <div
      v-if="activityStore.isLoading && activityStore.activities.length === 0"
      class="loading-state"
    >
      <p>正在加载活动...</p>
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else-if="activityStore.activities.length > 0" class="activity-list">
      <ActivityCard
        v-for="activity in activityStore.activities"
        :key="activity.activityId"
        :activity="activity"
      />
      <div class="load-more-sentinel">
        <p v-if="activityStore.isLoading">正在加载更多...</p>
        <p v-if="!activityStore.hasMore && !activityStore.isLoading">没有更多活动了</p>
      </div>
    </div>

    <div v-else class="empty-state">
      <p>暂时还没有任何活动，快去发起一个吧！</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue' // 1. 额外导入 onUnmounted
import { useActivityStore } from '@/stores/activity'
import ActivityCard from '@/components/activity/ActivityCard.vue'

const activityStore = useActivityStore()

// 2. 定义处理滚动事件的函数
const handleScroll = () => {
  // document.documentElement.scrollTop: 滚动条垂直方向滚动的距离
  // document.documentElement.clientHeight: 浏览器窗口的可视高度
  // document.documentElement.scrollHeight: 整个页面的总高度

  // 当 (已滚动距离 + 窗口高度) >= (页面总高度 - 一个缓冲值) 时，我们就认为滚动到底部了
  const nearBottom =
    document.documentElement.scrollTop + document.documentElement.clientHeight >=
    document.documentElement.scrollHeight - 200

  if (nearBottom) {
    // 调用 store 的 action 来加载下一页
    activityStore.fetchActivities()
  }
}

onMounted(() => {
  // 重置 store 状态，以防从其他页面返回时数据混乱
  activityStore.reset()
  activityStore.fetchActivities()

  // 3. 添加事件监听器
  window.addEventListener('scroll', handleScroll)
})

// 4. 在组件卸载时，移除事件监听器
onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.home-container {
  width: 100%;
  max-width: 800px; /* 给内容一个最大宽度，在大屏幕上更好看 */
  margin: 0 auto; /* 水平居中 */
  padding: 1rem;
}

.loading-state,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #909399;
}

.load-more-sentinel {
  text-align: center;
  padding: 2rem;
  color: #909399;
}
</style>
