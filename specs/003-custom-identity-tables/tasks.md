# Tasks: 自訂身分驗證資料表結構

**Input**: Design documents from `/specs/003-custom-identity-tables/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: 未在 spec.md 中明確要求 TDD，但 plan.md Constitution Check 要求 Service 層單元測試覆蓋率 ≥ 80%。測試任務包含於各 User Story Phase 中。

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend Models**: `Zheng.Infrastructure/Models/`
- **Backend Services**: `Zheng.Bll/Services/`, `Zheng.Bll/Interfaces/`
- **Backend Controllers**: `Library.Server/Controllers/`
- **Backend Filters**: `Library.Server/Filters/`
- **Backend Data**: `Library.Server/Data/`, `Zheng.Infra.Data/Models/`
- **Backend Tests**: `Library.Server.Tests/Services/`
- **Migrations**: `Library.Server/Migrations/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 安裝依賴套件、建立專案基礎配置

- [X] T001 安裝 BCrypt.Net-Next NuGet 套件至 Zheng.Bll/Zheng.Bll.csproj
- [X] T002 [P] 在 Zheng.Infrastructure/Models/ 建立 TokenType 列舉常數 (RefreshToken, EmailVerification, PasswordReset) 於 Zheng.Mdl/Enum.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 建立所有自訂實體、DbContext 配置與 EF Core Migration —— 所有 User Story 都依賴此階段完成

**⚠️ CRITICAL**: 沒有完成此階段，任何 User Story 的 Service/Controller 改寫都無法進行

### 2.1 Entity Models

- [X] T003 [P] 建立 User 實體（含 PasswordHashVersion, IsActive, 鎖定欄位等 18 個屬性）於 Zheng.Infrastructure/Models/User.cs
- [X] T004 [P] 建立 Role 實體（含 Description, ConcurrencyStamp 等 7 個屬性）於 Zheng.Infrastructure/Models/Role.cs
- [X] T005 [P] 建立 UserRole 實體（複合主鍵 UserId+RoleId, AssignedAt）於 Zheng.Infrastructure/Models/UserRole.cs
- [X] T006 [P] 建立 RolePermission 實體（RoleId, Permission, 唯一約束）於 Zheng.Infrastructure/Models/RolePermission.cs
- [X] T007 [P] 建立 UserExternalLogin 實體（LoginProvider, ProviderKey, 唯一約束）於 Zheng.Infrastructure/Models/UserExternalLogin.cs
- [X] T008 [P] 建立 UserToken 實體（TokenType, TokenValue, ExpiryDate, IsUsed, IsRevoked）於 Zheng.Infrastructure/Models/UserToken.cs

### 2.2 Fluent API Configuration

- [X] T009 [P] 建立 UserConfiguration（Fluent API 配置 unique index, 預設值, 關聯）於 Zheng.Infra.Data/Models/Configurations/UserConfiguration.cs
- [X] T010 [P] 建立 RoleConfiguration（Fluent API 配置 unique index, 關聯）於 Zheng.Infra.Data/Models/Configurations/RoleConfiguration.cs
- [X] T011 [P] 建立 UserRoleConfiguration（複合主鍵, cascade delete）於 Zheng.Infra.Data/Models/Configurations/UserRoleConfiguration.cs
- [X] T012 [P] 建立 RolePermissionConfiguration（唯一約束 RoleId+Permission）於 Zheng.Infra.Data/Models/Configurations/RolePermissionConfiguration.cs
- [X] T013 [P] 建立 UserExternalLoginConfiguration（唯一約束 LoginProvider+ProviderKey）於 Zheng.Infra.Data/Models/Configurations/UserExternalLoginConfiguration.cs
- [X] T014 [P] 建立 UserTokenConfiguration（索引 TokenValue, UserId+TokenType）於 Zheng.Infra.Data/Models/Configurations/UserTokenConfiguration.cs

### 2.3 DbContext Update

- [X] T015 更新 LibraryContext：移除舊 AspNet* DbSets，新增 Users/Roles/UserRoles/RolePermissions/UserExternalLogins/UserTokens DbSets，套用 Fluent API Configurations 於 Zheng.Infra.Data/Models/LibraryContext.cs

### 2.4 Migration

- [X] T016 建立 EF Core Migration: AddCustomIdentityTables（建立 6 張新自訂資料表）於 Library.Server/Migrations/
- [X] T017 建立 EF Core Migration: MigrateIdentityData（SQL 腳本搬移 AspNet* 資料至新表，含 GUID cast 與 PasswordHashVersion=1 設定）於 Library.Server/Migrations/

