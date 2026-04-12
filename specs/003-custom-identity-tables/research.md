# Research: 自訂身分驗證資料表結構

**Feature**: 003-custom-identity-tables  
**Date**: 2026-04-11  
**Status**: Complete

## Research Task 1: 密碼雜湊演算法選擇

### Decision: BCrypt (via BCrypt.Net-Next)

### Rationale
- BCrypt 是業界廣泛採用的密碼雜湊演算法，內建 salt 管理與可調整的 work factor
- BCrypt.Net-Next 是 .NET 生態中維護最活躍的 BCrypt 實作，NuGet 下載量超過 5000 萬
- Work factor（cost）設定為 12（預設），每次雜湊約 250ms，在安全性與使用者體驗間取得平衡
- 雜湊結果為 60 字元字串（含演算法版本、cost、salt 與 hash），可直接存入 `nvarchar(100)` 欄位

### Alternatives Considered
- **PBKDF2**：ASP.NET Core Identity 預設使用，但需自行管理 salt、iteration count、輸出格式
- **Argon2**：最新的密碼雜湊標準（Password Hashing Competition 優勝），但 .NET 生態庫較少且成熟度不如 BCrypt
- **保留 Identity PasswordHasher**：仍依賴 Identity 框架，不符合完全脫離 Identity 的目標

### Migration Consideration
- 現有 Identity 密碼使用 PBKDF2（Identity V3 格式），無法直接轉換為 BCrypt
- 策略：遷移時保留原始 PasswordHash，新增 `PasswordHashVersion` 欄位標記版本
  - V1 = Identity PBKDF2 格式（舊密碼）
  - V2 = BCrypt 格式（新密碼）
- 使用者首次以舊密碼登入成功後，自動以 BCrypt 重新雜湊並更新版本為 V2
- 這樣不需要強制所有使用者重設密碼

---

## Research Task 2: Email 驗證 Token 機制（取代 Identity DataProtection Token）

### Decision: HMACSHA256 自訂 Token + 資料庫儲存

### Rationale
- Identity 的 Email Token 依賴 DataProtection API，與 Identity 框架緊密耦合
- 自訂方案使用 HMACSHA256 生成不可預測的 Token，存入 UserToken 資料表
- Token 包含過期時間（24 小時）與用途標記（EmailVerification / PasswordReset）
- 使用後標記為已使用，防止重複使用

### Token 生成流程
1. 產生 32 bytes 隨機數（`RandomNumberGenerator.GetBytes`）
2. 使用 HMACSHA256 簽章（key = JWT 密鑰）
3. 組合為 Base64Url 編碼字串
4. 存入 UserToken 表（UserId, TokenType, TokenValue, ExpiryDate, IsUsed）

### Alternatives Considered
- **純隨機 Token**：安全性足夠但無法驗證完整性，需要每次都查資料庫
- **JWT 格式 Token**：可自驗證但過長，不適合 URL query string
- **DataProtection API（獨立使用）**：可行但增加不必要的複雜度，且失去明確的 Token 生命週期管理

---

## Research Task 3: 帳號鎖定機制設計

### Decision: 自訂鎖定邏輯（資料庫欄位 + Service 層判斷）

### Rationale
- Identity 的鎖定機制透過 `LockoutEnd`、`LockoutEnabled`、`AccessFailedCount` 三個欄位實現
- 自訂實體沿用相同欄位設計，在 AuthService 中實作相同邏輯
- 鎖定策略：連續失敗 5 次後鎖定 15 分鐘，之後自動解鎖

### 實作細節
- `AccessFailedCount`：每次密碼驗證失敗時遞增，登入成功時歸零
- `LockoutEnd`：鎖定結束時間（UTC），為 null 或過去時間表示未鎖定
- `LockoutEnabled`：是否啟用鎖定（管理員帳號可關閉）
- 判斷邏輯：`LockoutEnabled && LockoutEnd != null && LockoutEnd > DateTime.UtcNow`

### Alternatives Considered
- **Redis 快取鎖定**：效能更好但增加基礎設施依賴，對目前規模不必要
- **IP 層級鎖定**：可防止分散式暴力破解，但超出本次改寫範圍

---

## Research Task 4: 外部登入（OAuth）整合方式

### Decision: 保留 ASP.NET Core Authentication middleware，僅替換資料儲存

