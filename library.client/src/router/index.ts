import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useMenuStore } from '@/stores/menu'
import Layout from '@/layout/Layout.vue'

// 靜態路由：不需要權限控制的基礎路由
const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/LoginView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/verify-email',
    name: 'VerifyEmail',
    component: () => import('@/views/VerifyEmailView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/oauth/callback',
    name: 'OAuthCallback',
    component: () => import('@/views/OAuthCallbackView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    name: 'Layout',  // 添加 name
    component: Layout,
    meta: { requiresAuth: true },
    children: [
      // 首頁保留為靜態路由
      {
        path: '',
        name: 'Home',
        component: () => import('@/views/HomeView.vue'),
        meta: { requiresAuth: true, title: '首頁' }
      }
      // 其他路由將通過 API 動態載入
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// Navigation guard to check authentication and load dynamic routes
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  const menuStore = useMenuStore()

  // 初始化 auth store（從 storage 載入 token）
  if (!authStore.token) {
    authStore.initialize()
  }

  const isAuthenticated = authStore.isAuthenticated

  // If route requires auth and user is not authenticated
  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
    return
  }

  // If user is authenticated and trying to access login/register page
  if ((to.path === '/login' || to.path === '/register') && isAuthenticated) {
    next('/')
    return
  }

  // 如果已登入但尚未載入動態路由，載入之
  if (isAuthenticated && !menuStore.routesLoaded && to.path !== '/login') {
    try {
      console.log('🔄 正在載入動態路由...')
      const success = await menuStore.loadNavigationMenus()
      
      if (success) {
        // 路由載入成功，重新導航到目標路徑
        // 使用 replace 避免在歷史記錄中留下額外的條目
        next({ ...to, replace: true })
        return
      } else {
        console.warn('⚠️ 動態路由載入失敗，繼續使用靜態路由')
      }
    } catch (error) {
      console.error('❌ 載入動態路由時發生錯誤:', error)
    }
  }

  // Otherwise, proceed normally
  next()
})

export default router