### 2.5 Core Service Interfaces

- [X] T018 [P] 建立 IAuthService 介面（ValidatePasswordAsync, HashPassword, VerifyPassword, GenerateEmailVerificationToken, VerifyEmailToken, GeneratePasswordResetToken, VerifyPasswordResetToken）於 Zheng.Bll/Interfaces/IAuthService.cs
- [X] T019 [P] 建立 IUserService 介面（FindById, FindByName, FindByEmail, GetUsers, CreateUser, UpdateUser, DeleteUser, ResetPassword, Lock/Unlock, GetRoles, SetRoles）於 Zheng.Bll/Interfaces/IUserService.cs
- [X] T020 [P] 建立 IRoleService 介面（GetRoles, GetRoleWithPermissions, CreateRole, UpdateRole, DeleteRole, GetPermissions, GetUsersInRole）於 Zheng.Bll/Interfaces/IRoleService.cs

**Checkpoint**: 資料模型與 Migration 就緒，所有 User Story 可開始實作 Service 層

---

## Phase 3: User Story 1 - 使用者帳號管理（自訂資料表） (Priority: P1) 🎯 MVP

**Goal**: 系統管理員可使用自訂資料表結構 CRUD 使用者帳號，取代 UserManager

**Independent Test**: 透過 UserService 直接操作自訂資料表，驗證 CRUD 操作正確

### Implementation for User Story 1

- [X] T021 [US1] 實作 AuthService 的密碼雜湊部分：HashPassword（BCrypt work factor=12）與 VerifyPassword（支援 V1 PBKDF2 相容 + V2 BCrypt）於 Zheng.Bll/Services/AuthService.cs
- [X] T022 [US1] 實作 UserService：FindByIdAsync, FindByNameAsync, FindByEmailAsync, GetUsersAsync（分頁+關鍵字查詢含角色 Join）, GetUserWithRolesAsync 於 Zheng.Bll/Services/UserService.cs
- [X] T023 [US1] 實作 UserService：CreateUserAsync（BCrypt 雜湊密碼, NormalizedUserName/Email 正規化, 指派角色）於 Zheng.Bll/Services/UserService.cs
- [X] T024 [US1] 實作 UserService：UpdateUserAsync, DeleteUserAsync（軟刪除 IsActive=false）, ResetPasswordAsync（BCrypt + PasswordHashVersion=2）, LockUserAsync, UnlockUserAsync 於 Zheng.Bll/Services/UserService.cs
- [X] T025 [US1] 實作 UserService：GetRolesAsync, SetRolesAsync（批次替換使用者角色）於 Zheng.Bll/Services/UserService.cs
- [X] T026 [US1] 改寫 UserController：移除 UserManager/RoleManager 注入，改注入 IUserService，所有端點改呼叫 UserService 方法，維持相同 API 回應格式 於 Library.Server/Controllers/UserController.cs
- [X] T027 [US1] 在 Program.cs 註冊 IUserService → UserService (Scoped) 於 Library.Server/Program.cs

### Tests for User Story 1

- [X] T028 [P] [US1] 撰寫 UserServiceTests：CreateUser_ValidInput_CreatesUserWithBCryptHash, GetUsers_WithPagination_ReturnsPagedResult, DeleteUser_SetsIsActiveFalse, ResetPassword_UpdatesHashAndVersion 於 Library.Server.Tests/Services/UserServiceTests.cs

**Checkpoint**: UserController 所有端點可正常運作，使用者 CRUD 經由自訂資料表完成

---

## Phase 4: User Story 2 - 使用者登入與 JWT 令牌核發 (Priority: P1) 🎯 MVP

**Goal**: 使用者可透過自訂驗證邏輯登入，系統核發 JWT Token，不依賴 SignInManager

**Independent Test**: 呼叫登入 API，驗證正確帳密回傳 JWT Token，錯誤帳密回傳錯誤訊息

### Implementation for User Story 2

- [X] T029 [US2] 實作 AuthService：ValidatePasswordAsync（查詢使用者、檢查 EmailConfirmed、檢查鎖定狀態、驗證密碼、更新 AccessFailedCount/LockoutEnd、密碼版本自動升級 V1→V2）於 Zheng.Bll/Services/AuthService.cs
- [X] T030 [US2] 改寫 JwtService：移除 UserManager<IdentityUser> 依賴，GenerateAccessTokenAsync 改接收自訂 User 實體，GetRolesAsync 改從 LibraryContext 查詢 UserRole+Role 於 Zheng.Bll/Services/JwtService.cs
- [X] T031 [US2] 更新 IJwtService 介面：GenerateAccessTokenAsync 參數從 IdentityUser 改為 User 實體 於 Zheng.Bll/Interfaces/IJwtService.cs
- [X] T032 [US2] 改寫 AccountController.Login：移除 UserManager/SignInManager 注入，改用 IAuthService.ValidatePasswordAsync + IJwtService.GenerateAccessTokenAsync，維持相同回應格式 於 Library.Server/Controllers/AccountController.cs
- [X] T033 [US2] 在 Program.cs 註冊 IAuthService → AuthService (Scoped) 於 Library.Server/Program.cs

