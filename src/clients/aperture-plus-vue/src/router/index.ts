import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import HomeView from '../views/HomeView.vue'
import ProfileView from '../views/ProfileView.vue'
import ProfilePortfolio from '@/components/profile/ProfilePortfolio.vue'
import ProfileRatings from '@/components/profile/ProfileRatings.vue'
import GalleryDetailView from '../views/GalleryDetailView.vue'

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
      meta: { requiresAuth: true }, // **关键！** 我们给这个路由加了一个元信息
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
      path: '/profile',
      name: 'profile',
      component: ProfileView,
      meta: { requiresAuth: true }, // 同样需要登录保护
      // ↓↓↓ 这就是嵌套路由 ↓↓↓
      children: [
        {
          // 默认子路由: 当访问 /profile 时，重定向到作品集
          path: '',
          redirect: { name: 'profile-portfolio' },
        },
        {
          path: 'portfolio', // 匹配 /profile/portfolio
          name: 'profile-portfolio',
          component: ProfilePortfolio,
        },
        {
          path: 'portfolio/:galleryId',
          name: 'gallery-detail',
          component: GalleryDetailView,
        },
        {
          path: 'ratings', // 匹配 /profile/ratings
          name: 'profile-ratings',
          component: ProfileRatings,
        },
      ],
    },
  ],
})
import { useAuthStore } from '@/stores/auth'

// 全局前置守卫
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // 检查目标路由是否需要认证
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    // 如果用户未认证，则重定向到登录页
    next({ name: 'login' })
  } else {
    // 否则，允许导航
    next()
  }
})
export default router
