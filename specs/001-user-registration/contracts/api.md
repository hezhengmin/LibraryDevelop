# API Contracts: 使用者註冊服務

Base URL: `/api/Account`

## POST /Register

註冊新使用者帳號。

### Request

```json
{
  "userName": "string (required)",
  "email": "string (required, email format)",
  "password": "string (required, min 6 chars)",
  "confirmPassword": "string (required, must match password)"
}
```

### Response — 200 Success

```json
{
  "succeeded": true,
  "errors": [],
  "message": "註冊成功，請至信箱完成電子郵件驗證",
  "data": null
}
```

### Response — 400 Validation Error (Model State)

```json
{
  "succeeded": false,
  "errors": ["請輸入帳號", "密碼長度須介於6~100字元"],
  "message": null,
  "data": null
}
```

### Response — 400 Business Error

```json
{
  "succeeded": false,
  "errors": ["註冊失敗：此電子郵件已被使用"],
  "message": null,
  "data": null
}
```

---

## GET /ConfirmEmail?userId={userId}&token={token}

驗證使用者電子郵件。使用者點擊驗證信中的連結時觸發。

### Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | string | Yes | 使用者 ID |
| token | string | Yes | URL-encoded 驗證 token |

### Response — 200 Success

重導至前端驗證成功頁面，或回傳：

```json
{
  "succeeded": true,
  "errors": [],
  "message": "電子郵件驗證成功，請前往登入",
  "data": null
}
```

### Response — 400 Invalid/Expired Token

```json
{
  "succeeded": false,
  "errors": ["驗證連結無效或已過期"],
  "message": null,
  "data": null
}
```

### Response — 400 Already Confirmed

```json
{
  "succeeded": true,
  "errors": [],
  "message": "帳號已驗證，請直接登入",
  "data": null
}
```

---

## POST /ResendVerification

重新發送電子郵件驗證信。

### Request

```json
{
  "email": "string (required, email format)"
}
```

### Response — 200 (Always — prevents account enumeration)

```json
{
  "succeeded": true,
  "errors": [],
  "message": "若該信箱已註冊，驗證信已發送",
  "data": null
}
```

### Response — 429 Rate Limited

```json
{
  "succeeded": false,
  "errors": ["請稍後再試"],
  "message": null,
  "data": null
}
```

---

## POST /Login (existing — modified behavior)

已存在的登入 endpoint，新增未驗證帳號的錯誤處理。

### New Error Response — 403 Email Not Confirmed

```json
{
  "succeeded": false,
  "errors": ["請先完成電子郵件驗證"],
  "message": null,
  "data": {
    "requiresEmailVerification": true
  }
}
```

---

## Frontend Routes

| Route | Component | Auth Required |
|-------|-----------|---------------|
| `/register` | `RegisterView.vue` | No |
| `/verify-email` | `VerifyEmailView.vue` | No |
