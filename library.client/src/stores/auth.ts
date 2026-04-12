import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import accountApi from '@/api/account'
import type { ApiResult, LoginResponseData } from '@/api/account'

interface UserInfo {
  id?: string
  username: string
  email?: string
  avatar?: string
  roles?: string[]
}

interface LoginCredentials {
  username: string
  password: string
  remember?: boolean
}

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(null)
  const user = ref<UserInfo | null>(null)
  const loading = ref(false)

  // Getters
  const isAuthenticated = computed(() => !!token.value)
  const userInfo = computed(() => user.value)

  // Actions
  
  /**
   * 初始化 - 從 localStorage/sessionStorage 載入 token
   */
  const initialize = () => {
    const savedToken = localStorage.getItem('token') || sessionStorage.getItem('token')
    if (savedToken) {
      token.value = savedToken
      fetchUserInfo()
    }
  }

  /**
   * 登入
   */
  const login = async (credentials: LoginCredentials) => {
    loading.value = true
    try {
      // 呼叫登入 API
      const response = await accountApi.login({
        userName: credentials.username,
        password: credentials.password
      })

      // 檢查 API 回應
      if (!response.succeeded) {
        // 登入失敗
        const errorMessage = response.errors?.length > 0 
          ? response.errors.join(', ') 
          : response.message || '登入失敗，請檢查您的帳號密碼'
        ElMessage.error(errorMessage)
        return { success: false, error: errorMessage }
      }

      // 登入成功 - 處理後端返回的資料
      if (response.data) {
        // 儲存 accessToken
        token.value = response.data.accessToken
        
        // 建立用戶資訊物件
        user.value = {
          username: response.data.userName,
          roles: ['user']
        }

        // 根據 remember 選擇儲存位置
        const storage = credentials.remember ? localStorage : sessionStorage
        storage.setItem('token', response.data.accessToken)
        storage.setItem('refreshToken', response.data.refreshToken)
        storage.setItem('user', JSON.stringify(user.value))

        ElMessage.success(response.message || '登入成功')
        return { success: true }
      } else {
        ElMessage.error('登入回應資料格式錯誤')
        return { success: false, error: '登入回應資料格式錯誤' }
      }
    } catch (error: any) {
      console.error('Login error:', error)
      
      // 處理錯誤訊息
      let errorMessage = '登入失敗，請檢查您的帳號密碼'
      
      if (error.response?.data) {
        const errorData = error.response.data
        if (errorData.errors && Array.isArray(errorData.errors) && errorData.errors.length > 0) {
          errorMessage = errorData.errors.join(', ')
        } else if (errorData.message) {
          errorMessage = errorData.message
        } else if (typeof errorData === 'string') {
          errorMessage = errorData
        }
      } else if (error.message) {
        errorMessage = error.message
      }
      
      ElMessage.error(errorMessage)
      return { success: false, error }
    } finally {
      loading.value = false
    }
  }

  /**
   * 登出
   */
  const logout = async () => {
    try {
      // 呼叫登出 API
      if (token.value) {
        try {
          await accountApi.logout()
        } catch (error) {
          // 即使 API 失敗，也要清除本地狀態
          console.error('Logout API error:', error)
        }
      }

      // 清除狀態
      token.value = null
      user.value = null

      // 清除儲存
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      localStorage.removeItem('refreshToken')
      sessionStorage.removeItem('token')
      sessionStorage.removeItem('user')
      sessionStorage.removeItem('refreshToken')

      ElMessage.success('已成功登出')
    } catch (error) {
      console.error('Logout error:', error)
    }
  }

  /**
   * 獲取用戶資訊
   */
  const fetchUserInfo = async () => {
    if (!token.value) return

    try {
      // 從 storage 載入用戶資訊
      const savedUser = localStorage.getItem('user') || sessionStorage.getItem('user')
      if (savedUser) {
        user.value = JSON.parse(savedUser)
      }

      // 如果需要從 API 獲取最新的用戶資訊
      try {
        const userInfo = await accountApi.getCurrentUser()
        user.value = userInfo
        
        // 更新儲存的用戶資訊
        const storage = localStorage.getItem('token') ? localStorage : sessionStorage
        storage.setItem('user', JSON.stringify(userInfo))
      } catch (error) {
        // 如果 API 失敗，使用已存儲的用戶資訊
        console.warn('Failed to fetch fresh user info, using cached data')
      }
    } catch (error) {
      console.error('Failed to fetch user info:', error)
      // 如果獲取用戶資訊失敗，可能 token 已過期
      await logout()
    }
  }

  /**
   * 更新用戶資訊
   */
  const updateUserInfo = (newUserInfo: Partial<UserInfo>) => {
    if (user.value) {
      user.value = { ...user.value, ...newUserInfo }
      
      // 更新儲存
      const storage = localStorage.getItem('token') ? localStorage : sessionStorage
      storage.setItem('user', JSON.stringify(user.value))
    }
  }

  /**
   * 檢查 token 是否有效
   */
  const checkTokenValidity = async () => {
    if (!token.value) return false

    try {
      return await accountApi.verifyToken()
    } catch (error) {
      await logout()
      return false
    }
  }

  /**
   * 從 OAuth 社群登入設定 tokens
   */
  const setTokensFromOAuth = (accessToken: string, refreshToken: string, userName: string) => {
    token.value = accessToken
    user.value = {
      username: userName,
      roles: ['user']
    }

    localStorage.setItem('token', accessToken)
    localStorage.setItem('refreshToken', refreshToken)
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  return {
    // State
    token,
    user,
    loading,
    
    // Getters
    isAuthenticated,
    userInfo,
    
    // Actions
    initialize,
    login,
    logout,
    fetchUserInfo,
    updateUserInfo,
    checkTokenValidity,
    setTokensFromOAuth
  }
})
