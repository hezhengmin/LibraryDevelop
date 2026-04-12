<template>
  <div class="sys-menu-management">
    <!-- 頁面標題與操作區 -->
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">系統選單管理</h1>
        <p class="page-subtitle">管理系統選單結構與權限設定</p>
      </div>
      <div class="header-actions">
        <el-button type="default" :icon="RefreshRight" @click="loadMenuTree">重新整理</el-button>
        <el-button type="primary" :icon="Plus" @click="handleCreate">新增根選單</el-button>
      </div>
    </div>

    <!-- 搜尋與篩選區 -->
    <el-card class="filter-card" shadow="never">
      <el-form :inline="true" :model="filterParams" class="filter-form">
        <el-form-item label="搜尋">
          <el-input
            v-model="filterParams.searchTerm"
            placeholder="選單名稱或編號"
            clearable
            style="width: 200px"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select
            v-model="filterParams.status"
            placeholder="全部"
            clearable
            style="width: 120px"
          >
            <el-option label="全部" :value="undefined" />
            <el-option label="啟用" :value="true" />
            <el-option label="停用" :value="false" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button :icon="RefreshLeft" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 選單樹狀列表 -->
    <el-card class="menu-tree-card" shadow="never">
      <div v-loading="loading" class="tree-container">
        <el-tree
          ref="treeRef"
          :data="filteredMenus"
          node-key="id"
          :props="treeProps"
          :expand-on-click-node="false"
          :default-expand-all="true"
          :filter-node-method="filterNode"
          class="menu-tree"
        >
          <template #default="{ node, data }">
            <div class="custom-tree-node">
              <div class="node-content">
                <el-icon class="node-icon" :style="{ color: data.status ? '#409eff' : '#909399' }">
                  <component :is="data.icon || 'Menu'" />
                </el-icon>
                <span class="node-label">{{ data.name }}</span>
                <el-tag v-if="!data.status" type="danger" size="small" class="node-tag">停用</el-tag>
                <el-tag v-if="data.hidden" type="warning" size="small" class="node-tag">隱藏</el-tag>
                <el-tag v-if="data.isMenu" type="primary" size="small" class="node-tag">選單</el-tag>
                <el-tag v-else type="info" size="small" class="node-tag">按鈕</el-tag>
                <span class="node-info">{{ data.moduleNo }}</span>
              </div>
              <div class="node-actions">
                <el-button type="primary" link :icon="View" size="small" @click.stop="handleView(data)">
                  查看
                </el-button>
                <el-button type="primary" link :icon="Edit" size="small" @click.stop="handleEdit(data)">
                  編輯
                </el-button>
                <el-dropdown trigger="click" @command="(command: string) => handleCommand(command, data)">
                  <el-button type="primary" link size="small" @click.stop>
                    更多<el-icon class="el-icon--right"><ArrowDown /></el-icon>
                  </el-button>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item :icon="Plus" command="addChild">新增子選單</el-dropdown-item>
                      <el-dropdown-item :icon="Delete" command="delete" divided>刪除</el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
              </div>
            </div>
          </template>
        </el-tree>

        <!-- 空狀態 -->
        <el-empty v-if="!loading && filteredMenus.length === 0" description="暫無選單資料">
          <el-button type="primary" :icon="Plus" @click="handleCreate">新增第一個選單</el-button>
        </el-empty>
      </div>

      <!-- 統計資訊 -->
      <div class="menu-stats">
        <span>總計：{{ totalMenuCount }} 個選單</span>
        <span v-if="filterParams.searchTerm || filterParams.status !== undefined">
          （篩選結果：{{ filteredMenuCount }} 個）
        </span>
      </div>
    </el-card>

    <!-- 新增/編輯選單對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="700px"
      :close-on-click-modal="false"
    >
      <el-form ref="formRef" :model="formData" :rules="rules" label-width="110px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="選單名稱" prop="name">
              <el-input v-model="formData.name" placeholder="請輸入選單名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="選單編號" prop="moduleNo">
              <el-input v-model="formData.moduleNo" placeholder="如：SYS001" clearable />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="父選單" prop="parentModuleId">
              <el-tree-select
                v-model="formData.parentModuleId"
                :data="parentMenuOptions"
                :props="{ value: 'id', label: 'name', children: 'children' }"
                placeholder="請選擇父選單"
                clearable
                filterable
                check-strictly
                :render-after-expand="false"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="層級" prop="levels">
              <el-input-number
                v-model="formData.levels"
                :min="1"
                :max="10"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="路由路徑" prop="url">
              <el-input v-model="formData.url" placeholder="如：/system/menu" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="元件路徑" prop="component">
              <el-input v-model="formData.component" placeholder="如：SystemMenuView" clearable />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="圖示" prop="icon">
              <el-input v-model="formData.icon" placeholder="如：Menu" clearable>
                <template #prefix>
                  <el-icon v-if="formData.icon">
                    <component :is="formData.icon" />
                  </el-icon>
                </template>
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="排序順序" prop="sortOrder">
              <el-input-number
                v-model="formData.sortOrder"
                :min="0"
                :max="9999"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="類型" prop="isMenu">
              <el-radio-group v-model="formData.isMenu">
                <el-radio :value="true">選單</el-radio>
                <el-radio :value="false">按鈕</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="status">
              <el-switch
                v-model="formData.status"
                active-text="啟用"
                inactive-text="停用"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="是否隱藏">
              <el-switch
                v-model="formData.hidden"
                active-text="隱藏"
                inactive-text="顯示"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="預設展開">
              <el-switch
                v-model="formData.isOpen"
                active-text="展開"
                inactive-text="摺疊"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="提示文字" prop="tips">
          <el-input v-model="formData.tips" placeholder="滑鼠懸停時顯示的提示文字" clearable />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 查看詳情對話框 -->
    <el-dialog v-model="viewDialogVisible" title="選單詳情" width="700px">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="選單 ID">{{ viewMenu?.id }}</el-descriptions-item>
        <el-descriptions-item label="父選單 ID">
          {{ viewMenu?.parentModuleId === 0 ? '根選單' : viewMenu?.parentModuleId }}
        </el-descriptions-item>
        <el-descriptions-item label="選單名稱">{{ viewMenu?.name }}</el-descriptions-item>
        <el-descriptions-item label="選單編號">{{ viewMenu?.moduleNo }}</el-descriptions-item>
        <el-descriptions-item label="路由路徑">{{ viewMenu?.url || '-' }}</el-descriptions-item>
        <el-descriptions-item label="元件路徑">{{ viewMenu?.component || '-' }}</el-descriptions-item>
        <el-descriptions-item label="圖示">
          <el-icon v-if="viewMenu?.icon">
            <component :is="viewMenu.icon" />
          </el-icon>
          <span v-else>-</span>
        </el-descriptions-item>
        <el-descriptions-item label="層級">{{ viewMenu?.levels }}</el-descriptions-item>
        <el-descriptions-item label="排序順序">{{ viewMenu?.sortOrder }}</el-descriptions-item>
        <el-descriptions-item label="類型">
          <el-tag v-if="viewMenu?.isMenu" type="primary" size="small">選單</el-tag>
          <el-tag v-else type="info" size="small">按鈕</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="狀態">
          <el-tag v-if="viewMenu?.status" type="success" size="small">啟用</el-tag>
          <el-tag v-else type="danger" size="small">停用</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="是否隱藏">
          <el-tag v-if="viewMenu?.hidden" type="warning" size="small">隱藏</el-tag>
          <el-tag v-else type="success" size="small">顯示</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="預設展開">
          <el-tag v-if="viewMenu?.isOpen" type="success" size="small">展開</el-tag>
          <el-tag v-else type="info" size="small">摺疊</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="提示文字" :span="2">{{ viewMenu?.tips || '-' }}</el-descriptions-item>
        <el-descriptions-item label="建立時間">{{ formatDate(viewMenu?.createdAt) }}</el-descriptions-item>
        <el-descriptions-item label="建立者">{{ viewMenu?.createdBy }}</el-descriptions-item>
        <el-descriptions-item label="修改時間">{{ formatDate(viewMenu?.updatedAt) }}</el-descriptions-item>
        <el-descriptions-item label="修改者">{{ viewMenu?.updatedBy || '-' }}</el-descriptions-item>
      </el-descriptions>

      <template #footer>
        <el-button @click="viewDialogVisible = false">關閉</el-button>
        <el-button type="primary" @click="handleEditFromView">編輯</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  Plus,
  Edit,
  Delete,
  View,
  Menu,
  Search,
  RefreshLeft,
  RefreshRight,
  Hide,
  ArrowDown
} from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import sysMenuApi from '@/api/sysMenu'
import type { SysMenu, SysMenuCreateInput, SysMenuUpdateInput } from '@/api/sysMenu'

