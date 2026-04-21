import apiClient from './axios'
import type { ApiResult } from './account'

/**
 * 角色資料
 */
export interface Role {
  id: string
  name: string
  normalizedName?: string
  concurrencyStamp?: string
}

/**
 * 角色詳情
 */
export interface RoleDetail {
  id: string
  name: string
  permissions: string[]
}

/**
 * 權限
 */
export interface Permission {
  type: string
  value: string
}

/**
 * 可用權限（舊格式，保留供相容）
 */
export interface AvailablePermission {
  value: string
  label: string
  category: string
}

/**
 * 權限樹節點（從 SysMenu 生成）
 */
export interface PermissionTreeNode {
  id: number
  label: string
  moduleNo: string
  isMenu: boolean
  children?: PermissionTreeNode[]
}

/**
 * 角色輸入模型
 */
export interface RoleInput {
  name: string
  permissions?: string[]
}

/**
 * 角色使用者
 */
export interface RoleUser {
  id: string
  userName: string
  email?: string
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
 * 角色相關 API
 */
export const roleApi = {
  /**
   * 取得所有角色列表（支援分頁）
   * @param pageNumber 頁碼（預設為 1）
   * @param pageSize 每頁筆數（預設為 10）
   */
  async getRoles(pageNumber: number = 1, pageSize: number = 10): Promise<ApiResult<PagedResult<Role>>> {
    const response = await apiClient.get<ApiResult<PagedResult<Role>>>('/api/Role', {
      params: { pageNumber, pageSize }
    })
    return response.data
  },

  /**
   * 取得單一角色詳情
   */
  async getRole(id: string): Promise<ApiResult<RoleDetail>> {
    const response = await apiClient.get<ApiResult<RoleDetail>>(`/api/Role/${id}`)
    return response.data
  },

  /**
   * 新增角色
   */
  async createRole(data: RoleInput): Promise<ApiResult> {
    const response = await apiClient.post<ApiResult>('/api/Role', data)
    return response.data
  },

  /**
   * 更新角色
   */
  async updateRole(id: string, data: RoleInput): Promise<ApiResult> {
    const response = await apiClient.put<ApiResult>(`/api/Role/${id}`, data)
    return response.data
  },

  /**
   * 刪除角色
   */
  async deleteRole(id: string): Promise<ApiResult> {
    const response = await apiClient.delete<ApiResult>(`/api/Role/${id}`)
    return response.data
  },

  /**
   * 取得所有可用的權限列表
   */
  async getAvailablePermissions(): Promise<ApiResult<PermissionTreeNode[]>> {
    const response = await apiClient.get<ApiResult<PermissionTreeNode[]>>('/api/Role/permissions')
    return response.data
  },

  /**
   * 取得指定角色的使用者列表
   */
  async getRoleUsers(id: string): Promise<ApiResult<RoleUser[]>> {
    const response = await apiClient.get<ApiResult<RoleUser[]>>(`/api/Role/${id}/users`)
    return response.data
  }
}

export default roleApi
