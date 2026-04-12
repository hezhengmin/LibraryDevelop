<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { booksApi, StatusType, type Book, type BookQueryInput, type BookInput, type BookUpdate } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'

// 書籍列表數據
const books = ref<Book[]>([])
const loading = ref(false)
const total = ref(0)

// 查詢參數
const queryParams = ref<BookQueryInput>({
  pageNumber: 1,
  pageSize: 10,
  searchTerm: ''
})

// 搜尋關鍵字
const searchTerm = ref('')

// 對話框相關
const dialogVisible = ref(false)
const dialogTitle = ref('')
const dialogMode = ref<'create' | 'edit'>('create')
const formRef = ref<FormInstance>()

// 書籍詳情對話框
const detailDialogVisible = ref(false)
const currentBook = ref<Book | null>(null)

// 表單數據
const formData = reactive<BookInput>({
  title: '',
  status: StatusType.Stock,
  numberOfCopies: 0,
  isbn: '',
  issn: '',
  gpn: '',
  publisher: '',
  rightCondition: '',
  creator: '',
  publishDate: new Date().toISOString().split('T')[0],
  edition: '',
  cover: '',
  classify: '',
  gpnType: '',
  subject: '',
  governance: '',
  grade: '',
  pages: 0,
  size: '',
  binding: '',
  language: '中文',
  introduction: '',
  catalog: '',
  price: 0,
  targetPeople: '',
  types: '',
  attachment: '',
  url: '',
  duration: '',
  numbers: '',
  restriction: '',
  ceasedDate: '',
  authority: ''
})

// 編輯時的書籍ID
const editingBookId = ref<string>('')

// 表單驗證規則
const formRules: FormRules = {
  title: [
    { required: true, message: '請輸入書名', trigger: 'blur' },
    { max: 150, message: '書名長度不能超過150個字元', trigger: 'blur' }
  ],
  gpn: [
    { required: true, message: '請輸入GPN', trigger: 'blur' },
    { max: 10, message: 'GPN長度不能超過10個字元', trigger: 'blur' }
  ],
  publisher: [
    { required: true, message: '請輸入出版單位', trigger: 'blur' },
    { max: 50, message: '出版單位長度不能超過50個字元', trigger: 'blur' }
  ],
  rightCondition: [
    { required: true, message: '請輸入出版情況', trigger: 'blur' },
    { max: 50, message: '出版情況長度不能超過50個字元', trigger: 'blur' }
  ],
  creator: [
    { required: true, message: '請輸入作者資訊', trigger: 'blur' },
    { max: 500, message: '作者資訊長度不能超過500個字元', trigger: 'blur' }
  ],
  publishDate: [
    { required: true, message: '請選擇出版日期', trigger: 'change' }
  ],
  gpnType: [
    { required: true, message: '請輸入出版品分類', trigger: 'blur' },
    { max: 50, message: '出版品分類長度不能超過50個字元', trigger: 'blur' }
  ],
  language: [
    { required: true, message: '請輸入語言', trigger: 'blur' },
    { max: 10, message: '語言長度不能超過10個字元', trigger: 'blur' }
  ],
  attachment: [
    { required: true, message: '請輸入附件', trigger: 'blur' },
    { max: 50, message: '附件長度不能超過50個字元', trigger: 'blur' }
  ],
  duration: [
    { required: true, message: '請輸入播放時間長度', trigger: 'blur' },
    { max: 50, message: '播放時間長度不能超過50個字元', trigger: 'blur' }
  ],
  authority: [
    { required: true, message: '請輸入授權資訊', trigger: 'blur' },
    { max: 500, message: '授權資訊長度不能超過500個字元', trigger: 'blur' }
  ]
}

// 獲取書籍狀態顯示文字
const getStatusText = (status: StatusType): string => {
  return status === StatusType.Stock ? '有庫存' : '無庫存'
}

// 獲取書籍狀態標籤類型
const getStatusType = (status: StatusType): 'success' | 'info' => {
  return status === StatusType.Stock ? 'success' : 'info'
}

// 載入書籍列表
const loadBooks = async () => {
  loading.value = true
  try {
    const result = await booksApi.getBooks(queryParams.value)
    if (result.succeeded && result.data) {
      books.value = result.data.items
      total.value = result.data.totalCount
    } else {
      ElMessage.error(result.message || '載入書籍列表失敗')
    }
  } catch (error) {
    console.error('載入書籍列表錯誤:', error)
    ElMessage.error('載入書籍列表失敗')
  } finally {
    loading.value = false
  }
}