// 狀態管理
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const menus = ref<SysMenu[]>([]) // 完整選單樹
const viewMenu = ref<SysMenu | null>(null)
const formRef = ref<FormInstance>()
const treeRef = ref()

// 篩選參數
const filterParams = ref({
  searchTerm: '',
  status: undefined as boolean | undefined
})

// Tree 組件配置
const treeProps = {
  children: 'children',
  label: 'name'
}

// 表單資料
const formData = ref<SysMenuCreateInput>({
  parentModuleId: 0,
  moduleNo: '',
  component: '',
  hidden: false,
  icon: '',
  isMenu: true,
  isOpen: false,
  levels: 1,
  name: '',
  sortOrder: 0,
  status: true,
  tips: '',
  url: ''
})

const editingMenuId = ref<number | null>(null)

// 對話框標題
const dialogTitle = computed(() => (editingMenuId.value ? '編輯選單' : '新增選單'))

// 表單驗證規則
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入選單名稱', trigger: 'blur' },
    { min: 2, max: 64, message: '選單名稱長度應在 2 到 64 個字元之間', trigger: 'blur' }
  ],
  moduleNo: [
    { required: true, message: '請輸入選單編號', trigger: 'blur' },
    { max: 32, message: '選單編號長度不可超過 32 個字元', trigger: 'blur' }
  ],
  parentModuleId: [{ required: true, message: '請選擇父選單', trigger: 'change' }],
  levels: [{ required: true, message: '請輸入層級', trigger: 'blur' }],
  sortOrder: [{ required: true, message: '請輸入排序順序', trigger: 'blur' }],
  isMenu: [{ required: true, message: '請選擇類型', trigger: 'change' }],
  status: [{ required: true, message: '請設定狀態', trigger: 'change' }]
}

