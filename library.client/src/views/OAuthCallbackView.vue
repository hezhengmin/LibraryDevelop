<template>
  <div class="oauth-callback-container">
    <div class="callback-card">
      <div v-if="error" class="callback-error">
        <el-result icon="error" :title="error" sub-title="請返回登入頁面重試">
          <template #extra>
            <el-button type="primary" @click="router.push('/login')">返回登入</el-button>
          </template>
        </el-result>
      </div>
      <div v-else class="callback-loading">
        <el-result icon="success" title="登入成功" sub-title="正在為您導向系統...">
          <template #extra>
            <div class="loading-spinner">
              <el-icon class="is-loading" :size="24"><Loading /></el-icon>
            </div>
          </template>
        </el-result>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { Loading } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { ElMessage } from 'element-plus'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const error = ref<string | null>(null)

onMounted(() => {
  const query = route.query

  if (query.error) {
    error.value = query.error as string
    ElMessage.error(error.value)
    return
  }

  const accessToken = query.accessToken as string
  const refreshToken = query.refreshToken as string
  const userName = query.userName as string

  if (!accessToken || !refreshToken || !userName) {
    error.value = '登入資訊不完整，請重新登入'
    return
  }

  // Store tokens (same as regular login flow)
  authStore.setTokensFromOAuth(accessToken, refreshToken, userName)
  ElMessage.success('社群登入成功')
  router.replace('/')
})
</script>

<style scoped lang="scss">
.oauth-callback-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.callback-card {
  background: #ffffff;
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.loading-spinner {
  display: flex;
  justify-content: center;
  margin-top: 8px;
}
</style>
