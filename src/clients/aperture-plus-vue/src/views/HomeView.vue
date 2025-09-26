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
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { useActivityStore } from '@/stores/activity'
import ActivityCard from '@/components/activity/ActivityCard.vue'

const activityStore = useActivityStore()

// --- 下拉加载逻辑 ---
const observer = ref<IntersectionObserver | null>(null)
const sentinel = ref<HTMLElement | null>(null)

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
</style>
