/**
 * API 模組統一導出
 */
export { default as accountApi } from './account'
export type { LoginRequest, ApiResult, LoginResponseData } from './account'
export { default as roleApi } from './role'
export type { Role, RoleDetail, RoleInput, AvailablePermission, Permission, RoleUser } from './role'
export { default as userApi } from './user'
export type { User, UserDetail, UserInput, UserUpdate, ResetPassword, SetRoles } from './user'
export { default as sysMenuApi } from './sysMenu'
export type { SysMenu, SysMenuCreateInput, SysMenuUpdateInput } from './sysMenu'
export { default as booksApi } from './books'
export { StatusType } from './books'
export type { Book, BookQueryInput, BookInput, BookUpdate } from './books'
export { default as apiClient } from './axios'
