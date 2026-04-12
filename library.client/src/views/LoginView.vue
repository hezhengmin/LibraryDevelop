<template>
  <div class="login-container">
    <el-form ref="loginFormRef" :model="loginForm" :rules="rules" class="login-form" @submit.prevent="handleLogin">

      <div class="title-container">
        <h3 class="title">圖書館管理系統</h3>
        <p class="subtitle">請登入您的帳號</p>
      </div>

      <el-form-item prop="username">
        <span class="svg-container">
          <el-icon><User /></el-icon>
        </span>
        <el-input
          v-model="loginForm.username"
          placeholder="使用者名稱"
          name="username"
          type="text"
          tabindex="1"
          autocomplete="on"
          ref="usernameInputRef"
          @keyup.enter="handleLogin"
        />
      </el-form-item>

      <el-form-item prop="password">
        <span class="svg-container">
          <el-icon><Lock /></el-icon>
        </span>
        <el-input
          v-model="loginForm.password"
          :type="passwordType"
          placeholder="密碼"
          name="password"
          tabindex="2"
          autocomplete="on"
          @keyup.enter="handleLogin"
        />
        <span class="show-pwd" @click="togglePassword">
          <el-icon>
            <View v-if="passwordType === 'password'" />
            <Hide v-else />
          </el-icon>
        </span>
      </el-form-item>

      <div class="form-options">
        <el-checkbox v-model="loginForm.remember">記住我</el-checkbox>
      </div>

      <el-button :loading="authStore.loading" type="primary" style="width: 100%; margin-bottom: 30px" @click.prevent="handleLogin">
        登入
      </el-button>

      <!-- Social Login -->
      <div class="social-divider">
        <span class="divider-text">或使用社群帳號登入</span>
      </div>

      <div class="social-login-buttons">
        <el-button class="social-btn google-btn" :loading="socialLoading === 'Google'" :disabled="!!socialLoading" @click="handleSocialLogin('Google')">
          <svg class="social-icon" viewBox="0 0 24 24" width="18" height="18">
            <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92a5.06 5.06 0 0 1-2.2 3.32v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.1z"/>
            <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
            <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
            <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
          </svg>
          <span>Google</span>
        </el-button>
        <el-button class="social-btn facebook-btn" :loading="socialLoading === 'Facebook'" :disabled="!!socialLoading" @click="handleSocialLogin('Facebook')">
          <svg class="social-icon" viewBox="0 0 24 24" width="18" height="18">
            <path fill="#fff" d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
          </svg>
          <span>Facebook</span>
        </el-button>
      </div>

      <!-- Email Verification Notice -->
      <div v-if="showVerificationNotice" class="verification-notice">
        <el-alert title="請先完成電子郵件驗證" type="warning" :closable="false" show-icon>
          <template #default>
            <div class="verification-content">
              <p>您的帳號尚未完成電子郵件驗證，請至信箱查收驗證信。</p>
              <div class="resend-section">
                <el-input v-model="resendEmail" placeholder="輸入註冊時的電子郵件" size="default" clearable class="resend-input" />
                <el-button type="primary" size="default" :loading="resending" :disabled="resendCooldown > 0" @click="handleResend">
                  {{ resendCooldown > 0 ? `${resendCooldown}s 後可重發` : '重新發送' }}
                </el-button>
              </div>
            </div>
          </template>
        </el-alert>
      </div>

      <!-- Register Link -->
      <div class="register-link">
        <span>還沒有帳號？</span>
        <a @click.prevent="router.push('/register')">立即註冊</a>
      </div>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted, nextTick } from "vue";
import { useRouter } from "vue-router";
import { ElMessage } from "element-plus";
import { User, Lock, View, Hide } from "@element-plus/icons-vue";
import type { FormInstance, FormRules } from "element-plus";
import { useAuthStore } from "@/stores/auth";
import { accountApi } from "@/api/account";

const router = useRouter();
const authStore = useAuthStore();
const loginFormRef = ref<FormInstance>();
// 自動聚焦使用者名稱欄位
const usernameInputRef = ref<any>();

const showVerificationNotice = ref(false);
const resendEmail = ref("");
const resending = ref(false);
const resendCooldown = ref(0);
const socialLoading = ref<string | null>(null);
const passwordType = ref("password");

const togglePassword = () => {
  passwordType.value = passwordType.value === "password" ? "" : "password";
};

interface LoginForm {
  username: string;
  password: string;
  remember: boolean;
}

const loginForm = reactive<LoginForm>({
  username: "",
  password: "",
  remember: false,
});
// 自動聚焦於使用者名稱欄位
onMounted(() => {
  nextTick(() => {
    usernameInputRef.value?.focus && usernameInputRef.value.focus();
  });
});

const rules = reactive<FormRules<LoginForm>>({
  username: [
    { required: true, message: "請輸入使用者名稱", trigger: "blur" },
    { min: 3, max: 20, message: "使用者名稱長度應在 3 到 20 個字元之間", trigger: "blur" },
  ],
  password: [
    { required: true, message: "請輸入密碼", trigger: "blur" },
    { min: 6, message: "密碼長度至少為 6 個字元", trigger: "blur" },
  ],
});

