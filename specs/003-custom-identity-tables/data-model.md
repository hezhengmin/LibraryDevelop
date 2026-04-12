# Data Model: 自訂身分驗證資料表結構

**Feature**: 003-custom-identity-tables  
**Date**: 2026-04-11  
**Status**: Complete

## Entity Overview

```
┌──────────┐     ┌───────────┐     ┌──────────┐
│   User   │────<│  UserRole  │>────│   Role   │
└──────────┘     └───────────┘     └──────────┘
     │                                   │
     │                                   │
     ▼                                   ▼
┌──────────────────┐          ┌─────────────────┐
│ UserExternalLogin │          │ RolePermission  │
└──────────────────┘          └─────────────────┘
     │
     │
     ▼
┌──────────┐
│ UserToken │
└──────────┘
```

---

## Entity: User

**取代**: AspNetUsers  
**用途**: 系統使用者帳號，儲存身分驗證所需的所有資訊

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| Id | Guid | No | 主鍵 (PK) |
| UserName | nvarchar(256) | No | 登入帳號，唯一索引 |
| NormalizedUserName | nvarchar(256) | No | 大寫正規化帳號，唯一索引（用於不分大小寫查詢） |
| Email | nvarchar(256) | No | 電子郵件，唯一索引 |
| NormalizedEmail | nvarchar(256) | No | 大寫正規化信箱，唯一索引 |
| EmailConfirmed | bit | No | Email 是否已驗證，預設 false |
| PasswordHash | nvarchar(500) | Yes | 密碼雜湊值（BCrypt 格式或 Identity PBKDF2 格式） |
| PasswordHashVersion | int | No | 密碼雜湊版本：1 = Identity PBKDF2, 2 = BCrypt |
| SecurityStamp | nvarchar(100) | Yes | 安全戳記，密碼/權限變更時更新（用於使現有 Token 失效） |
| PhoneNumber | nvarchar(50) | Yes | 電話號碼 |
| PhoneNumberConfirmed | bit | No | 電話是否已驗證，預設 false |
| TwoFactorEnabled | bit | No | 是否啟用雙因素驗證，預設 false |
| LockoutEnd | datetimeoffset | Yes | 鎖定結束時間（UTC），null 表示未鎖定 |
| LockoutEnabled | bit | No | 是否啟用鎖定功能，預設 true |
| AccessFailedCount | int | No | 連續登入失敗次數，預設 0 |
| IsActive | bit | No | 帳號是否啟用，預設 true（軟刪除用） |
| CreatedAt | datetime2 | No | 建立時間 |
| UpdatedAt | datetime2 | Yes | 最後更新時間 |
| ConcurrencyStamp | nvarchar(100) | Yes | 並行控制戳記 |

**Indexes**:
- `IX_User_NormalizedUserName` (unique) on NormalizedUserName
- `IX_User_NormalizedEmail` (unique) on NormalizedEmail

**Relationships**:
- One-to-many → UserRole
- One-to-many → UserExternalLogin
- One-to-many → UserToken
- One-to-many → Loan（透過 Account 關聯，需確認遷移策略）

**Validation Rules**:
- UserName: 必填，最大 256 字元，唯一
- Email: 必填，最大 256 字元，唯一，符合 Email 格式
- PasswordHash: 新建帳號時必填（OAuth 帳號可為 null）
- PasswordHashVersion: 預設 2（BCrypt），遷移資料為 1

---

## Entity: Role

**取代**: AspNetRoles  
**用途**: 角色定義，用於群組化權限

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| Id | Guid | No | 主鍵 (PK) |
| Name | nvarchar(256) | No | 角色名稱 |
| NormalizedName | nvarchar(256) | No | 大寫正規化名稱，唯一索引 |
| Description | nvarchar(500) | Yes | 角色描述 |
| ConcurrencyStamp | nvarchar(100) | Yes | 並行控制戳記 |
| CreatedAt | datetime2 | No | 建立時間 |
| UpdatedAt | datetime2 | Yes | 最後更新時間 |

**Indexes**:
- `IX_Role_NormalizedName` (unique) on NormalizedName

**Relationships**:
- One-to-many → UserRole
- One-to-many → RolePermission

**Validation Rules**:
- Name: 必填，最大 256 字元，唯一

---

## Entity: UserRole

**取代**: AspNetUserRoles  
**用途**: 使用者與角色的多對多關聯

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| UserId | Guid | No | FK → User.Id |
| RoleId | Guid | No | FK → Role.Id |
| AssignedAt | datetime2 | No | 指派時間 |

**Primary Key**: Composite (UserId, RoleId)

**Indexes**:
- PK on (UserId, RoleId)
- `IX_UserRole_RoleId` on RoleId（EF Core 自動建立 FK 索引）

