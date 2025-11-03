<template>
  <div class="profile-layout">
    <ProfileSidebar :user="user" :is-owner="false" />

    <div class="content">
      <ProfilePortfolio :portfolio="portfolio" :is-loading="isLoadingPortfolio" :is-owner="false" />

      <ProfileRatings
        :stats="stats"
        :received-ratings="receivedRatings"
        :sent-ratings="[]"
        :is-loading="isLoadingRatings"
        :is-stats-loading="isLoadingStats"
        :is-sent-loading="false"
        :is-owner="false"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import apiClient from '@/api/axios' // 假设我们直接调用 API
import ProfileSidebar from '@/components/profile/ProfileSidebar.vue'
import ProfilePortfolio from '@/components/profile/ProfilePortfolio.vue'
import ProfileRatings from '@/components/profile/ProfileRatings.vue'

// 引入所有需要的类型
import type { UserProfile } from '@/stores/user'
import type { Portfolio } from '@/types/portfolio'
import type { RatingStats, ReceivedRating } from '@/types/rating'

const route = useRoute()
const userId = route.params.userId as string

// --- User Profile State ---
const user = ref<UserProfile | null>(null)
const isLoadingUser = ref(true)

// --- Portfolio State ---
const portfolio = ref<Portfolio | null>(null)
const isLoadingPortfolio = ref(true)

// --- Ratings State ---
const stats = ref<RatingStats | null>(null)
const receivedRatings = ref<ReceivedRating[]>([])
const isLoadingRatings = ref(true)
const isLoadingStats = ref(true)

// --- API Endpoints (假设) ---
const userProfileBaseUrl = import.meta.env.VITE_API_USERPROFILE_BASE_URL
const portfolioBaseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
const ratingBaseUrl = import.meta.env.VITE_API_RATING_BASE_URL

// --- Fetching Logic ---
onMounted(async () => {
  if (!userId) return

  // 1. Fetch User Profile
  // (假设: /userprofile/GetUserProfileById/{id} 是公开接口)
  try {
    const profileResponse = await apiClient.get<UserProfile>(
      `${userProfileBaseUrl}/userprofile/GetUserProfileById/${userId}`,
    )
    user.value = profileResponse.data
  } catch (e) {
    console.error('Failed to fetch user profile', e)
  } finally {
    isLoadingUser.value = false
  }

  // 2. Fetch Portfolio
  // (假设: /portfolios/GetPortfolioByUserId/{id} 是新接口)
  try {
    const portfolioResponse = await apiClient.get<Portfolio>(
      `${portfolioBaseUrl}/portfolios/GetPortfolioByUserId/${userId}`,
    )
    portfolio.value = portfolioResponse.data
  } catch (e) {
    console.error('Failed to fetch portfolio', e)
    // 即使失败，也设置一个空作品集对象
    portfolio.value = { portfolioId: '', userId: userId, galleries: [] }
  } finally {
    isLoadingPortfolio.value = false
  }

  // 3. Fetch Ratings Stats
  // (假设: /ratings/statistics/{id} 是新接口)
  try {
    const statsResponse = await apiClient.get<RatingStats>(
      `${ratingBaseUrl}/ratings/statistics/${userId}`,
    )
    stats.value = statsResponse.data
  } catch (e) {
    console.error('Failed to fetch stats', e)
  } finally {
    isLoadingStats.value = false
  }

  // 4. Fetch Received Ratings
  // (假设: /ratings/my-received-ratings/{id} 是新接口)
  try {
    const ratingsResponse = await apiClient.get<ReceivedRating[]>(
      `${ratingBaseUrl}/my-received-ratings/${userId}`,
    )
    receivedRatings.value = ratingsResponse.data
  } catch (e) {
    console.error('Failed to fetch ratings', e)
  } finally {
    isLoadingRatings.value = false
  }
})
</script>

<style scoped>
/* 样式与 ProfileView 完全一致 */
.profile-layout {
  display: flex;
  width: 100%;
  height: 100%;
  background-color: #f5f7fa;
}

.content {
  flex-grow: 1;
  overflow-y: auto;
  height: 100%;
  /* 为多个组件添加一点间距 */
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}
</style>
