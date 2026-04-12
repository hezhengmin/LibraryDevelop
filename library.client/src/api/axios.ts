import axios from "axios";
import type { AxiosError, InternalAxiosRequestConfig } from "axios";
import { ElMessage } from "element-plus";

// 開發環境使用代理，生產環境使用環境變數
const getBaseURL = () => {
  // 開發環境使用相對路徑，透過 Vite 代理轉發
  if (import.meta.env.DEV) {
    return '';
  }
  // 生產環境使用完整的 API URL
  return import.meta.env.VITE_API_URL || 'https://localhost:7019';
};

const apiClient = axios.create({
  baseURL: getBaseURL(),
  timeout: 30000,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Add auth token if needed
    const token = localStorage.getItem("token") || sessionStorage.getItem("token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    console.error("Request error:", error);
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    // Handle errors globally
    console.error("API Error:", error);

    if (error.response) {
      const { status, data } = error.response;

      switch (status) {
        case 401:
          // Unauthorized - token 可能過期
          ElMessage.error("登入已過期，請重新登入");
          // 清除 token 並重定向到登入頁
          localStorage.removeItem("token");
          sessionStorage.removeItem("token");
          if (window.location.pathname !== "/login") {
            window.location.href = "/login";
          }
          break;

        case 403:
          // Forbidden - 沒有權限
          ElMessage.error("您沒有權限執行此操作");
          break;

        case 404:
          // Not Found
          ElMessage.error("請求的資源不存在");
          break;

        case 500:
          // Internal Server Error
          ElMessage.error("伺服器錯誤，請稍後再試");
          break;

        case 503:
          // Service Unavailable
          ElMessage.error("服務暫時無法使用，請稍後再試");
          break;

        default:
          // 其他錯誤
          if (data && typeof data === "object" && "message" in data) {
            ElMessage.error((data as any).message || "操作失敗");
          }
      }
    } else if (error.request) {
      // 請求已發送但沒有收到回應
      ElMessage.error("網路連線錯誤，請檢查您的網路連線");
    } else {
      // 設定請求時發生錯誤
      ElMessage.error("請求發生錯誤");
    }

    return Promise.reject(error);
  }
);

export default apiClient;