### Rationale
- Google/Facebook OAuth 的核心流程由 `Microsoft.AspNetCore.Authentication.Google/Facebook` 套件處理
- 這些套件不依賴 Identity 框架，僅依賴 ASP.NET Core Authentication middleware
- 改寫僅需替換「查找/儲存外部登入資訊」的部分，從 `UserManager.FindByLoginAsync` 改為直接查詢 UserExternalLogin 資料表
- OAuth challenge/callback 流程維持不變

### Key Changes
- 移除 `AddIdentityApiEndpoints` 但保留 `.AddAuthentication().AddGoogle().AddFacebook()`
- ExternalLoginService 改為注入 `LibraryContext` 直接操作 UserExternalLogin
- ExternalLoginController 的 OAuth challenge 邏輯不變

### Alternatives Considered
- **完全自訂 OAuth 流程**：需自行處理 authorization code exchange，風險高且不必要
- **保留 Identity 僅用於 OAuth**：形成奇怪的半依賴狀態，維護成本高

---

## Research Task 5: 資料遷移策略

### Decision: EF Core Migration + SQL 資料搬移腳本

### Rationale
- 使用 EF Core Migration 建立新資料表
- 在同一 Migration 或獨立 SQL 腳本中，將 AspNet* 資料複製到新表
- 舊表暫時保留（不在 Migration 中刪除），待驗證完成後手動刪除

### Migration 步驟
1. **Migration 1: AddCustomIdentityTables** — 建立新的 User、Role、UserRole、RolePermission、UserExternalLogin、UserToken 表
2. **Migration 2: MigrateIdentityData** — SQL 腳本將 AspNet* 資料搬移至新表
3. **手動驗證** — 確認資料完整性後，手動執行 Migration 移除舊表

### ID 轉換策略
- Identity 使用 `string` 型別的 GUID 作為 PK（如 `"a1b2c3d4-..."`)
- 新表使用 `Guid` 型別 PK
- 遷移時使用 `CAST(Id AS uniqueidentifier)` 轉換，保持原始 ID 值

### 密碼遷移
- 原始 `PasswordHash` 保留不變
- 新增 `PasswordHashVersion = 1`（Identity PBKDF2 格式）
- 使用者下次登入時自動升級為 BCrypt（Version = 2）

### Alternatives Considered
- **雙寫期**：同時寫入新舊表，過渡期完成後切換 — 過於複雜
- **強制重設密碼**：要求所有使用者重設密碼 — 使用者體驗差
- **離線遷移工具**：獨立的 Console 應用程式執行遷移 — 對此規模不必要

---

## Research Task 6: ApplicationDbContext 合併策略

### Decision: 移除 ApplicationDbContext，統一使用 LibraryContext

### Rationale
- 現有架構有兩個 DbContext：`ApplicationDbContext`（Identity 專用）和 `LibraryContext`（業務 + Identity 模型映射）
- 移除 Identity 後，`ApplicationDbContext` 完全沒有存在意義
- 統一為單一 `LibraryContext`，配置所有自訂實體的 Fluent API

### Changes
- 刪除 `ApplicationDbContext` 類別與檔案
- Program.cs 移除 `ApplicationDbContext` 的 DI 註冊
- 所有新實體在 `LibraryContext.OnModelCreating()` 中使用 `IEntityTypeConfiguration<T>` 配置
- LibraryContext 移除原有的 `DbSet<AspNetUser>` 等，替換為新實體 DbSet

### Alternatives Considered
- **保留兩個 DbContext**：自訂身分表放在新的 AuthDbContext — 增加不必要的複雜度
- **建立第三個 DbContext**：違反 Constitution 的單一職責原則在此場景下不適用

---

## Research Task 7: PermissionAuthorizationHandler 改寫方式

### Decision: 改為直接查詢 LibraryContext

### Rationale
- 現有 Handler 透過 `UserManager.GetRolesAsync()` + `RoleManager.GetClaimsAsync()` 查詢權限
- 改寫後直接以一條 LINQ 查詢從 User → UserRole → Role → RolePermission 取得所有權限
- 效能更好（單次查詢 vs 多次 UserManager/RoleManager 呼叫）

### 查詢設計
```csharp
var permissions = await _context.RolePermissions
    .Where(rp => _context.UserRoles
        .Where(ur => ur.UserId == userId)
        .Select(ur => ur.RoleId)
        .Contains(rp.RoleId))
    .Select(rp => rp.Permission)
    .Distinct()
    .ToListAsync();
```

### Alternatives Considered
- **快取使用者權限**：在 JWT Claims 中包含所有權限 — Token 會過大且無法即時反映權限變更
- **Redis 快取**：有效但增加基礎設施依賴，目前規模不需要
