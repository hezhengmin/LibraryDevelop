# Quickstart: 使用者註冊服務

## 前置條件

- .NET 8 SDK
- Node.js 20+
- SQL Server（LocalDB 或完整版本）
- 已執行 `dotnet ef database update`

## 1. 安裝新增的套件

### 後端

```bash
cd Library.Server
dotnet add package MailKit
```

### 前端（如需 Vitest）

```bash
cd library.client
npm install -D vitest @vue/test-utils jsdom
```

## 2. 設定 Email（開發環境）

在 `appsettings.Development.json` 加入：

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.ethereal.email",
    "SmtpPort": 587,
    "SmtpUser": "your-ethereal-user@ethereal.email",
    "SmtpPassword": "your-ethereal-password",
    "SenderEmail": "noreply@library.local",
    "SenderName": "圖書管理系統"
  }
}
```

> 可至 https://ethereal.email/ 免費產生測試帳號。

## 3. 驗證流程

1. 啟動後端：`dotnet run --project Library.Server`
2. 啟動前端：`cd library.client && npm run dev`
3. 瀏覽 `https://localhost:50904/register`
4. 填寫帳號、Email、密碼後提交
5. 至 Ethereal 信箱查看驗證信
6. 點擊驗證連結完成驗證
7. 回到登入頁面進行登入

## 4. 測試

### 後端單元測試

```bash
cd Library.Server.Tests
dotnet test
```

### 前端元件測試

```bash
cd library.client
npx vitest run
```

## 5. 關鍵 API 測試（curl）

### 註冊

```bash
curl -X POST https://localhost:7019/api/Account/Register \
  -H "Content-Type: application/json" \
  -d '{"userName":"testuser","email":"test@example.com","password":"Test123","confirmPassword":"Test123"}'
```

### 重新發送驗證信

```bash
curl -X POST https://localhost:7019/api/Account/ResendVerification \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com"}'
```