// 格式化日期
const formatDate = (date?: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-TW')
}

// 遞迴計算選單總數
const countMenus = (menuList: SysMenu[]): number => {
  let count = menuList.length
  menuList.forEach(menu => {
    if (menu.children && menu.children.length > 0) {
      count += countMenus(menu.children)
    }
  })
  return count
}

// 總選單數
const totalMenuCount = computed(() => countMenus(menus.value))

// 遞迴篩選選單
const filterMenuTree = (menuList: SysMenu[], searchTerm: string, status?: boolean): SysMenu[] => {
  return menuList
    .map(menu => {
      const menuCopy = { ...menu }
      
      if (menuCopy.children && menuCopy.children.length > 0) {
        menuCopy.children = filterMenuTree(menuCopy.children, searchTerm, status)
      } else {
        menuCopy.children = []
      }
      
      const matchesSearch = !searchTerm || 
        menu.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        menu.moduleNo.toLowerCase().includes(searchTerm.toLowerCase())
      
      const matchesStatus = status === undefined || menu.status === status
      
      if ((matchesSearch && matchesStatus) || menuCopy.children.length > 0) {
        return menuCopy
      }
      
      return null
    })
    .filter(menu => menu !== null) as SysMenu[]
}

// 篩選後的選單
const filteredMenus = computed(() => {
  return filterMenuTree(
    menus.value,
    filterParams.value.searchTerm,
    filterParams.value.status
  )
})

