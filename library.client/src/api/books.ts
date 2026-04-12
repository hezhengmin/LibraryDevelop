import apiClient from './axios'
import type { ApiResult } from './account'

/**
 * 書籍狀態類型
 */
export enum StatusType {
  None = 0,  // 無庫存
  Stock = 1  // 有庫存
}

/**
 * 書籍資料
 */
export interface Book {
  id: string
  title: string
  status: StatusType
  numberOfCopies: number
  isbn: string
  issn: string
  gpn: string
  publisher: string
  creator: string
  publishDate: string
  edition: string
  cover: string
  classify: string
  gpnType: string
  subject: string
  language: string
  introduction: string
  price: number
  targetPeople: string
}

/**
 * 書籍查詢參數
 */
export interface BookQueryInput {
  pageNumber: number
  pageSize: number
  searchTerm?: string
}

/**
 * 書籍新增輸入
 */
export interface BookInput {
  title: string
  status: StatusType
  numberOfCopies: number
  isbn?: string
  issn?: string
  gpn: string
  publisher: string
  rightCondition: string
  creator: string
  publishDate: string
  edition?: string
  cover?: string
  classify?: string
  gpnType: string
  subject?: string
  governance?: string
  grade?: string
  pages?: number
  size?: string
  binding?: string
  language: string
  introduction?: string
  catalog?: string
  price?: number
  targetPeople?: string
  types?: string
  attachment: string
  url?: string
  duration: string
  numbers?: string
  restriction?: string
  ceasedDate?: string
  authority: string
}

/**
 * 書籍更新輸入
 */
export interface BookUpdate {
  title?: string
  status?: StatusType
  numberOfCopies?: number
  isbn?: string
  issn?: string
  gpn?: string
  publisher?: string
  rightCondition?: string
  creator?: string
  publishDate?: string
  edition?: string
  cover?: string
  classify?: string
  gpnType?: string
  subject?: string
  governance?: string
  grade?: string
  pages?: number
  size?: string
  binding?: string
  language?: string
  introduction?: string
  catalog?: string
  price?: number
  targetPeople?: string
  types?: string
  attachment?: string
  url?: string
  duration?: string
  numbers?: string
  restriction?: string
  ceasedDate?: string
  authority?: string
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
 * 書籍相關 API
 */
export const booksApi = {
  /**
   * 取得書籍列表（支援分頁和搜尋）
   * @param queryModel 查詢參數
   */
  async getBooks(queryModel: BookQueryInput): Promise<ApiResult<PagedResult<Book>>> {
    const response = await apiClient.post<ApiResult<PagedResult<Book>>>('/api/Book/GetBooks', queryModel)
    return response.data
  },

  /**
   * 取得單一書籍詳情
   * @param id 書籍ID
   */
  async getBookById(id: string): Promise<ApiResult<Book>> {
    const response = await apiClient.get<ApiResult<Book>>(`/api/Book/${id}`)
    return response.data
  },

  /**
   * 新增書籍
   * @param data 書籍資料
   */
  async createBook(data: BookInput): Promise<ApiResult<Book>> {
    const response = await apiClient.post<ApiResult<Book>>('/api/Book', data)
    return response.data
  },

  /**
   * 更新書籍
   * @param id 書籍ID
   * @param data 更新資料
   */
  async updateBook(id: string, data: BookUpdate): Promise<ApiResult> {
    const response = await apiClient.put<ApiResult>(`/api/Book/${id}`, data)
    return response.data
  },

  /**
   * 刪除書籍
   * @param id 書籍ID
   */
  async deleteBook(id: string): Promise<ApiResult> {
    const response = await apiClient.delete<ApiResult>(`/api/Book/${id}`)
    return response.data
  }
}

export default booksApi
