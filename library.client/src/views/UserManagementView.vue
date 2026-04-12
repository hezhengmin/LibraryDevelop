<template>
  <div class="user-management">
    <!-- 頁面標題與操作區 -->
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">帳號管理</h1>
        <p class="page-subtitle">管理系統使用者與角色指派</p>
      </div>
      <el-button type="primary" :icon="Plus" @click="handleCreate">新增使用者</el-button>
    </div>

    <!-- 使用者列表 -->
    <el-card class="user-list-card" shadow="never">
      <el-table v-loading="loading" :data="users" stripe style="width: 100%">
        <el-table-column prop="userName" label="使用者名稱" min-width="150">
          <template #default="{ row }">
            <div class="user-name">
              <el-icon class="user-icon">
                <Avatar />
              </el-icon>
              <span>{{ row.userName }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column prop="email" label="電子郵件" min-width="200" show-overflow-tooltip />

        <el-table-column prop="phoneNumber" label="電話" min-width="130" />

        <el-table-column label="角色" min-width="200">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role" size="small" class="role-tag">
              {{ role }}
            </el-tag>
            <span v-if="!row.roles || row.roles.length === 0" class="text-muted">無角色</span>
          </template>
        </el-table-column>

        <el-table-column label="狀態" width="100" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.isLocked" type="danger" size="small">已鎖定</el-tag>
            <el-tag v-else type="success" size="small">正常</el-tag>
          </template>
        </el-table-column>

        <el-table-column label="操作" width="350" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link :icon="View" @click="handleView(row)">查看</el-button>
            <el-button type="primary" link :icon="Edit" @click="handleEdit(row)">編輯</el-button>
            <el-button type="warning" link :icon="Key" @click="handleResetPassword(row)">重設密碼</el-button>
            <el-button v-if="!row.isLocked" type="warning" link :icon="Lock" @click="handleLock(row)">鎖定</el-button>
            <el-button v-else type="success" link :icon="Unlock" @click="handleUnlock(row)">解鎖</el-button>
            <el-button type="danger" link :icon="Delete" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageNumber"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.totalCount"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 新增使用者對話框 -->
    <el-dialog v-model="createDialogVisible" title="新增使用者" width="600px" :close-on-click-modal="false">
      <el-form ref="createFormRef" :model="createFormData" :rules="createRules" label-width="100px">
        <el-form-item label="使用者名稱" prop="userName">
          <el-input v-model="createFormData.userName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>

        <el-form-item label="密碼" prop="password">
          <el-input v-model="createFormData.password" type="password" placeholder="請輸入密碼" show-password clearable />
        </el-form-item>

        <el-form-item label="電子郵件" prop="email">
          <el-input v-model="createFormData.email" placeholder="請輸入電子郵件" clearable />
        </el-form-item>

        <el-form-item label="電話" prop="phoneNumber">
          <el-input v-model="createFormData.phoneNumber" placeholder="請輸入電話號碼" clearable />
        </el-form-item>

        <el-form-item label="角色" prop="roles">
          <el-select v-model="createFormData.roles" multiple placeholder="請選擇角色" style="width: 100%">
            <el-option v-for="role in availableRoles" :key="role.id" :label="role.name" :value="role.name" />
          </el-select>
        </el-form-item>

        <el-form-item label="信箱已驗證">
          <el-switch v-model="createFormData.emailConfirmed" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="createDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleCreateSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 編輯使用者對話框 -->
    <el-dialog v-model="editDialogVisible" title="編輯使用者" width="600px" :close-on-click-modal="false">
      <el-form ref="editFormRef" :model="editFormData" :rules="editRules" label-width="100px">
        <el-form-item label="使用者名稱">
          <el-input :model-value="editingUser?.userName" disabled />
        </el-form-item>

        <el-form-item label="電子郵件" prop="email">
          <el-input v-model="editFormData.email" placeholder="請輸入電子郵件" clearable />
        </el-form-item>

        <el-form-item label="電話" prop="phoneNumber">
          <el-input v-model="editFormData.phoneNumber" placeholder="請輸入電話號碼" clearable />
        </el-form-item>

        <el-form-item label="角色" prop="roles">
          <el-select v-model="editFormData.roles" multiple placeholder="請選擇角色" style="width: 100%">
            <el-option v-for="role in availableRoles" :key="role.id" :label="role.name" :value="role.name" />
          </el-select>
        </el-form-item>

        <el-form-item label="信箱已驗證">
          <el-switch v-model="editFormData.emailConfirmed" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleEditSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看詳情對話框 -->
    <el-dialog v-model="viewDialogVisible" title="使用者詳情" width="600px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="使用者 ID">{{ viewUser?.id }}</el-descriptions-item>
        <el-descriptions-item label="使用者名稱">{{ viewUser?.userName }}</el-descriptions-item>
        <el-descriptions-item label="電子郵件">
          {{ viewUser?.email || '未設定' }}
          <el-tag v-if="viewUser?.emailConfirmed" type="success" size="small" style="margin-left: 8px">已驗證</el-tag>
          <el-tag v-else type="info" size="small" style="margin-left: 8px">未驗證</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="電話">
          {{ viewUser?.phoneNumber || '未設定' }}
          <el-tag v-if="viewUser?.phoneNumberConfirmed" type="success" size="small" style="margin-left: 8px">已驗證</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag v-if="viewUser?.isLocked" type="danger">已鎖定</el-tag>
          <el-tag v-else type="success">正常</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="登入失敗次數">{{ viewUser?.accessFailedCount }}</el-descriptions-item>
        <el-descriptions-item label="角色列表">
          <el-tag v-for="role in viewUser?.roles" :key="role" class="role-tag">
            {{ role }}
          </el-tag>
          <span v-if="!viewUser?.roles || viewUser.roles.length === 0" class="text-muted">無角色</span>
        </el-descriptions-item>
      </el-descriptions>

      <template #footer>
        <el-button @click="viewDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleEditFromView">編輯</el-button>
      </template>
    </el-dialog>

    <!-- 重設密碼對話框 -->
    <el-dialog v-model="resetPasswordDialogVisible" title="重設密碼" width="450px" :close-on-click-modal="false">
      <el-form ref="resetPasswordFormRef" :model="resetPasswordFormData" :rules="resetPasswordRules" label-width="80px">
        <el-form-item label="新密碼" prop="newPassword">
          <el-input v-model="resetPasswordFormData.newPassword" type="password" placeholder="請輸入新密碼" show-password clearable />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="resetPasswordDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleResetPasswordSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete, View, Avatar, Key, Lock, Unlock } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import userApi from '@/api/user'
import roleApi from '@/api/role'
import type { User, UserDetail, UserInput, UserUpdate, ResetPassword, PagedResult } from '@/api/user'
import type { Role } from '@/api/role'

// 狀態管理
const loading = ref(false)
const submitting = ref(false)
const createDialogVisible = ref(false)
const editDialogVisible = ref(false)
const viewDialogVisible = ref(false)
const resetPasswordDialogVisible = ref(false)
const users = ref<User[]>([])
const availableRoles = ref<Role[]>([])
const viewUser = ref<UserDetail | null>(null)
const editingUser = ref<User | null>(null)
const resetPasswordUser = ref<User | null>(null)
const createFormRef = ref<FormInstance>()
const editFormRef = ref<FormInstance>()
const resetPasswordFormRef = ref<FormInstance>()

// 分頁參數
const pagination = ref({
  pageNumber: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 0
})

// 新增表單資料
const createFormData = ref<UserInput>({
  userName: '',
  password: '',
  email: '',
  phoneNumber: '',
  emailConfirmed: false,
  roles: []
})

// 編輯表單資料
const editFormData = ref<UserUpdate>({
  email: '',
  phoneNumber: '',
  emailConfirmed: false,
  roles: []
})

// 重設密碼表單資料
const resetPasswordFormData = ref<ResetPassword>({
  newPassword: ''
})

// 新增表單驗證規則
const createRules: FormRules = {
  userName: [
    { required: true, message: '請輸入使用者名稱', trigger: 'blur' },
    { min: 3, max: 50, message: '使用者名稱長度應在 3 到 50 個字元之間', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '請輸入密碼', trigger: 'blur' },
    { min: 6, max: 100, message: '密碼長度應在 6 到 100 個字元之間', trigger: 'blur' }
  ],
  email: [
    { type: 'email' as const, message: '電子郵件格式不正確', trigger: 'blur' }
  ]
}

// 編輯表單驗證規則
const editRules: FormRules = {
  email: [
    { type: 'email' as const, message: '電子郵件格式不正確', trigger: 'blur' }
  ]
}

// 重設密碼驗證規則
const resetPasswordRules: FormRules = {
  newPassword: [
    { required: true, message: '請輸入新密碼', trigger: 'blur' },
    { min: 6, max: 100, message: '密碼長度應在 6 到 100 個字元之間', trigger: 'blur' }
  ]
}

// 載入使用者列表
const loadUsers = async () => {
  loading.value = true
  try {
    const response = await userApi.getUsers(pagination.value.pageNumber, pagination.value.pageSize)
    if (response.succeeded && response.data) {
      users.value = response.data.items || []
      pagination.value.totalCount = response.data.totalCount
      pagination.value.totalPages = response.data.totalPages
    } else {
      ElMessage.error(response.message || '載入使用者列表失敗')
    }
  } catch (error) {
    console.error('Load users error:', error)
    ElMessage.error('載入使用者列表失敗')
  } finally {
    loading.value = false
  }
}

// 載入可用角色
const loadAvailableRoles = async () => {
  try {
    const response = await roleApi.getRoles(1, 100) // 載入所有角色
    if (response.succeeded && response.data) {
      availableRoles.value = response.data.items || []
    }
  } catch (error) {
    console.error('Load roles error:', error)
  }
}

// 處理分頁大小變更
const handleSizeChange = (newSize: number) => {
  pagination.value.pageSize = newSize
  pagination.value.pageNumber = 1
  loadUsers()
}

// 處理頁碼變更
const handlePageChange = (newPage: number) => {
  pagination.value.pageNumber = newPage
  loadUsers()
}

// 新增使用者
const handleCreate = () => {
  createFormData.value = {
    userName: '',
    password: '',
    email: '',
    phoneNumber: '',
    emailConfirmed: false,
    roles: []
  }
  createDialogVisible.value = true
  createFormRef.value?.clearValidate()
}

// 提交新增
const handleCreateSubmit = async () => {
  if (!createFormRef.value) return

  try {
    const valid = await createFormRef.value.validate()
    if (!valid) return

    submitting.value = true

    const response = await userApi.createUser(createFormData.value)

    if (response.succeeded) {
      ElMessage.success(response.message || '使用者建立成功')
      createDialogVisible.value = false
      await loadUsers()
    } else {
      ElMessage.error(response.message || '使用者建立失敗')
    }
  } catch (error) {
    console.error('Create user error:', error)
    ElMessage.error('使用者建立失敗')
  } finally {
    submitting.value = false
  }
}

// 查看使用者
const handleView = async (user: User) => {
  try {
    const response = await userApi.getUser(user.id)
    if (response.succeeded && response.data) {
      viewUser.value = response.data
      viewDialogVisible.value = true
    } else {
      ElMessage.error(response.message || '載入使用者詳情失敗')
    }
  } catch (error) {
    console.error('View user error:', error)
    ElMessage.error('載入使用者詳情失敗')
  }
}

// 編輯使用者
const handleEdit = async (user: User) => {
  try {
    const response = await userApi.getUser(user.id)
    if (response.succeeded && response.data) {
      editingUser.value = user
      editFormData.value = {
        email: response.data.email,
        phoneNumber: response.data.phoneNumber,
        emailConfirmed: response.data.emailConfirmed,
        roles: response.data.roles
      }
      editDialogVisible.value = true
      editFormRef.value?.clearValidate()
    } else {
      ElMessage.error(response.message || '載入使用者詳情失敗')
    }
  } catch (error) {
    console.error('Edit user error:', error)
    ElMessage.error('載入使用者詳情失敗')
  }
}

// 從查看對話框進入編輯
const handleEditFromView = () => {
  if (viewUser.value) {
    editingUser.value = { 
      id: viewUser.value.id, 
      userName: viewUser.value.userName,
      email: viewUser.value.email,
      emailConfirmed: viewUser.value.emailConfirmed,
      phoneNumber: viewUser.value.phoneNumber,
      isLocked: viewUser.value.isLocked,
      roles: viewUser.value.roles
    }
    editFormData.value = {
      email: viewUser.value.email,
      phoneNumber: viewUser.value.phoneNumber,
      emailConfirmed: viewUser.value.emailConfirmed,
      roles: viewUser.value.roles
    }
    viewDialogVisible.value = false
    editDialogVisible.value = true
  }
}

// 提交編輯
const handleEditSubmit = async () => {
  if (!editFormRef.value || !editingUser.value) return

  try {
    const valid = await editFormRef.value.validate()
    if (!valid) return

    submitting.value = true

    const response = await userApi.updateUser(editingUser.value.id, editFormData.value)

    if (response.succeeded) {
      ElMessage.success(response.message || '使用者更新成功')
      editDialogVisible.value = false
      await loadUsers()
    } else {
      ElMessage.error(response.message || '使用者更新失敗')
    }
  } catch (error) {
    console.error('Update user error:', error)
    ElMessage.error('使用者更新失敗')
  } finally {
    submitting.value = false
  }
}

// 刪除使用者
const handleDelete = async (user: User) => {
  try {
    await ElMessageBox.confirm(`確定要刪除使用者「${user.userName}」嗎？`, '刪除確認', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await userApi.deleteUser(user.id)
    if (response.succeeded) {
      ElMessage.success(response.message || '使用者刪除成功')
      await loadUsers()
    } else {
      ElMessage.error(response.message || '使用者刪除失敗')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Delete user error:', error)
      ElMessage.error('使用者刪除失敗')
    }
  }
}

// 重設密碼
const handleResetPassword = (user: User) => {
  resetPasswordUser.value = user
  resetPasswordFormData.value = {
    newPassword: ''
  }
  resetPasswordDialogVisible.value = true
  resetPasswordFormRef.value?.clearValidate()
}

// 提交重設密碼
const handleResetPasswordSubmit = async () => {
  if (!resetPasswordFormRef.value || !resetPasswordUser.value) return

  try {
    const valid = await resetPasswordFormRef.value.validate()
    if (!valid) return

    submitting.value = true

    const response = await userApi.resetPassword(resetPasswordUser.value.id, resetPasswordFormData.value)

    if (response.succeeded) {
      ElMessage.success(response.message || '密碼重設成功')
      resetPasswordDialogVisible.value = false
    } else {
      ElMessage.error(response.message || '密碼重設失敗')
    }
  } catch (error) {
    console.error('Reset password error:', error)
    ElMessage.error('密碼重設失敗')
  } finally {
    submitting.value = false
  }
}

// 鎖定使用者
const handleLock = async (user: User) => {
  try {
    await ElMessageBox.confirm(`確定要鎖定使用者「${user.userName}」嗎？`, '鎖定確認', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await userApi.lockUser(user.id)
    if (response.succeeded) {
      ElMessage.success(response.message || '使用者已鎖定')
      await loadUsers()
    } else {
      ElMessage.error(response.message || '鎖定使用者失敗')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Lock user error:', error)
      ElMessage.error('鎖定使用者失敗')
    }
  }
}

// 解鎖使用者
const handleUnlock = async (user: User) => {
  try {
    const response = await userApi.unlockUser(user.id)
    if (response.succeeded) {
      ElMessage.success(response.message || '使用者已解鎖')
      await loadUsers()
    } else {
      ElMessage.error(response.message || '解鎖使用者失敗')
    }
  } catch (error) {
    console.error('Unlock user error:', error)
    ElMessage.error('解鎖使用者失敗')
  }
}

// 初始化
onMounted(() => {
  loadUsers()
  loadAvailableRoles()
})
</script>

<style scoped lang="scss">
.user-management {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header-content {
  flex: 1;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 8px 0;
}

.page-subtitle {
  font-size: 14px;
  color: #6b7280;
  margin: 0;
}

.user-list-card {
  border-radius: 8px;

  :deep(.el-card__body) {
    padding: 0;
  }
}

.user-name {
  display: flex;
  align-items: center;
  gap: 8px;
}

.user-icon {
  color: var(--el-color-primary);
  font-size: 18px;
}

.role-tag {
  margin-right: 8px;
  margin-bottom: 4px;
}

.text-muted {
  color: #909399;
  font-size: 14px;
}

:deep(.el-table) {
  .el-table__header th {
    background-color: #f5f7fa;
    color: #606266;
    font-weight: 600;
  }
}

:deep(.el-dialog) {
  border-radius: 8px;
}

:deep(.el-descriptions__label) {
  font-weight: 600;
  width: 140px;
}

:deep(.el-descriptions__content) {
  word-break: break-all;
}

.pagination-container {
  padding: 16px;
  display: flex;
  justify-content: flex-end;
  border-top: 1px solid #e4e7ed;
}
</style>