// 篩選後的選單數量
const filteredMenuCount = computed(() => countMenus(filteredMenus.value))

// Tree 過濾方法
const filterNode = (value: string, data: SysMenu) => {
  if (!value) return true
  return data.name.toLowerCase().includes(value.toLowerCase()) ||
         data.moduleNo.toLowerCase().includes(value.toLowerCase())
}

// 監聽搜尋詞變化，觸發樹過濾
watch(() => filterParams.value.searchTerm, (val) => {
  if (treeRef.value) {
    treeRef.value.filter(val)
  }
})

// 父選單選項（包含根選單選項並過濾當前編輯的選單）
const parentMenuOptions = computed(() => {
  const rootOption = {
    id: 0,
    name: '根選單',
    children: [] as SysMenu[]
  }
  
  // 移除當前編輯的選單（防止選擇自己為父選單）
  const filterCurrentMenu = (menuList: SysMenu[]): SysMenu[] => {
    return menuList
      .filter(menu => menu.id !== editingMenuId.value)
      .map(menu => ({
        ...menu,
        children: menu.children ? filterCurrentMenu(menu.children) : []
      }))
  }
  
  rootOption.children = filterCurrentMenu(menus.value)
  return [rootOption]
})

// 建立選單樹結構
const buildMenuTree = (menuList: SysMenu[]): SysMenu[] => {
  const menuMap = new Map<number, SysMenu & { children: SysMenu[] }>()
  const rootMenus: SysMenu[] = []
  
  menuList.forEach(menu => {
    menuMap.set(menu.id, { ...menu, children: [] })
  })
  
  menuList.forEach(menu => {
    const currentMenu = menuMap.get(menu.id)!
    if (menu.parentModuleId === 0) {
      rootMenus.push(currentMenu)
    } else {
      const parentMenu = menuMap.get(menu.parentModuleId)
      if (parentMenu) {
        if (!parentMenu.children) {
          parentMenu.children = []
        }
        parentMenu.children.push(currentMenu)
      } else {
        rootMenus.push(currentMenu)
      }
    }
  })
  
  const sortMenus = (menuList: SysMenu[]): SysMenu[] => {
    return menuList
      .sort((a, b) => a.sortOrder - b.sortOrder)
      .map(menu => ({
        ...menu,
        children: menu.children && menu.children.length > 0 ? sortMenus(menu.children) : []
      }))
  }
  
  return sortMenus(rootMenus)
}

// 載入選單樹
const loadMenuTree = async () => {
  loading.value = true
  try {
    const response = await sysMenuApi.getMenuTree()
    if (response.succeeded && response.data) {
      if (response.data.length > 0 && response.data[0].children !== undefined) {
        menus.value = response.data
      } else {
        menus.value = buildMenuTree(response.data)
      }
    } else {
      ElMessage.error(response.message || '載入選單列表失敗')
    }
  } catch (error: any) {
    ElMessage.error(error.message || '載入選單列表失敗')
  } finally {
    loading.value = false
  }
}

// 重置篩選
const handleReset = () => {
  filterParams.value = {
    searchTerm: '',
    status: undefined
  }
}

// 新增選單
const handleCreate = () => {
  editingMenuId.value = null
  formData.value = {
    parentModuleId: 0,
    moduleNo: '',
    component: '',
    hidden: false,
    icon: '',
    isMenu: true,
    isOpen: false,
    levels: 1,
    name: '',
    sortOrder: 0,
    status: true,
    tips: '',
    url: ''
  }
  dialogVisible.value = true
}

