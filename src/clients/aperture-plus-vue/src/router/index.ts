// src/router/index.ts

import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import HomeView from '../views/HomeView.vue'

// 1. 导入所有 Profile 相关的视图
import ProfileView from '../views/ProfileView.vue'
import ProfilePortfolio from '@/components/profile/ProfilePortfolio.vue'
import ProfileRatings from '@/components/profile/ProfileRatings.vue'
import GalleryDetailView from '../views/GalleryDetailView.vue'
import ActivityDetailView from '../views/ActivityDetailView.vue'
import ActivityHistory from '@/components/profile/ActivityHistory.vue'

// 2. 导入新的 PublicProfileView
import PublicProfileView from '../views/PublicProfileView.vue'

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
      // "我的主页" 路由
      path: '/profile',
      name: 'profile',
      component: ProfileView,
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
          // 3. 【修改】传递 isOwner prop
          // 注意：Portfolio 组件内部仍从 store 获取数据
          props: { isOwner: true },
        },
        {
          path: 'portfolio/:galleryId',
          name: 'gallery-detail',
          component: GalleryDetailView,
        },
        {
          path: 'ratings',
          name: 'profile-ratings',
          component: ProfileRatings,
          // 4. 【修改】传递 isOwner prop
          props: { isOwner: true },
        },
        {
          path: 'history',
          name: 'profile-history',
          component: ActivityHistory,
        },
      ],
    },
    {
      // 5. 【新增】"他人主页" 路由
      path: '/user/:userId',
      name: 'public-profile',
      component: PublicProfileView,
      meta: { requiresAuth: true },
      // 注意：这个视图不使用子路由，它自己组合了所有组件
    },
    {
      path: '/activity/:id',
      name: 'activity-detail',
      component: ActivityDetailView,
      meta: { requiresAuth: true },
    },
  ],
})

// 路由守卫 (保持不变)
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
