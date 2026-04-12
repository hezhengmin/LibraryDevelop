<template>
  <div class="role-management">
    <!-- 頁面標題與操作區 -->
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">角色權限管理</h1>
        <p class="page-subtitle">管理系統角色與權限設定</p>
      </div>
      <el-button type="primary" :icon="Plus" @click="handleCreate">新增角色</el-button>
    </div>

    <!-- 角色列表 -->
    <el-card class="role-list-card" shadow="never">
      <el-table v-loading="loading" :data="roles" stripe style="width: 100%">
        <el-table-column prop="name" label="角色名稱" min-width="200">
          <template #default="{ row }">
            <div class="role-name">
              <el-icon class="role-icon">
                <User />
              </el-icon>
              <span>{{ row.name }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column prop="id" label="角色 ID" min-width="280" show-overflow-tooltip />

        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link :icon="View" @click="handleView(row)">查看</el-button>
            <el-button type="primary" link :icon="Edit" @click="handleEdit(row)">編輯</el-button>
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

    <!-- 新增/編輯對話框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="formData" :rules="rules" label-width="100px">
        <el-form-item label="角色名稱" prop="name">
          <el-input v-model="formData.name" placeholder="請輸入角色名稱" clearable />
        </el-form-item>

        <el-form-item label="角色權限">
          <div class="permissions-container">
            <el-tree
              ref="treeRef"
              :data="permissionTree"
              show-checkbox
              node-key="moduleNo"
              :props="{ label: 'label', children: 'children' }"
              :default-checked-keys="formData.permissions"
              default-expand-all
            />
          </div>
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看詳情對話框 -->
    <el-dialog v-model="viewDialogVisible" title="角色詳情" width="600px">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="角色 ID">{{ viewRole?.id }}</el-descriptions-item>
        <el-descriptions-item label="角色名稱">{{ viewRole?.name }}</el-descriptions-item>
        <el-descriptions-item label="權限列表">
          <el-tag v-for="permission in viewRole?.permissions" :key="permission.value" class="permission-tag">
            {{ getPermissionName(permission.value) }}
          </el-tag>
          <span v-if="!viewRole?.permissions || viewRole.permissions.length === 0" class="text-muted">無權限</span>
        </el-descriptions-item>
      </el-descriptions>

      <template #footer>
        <el-button @click="viewDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleEditFromView">編輯</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue'
import { ElMessage, ElMessageBox, ElTree } from 'element-plus'
import { Plus, Edit, Delete, View, User } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import roleApi from '@/api/role'
import type { Role, RoleDetail, RoleInput, PermissionTreeNode } from '@/api/role'

// 狀態管理
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const roles = ref<Role[]>([])
const permissionTree = ref<PermissionTreeNode[]>([])
const viewRole = ref<RoleDetail | null>(null)
const formRef = ref<FormInstance>()
const treeRef = ref<InstanceType<typeof ElTree>>()

// 分頁參數
const pagination = ref({
  pageNumber: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 0
})

// 表單資料
const formData = ref<RoleInput>({
  name: '',
  permissions: []
})

const editingRoleId = ref<string | null>(null)

// 對話框標題
const dialogTitle = computed(() => (editingRoleId.value ? '編輯角色' : '新增角色'))

// 從樹狀資料中遞迴找到 moduleNo 對應的名稱
const getPermissionName = (moduleNo: string): string => {
  const find = (nodes: PermissionTreeNode[]): string | null => {
    for (const node of nodes) {
      if (node.moduleNo === moduleNo) return node.label
      if (node.children?.length) {
        const found = find(node.children)
        if (found) return found
      }
    }
    return null
  }
  return find(permissionTree.value) || moduleNo
}

// 收集所有葉節點的 moduleNo（用來過濾 setCheckedKeys）
const collectLeafModuleNos = (nodes: PermissionTreeNode[]): Set<string> => {
  const leaves = new Set<string>()
  const walk = (list: PermissionTreeNode[]) => {
    for (const n of list) {
      if (!n.children || n.children.length === 0) {
        leaves.add(n.moduleNo)
      } else {
        walk(n.children)
      }
    }
  }
  walk(nodes)
  return leaves
}

// 表單驗證規則
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入角色名稱', trigger: 'blur' },
    { min: 2, max: 50, message: '角色名稱長度應在 2 到 50 個字元之間', trigger: 'blur' }
  ]
}

// 載入角色列表
const loadRoles = async () => {
  loading.value = true
  try {
    const response = await roleApi.getRoles(pagination.value.pageNumber, pagination.value.pageSize)
    if (response.succeeded && response.data) {
      roles.value = response.data.items || []
      pagination.value.totalCount = response.data.totalCount
      pagination.value.totalPages = response.data.totalPages
    } else {
      ElMessage.error(response.message || '載入角色列表失敗')
    }
  } catch (error) {
    console.error('Load roles error:', error)
    ElMessage.error('載入角色列表失敗')
  } finally {
    loading.value = false
  }
}