// 新增子選單
const handleCreateChild = (row: SysMenu) => {
  editingMenuId.value = null
  formData.value = {
    parentModuleId: row.id,
    moduleNo: '',
    component: '',
    hidden: false,
    icon: '',
    isMenu: true,
    isOpen: false,
    levels: row.levels + 1,
    name: '',
    sortOrder: 0,
    status: true,
    tips: '',
    url: ''
  }
  dialogVisible.value = true
}

// 編輯選單
const handleEdit = (row: SysMenu) => {
  editingMenuId.value = row.id
  formData.value = {
    parentModuleId: row.parentModuleId,
    moduleNo: row.moduleNo,
    component: row.component || '',
    hidden: row.hidden || false,
    icon: row.icon || '',
    isMenu: row.isMenu,
    isOpen: row.isOpen || false,
    levels: row.levels,
    name: row.name,
    sortOrder: row.sortOrder,
    status: row.status,
    tips: row.tips || '',
    url: row.url || ''
  }
  dialogVisible.value = true
}

// 查看詳情
const handleView = (row: SysMenu) => {
  viewMenu.value = row
  viewDialogVisible.value = true
}

// 從查看詳情轉到編輯
const handleEditFromView = () => {
  viewDialogVisible.value = false
  if (viewMenu.value) {
    handleEdit(viewMenu.value)
  }
}

// 提交表單
const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    submitting.value = true
    try {
      if (editingMenuId.value) {
        const updateData: SysMenuUpdateInput = {
          ...formData.value
        }
        const response = await sysMenuApi.updateSysMenu(editingMenuId.value, updateData)
        if (response.succeeded) {
          ElMessage.success('選單更新成功')
          dialogVisible.value = false
          loadMenuTree()
        } else {
          ElMessage.error(response.message || '選單更新失敗')
        }
      } else {
        const response = await sysMenuApi.createSysMenu(formData.value)
        if (response.succeeded) {
          ElMessage.success('選單新增成功')
          dialogVisible.value = false
          loadMenuTree()
        } else {
          ElMessage.error(response.message || '選單新增失敗')
        }
      }
    } catch (error: any) {
      ElMessage.error(error.message || '操作失敗')
    } finally {
      submitting.value = false
    }
  })
}

// 刪除選單
const handleDelete = async (row: SysMenu) => {
  try {
    await ElMessageBox.confirm(`確定要刪除選單「${row.name}」嗎？此操作不可復原。`, '確認刪除', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    loading.value = true
    const response = await sysMenuApi.deleteSysMenu(row.id)
    if (response.succeeded) {
      ElMessage.success('選單刪除成功')
      loadMenuTree()
    } else {
      ElMessage.error(response.message || '選單刪除失敗')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '選單刪除失敗')
    }
  } finally {
    loading.value = false
  }
}

// 處理下拉選單命令
const handleCommand = (command: string, row: SysMenu) => {
  if (command === 'addChild') {
    handleCreateChild(row)
  } else if (command === 'delete') {
    handleDelete(row)
  }
}

// 初始化
onMounted(() => {
  loadMenuTree()
})
</script>

<style scoped lang="scss">
.sys-menu-management {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;

  .header-content {
    .page-title {
      font-size: 24px;
      font-weight: 600;
      color: #303133;
      margin: 0 0 8px 0;
    }

    .page-subtitle {
      font-size: 14px;
      color: #909399;
      margin: 0;
    }
  }

  .header-actions {
    display: flex;
    gap: 10px;
  }
}

.filter-card {
  margin-bottom: 20px;

  .filter-form {
    margin: 0;
  }
}