// 搜尋
const handleSearch = () => {
  queryParams.value.searchTerm = searchTerm.value
  queryParams.value.pageNumber = 1
  loadBooks()
}

// 重置搜尋
const handleReset = () => {
  searchTerm.value = ''
  queryParams.value.searchTerm = ''
  queryParams.value.pageNumber = 1
  loadBooks()
}

// 分頁改變
const handlePageChange = (page: number) => {
  queryParams.value.pageNumber = page
  loadBooks()
}

// 每頁數量改變
const handleSizeChange = (size: number) => {
  queryParams.value.pageSize = size
  queryParams.value.pageNumber = 1
  loadBooks()
}

// 重置表單
const resetForm = () => {
  Object.assign(formData, {
    title: '',
    status: StatusType.Stock,
    numberOfCopies: 0,
    isbn: '',
    issn: '',
    gpn: '',
    publisher: '',
    rightCondition: '',
    creator: '',
    publishDate: new Date().toISOString().split('T')[0],
    edition: '',
    cover: '',
    classify: '',
    gpnType: '',
    subject: '',
    governance: '',
    grade: '',
    pages: 0,
    size: '',
    binding: '',
    language: '中文',
    introduction: '',
    catalog: '',
    price: 0,
    targetPeople: '',
    types: '',
    attachment: '',
    url: '',
    duration: '',
    numbers: '',
    restriction: '',
    ceasedDate: '',
    authority: ''
  })
  formRef.value?.clearValidate()
}

// 開啟新增對話框
const handleCreate = () => {
  dialogMode.value = 'create'
  dialogTitle.value = '新增書籍'
  resetForm()
  dialogVisible.value = true
}

// 開啟編輯對話框
const handleEdit = async (row: Book) => {
  try {
    const result = await booksApi.getBookById(row.id)
    if (result.succeeded && result.data) {
      dialogMode.value = 'edit'
      dialogTitle.value = '編輯書籍'
      editingBookId.value = row.id
      
      // 填充表單數據
      Object.assign(formData, {
        title: result.data.title,
        status: result.data.status,
        numberOfCopies: result.data.numberOfCopies,
        isbn: result.data.isbn || '',
        issn: result.data.issn || '',
        gpn: result.data.gpn,
        publisher: result.data.publisher,
        rightCondition: '', // BookDto 中沒有此欄位，需要從 Book 實體取得
        creator: result.data.creator,
        publishDate: result.data.publishDate ? new Date(result.data.publishDate).toISOString().split('T')[0] : '',
        edition: result.data.edition || '',
        cover: result.data.cover || '',
        classify: result.data.classify || '',
        gpnType: result.data.gpnType,
        subject: result.data.subject || '',
        governance: '',
        grade: '',
        pages: 0,
        size: '',
        binding: '',
        language: result.data.language,
        introduction: result.data.introduction || '',
        catalog: '',
        price: result.data.price,
        targetPeople: result.data.targetPeople || '',
        types: '',
        attachment: '',
        url: '',
        duration: '',
        numbers: '',
        restriction: '',
        ceasedDate: '',
        authority: ''
      })
      
      dialogVisible.value = true
    } else {
      ElMessage.error(result.message || '載入書籍詳情失敗')
    }
  } catch (error) {
    console.error('載入書籍詳情錯誤:', error)
    ElMessage.error('載入書籍詳情失敗')
  }
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        if (dialogMode.value === 'create') {
          const result = await booksApi.createBook(formData)
          if (result.succeeded) {
            ElMessage.success('新增書籍成功')
            dialogVisible.value = false
            loadBooks()
          } else {
            ElMessage.error(result.message || '新增書籍失敗')
          }
        } else {
          // 編輯模式：只傳送有變更的欄位
          const updateData: BookUpdate = {
            title: formData.title,
            status: formData.status,
            numberOfCopies: formData.numberOfCopies,
            isbn: formData.isbn,
            issn: formData.issn,
            gpn: formData.gpn,
            publisher: formData.publisher,
            creator: formData.creator,
            publishDate: formData.publishDate,
            edition: formData.edition,
            cover: formData.cover,
            classify: formData.classify,
            gpnType: formData.gpnType,
            subject: formData.subject,
            language: formData.language,
            introduction: formData.introduction,
            price: formData.price,
            targetPeople: formData.targetPeople
          }
          
          const result = await booksApi.updateBook(editingBookId.value, updateData)
          if (result.succeeded) {
            ElMessage.success('更新書籍成功')
            dialogVisible.value = false
            loadBooks()
          } else {
            ElMessage.error(result.message || '更新書籍失敗')
          }
        }
      } catch (error) {
        console.error('提交表單錯誤:', error)
        ElMessage.error(dialogMode.value === 'create' ? '新增書籍失敗' : '更新書籍失敗')
      } finally {
        loading.value = false
      }
    }
  })
}