### Tests for User Story 2

- [X] T034 [P] [US2] 撰寫 AuthServiceTests：ValidatePassword_CorrectPassword_ReturnsSuccess, ValidatePassword_WrongPassword_IncrementsFailCount, ValidatePassword_LockedAccount_ReturnsLocked, ValidatePassword_EmailNotConfirmed_ReturnsError, ValidatePassword_V1Password_UpgradesToV2 於 Library.Server.Tests/Services/AuthServiceTests.cs

**Checkpoint**: 登入 API 可正常運作，舊密碼（PBKDF2）可登入並自動升級為 BCrypt

---

## Phase 5: User Story 3 - 角色與權限管理（自訂資料表） (Priority: P2)

**Goal**: 系統管理員可 CRUD 角色、指派權限，PermissionAuthorizationHandler 從自訂資料表查詢權限

**Independent Test**: 建立角色、指派權限、指派給使用者，驗證使用者可存取受保護 API

### Implementation for User Story 3

- [X] T035 [US3] 實作 RoleService：GetRolesAsync（分頁+UserCount）, GetRoleWithPermissionsAsync, CreateRoleAsync（含批次新增 RolePermission）, UpdateRoleAsync（差異更新 Permission）, DeleteRoleAsync（檢查無使用者指派）於 Zheng.Bll/Services/RoleService.cs
- [X] T036 [US3] 實作 RoleService：GetPermissionsAsync, GetUsersInRoleAsync 於 Zheng.Bll/Services/RoleService.cs
- [X] T037 [US3] 改寫 RoleController：移除 RoleManager/UserManager 注入，改注入 IRoleService，所有端點改呼叫 RoleService 方法，維持相同 API 回應格式 於 Library.Server/Controllers/RoleController.cs
- [X] T038 [US3] 改寫 PermissionAuthorizationHandler：移除 UserManager/RoleManager 注入，改注入 LibraryContext，以單條 LINQ 查詢 UserRole→Role→RolePermission 取得權限 於 Library.Server/Filters/PermissionAuthorizationHandler.cs
- [X] T039 [US3] 在 Program.cs 註冊 IRoleService → RoleService (Scoped) 於 Library.Server/Program.cs

### Tests for User Story 3

- [X] T040 [P] [US3] 撰寫 RoleServiceTests：CreateRole_WithPermissions_InsertsRoleAndPermissions, UpdateRole_ChangedPermissions_DiffUpdate, DeleteRole_WithUsers_ReturnsError, GetRoleWithPermissions_ReturnsPermissionList 於 Library.Server.Tests/Services/RoleServiceTests.cs

**Checkpoint**: RoleController 所有端點正常運作，PermissionAuthorizationHandler 從自訂資料表查詢權限

---

## Phase 6: User Story 4 - 使用者註冊與 Email 驗證（自訂實作） (Priority: P2)

**Goal**: 使用者可自行註冊帳號，系統使用自訂 Token 機制發送驗證信，點擊連結完成驗證

**Independent Test**: 呼叫註冊 API → 確認使用者建立 → 驗證 Token 存入 UserToken → 呼叫 ConfirmEmail 完成驗證

### Implementation for User Story 4

- [X] T041 [US4] 實作 AuthService：GenerateEmailVerificationTokenAsync（HMACSHA256 Token 存入 UserToken 表, 24hr 過期）與 VerifyEmailTokenAsync（查詢 UserToken + 標記 IsUsed）於 Zheng.Bll/Services/AuthService.cs
- [X] T042 [US4] 實作 AuthService：GeneratePasswordResetTokenAsync（HMACSHA256 Token, 1hr 過期）與 VerifyPasswordResetTokenAsync 於 Zheng.Bll/Services/AuthService.cs
- [X] T043 [US4] 改寫 RegistrationService：移除 UserManager 依賴，改用 IUserService.CreateUserAsync 建立帳號 + IAuthService.GenerateEmailVerificationTokenAsync 產生 Token，維持 IEmailSender 發送驗證信 於 Zheng.Bll/Services/RegistrationService.cs
- [X] T044 [US4] 更新 IRegistrationService 介面（如有參數類型變更）於 Zheng.Bll/Interfaces/IRegistrationService.cs
- [X] T045 [US4] 改寫 AccountController.Register/ConfirmEmail/ResendVerification：移除 UserManager 依賴，改用 IRegistrationService + IAuthService，維持相同 API 回應格式 於 Library.Server/Controllers/AccountController.cs