// 載入可用權限（樹狀結構）
const loadAvailablePermissions = async () => {
  try {
    const response = await roleApi.getAvailablePermissions()
    if (response.succeeded) {
      permissionTree.value = response.data || []
    }
  } catch (error) {
    console.error('Load permissions error:', error)
  }
}

// 處理分頁大小變更
const handleSizeChange = (newSize: number) => {
  pagination.value.pageSize = newSize
  pagination.value.pageNumber = 1
  loadRoles()
}

// 處理頁碼變更
const handlePageChange = (newPage: number) => {
  pagination.value.pageNumber = newPage
  loadRoles()
}

// 新增角色
const handleCreate = () => {
  editingRoleId.value = null
  formData.value = {
    name: '',
    permissions: []
  }
  dialogVisible.value = true
  nextTick(() => {
    formRef.value?.clearValidate()
    treeRef.value?.setCheckedKeys([])
  })
}

// 查看角色
const handleView = async (role: Role) => {
  try {
    const response = await roleApi.getRole(role.id)
    if (response.succeeded && response.data) {
      viewRole.value = {
        id: response.data.id,
        name: response.data.name,
        permissions: response.data.permissions || []
      }
      viewDialogVisible.value = true
    } else {
      ElMessage.error(response.message || '載入角色詳情失敗')
    }
  } catch (error) {
    console.error('View role error:', error)
    ElMessage.error('載入角色詳情失敗')
  }
}

// 編輯角色
const handleEdit = async (role: Role) => {
  try {
    const response = await roleApi.getRole(role.id)
    if (response.succeeded && response.data) {
      const existingPermissions = response.data.permissions?.map(p => p.value) || []
      editingRoleId.value = role.id
      formData.value = {
        name: response.data.name,
        permissions: existingPermissions
      }
      dialogVisible.value = true
      nextTick(() => {
        formRef.value?.clearValidate()
        // el-tree 的 setCheckedKeys 只設定葉節點，父節點會自動聯動
        const leaves = collectLeafModuleNos(permissionTree.value)
        const leafKeys = existingPermissions.filter(k => leaves.has(k))
        treeRef.value?.setCheckedKeys(leafKeys)
      })
    } else {
      ElMessage.error(response.message || '載入角色詳情失敗')
    }
  } catch (error) {
    console.error('Edit role error:', error)
    ElMessage.error('載入角色詳情失敗')
  }
}

// 從查看對話框進入編輯
const handleEditFromView = () => {
  if (viewRole.value) {
    const existingPermissions = viewRole.value.permissions?.map(p => p.value) || []
    editingRoleId.value = viewRole.value.id
    formData.value = {
      name: viewRole.value.name,
      permissions: existingPermissions
    }
    viewDialogVisible.value = false
    dialogVisible.value = true
    nextTick(() => {
      const leaves = collectLeafModuleNos(permissionTree.value)
      const leafKeys = existingPermissions.filter(k => leaves.has(k))
      treeRef.value?.setCheckedKeys(leafKeys)
    })
  }
}

// 刪除角色
const handleDelete = async (role: Role) => {
  try {
    await ElMessageBox.confirm(`確定要刪除角色「${role.name}」嗎？`, '刪除確認', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await roleApi.deleteRole(role.id)
    if (response.succeeded) {
      ElMessage.success(response.message || '角色刪除成功')
      await loadRoles()
    } else {
      ElMessage.error(response.message || '角色刪除失敗')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Delete role error:', error)
      ElMessage.error('角色刪除失敗')
    }
  }
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitting.value = true

    // 從 el-tree 收集勾選的權限（含半選的父節點）
    const checkedKeys = treeRef.value?.getCheckedKeys() as string[] || []
    const halfCheckedKeys = treeRef.value?.getHalfCheckedKeys() as string[] || []
    formData.value.permissions = [...checkedKeys, ...halfCheckedKeys]

    let response
    if (editingRoleId.value) {
      // 更新角色
      response = await roleApi.updateRole(editingRoleId.value, formData.value)
    } else {
      // 新增角色
      response = await roleApi.createRole(formData.value)
    }

    if (response.succeeded) {
      ElMessage.success(response.message || '操作成功')
      dialogVisible.value = false
      await loadRoles()
    } else {
      ElMessage.error(response.message || '操作失敗')
    }
  } catch (error) {
    console.error('Submit error:', error)
    ElMessage.error('操作失敗')
  } finally {
    submitting.value = false
  }
}

// 初始化
onMounted(() => {
  loadRoles()
  loadAvailablePermissions()
})
</script>

<style scoped lang="scss">
.role-management {
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

.role-list-card {
  border-radius: 8px;

  :deep(.el-card__body) {
    padding: 0;
  }
}

.role-name {
  display: flex;
  align-items: center;
  gap: 8px;
}

.role-icon {
  color: var(--el-color-primary);
  font-size: 18px;
}

.permissions-container {
  width: 100%;
  max-height: 400px;
  overflow-y: auto;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 12px;
}

.permission-tag {
  margin-right: 8px;
  margin-bottom: 8px;
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
  width: 120px;
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
