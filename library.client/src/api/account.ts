import apiClient from './axios'

/**
 * 登入請求參數
 */
export interface LoginRequest {
  userName: string  // 後端使用 UserName
  password: string
}

/**
 * 註冊請求參數
 */
export interface RegisterRequest {
  userName: string
  email: string
  password: string
  confirmPassword: string
}

/**
 * 後端統一回應格式
 */
export interface ApiResult<T = any> {
  succeeded: boolean
  errors: string[]
  message: string | null
  data: T | null
}

/**
 * 登入回應資料
 */
export interface LoginResponseData {
  accessToken: string
  refreshToken: string
  userName: string
}

/**
 * 帳號相關 API
 */
export const accountApi = {
  /**
   * 登入
   * @param data 登入資料
   */
  async login(data: LoginRequest): Promise<ApiResult<LoginResponseData>> {
    const response = await apiClient.post<ApiResult<LoginResponseData>>('/api/Account/Login', {
      userName: data.userName,
      password: data.password
    })
    return response.data
  },

  /**
   * 註冊
   * @param data 註冊資料
   */
  async register(data: RegisterRequest): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>('/api/Account/Register', data)
    return response.data
  },

  /**
   * 重新發送驗證信
   * @param email 電子郵件
   */
  async resendVerification(email: string): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>('/api/Account/ResendVerification', { email })
    return response.data
  },

  /**
   * 確認電子郵件驗證
   * @param userId 使用者 ID
   * @param token 驗證 token
   */
  async confirmEmail(userId: string, token: string): Promise<ApiResult> {
    const response = await apiClient.get<ApiResult>('/api/Account/ConfirmEmail', {
      params: { userId, token }
    })
    return response.data
  },

  /**
   * 登出
   */
  async logout(): Promise<void> {
    await apiClient.post('/api/Account/Logout')
  },

  /**
   * 獲取當前用戶資訊
   */
  async getCurrentUser(): Promise<any> {
    const response = await apiClient.get<ApiResult>('/api/Account/CurrentUser')
    return response.data.data
  },

  /**
   * 刷新 Token
   * @param refreshToken 刷新令牌
   */
  async refreshToken(refreshToken: string): Promise<{ token: string; refreshToken: string }> {
    const response = await apiClient.post('/api/Account/RefreshToken', { refreshToken })
    return response.data
  },

  /**
   * 修改密碼
   * @param data 密碼資料
   */
  async changePassword(data: { oldPassword: string; newPassword: string }): Promise<void> {
    await apiClient.post('/api/Account/ChangePassword', data)
  },

  /**
   * 驗證 Token
   */
  async verifyToken(): Promise<boolean> {
    try {
      await apiClient.get('/api/Account/Verify')
      return true
    } catch {
      return false
    }
  }
}

export default accountApi