### Tests for User Story 4

- [X] T046 [P] [US4] 撰寫 AuthServiceTests（Token 部分）：GenerateEmailToken_CreatesTokenInDB, VerifyEmailToken_ValidToken_MarksUsed, VerifyEmailToken_ExpiredToken_ReturnsFalse, VerifyEmailToken_UsedToken_ReturnsFalse 於 Library.Server.Tests/Services/AuthServiceTests.cs

**Checkpoint**: 註冊 → 驗證信 → 點擊驗證連結的完整流程可正常運作

---

## Phase 7: User Story 5 - 社群帳號（OAuth）登入整合 (Priority: P3)

**Goal**: Google/Facebook OAuth 登入改用自訂 UserExternalLogin 資料表儲存提供者資訊

**Independent Test**: 執行 OAuth 流程，驗證外部登入資訊存入 UserExternalLogin 表，使用者正確獲取 JWT Token

### Implementation for User Story 5

- [X] T047 [US5] 改寫 ExternalLoginService：移除 UserManager 依賴，改用 LibraryContext 直接查詢 UserExternalLogin 表（FindByLoginAsync → LINQ 查詢, AddLoginAsync → 直接 Insert, CreateAsync → IUserService.CreateUserAsync）於 Zheng.Bll/Services/ExternalLoginService.cs
- [X] T048 [US5] 更新 IExternalLoginService 介面（如有參數類型變更）於 Zheng.Bll/Interfaces/IExternalLoginService.cs
- [X] T049 [US5] 改寫 ExternalLoginController：移除 UserManager 依賴，確保 Callback 端點使用更新後的 ExternalLoginService + JwtService 核發 Token 於 Library.Server/Controllers/ExternalLoginController.cs

**Checkpoint**: Google/Facebook OAuth 登入完整流程正常運作，外部登入資訊儲存於自訂資料表

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: 移除 Identity 依賴、清理舊程式碼、確保全系統一致性

- [X] T050 修改 Program.cs：移除 AddIdentityApiEndpoints<IdentityUser>() 與 AddRoles<IdentityRole>() 與 AddEntityFrameworkStores<ApplicationDbContext>() 配置，移除 ApplicationDbContext 的 DI 註冊，保留 JWT Bearer + Google + Facebook Authentication 於 Library.Server/Program.cs
- [X] T051 刪除 ApplicationDbContext 類別檔案 Library.Server/Data/ApplicationDbContext .cs
- [X] T052 移除 Library.Server.csproj 中 Microsoft.AspNetCore.Identity.EntityFrameworkCore 套件引用 於 Library.Server/Library.Server.csproj
- [X] T053 清理 LibraryContext：移除舊 AspNet* 模型檔案（AspNetUser.cs, AspNetRole.cs, AspNetRoleClaim.cs, AspNetUserClaim.cs, AspNetUserLogin.cs, AspNetUserToken.cs）與 Account.cs、Token.cs 自 Zheng.Infra.Data/Models/
- [X] T054 更新 Loan 實體的外鍵關係：從 Account → 改為 User（或建立相容遷移方案）於 Zheng.Infra.Data/Models/ 與 LibraryContext
- [X] T055 完整編譯驗證：執行 dotnet build 確認零錯誤零警告
- [X] T056 執行 quickstart.md 驗證清單：確認登入、舊密碼相容、角色權限、社群登入、Email 驗證均正常

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: 無依賴 — 可立即開始
- **Foundational (Phase 2)**: 依賴 Phase 1 — **阻塞所有 User Story**
- **US1 使用者帳號管理 (Phase 3)**: 依賴 Phase 2 完成
- **US2 使用者登入 (Phase 4)**: 依賴 Phase 2 + T021（AuthService 密碼部分）
- **US3 角色與權限 (Phase 5)**: 依賴 Phase 2，可與 US1/US2 並行
- **US4 註冊與 Email 驗證 (Phase 6)**: 依賴 US1 (T022-T023 UserService) + US2 (T029 AuthService)
- **US5 OAuth 登入 (Phase 7)**: 依賴 US1 (T022 UserService) + US2 (T030 JwtService)
- **Polish (Phase 8)**: 依賴所有 User Story 完成

