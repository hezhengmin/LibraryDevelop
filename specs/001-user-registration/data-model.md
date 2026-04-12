# Data Model: 使用者註冊服務

## Entities

### IdentityUser (existing — ASP.NET Core Identity)

已存在於 `IdentityDbContext<IdentityUser>`，無需新建 Entity。

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | string (GUID) | PK | 自動產生 |
| UserName | string | Required, Unique | 已使用中 |
| Email | string | Required, Unique | **新增使用**（既有欄位，目前未設定） |
| EmailConfirmed | bool | Default: false | **新增使用**（既有欄位） |
| NormalizedEmail | string | Unique Index | Identity 內部使用，自動維護 |
| PasswordHash | string | — | Identity PBKDF2，自動產生 |
| SecurityStamp | string | — | Token 驗證用，confirm email 後自動輪換 |

### 無新增 Entity

- 不需要獨立的 `EmailVerification` 表。
- ASP.NET Core Identity 已透過 Data Protection Token Provider
  處理 token 產生與驗證，token 不儲存於 DB。

## Relationships

```text
IdentityUser (1) ──── (*) IdentityUserRole ──── (1) IdentityRole
```

無新增關聯。

## Validation Rules

### 註冊時

| Field | Rule | Source |
|-------|------|--------|
| UserName | Required, 不可重複 | DataAnnotation + Identity UserManager |
| Email | Required, 格式驗證 (RFC 5322), 不可重複 | DataAnnotation + Identity UserManager |
| Password | Required, 最少 6 字元 | DataAnnotation + IdentityOptions |
| ConfirmPassword | Required, 必須與 Password 相同 | DataAnnotation [Compare] |

### Email 驗證時

| Field | Rule | Source |
|-------|------|--------|
| UserId | Required, 必須存在 | UserManager.FindByIdAsync |
| Token | Required, URL decoded, 未過期 | UserManager.ConfirmEmailAsync |

## State Transitions

```text
[未註冊] ──Register──> [已註冊/未驗證] ──ConfirmEmail──> [已驗證/可登入]
                            │                                  │
                            │  (ResendVerification)            │
                            └──────────────┘                   │
                                                               │
                     [登入被拒] <──Login(未驗證)──────────────────┘
                                                  (已驗證才放行)
```

## Migration Notes

無需新增 migration。`IdentityUser` 已包含 `Email` 與
`EmailConfirmed` 欄位。僅需在程式碼中開始使用這些欄位。
