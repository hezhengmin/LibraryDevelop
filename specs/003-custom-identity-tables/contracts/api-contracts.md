# API Contracts: 自訂身分驗證資料表結構

**Feature**: 003-custom-identity-tables  
**Date**: 2026-04-11  
**Principle**: 所有 API 端點維持與現有介面完全相容，前端零修改

---

## Account API（登入/註冊）

> 回應格式維持 `CustomResponse` 標準結構：`{ success, message, data }`

### POST /api/Account/Login

**Request**:
```json
{
  "userName": "string (required, max 256)",
  "password": "string (required, 6-100)"
}
```

**Response 200**:
```json
{
  "success": true,
  "message": "登入成功",
  "data": {
    "accessToken": "string (JWT)",
    "refreshToken": "string"
  }
}
```

**Response 401** (密碼錯誤 / Email 未驗證 / 帳號鎖定):
```json
{
  "success": false,
  "message": "帳號或密碼錯誤 | 請先完成 Email 驗證 | 帳號已被鎖定，請稍後再試",
  "data": null
}
```

**行為變更**: 無。內部改用 AuthService 驗證密碼，但回應格式與錯誤訊息完全一致。

---

### POST /api/Account/Register

**Request**:
```json
{
  "userName": "string (required, max 256)",
  "email": "string (required, email format)",
  "password": "string (required, 6-100)"
}
```

**Response 200**:
```json
{
  "success": true,
  "message": "註冊成功，請至信箱確認驗證信",
  "data": null
}
```

**Response 400** (帳號/Email 已存在):
```json
{
  "success": false,
  "message": "使用者名稱已存在 | Email 已被使用",
  "data": null
}
```

**行為變更**: 無。密碼改以 BCrypt 雜湊（新帳號預設 PasswordHashVersion=2）。

---

### GET /api/Account/ConfirmEmail?userId={guid}&token={base64url}

**Response 200**: `{ success: true, message: "Email 驗證成功" }`  
**Response 400**: `{ success: false, message: "驗證連結無效或已過期" }`

**行為變更**: Token 改從 UserToken 表查找驗證（TokenType=EmailVerification），而非透過 Identity DataProtection。

---

### POST /api/Account/ResendVerification

**Request**: `{ "email": "string" }`  
**Response 200**: `{ success: true, message: "驗證信已重新發送" }`  
**Response 429**: `{ success: false, message: "請等待一分鐘後再試" }`

**行為變更**: 無。Rate limit 邏輯維持不變（IMemoryCache）。

---

## User API（使用者管理）

### GET /api/User?page={int}&pageSize={int}&keyword={string}

**Response 200**:
```json
{
  "success": true,
  "message": "取得使用者列表成功",
  "data": {
    "items": [
      {
        "id": "guid",
        "userName": "string",
        "email": "string",
        "emailConfirmed": true,
        "phoneNumber": "string | null",
        "lockoutEnd": "datetime | null",
        "lockoutEnabled": true,
        "accessFailedCount": 0,
        "roles": ["角色名稱1", "角色名稱2"]
      }
    ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 10
  }
}
```

**行為變更**: 無。資料來源改為 User + UserRole + Role 自訂表查詢。

---

### GET /api/User/{id}

**Response 200**: 同列表項目結構，但為單一物件。  
**行為變更**: 無。

---

### POST /api/User

**Request**:
```json
{
  "userName": "string",
  "email": "string",
  "password": "string",
  "roles": ["roleId1", "roleId2"]
}
```

**Response 200**: `{ success: true, message: "建立使用者成功" }`  
**行為變更**: 密碼以 BCrypt 雜湊。

---

### PUT /api/User/{id}

**Request**:
```json
{
  "userName": "string",
  "email": "string",
  "roles": ["roleId1", "roleId2"]
}
```

**Response 200**: `{ success: true, message: "更新使用者成功" }`  
**行為變更**: 無。

---

### DELETE /api/User/{id}

**Response 200**: `{ success: true, message: "刪除使用者成功" }`  
**行為變更**: 改為軟刪除（設定 IsActive=false），而非實際刪除記錄。

---

### POST /api/User/{id}/reset-password

**Request**: `{ "newPassword": "string" }`  
**Response 200**: `{ success: true, message: "密碼重設成功" }`  
**行為變更**: 新密碼以 BCrypt 雜湊，PasswordHashVersion 更新為 2。

---