.menu-tree-card {
  .tree-container {
    min-height: 400px;
    
    .menu-tree {
      margin-top: 10px;
      
      :deep(.el-tree-node) {
        margin-bottom: 4px; // 增加節點之間的間距
      }
      
      :deep(.el-tree-node__content) {
        height: auto;
        min-height: 56px; // 增加最小高度從 48px 到 56px
        padding: 10px 0; // 增加上下 padding 從 8px 到 10px
        margin-bottom: 2px; // 增加底部間距
        background-color: #f5f7fa; // 改回較淺的灰色背景
        border: 1px solid #e4e7ed; // 添加細外框
        border-radius: 4px; // 添加圓角
        transition: all 0.3s ease; // 添加過渡動畫

        &:hover {
          background-color: #ecf5ff; // 懸停時背景色改為淺藍色
          border-color: #c6e2ff; // 懸停時外框顏色也變化
          box-shadow: 0 2px 4px rgba(0, 0, 0, 0.08); // 添加輕微陰影
        }
      }

      // 交替背景色（斑馬紋效果）
      :deep(.el-tree-node:nth-child(even) .el-tree-node__content) {
        background-color: #fafafa; // 偶數行使用更淺的灰色背景
        border-color: #e4e7ed; // 保持外框顏色一致

        &:hover {
          background-color: #ecf5ff;
          border-color: #c6e2ff;
        }
      }

      :deep(.el-tree-node__children) {
        margin-top: 4px; // 子節點與父節點之間的間距

        // 子節點也有背景色和外框
        .el-tree-node__content {
          background-color: #f5f7fa;
          border: 1px solid #e4e7ed;

          &:hover {
            background-color: #ecf5ff;
            border-color: #c6e2ff;
          }
        }

        .el-tree-node:nth-child(even) .el-tree-node__content {
          background-color: #fafafa; // 子節點偶數行也使用淺色
          border-color: #e4e7ed;

          &:hover {
            background-color: #ecf5ff;
            border-color: #c6e2ff;
          }
        }
      }
      
      // 擴展/收合圖示放大
      :deep(.el-tree-node__expand-icon) {
        font-size: 18px; // 從預設的 12px 放大到 18px
        padding: 8px; // 增加點擊區域
      }
      
      .custom-tree-node {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 4px 12px; // 增加內部 padding
        min-height: 40px; // 確保最小高度
        
        .node-content {
          display: flex;
          align-items: center;
          gap: 8px;
          flex: 1;
          flex-wrap: wrap; // 允許換行，避免重疊
          padding: 4px 0; // 增加上下間距
          
          .node-icon {
            font-size: 18px;
            flex-shrink: 0; // 防止圖示被壓縮
          }
          
          .node-label {
            font-size: 14px;
            font-weight: 500;
            color: #303133;
            white-space: nowrap; // 防止文字換行
            flex-shrink: 0;
          }
          
          .node-tag {
            margin-left: 4px;
            flex-shrink: 0;
          }
          
          .node-info {
            margin-left: 8px;
            font-size: 12px;
            color: #909399;
            white-space: nowrap;
            flex-shrink: 0;
          }
        }
        
        .node-actions {
          display: flex; // 始終顯示，不再用 none
          align-items: center;
          gap: 8px; // 增加間距從 4px 到 8px
          flex-shrink: 0; // 防止操作按鈕被壓縮
          margin-left: 12px;
          
          // 確保按鈕在同一行
          .el-button {
            white-space: nowrap;
          }
          
          :deep(.el-dropdown) {
            .el-button {
              padding: 5px 8px;
            }
          }
        }
      }
      
      // 移除 hover 顯示的樣式，因為現在始終顯示
      // :deep(.el-tree-node__content):hover {
      //   .custom-tree-node .node-actions {
      //     display: flex;
      //   }
      // }
    }
  }

  .menu-stats {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 20px;
    padding: 10px 0;
    border-top: 1px solid #e4e7ed;
    color: #606266;
    font-size: 14px;
  }
}

.text-warning {
  color: #e6a23c;
}

.text-success {
  color: #67c23a;
}

.text-muted {
  color: #909399;
}
</style>
