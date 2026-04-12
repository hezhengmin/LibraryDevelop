<template>
  <div class="login-container">
    <div class="login-card">
      <!-- Logo/Title Section -->
      <div class="login-header">
        <h1 class="login-title">建立帳號</h1>
        <p class="login-subtitle">請填寫以下資訊完成註冊</p>
      </div>

      <!-- Register Form -->
      <el-form ref="registerFormRef" :model="registerForm" :rules="rules" class="login-form" @submit.prevent="handleRegister">
        <el-form-item prop="userName">
          <el-input v-model="registerForm.userName" placeholder="使用者名稱" size="large" :prefix-icon="User" clearable />
        </el-form-item>

        <el-form-item prop="email">
          <el-input v-model="registerForm.email" placeholder="電子郵件" size="large" :prefix-icon="Message" clearable />
        </el-form-item>

        <el-form-item prop="password">
          <el-input v-model="registerForm.password" type="password" placeholder="密碼" size="large" :prefix-icon="Lock" show-password />
        </el-form-item>

        <el-form-item prop="confirmPassword">
          <el-input v-model="registerForm.confirmPassword" type="password" placeholder="確認密碼" size="large" :prefix-icon="Lock" show-password @keyup.enter="handleRegister" />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" size="large" class="login-button" :loading="loading" @click="handleRegister">註冊</el-button>
        </el-form-item>
      </el-form>

      <!-- Login Link -->
      <div class="register-link">
        <span class="register-text">已經有帳號？</span>
        <el-link type="primary" :underline="false" class="register-action" @click="router.push('/login')">立即登入</el-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock, Message } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import { accountApi } from '@/api/account'

const router = useRouter()
const registerFormRef = ref<FormInstance>()
const loading = ref(false)

interface RegisterForm {
  userName: string
  email: string
  password: string
  confirmPassword: string
}

const registerForm = reactive<RegisterForm>({
  userName: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const rules = reactive<FormRules<RegisterForm>>({
  userName: [
    { required: true, message: '請輸入使用者名稱', trigger: 'blur' },
    { min: 3, max: 20, message: '使用者名稱長度應在 3 到 20 個字元之間', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '請輸入電子郵件', trigger: 'blur' },
    { type: 'email', message: '電子郵件格式不正確', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '請輸入密碼', trigger: 'blur' },
    { min: 6, message: '密碼長度至少為 6 個字元', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '請確認密碼', trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value !== registerForm.password) {
          callback(new Error('密碼與確認密碼不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
})

const handleRegister = async () => {
  if (!registerFormRef.value) return

  try {
    const valid = await registerFormRef.value.validate()
    if (!valid) return

    loading.value = true
    const result = await accountApi.register({
      userName: registerForm.userName,
      email: registerForm.email,
      password: registerForm.password,
      confirmPassword: registerForm.confirmPassword
    })

    if (result.succeeded) {
      ElMessage.success(result.message || '註冊成功，請至信箱完成電子郵件驗證')
      router.push('/login')
    } else {
      const errorMsg = result.errors?.join('；') || '註冊失敗'
      ElMessage.error(errorMsg)
    }
  } catch (error: any) {
    const msg = error?.response?.data?.errors?.join('；') || '註冊失敗，請稍後再試'
    ElMessage.error(msg)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
// 變數定義 (same as LoginView)
$border-color-base: #dcdfe6;
$border-color-hover: #c0c4cc;
$spacing-base: 24px;
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
  max-width: 420px;
  background: #ffffff;
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(255, 255, 255, 0.1) inset;
  border: 2px solid rgba(102, 126, 234, 0.2);
  transform-style: preserve-3d;
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
  margin-bottom: 32px;
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

.login-form {
  :deep(.el-form-item) {
    margin-bottom: $spacing-base;
  }

  :deep(.el-input__wrapper) {
    box-shadow: 0 0 0 1px $border-color-base inset;
    transition: $transition-base;
    border-radius: 8px;
    padding: 12px 16px;

    &:hover {
      box-shadow: 0 0 0 1px $border-color-hover inset;
    }

    &.is-focus {
      box-shadow: 0 0 0 2px var(--el-color-primary) inset !important;
    }
  }

  :deep(.el-input__inner) {
    font-size: 15px;
  }
}

.login-button {
  width: 100%;
  height: 44px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 8px;
  transition: $transition-base;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
  }

  &:active {
    transform: translateY(0);
  }
}

.register-link {
  text-align: center;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #e5e7eb;
}

.register-text {
  color: #6b7280;
  font-size: 14px;
}

.register-action {
  margin-left: 8px;
  font-weight: 600;
}
</style>