### User Story Dependencies

- **US1 (P1)**: Phase 2 完成後可立即開始 — 無其他 Story 依賴
- **US2 (P1)**: Phase 2 完成後可立即開始 — 依賴 T021（AuthService 密碼雜湊，屬 US1 內），建議 US1 先行
- **US3 (P2)**: Phase 2 完成後可立即開始 — 與 US1/US2 完全獨立
- **US4 (P2)**: 依賴 US1 的 UserService 與 US2 的 AuthService — 需在 US1+US2 完成後開始
- **US5 (P3)**: 依賴 US1 的 UserService 與 US2 的 JwtService — 需在 US1+US2 完成後開始

### Within Each User Story

- Service 介面（Phase 2 T018-T020）先於 Service 實作
- Service 實作先於 Controller 改寫
- Controller 改寫先於 DI 註冊
- 核心實作先於測試（非 TDD）

### Parallel Opportunities

- Phase 2 所有 Entity Models (T003-T008) 可完全並行
- Phase 2 所有 Fluent API Configurations (T009-T014) 可完全並行
- Phase 2 所有 Service 介面 (T018-T020) 可完全並行
- US1 與 US3 可並行開發（不同 Service、不同 Controller）
- 每個 Phase 的測試任務標記 [P]，可與其他 Phase 的測試並行

---

## Parallel Example: Foundational Phase

```bash
# Launch all entity models together (T003-T008):
Task: "建立 User 實體 於 Zheng.Infrastructure/Models/User.cs"
Task: "建立 Role 實體 於 Zheng.Infrastructure/Models/Role.cs"
Task: "建立 UserRole 實體 於 Zheng.Infrastructure/Models/UserRole.cs"
Task: "建立 RolePermission 實體 於 Zheng.Infrastructure/Models/RolePermission.cs"
Task: "建立 UserExternalLogin 實體 於 Zheng.Infrastructure/Models/UserExternalLogin.cs"
Task: "建立 UserToken 實體 於 Zheng.Infrastructure/Models/UserToken.cs"

# Then launch all Fluent API configs together (T009-T014):
Task: "建立 UserConfiguration 於 Configurations/UserConfiguration.cs"
Task: "建立 RoleConfiguration 於 Configurations/RoleConfiguration.cs"
# ... (all 6 configs in parallel)

# Then launch all service interfaces together (T018-T020):
Task: "建立 IAuthService 於 Zheng.Bll/Interfaces/IAuthService.cs"
Task: "建立 IUserService 於 Zheng.Bll/Interfaces/IUserService.cs"
Task: "建立 IRoleService 於 Zheng.Bll/Interfaces/IRoleService.cs"
```

## Parallel Example: US1 + US3 Simultaneously

```bash
# After Phase 2 completes, launch in parallel:
# Developer A: US1 (User Story 1 - 使用者帳號管理)
Task: "T021 實作 AuthService 密碼雜湊"
Task: "T022-T025 實作 UserService"
Task: "T026 改寫 UserController"

# Developer B: US3 (User Story 3 - 角色與權限管理)
Task: "T035-T036 實作 RoleService"
Task: "T037 改寫 RoleController"
Task: "T038 改寫 PermissionAuthorizationHandler"
```

---

## Implementation Strategy

### MVP First (US1 + US2 Only)

1. Complete Phase 1: Setup (T001-T002)
2. Complete Phase 2: Foundational (T003-T020) — CRITICAL
3. Complete Phase 3: US1 使用者帳號管理 (T021-T028)
4. Complete Phase 4: US2 使用者登入 (T029-T034)
5. **STOP and VALIDATE**: 登入 + 使用者管理可獨立運作

### Incremental Delivery

1. Setup + Foundational → 資料模型就緒
2. US1 + US2 → 登入與使用者管理可運作 (MVP!)
3. US3 → 角色權限管理上線
4. US4 → 自訂註冊與 Email 驗證上線
5. US5 → 社群登入改寫完成
6. Polish → 移除 Identity 依賴，清理舊程式碼

---

## Notes

- [P] tasks = 不同檔案、無依賴，可並行執行
- [Story] label = 對應 spec.md 的 User Story 編號
- 每個 User Story 可獨立完成與測試
- 密碼遷移採漸進升級策略（V1→V2），無需強制重設密碼
- Phase 8（Polish）在「所有 US」完成後再執行，避免過早移除 Identity 導致編譯中斷
- Program.cs 的 Identity 移除 (T050) 必須在所有 Controller 改寫完成後才能執行
