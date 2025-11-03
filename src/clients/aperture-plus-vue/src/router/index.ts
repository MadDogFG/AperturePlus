// src/router/index.ts

import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import HomeView from '../views/HomeView.vue'
import ProfileView from '../views/ProfileView.vue'
import ProfilePortfolio from '@/components/profile/ProfilePortfolio.vue'
import ProfileRatings from '@/components/profile/ProfileRatings.vue'
import GalleryDetailView from '../views/GalleryDetailView.vue'
import ActivityDetailView from '../views/ActivityDetailView.vue'
import ActivityHistory from '@/components/profile/ActivityHistory.vue'
import PublicProfileView from '../views/PublicProfileView.vue' // 导入 Public view

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/home',
    },
    {
      path: '/home',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true },
    },
    // ... (login, register 保持不变) ...
    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
    },
    {
      // --- 路由 1: "我的主页" ---
      path: '/profile',
      name: 'profile',
      component: ProfileView, // 容器 (侧边栏 + RouterView)
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: { name: 'profile-portfolio' },
        },
        {
          path: 'portfolio',
          name: 'profile-portfolio',
          component: ProfilePortfolio,
          props: { isOwner: true }, // 传入 isOwner
        },
        {
          path: 'ratings',
          name: 'profile-ratings',
          component: ProfileRatings,
          props: { isOwner: true }, // 传入 isOwner
        },
        {
          path: 'history',
          name: 'profile-history',
          component: ActivityHistory,
        },
        // "我的" 相册详情页
        {
          path: 'portfolio/:galleryId',
          name: 'gallery-detail', // 确保这个 name 唯一
          component: GalleryDetailView,
          props: (route) => ({
            galleryId: route.params.galleryId,
            isOwner: true, // 自动传入 isOwner
          }),
        },
      ],
    },
    {
      // --- 路由 2: "他人主页" ---
      path: '/user/:userId',
      name: 'public-profile',
      component: PublicProfileView, // 容器 (侧边栏 + RouterView)
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: (to) => ({
            name: 'public-portfolio',
            params: { userId: to.params.userId },
          }),
        },
        {
          path: 'portfolio',
          name: 'public-portfolio',
          component: ProfilePortfolio,
          props: { isOwner: false }, // 传入 isOwner
        },
        {
          path: 'ratings',
          name: 'public-ratings',
          component: ProfileRatings,
          props: { isOwner: false }, // 传入 isOwner
        },
        // "他人" 相册详情页
        {
          // 【重要】我们复用 'gallery-detail' name，但需要确保路由结构匹配
          // 或者使用一个新 name，这里我们尝试复用
          // **更新：** 复用 name 会导致歧义。我们用新 name。
          path: 'portfolio/:galleryId',
          name: 'public-gallery-detail', // 使用新 name
          component: GalleryDetailView,
          props: (route) => ({
            galleryId: route.params.galleryId,
            userId: route.params.userId,
            isOwner: false, // 自动传入 isOwner
          }),
        },
      ],
    },
    {
      path: '/activity/:id',
      name: 'activity-detail',
      component: ActivityDetailView,
      meta: { requiresAuth: true },
    },
  ],
})

// ... (路由守卫保持不变) ...
import { useAuthStore } from '@/stores/auth'
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router
