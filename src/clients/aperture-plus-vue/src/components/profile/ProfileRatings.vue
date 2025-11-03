<template>
  <div class="ratings-container">
    <h1>{{ isOwner ? '我的评价' : 'TA 的评价' }}</h1>

    <el-card class="stats-card" shadow="never" v-loading="ratingStore.isStatsLoading">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-statistic title="累计收到评价 (次)" :value="ratingStore.stats?.totalCount ?? 0" />
        </el-col>
        <el-col :span="12">
          <el-statistic title="收到好评率" :value="positiveRate">
            <template #suffix>%</template>
          </el-statistic>
        </el-col>
      </el-row>
    </el-card>

    <el-tabs v-model="activeTab" class="ratings-tabs">
      <el-tab-pane label="收到的评价" name="received">
        <div v-if="ratingStore.isLoading" class="loading-state">
          <el-skeleton :rows="5" animated />
        </div>
        <div v-else-if="ratingStore.receivedRatings.length > 0" class="ratings-list">
          <el-card
            v-for="rating in ratingStore.receivedRatings"
            :key="rating.ratingId"
            class="rating-card"
          >
            <template #header>
              <div class="card-header">
                <span>
                  来自:
                  <strong>{{ rating.rateByUserName }}</strong>
                </span>
                <span class="rating-time">
                  {{ new Date(rating.createdAt).toLocaleDateString() }}
                </span>
              </div>
            </template>
            <div class="rating-content">
              <div class="score">
                <span>评分: </span>
                <el-icon v-for="i in rating.score" :key="i" color="#f56c6c">
                  <i-ep-star-filled />
                </el-icon>
                <el-icon v-for="i in 10 - rating.score" :key="i" color="#dcdfe6">
                  <i-ep-star-filled />
                </el-icon>
              </div>
              <p class="comments">
                评价内容:
                {{ rating.comments || '用户没有留下任何文字评价。' }}
              </p>
            </div>
            <template #footer>
              <div class="card-footer">
                <span>
                  评价角色:
                  <strong>
                    {{ rating.ratedUserRole === 'Photographer' ? '摄影师' : '模特' }}
                  </strong>
                </span>
              </div>
            </template>
          </el-card>
        </div>
        <el-empty v-else description="还没有收到任何评价哦" />
      </el-tab-pane>

      <el-tab-pane v-if="isOwner" label="我发送的评价" name="sent">
        <div v-if="ratingStore.isSentLoading" class="loading-state">
          <el-skeleton :rows="5" animated />
        </div>
        <div v-else-if="ratingStore.sentRatings.length > 0" class="ratings-list">
          <el-card
            v-for="rating in ratingStore.sentRatings"
            :key="rating.ratingId"
            class="rating-card"
          >
            <template #header>
              <div class="card-header">
                <span>
                  发给:
                  <strong>{{ rating.rateToUserName }}</strong>
                </span>
                <span class="rating-time">
                  {{ new Date(rating.submittedAt).toLocaleDateString() }}
                </span>
              </div>
            </template>
            <div class="rating-content">
              <div class="score">
                <span>评分: </span>
                <el-icon v-for="i in rating.score" :key="i" color="#f56c6c">
                  <i-ep-star-filled />
                </el-icon>
                <el-icon v-for="i in 10 - rating.score" :key="i" color="#dcdfe6">
                  <i-ep-star-filled />
                </el-icon>
              </div>
              <p class="comments">
                评价内容:
                {{ rating.comments || '您没有留下任何文字评价。' }}
              </p>
            </div>
          </el-card>
        </div>
        <el-empty v-else description="还没有发送过任何评价" />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
// 1. 【修复】恢复 store 的导入
import { useRatingStore } from '@/stores/rating'

// 2. 【修复】恢复 isOwner prop (由 router 提供)
defineProps({
  isOwner: {
    type: Boolean,
    default: false,
  },
})

// 3. 【修复】恢复 store 实例
const ratingStore = useRatingStore()
const activeTab = ref('received')

// 4. 【修复】计算属性依赖 store
const positiveRate = computed(() => {
  if (!ratingStore.stats || !ratingStore.stats.positiveRate) {
    return 0
  }
  // 假设后端返回的是 90.0
  return ratingStore.stats.positiveRate.toFixed(1)
})

// 5. 【修复】恢复 onMounted 数据获取
onMounted(() => {
  // 组件加载时，一次性获取所有需要的数据
  ratingStore.fetchRatingStats()
  ratingStore.fetchReceivedRatings()
  ratingStore.fetchSentRatings()
})
</script>

<style scoped>
/* 样式保持不变 */
.ratings-container {
  padding: 1rem 2rem;
  width: 100%;
  box-sizing: border-box;
}

.stats-card {
  margin-bottom: 2rem;
  border: 1px solid #e4e7ed;
  background-color: #fafafa;
}

.ratings-tabs {
  margin-top: 1rem;
}

.ratings-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}
.rating-card {
  border-radius: 8px;
}
.card-header,
.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.9em;
  color: #606266;
}
.rating-content .score {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-bottom: 1rem;
}
.rating-content .comments {
  line-height: 1.6;
}
</style>
