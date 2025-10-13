<template>
  <div class="ratings-container">
    <h1>收到的评价</h1>

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
            <span
              >来自: <strong>{{ rating.rateByUserName }}</strong></span
            >
            <span class="rating-time">{{ new Date(rating.createdAt).toLocaleDateString() }}</span>
          </div>
        </template>

        <div class="rating-content">
          <div class="score">
            <span>评分: </span>
            <el-icon v-for="i in rating.score" :key="i" color="#f56c6c"
              ><i-ep-star-filled
            /></el-icon>
            <el-icon v-for="i in 5 - rating.score" :key="i" color="#dcdfe6"
              ><i-ep-star-filled
            /></el-icon>
          </div>
          <p class="comments">评价内容: {{ rating.comments || '用户没有留下任何文字评价。' }}</p>
        </div>

        <template #footer>
          <div class="card-footer">
            <span
              >评价角色:
              <strong>{{
                rating.ratedUserRole === 'Photographer' ? '摄影师' : '模特'
              }}</strong></span
            >
          </div>
        </template>
      </el-card>
    </div>

    <el-empty v-else description="还没有收到任何评价哦" />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRatingStore } from '@/stores/rating'

const ratingStore = useRatingStore()

onMounted(() => {
  ratingStore.fetchReceivedRatings()
})
</script>

<style scoped>
.ratings-container {
  padding: 1rem 2rem;
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