**Relationships**:
- Many-to-one → User (cascade delete)
- Many-to-one → Role (cascade delete)

---

## Entity: RolePermission

**取代**: AspNetRoleClaims（Type="Permission" 的記錄）  
**用途**: 儲存每個角色擁有的權限項目

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| Id | int | No | 主鍵 (PK, identity) |
| RoleId | Guid | No | FK → Role.Id |
| Permission | nvarchar(256) | No | 權限值（對應 SysMenu.ModuleNo，如 "Books.View"） |

**Indexes**:
- `IX_RolePermission_RoleId` on RoleId
- `IX_RolePermission_RoleId_Permission` (unique) on (RoleId, Permission) — 防止重複指派

**Relationships**:
- Many-to-one → Role (cascade delete)

**Validation Rules**:
- Permission: 必填，最大 256 字元
- (RoleId, Permission) 組合唯一

---

## Entity: UserExternalLogin

**取代**: AspNetUserLogins  
**用途**: 儲存使用者的外部 OAuth 登入資訊

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| Id | int | No | 主鍵 (PK, identity) |
| UserId | Guid | No | FK → User.Id |
| LoginProvider | nvarchar(128) | No | OAuth 提供者名稱（如 "Google", "Facebook"） |
| ProviderKey | nvarchar(128) | No | OAuth 提供者端的使用者 ID |
| ProviderDisplayName | nvarchar(256) | Yes | 提供者顯示名稱 |
| CreatedAt | datetime2 | No | 連結時間 |

**Indexes**:
- `IX_UserExternalLogin_LoginProvider_ProviderKey` (unique) on (LoginProvider, ProviderKey)
- `IX_UserExternalLogin_UserId` on UserId

**Relationships**:
- Many-to-one → User (cascade delete)

**Validation Rules**:
- LoginProvider: 必填，最大 128 字元
- ProviderKey: 必填，最大 128 字元
- (LoginProvider, ProviderKey) 組合唯一

---

## Entity: UserToken

**取代**: AspNetUserTokens + Token（整合兩表功能）  
**用途**: 統一儲存所有類型的 Token（Refresh Token、Email 驗證、密碼重設）

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| Id | Guid | No | 主鍵 (PK) |
| UserId | Guid | No | FK → User.Id |
| TokenType | nvarchar(50) | No | Token 類型：RefreshToken / EmailVerification / PasswordReset |
| TokenValue | nvarchar(500) | No | Token 值 |
| AccessToken | nvarchar(max) | Yes | 對應的 JWT Access Token（僅 RefreshToken 類型使用） |
| ExpiryDate | datetime2 | No | Token 過期時間 |
| IsUsed | bit | No | 是否已使用，預設 false |
| IsRevoked | bit | No | 是否已撤銷，預設 false |
| CreatedAt | datetime2 | No | 建立時間 |
| CreatedBy | nvarchar(256) | Yes | 建立者 |

**Indexes**:
- `IX_UserToken_UserId` on UserId
- `IX_UserToken_TokenValue` on TokenValue（用於快速查找）
- `IX_UserToken_UserId_TokenType` on (UserId, TokenType)（用於查詢特定類型的 Token）

**Relationships**:
- Many-to-one → User (cascade delete)

**Validation Rules**:
- TokenType: 必填，限定值：RefreshToken, EmailVerification, PasswordReset
- TokenValue: 必填
- ExpiryDate: RefreshToken = 7 天, EmailVerification = 24 小時, PasswordReset = 1 小時

**State Transitions**:
- Created → Used（單次使用：Email 驗證、密碼重設）
- Created → Revoked（管理員撤銷、使用者登出）
- Created → Expired（自然過期，查詢時判斷）

---

## Identity Table → Custom Table Mapping

| Identity Table | Custom Entity | PK Type Change | Notes |
|----------------|---------------|----------------|-------|
| AspNetUsers | User | string → Guid | 新增 PasswordHashVersion, IsActive, CreatedAt, UpdatedAt |
| AspNetRoles | Role | string → Guid | 新增 Description, CreatedAt, UpdatedAt |
| AspNetUserRoles | UserRole | string → Guid | 新增 AssignedAt |
| AspNetRoleClaims | RolePermission | int → int | ClaimType/ClaimValue 簡化為單一 Permission 欄位 |
| AspNetUserLogins | UserExternalLogin | composite → int | 新增 Id 主鍵、CreatedAt |
| AspNetUserTokens | UserToken | composite → Guid | 與 Token 表整合，增加完整 Token 生命週期管理 |
| AspNetUserClaims | (移除) | - | 使用者層級 Claims 不再需要，權限由 RolePermission 管理 |
| Token | UserToken | Guid → Guid | 整合至 UserToken，統一 Token 管理 |
| Account | (移除) | - | Legacy 表，功能由 User 取代 |