// 取消對話框
const handleCancel = () => {
  dialogVisible.value = false
  resetForm()
}

// 查看詳情
const handleView = async (row: Book) => {
  try {
    const result = await booksApi.getBookById(row.id)
    if (result.succeeded && result.data) {
      currentBook.value = result.data
      detailDialogVisible.value = true
    } else {
      ElMessage.error(result.message || '載入書籍詳情失敗')
    }
  } catch (error) {
    console.error('載入書籍詳情錯誤:', error)
    ElMessage.error('載入書籍詳情失敗')
  }
}

// 刪除書籍
const handleDelete = async (row: Book) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除書籍「${row.title}」嗎？`,
      '刪除確認',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    
    loading.value = true
    const result = await booksApi.deleteBook(row.id)
    if (result.succeeded) {
      ElMessage.success('刪除書籍成功')
      loadBooks()
    } else {
      ElMessage.error(result.message || '刪除書籍失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除書籍錯誤:', error)
      ElMessage.error('刪除書籍失敗')
    }
  } finally {
    loading.value = false
  }
}

// 組件掛載時載入數據
onMounted(() => {
  loadBooks()
})
</script>

<template>
  <div class="books-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <h3>書籍資料</h3>
          <el-button type="primary" @click="handleCreate">新增書籍</el-button>
        </div>
      </template>

      <!-- 搜尋欄 -->
      <div class="search-bar">
        <el-input
          v-model="searchTerm"
          placeholder="請輸入書名、作者或ISBN搜尋"
          clearable
          style="width: 300px; margin-right: 10px"
          @keyup.enter="handleSearch"
        />
        <el-button type="primary" @click="handleSearch">搜尋</el-button>
        <el-button @click="handleReset">重置</el-button>
      </div>

      <!-- 表格 -->
      <el-table 
        :data="books" 
        stripe 
        style="width: 100%; margin-top: 20px"
        v-loading="loading"
      >
        <el-table-column prop="title" label="書名" min-width="200" />
        <el-table-column prop="creator" label="作者" width="150" />
        <el-table-column prop="publisher" label="出版社" width="150" />
        <el-table-column prop="isbn" label="ISBN" width="150" />
        <el-table-column prop="numberOfCopies" label="數量" width="80" align="center" />
        <el-table-column label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.status)">
              {{ getStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button size="small" type="primary" @click="handleView(scope.row)">
              查看
            </el-button>
            <el-button size="small" type="warning" @click="handleEdit(scope.row)">
              編輯
            </el-button>
            <el-button size="small" type="danger" @click="handleDelete(scope.row)">
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <div class="pagination">
        <el-pagination
          v-model:current-page="queryParams.pageNumber"
          v-model:page-size="queryParams.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 新增/編輯對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-tabs>
          <el-tab-pane label="基本資訊">
            <el-form-item label="書名" prop="title">
              <el-input v-model="formData.title" placeholder="請輸入書名" />
            </el-form-item>
            
            <el-form-item label="作者" prop="creator">
              <el-input v-model="formData.creator" placeholder="請輸入作者資訊" />
            </el-form-item>
            
            <el-form-item label="出版單位" prop="publisher">
              <el-input v-model="formData.publisher" placeholder="請輸入出版單位" />
            </el-form-item>
            
            <el-form-item label="出版日期" prop="publishDate">
              <el-date-picker
                v-model="formData.publishDate"
                type="date"
                placeholder="請選擇出版日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            
            <el-form-item label="語言" prop="language">
              <el-input v-model="formData.language" placeholder="請輸入語言" />
            </el-form-item>
            
            <el-form-item label="狀態" prop="status">
              <el-radio-group v-model="formData.status">
                <el-radio :label="StatusType.Stock">有庫存</el-radio>
                <el-radio :label="StatusType.None">無庫存</el-radio>
              </el-radio-group>
            </el-form-item>
            
            <el-form-item label="庫存數量" prop="numberOfCopies">
              <el-input-number v-model="formData.numberOfCopies" :min="0" />
            </el-form-item>
          </el-tab-pane>

          <el-tab-pane label="識別資訊">
            <el-form-item label="ISBN" prop="isbn">
              <el-input v-model="formData.isbn" placeholder="請輸入ISBN" />
            </el-form-item>
            
            <el-form-item label="ISSN" prop="issn">
              <el-input v-model="formData.issn" placeholder="請輸入ISSN" />
            </el-form-item>
            
            <el-form-item label="GPN" prop="gpn">
              <el-input v-model="formData.gpn" placeholder="請輸入GPN" />
            </el-form-item>
            
            <el-form-item label="出版品分類" prop="gpnType">
              <el-input v-model="formData.gpnType" placeholder="請輸入出版品分類" />
            </el-form-item>
            
            <el-form-item label="書籍分類" prop="classify">
              <el-input v-model="formData.classify" placeholder="請輸入書籍分類" />
            </el-form-item>
            
            <el-form-item label="主題分類" prop="subject">
              <el-input v-model="formData.subject" placeholder="請輸入主題分類" />
            </el-form-item>
          </el-tab-pane>

          <el-tab-pane label="其他資訊">
            <el-form-item label="版次" prop="edition">
              <el-input v-model="formData.edition" placeholder="請輸入版次" />
            </el-form-item>
            
            <el-form-item label="價格" prop="price">
              <el-input-number v-model="formData.price" :min="0" />
            </el-form-item>
            
            <el-form-item label="適用對象" prop="targetPeople">
              <el-input v-model="formData.targetPeople" placeholder="請輸入適用對象" />
            </el-form-item>
            
            <el-form-item label="書封連結" prop="cover">
              <el-input v-model="formData.cover" placeholder="請輸入書封連結" />
            </el-form-item>
            
            <el-form-item label="書籍介紹" prop="introduction">
              <el-input
                v-model="formData.introduction"
                type="textarea"
                :rows="4"
                placeholder="請輸入書籍介紹"
              />
            </el-form-item>
          </el-tab-pane>

          <el-tab-pane label="必填欄位">
            <el-form-item label="出版情況" prop="rightCondition">
              <el-input v-model="formData.rightCondition" placeholder="請輸入出版情況" />
            </el-form-item>
            
            <el-form-item label="附件" prop="attachment">
              <el-input v-model="formData.attachment" placeholder="請輸入附件" />
            </el-form-item>
            
            <el-form-item label="播放時間長度" prop="duration">
              <el-input v-model="formData.duration" placeholder="請輸入播放時間長度" />
            </el-form-item>
            
            <el-form-item label="授權資訊" prop="authority">
              <el-input v-model="formData.authority" placeholder="請輸入授權資訊" />
            </el-form-item>
          </el-tab-pane>
        </el-tabs>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="handleCancel">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="loading">
            {{ dialogMode === 'create' ? '新增' : '更新' }}
          </el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 詳情對話框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="書籍詳情"
      width="700px"
    >
      <el-descriptions v-if="currentBook" :column="2" border>
        <el-descriptions-item label="書名">{{ currentBook.title }}</el-descriptions-item>
        <el-descriptions-item label="作者">{{ currentBook.creator }}</el-descriptions-item>
        <el-descriptions-item label="出版社">{{ currentBook.publisher }}</el-descriptions-item>
        <el-descriptions-item label="出版日期">
          {{ currentBook.publishDate ? new Date(currentBook.publishDate).toLocaleDateString() : '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="ISBN">{{ currentBook.isbn || '-' }}</el-descriptions-item>
        <el-descriptions-item label="ISSN">{{ currentBook.issn || '-' }}</el-descriptions-item>
        <el-descriptions-item label="GPN">{{ currentBook.gpn }}</el-descriptions-item>
        <el-descriptions-item label="語言">{{ currentBook.language }}</el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag :type="getStatusType(currentBook.status)">
            {{ getStatusText(currentBook.status) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="庫存數量">{{ currentBook.numberOfCopies }}</el-descriptions-item>
        <el-descriptions-item label="價格">{{ currentBook.price }}</el-descriptions-item>
        <el-descriptions-item label="版次">{{ currentBook.edition || '-' }}</el-descriptions-item>
        <el-descriptions-item label="出版品分類">{{ currentBook.gpnType }}</el-descriptions-item>
        <el-descriptions-item label="書籍分類">{{ currentBook.classify || '-' }}</el-descriptions-item>
        <el-descriptions-item label="主題分類">{{ currentBook.subject || '-' }}</el-descriptions-item>
        <el-descriptions-item label="適用對象">{{ currentBook.targetPeople || '-' }}</el-descriptions-item>
        <el-descriptions-item label="書籍介紹" :span="2">
          {{ currentBook.introduction || '-' }}
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<style scoped>
.books-view {
  width: 100%;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 500;
}

.search-bar {
  display: flex;
  align-items: center;
}

.pagination {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>
