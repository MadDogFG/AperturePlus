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
                  <UserPopover :user-id="rating.rateByUserId" :user-name="rating.rateByUserName" />
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
                  <UserPopover :user-id="rating.rateToUserId" :user-name="rating.rateToUserName" />
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
import { onMounted, ref, computed, watch } from 'vue' // 【新增】 watch
import { useRatingStore } from '@/stores/rating'
import { useRoute } from 'vue-router' // 【新增】 useRoute
import UserPopover from '@/components/common/UserPopover.vue' // 【新增】 UserPopover

// 1. 接收来自路由的 prop
const props = defineProps({
  isOwner: {
    type: Boolean,
    default: false,
  },
})

const ratingStore = useRatingStore()
const route = useRoute() // 2. 获取当前路由
const activeTab = ref('received')

// 3. 计算属性依赖 store
const positiveRate = computed(() => {
  if (!ratingStore.stats || !ratingStore.stats.positiveRate) {
    return 0
  }
  return ratingStore.stats.positiveRate.toFixed(1)
})

// 4. 【核心】在 onMounted 中根据路由获取数据
onMounted(() => {
  fetchDataByRoute()
})

// 5. 【新增】监听路由变化
watch(
  () => route.params.userId,
  (newUserId, oldUserId) => {
    if (newUserId !== oldUserId) {
      fetchDataByRoute()
    }
  },
)

// 6. 【新增】统一的路由数据获取逻辑
function fetchDataByRoute() {
  if (props.isOwner) {
    // A. "我的" 主页，传入 null
    ratingStore.fetchRatings(null) // 假设 fetchRatings 已在 store 中统一
  } else {
    // B. "他人" 主页，从路由参数获取 userId
    const userId = route.params.userId as string
    ratingStore.fetchRatings(userId) // 假设 fetchRatings 已在 store 中统一
  }
}
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
