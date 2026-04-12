import apiClient from './axios'
import type { ApiResult } from './account'

/**
 * 系統選單資料
 */
export interface SysMenu {
  id: number
  parentModuleId: number
  moduleNo: string
  component?: string
  hidden?: boolean
  icon?: string
  isMenu: boolean
  isOpen?: boolean
  levels: number
  name: string
  sortOrder: number
  status: boolean
  tips?: string
  url?: string
  createdAt: string
  createdBy: string
  updatedAt?: string
  updatedBy?: string
  children?: SysMenu[]  // 樹狀結構的子選單
  hasChildren?: boolean // 是否有子選單
}

/**
 * 系統選單新增輸入
 */
export interface SysMenuCreateInput {
  parentModuleId: number
  moduleNo: string
  component?: string
  hidden?: boolean
  icon?: string
  isMenu: boolean
  isOpen?: boolean
  levels: number
  name: string
  sortOrder: number
  status: boolean
  tips?: string
  url?: string
}

/**
 * 系統選單更新輸入
 */
export interface SysMenuUpdateInput {
  parentModuleId?: number
  moduleNo?: string
  component?: string
  hidden?: boolean
  icon?: string
  isMenu?: boolean
  isOpen?: boolean
  levels?: number
  name?: string
  sortOrder?: number
  status?: boolean
  tips?: string
  url?: string
}

/**
 * 系統選單相關 API
 */
export const sysMenuApi = {
  /**
   * 取得單一系統選單詳情
   */
  async getSysMenu(id: number): Promise<ApiResult<SysMenu>> {
    const response = await apiClient.get<ApiResult<SysMenu>>(`/api/SysMenu/${id}`)
    return response.data
  },

  /**
   * 新增系統選單
   */
  async createSysMenu(data: SysMenuCreateInput): Promise<ApiResult<SysMenu>> {
    const response = await apiClient.post<ApiResult<SysMenu>>('/api/SysMenu', data)
    return response.data
  },

  /**
   * 更新系統選單
   */
  async updateSysMenu(id: number, data: SysMenuUpdateInput): Promise<ApiResult> {
    const response = await apiClient.put<ApiResult>(`/api/SysMenu/${id}`, data)
    return response.data
  },

  /**
   * 刪除系統選單
   */
  async deleteSysMenu(id: number): Promise<ApiResult> {
    const response = await apiClient.delete<ApiResult>(`/api/SysMenu/${id}`)
    return response.data
  },

  /**
   * 取得選單樹狀結構
   */
  async getMenuTree(): Promise<ApiResult<SysMenu[]>> {
    const response = await apiClient.get<ApiResult<SysMenu[]>>('/api/SysMenu/tree')
    return response.data
  },

  /**
   * 取得當前用戶的導航選單（用於左側選單顯示）
   */
  async getUserNavigationMenus(): Promise<ApiResult<SysMenu[]>> {
    const response = await apiClient.get<ApiResult<SysMenu[]>>('/api/SysMenu/navigation')
    return response.data
  }
}

export default sysMenuApi
