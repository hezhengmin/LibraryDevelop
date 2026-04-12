<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h1 class="login-title">電子郵件驗證</h1>
        <p class="login-subtitle">{{ statusMessage }}</p>
      </div>

      <div class="verify-status">
        <el-result v-if="status === 'loading'" icon="info" title="驗證中..." sub-title="正在驗證您的電子郵件，請稍候..." />
        <el-result v-else-if="status === 'success'" icon="success" title="驗證成功" :sub-title="resultMessage">
          <template #extra>
            <el-button type="primary" size="large" @click="router.push('/login')">前往登入</el-button>
          </template>
        </el-result>
        <el-result v-else icon="error" title="驗證失敗" :sub-title="resultMessage">
          <template #extra>
            <el-button type="primary" size="large" @click="router.push('/login')">返回登入</el-button>
          </template>
        </el-result>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { accountApi } from '@/api/account'

const router = useRouter()
const route = useRoute()

const status = ref<'loading' | 'success' | 'error'>('loading')
const statusMessage = ref('正在處理驗證請求...')
const resultMessage = ref('')

onMounted(async () => {
  const userId = route.query.userId as string
  const token = route.query.token as string

  if (!userId || !token) {
    status.value = 'error'
    statusMessage.value = '驗證失敗'
    resultMessage.value = '驗證連結無效，請確認連結是否正確'
    return
  }

  try {
    const result = await accountApi.confirmEmail(userId, token)

    if (result.succeeded) {
      status.value = 'success'
      statusMessage.value = '驗證完成'
      resultMessage.value = result.message || '電子郵件驗證成功，請前往登入'
    } else {
      status.value = 'error'
      statusMessage.value = '驗證失敗'
      resultMessage.value = result.errors?.join('；') || '驗證連結無效或已過期'
    }
  } catch (error: any) {
    status.value = 'error'
    statusMessage.value = '驗證失敗'
    resultMessage.value = error?.response?.data?.errors?.join('；') || '驗證過程發生錯誤，請稍後再試'
  }
})
</script>

<style scoped lang="scss">
$transition-base: all 0.3s ease;

.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  position: relative;
  width: 100%;
  max-width: 480px;
  background: #ffffff;
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(255, 255, 255, 0.1) inset;
  border: 2px solid rgba(102, 126, 234, 0.2);
  transition: $transition-base;

  &:hover {
    border-color: rgba(102, 126, 234, 0.4);
    box-shadow: 0 25px 70px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(255, 255, 255, 0.2) inset;
    transform: translateY(-5px);
  }

  @media (max-width: 640px) {
    padding: 30px 24px;
    max-width: 100%;
  }
}

.login-header {
  text-align: center;
  margin-bottom: 16px;
}

.login-title {
  font-size: 28px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 8px;
  letter-spacing: -0.5px;
}

.login-subtitle {
  font-size: 15px;
  color: #6b7280;
  font-weight: 400;
}

.verify-status {
  :deep(.el-result) {
    padding: 20px 0;
  }
}
</style>