### POST /api/User/{id}/lock | POST /api/User/{id}/unlock

**Response 200**: `{ success: true, message: "帳號已鎖定 | 帳號已解鎖" }`  
**行為變更**: 無。直接更新 User.LockoutEnd 欄位。

---

## Role API（角色管理）

### GET /api/Role?page={int}&pageSize={int}

**Response 200**:
```json
{
  "success": true,
  "message": "取得角色列表成功",
  "data": {
    "items": [
      {
        "id": "guid",
        "name": "string",
        "description": "string | null",
        "userCount": 5
      }
    ],
    "totalCount": 10,
    "page": 1,
    "pageSize": 10
  }
}
```

**行為變更**: 新增 `description` 欄位（可選）。

---

### GET /api/Role/{id}

**Response 200**:
```json
{
  "success": true,
  "data": {
    "id": "guid",
    "name": "string",
    "description": "string | null",
    "permissions": ["Books.View", "Books.Create", "Users.View"]
  }
}
```

**行為變更**: `permissions` 改從 RolePermission 表直接查詢，而非解析 Claims。

---

### POST /api/Role

**Request**:
```json
{
  "name": "string",
  "description": "string (optional)",
  "permissions": ["Books.View", "Books.Create"]
}
```

**Response 200**: `{ success: true, message: "建立角色成功" }`  
**行為變更**: 權限寫入 RolePermission 表而非 RoleClaims。

---

### PUT /api/Role/{id}

**Request**: 同 POST。  
**Response 200**: `{ success: true, message: "更新角色成功" }`  
**行為變更**: 無。

---

### DELETE /api/Role/{id}

**Response 200**: `{ success: true, message: "刪除角色成功" }`  
**Response 400**: `{ success: false, message: "此角色仍有使用者，無法刪除" }`  
**行為變更**: 無。

---

### GET /api/Role/permissions

**Response 200**: 權限樹結構（同現有，來源為 SysMenu）。  
**行為變更**: 無。此端點查詢 SysMenu，不涉及 Identity。

---

## External Login API

### GET /api/ExternalLogin/Challenge/{provider}

**行為變更**: 無。OAuth challenge 流程不依賴 Identity。

### GET /api/ExternalLogin/Callback/{provider}

**行為變更**: 內部改用 UserExternalLogin 表查詢/儲存，回應格式不變。

### POST /api/ExternalLogin/Link

**行為變更**: 改操作 UserExternalLogin 表。

---

## Service Interfaces（新增/修改）

### IAuthService（新增）

```csharp
Task<AuthResult> ValidatePasswordAsync(string userName, string password);
Task<string> HashPasswordAsync(string password);
Task<bool> VerifyPasswordAsync(string hashedPassword, int hashVersion, string providedPassword);
Task<string> GenerateEmailVerificationTokenAsync(Guid userId);
Task<bool> VerifyEmailTokenAsync(Guid userId, string token);
Task<string> GeneratePasswordResetTokenAsync(Guid userId);
Task<bool> VerifyPasswordResetTokenAsync(Guid userId, string token);
```

### IUserService（新增）

```csharp
Task<User?> FindByIdAsync(Guid id);
Task<User?> FindByNameAsync(string userName);
Task<User?> FindByEmailAsync(string email);
Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? keyword);
Task<UserDto?> GetUserWithRolesAsync(Guid id);
Task<Result> CreateUserAsync(CreateUserInput input);
Task<Result> UpdateUserAsync(Guid id, UpdateUserInput input);
Task<Result> DeleteUserAsync(Guid id);
Task<Result> ResetPasswordAsync(Guid id, string newPassword);
Task<Result> LockUserAsync(Guid id);
Task<Result> UnlockUserAsync(Guid id);
Task<IList<string>> GetRolesAsync(Guid userId);
Task<Result> SetRolesAsync(Guid userId, IList<Guid> roleIds);
```

### IRoleService（新增）

```csharp
Task<PagedResult<RoleDto>> GetRolesAsync(int page, int pageSize);
Task<RoleDetailDto?> GetRoleWithPermissionsAsync(Guid id);
Task<Result> CreateRoleAsync(CreateRoleInput input);
Task<Result> UpdateRoleAsync(Guid id, UpdateRoleInput input);
Task<Result> DeleteRoleAsync(Guid id);
Task<IList<string>> GetPermissionsAsync(Guid roleId);
Task<IList<UserDto>> GetUsersInRoleAsync(Guid roleId);
```
