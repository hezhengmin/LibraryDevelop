import apiClient from './axios'
import type { ApiResult } from './account'

/**
 * 使用者資料
 */
export interface User {
  id: string
  userName: string
  email?: string
  emailConfirmed: boolean
  phoneNumber?: string
  lockoutEnd?: string
  isLocked: boolean
  roles?: string[]
}

/**
 * 使用者詳情
 */
export interface UserDetail {
  id: string
  userName: string
  email?: string
  emailConfirmed: boolean
  phoneNumber?: string
  phoneNumberConfirmed: boolean
  lockoutEnd?: string
  isLocked: boolean
  accessFailedCount: number
  roles: string[]
}

/**
 * 使用者輸入
 */
export interface UserInput {
  userName: string
  password: string
  email?: string
  phoneNumber?: string
  emailConfirmed?: boolean
  roles?: string[]
}

/**
 * 使用者更新
 */
export interface UserUpdate {
  email?: string
  phoneNumber?: string
  emailConfirmed?: boolean
  roles?: string[]
}

/**
 * 重設密碼
 */
export interface ResetPassword {
  newPassword: string
}

/**
 * 設定角色
 */
export interface SetRoles {
  roles: string[]
}

/**
 * 分頁結果
 */
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

/**
 * 使用者相關 API
 */
export const userApi = {
  /**
   * 取得所有使用者列表（支援分頁）
   * @param pageNumber 頁碼（預設為 1）
   * @param pageSize 每頁筆數（預設為 10）
   */
  async getUsers(pageNumber: number = 1, pageSize: number = 10): Promise<ApiResult<PagedResult<User>>> {
    const response = await apiClient.get<ApiResult<PagedResult<User>>>('/api/User', {
      params: { pageNumber, pageSize }
    })
    return response.data
  },

  /**
   * 取得單一使用者詳情
   */
  async getUser(id: string): Promise<ApiResult<UserDetail>> {
    const response = await apiClient.get<ApiResult<UserDetail>>(`/api/User/${id}`)
    return response.data
  },

  /**
   * 新增使用者
   */
  async createUser(data: UserInput): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>('/api/User', data)
    return response.data
  },

  /**
   * 更新使用者
   */
  async updateUser(id: string, data: UserUpdate): Promise<ApiResult> {
    const response = await apiClient.put<ApiResult>(`/api/User/${id}`, data)
    return response.data
  },

  /**
   * 刪除使用者
   */
  async deleteUser(id: string): Promise<ApiResult> {
    const response = await apiClient.delete<ApiResult>(`/api/User/${id}`)
    return response.data
  },

  /**
   * 重設密碼
   */
  async resetPassword(id: string, data: ResetPassword): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>(`/api/User/${id}/reset-password`, data)
    return response.data
  },

  /**
   * 鎖定使用者
   */
  async lockUser(id: string): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>(`/api/User/${id}/lock`)
    return response.data
  },

  /**
   * 解鎖使用者
   */
  async unlockUser(id: string): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>(`/api/User/${id}/unlock`)
    return response.data
  },

  /**
   * 取得使用者的角色
   */
  async getUserRoles(id: string): Promise<ApiResult<string[]>> {
    const response = await apiClient.get<ApiResult<string[]>>(`/api/User/${id}/roles`)
    return response.data
  },

  /**
   * 設定使用者的角色
   */
  async setUserRoles(id: string, data: SetRoles): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>(`/api/User/${id}/roles`, data)
    return response.data
  }
}

export default userApi