const handleLogin = async () => {
  if (!loginFormRef.value) return;
  try {
    const valid = await loginFormRef.value.validate();
    if (valid) {
      showVerificationNotice.value = false;
      const result = await authStore.login({
        username: loginForm.username,
        password: loginForm.password,
        remember: loginForm.remember,
      });
      if (result.success) {
        router.push("/");
      } else if (result.error?.response?.data?.data?.requiresEmailVerification) {
        showVerificationNotice.value = true;
      }
    }
  } catch (error: any) {
    // 若有表單驗證錯誤，Element Plus 會自動顯示
    if (error && error.errors) return;
    ElMessage.error("請填寫完整的登入資訊");
  }
};

const handleSocialLogin = (provider: string) => {
  socialLoading.value = provider;
  setTimeout(() => {
    window.location.href = `/api/ExternalLogin/Challenge/${provider}`;
  }, 300); // 微延遲以顯示 loading 狀態
};

const handleResend = async () => {
  if (!resendEmail.value) {
    ElMessage.warning("請輸入電子郵件");
    return;
  }

  resending.value = true;
  try {
    const result = await accountApi.resendVerification(resendEmail.value);
    if (result.succeeded) {
      ElMessage.success(result.message || "若該信箱已註冊，驗證信已發送");
      resendCooldown.value = 60;
      const timer = setInterval(() => {
        resendCooldown.value--;
        if (resendCooldown.value <= 0) clearInterval(timer);
      }, 1000);
    } else {
      ElMessage.error(result.errors?.join("；") || "發送失敗，請稍後再試");
    }
  } catch (error: any) {
    const msg = error?.response?.data?.errors?.join("；") || "請稍後再試";
    ElMessage.error(msg);
  } finally {
    resending.value = false;
  }
};
</script>

<style lang="scss">
$bg: #2d3a4b;
$dark_gray: #889aa4;
$light_gray: #eee;
$cursor: #fff;

/* reset element-plus css for login */
.login-container {
  .el-input {
    height: 47px;
    width: 85%;

    .el-input__wrapper,
    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      box-shadow: none;
    }

    input {
      padding: 12px 5px 12px 15px;
      color: $light_gray;
      height: 47px;
      caret-color: $cursor;

      &:-webkit-autofill {
        box-shadow: 0 0 0px 1000px $bg inset !important;
        -webkit-text-fill-color: $cursor !important;
      }
    }
  }

  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(0, 0, 0, 0.1);
    border-radius: 5px;
    color: #454545;
  }

  .el-checkbox {
    color: rgba(255, 255, 255, 0.65);
  }
}
</style>

<style lang="scss" scoped>
$bg: #2d3a4b;
$dark_gray: #889aa4;
$light_gray: #eee;

.login-container {
  min-height: 100%;
  width: 100%;
  min-height: 100vh;
  background-color: $bg;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;

  .login-form {
    position: relative;
    width: 520px;
    max-width: 100%;
    padding: 160px 35px 0;
    margin: 0 auto;
    overflow: hidden;
  }

  .title-container {
    position: relative;
    text-align: center;
    margin-bottom: 40px;

    .title {
      font-size: 26px;
      color: $light_gray;
      margin: 0 0 10px;
      font-weight: bold;
    }

    .subtitle {
      font-size: 14px;
      color: $dark_gray;
      margin: 0;
    }
  }

  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }

  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }

  .form-options {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
    padding: 0 2px;
  }

  .social-divider {
    display: flex;
    align-items: center;
    margin: 0 0 20px;

    &::before,
    &::after {
      content: "";
      flex: 1;
      height: 1px;
      background: rgba(255, 255, 255, 0.1);
    }

    .divider-text {
      padding: 0 16px;
      font-size: 13px;
      color: $dark_gray;
      white-space: nowrap;
    }
  }

  .social-login-buttons {
    display: flex;
    gap: 12px;
    margin-bottom: 20px;

    .social-btn {
      flex: 1;
      height: 40px;
      font-size: 13px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
      border-radius: 5px;

      .social-icon {
        flex-shrink: 0;
      }
    }

    .google-btn {
      background: rgba(255, 255, 255, 0.1);
      border: 1px solid rgba(255, 255, 255, 0.2);
      color: $light_gray;

      &:hover {
        background: rgba(255, 255, 255, 0.2);
      }
    }

    .facebook-btn {
      background: #1877f2;
      border: 1px solid #1877f2;
      color: #fff;

      &:hover {
        background: #166fe5;
      }
    }
  }

  .verification-notice {
    margin-bottom: 20px;

    .verification-content {
      margin-top: 8px;

      p {
        margin: 0 0 12px;
        font-size: 13px;
      }
    }

    .resend-section {
      display: flex;
      gap: 8px;
      align-items: center;

      .resend-input {
        flex: 1;
      }
    }
  }

  .register-link {
    text-align: center;
    padding: 20px 0;

    span {
      font-size: 14px;
      color: $dark_gray;
    }

    a {
      color: #409eff;
      cursor: pointer;
      margin-left: 4px;

      &:hover {
        text-decoration: underline;
      }
    }
  }

  @media only screen and (max-width: 640px) {
    .login-form {
      padding: 100px 20px 0;
      width: 100%;
    }

    .social-login-buttons {
      flex-direction: column;
    }
  }
}
</style>
