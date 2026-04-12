# Implementation Plan: 自訂身分驗證資料表結構

**Branch**: `003-custom-identity-tables` | **Date**: 2026-04-11 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/003-custom-identity-tables/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

將現有 ASP.NET Core Identity 預設資料表（AspNet* 系列 7 張表）全面替換為自訂資料表結構，移除對 `UserManager<IdentityUser>`、`RoleManager<IdentityRole>`、`SignInManager<IdentityUser>` 的依賴，改以自訂 Service 層封裝所有身分驗證與授權邏輯。密碼雜湊改用 BCrypt，Token 機制改用 HMACSHA256 自訂生成，同時保持所有 API 端點介面與現有前端完全相容。

## Technical Context

**Language/Version**: C# 12 (.NET 8) / TypeScript 5.x  
**Primary Dependencies**: ASP.NET Core 8, EF Core 9, BCrypt.Net-Next, Vue 3, Element Plus, Pinia  
**Storage**: SQL Server (via EF Core Code First + Migrations)  
**Testing**: xUnit (後端), Vitest + Vue Test Utils (前端)  
**Target Platform**: Windows Server / IIS / Kestrel  
**Project Type**: Web application (SPA frontend + REST API backend)  
**Performance Goals**: API p95 ≤ 200ms (CRUD) / 500ms (複雜查詢), LCP ≤ 2.5s  
**Constraints**: 前端 API 介面必須向下相容（前端零修改）、既有資料必須完整遷移  
**Scale/Scope**: 現有 7 張 Identity 資料表替換為 6 張自訂資料表，影響 4 個 Controller、5 個 Service、1 個 AuthorizationHandler

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **程式品質**: 符合分層架構（Controller → Service → Repository），
  TypeScript strict mode、C# nullable enabled，通過靜態分析。
  資料模型採用 Code First，結構變更透過 EF Core Migrations 管理。
  — *所有新實體以 C# class 定義，使用 Fluent API 配置，透過 Migration 建立。Controller 不直接操作 DbContext，改由自訂 Service 封裝。*
- [x] **測試標準**: 定義測試計畫，Service/Repository 單元測試覆蓋率
  ≥ 80%，Controller endpoint 含正向與錯誤情境整合測試。
  — *新增 AuthService、UserService、RoleService 單元測試，AccountController/UserController/RoleController 整合測試覆蓋登入成功/失敗、鎖定、權限驗證等情境。*
- [x] **使用者體驗一致性**: 使用 Element Plus 設計系統，載入狀態與
  錯誤處理符合規範，樣式使用 SCSS + Tailwind CSS。
  — *前端不需修改（API 介面完全相容），使用者體驗無變化。*
- [x] **效能需求**: API p95 ≤ 200ms（CRUD）/ 500ms（複雜查詢），
  LCP ≤ 2.5s，初始 bundle ≤ 300KB gzip，無 N+1 查詢。
  — *自訂查詢可使用 Include/Select 精確控制載入，避免 N+1。使用者角色與權限查詢加入適當索引。*

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Zheng.Infrastructure/Models/        # 新增自訂實體 (Code First)
├── User.cs                         # 取代 AspNetUser
├── Role.cs                         # 取代 AspNetRole
├── UserRole.cs                     # 取代 AspNetUserRoles
├── RolePermission.cs               # 取代 AspNetRoleClaim
├── UserExternalLogin.cs            # 取代 AspNetUserLogin
└── UserToken.cs                    # 整合 Token + AspNetUserToken

Zheng.Bll/                          # 改寫 Service 層
├── Interfaces/
│   ├── IAuthService.cs             # 新增：自訂驗證服務介面
│   ├── IUserService.cs             # 新增：使用者管理服務介面
│   ├── IRoleService.cs             # 新增：角色管理服務介面
│   ├── IJwtService.cs              # 修改：移除 UserManager 依賴
│   ├── IRegistrationService.cs     # 修改：移除 UserManager 依賴
│   └── IExternalLoginService.cs    # 修改：移除 UserManager 依賴
├── Services/
│   ├── AuthService.cs              # 新增：登入驗證、密碼雜湊、鎖定邏輯
│   ├── UserService.cs              # 新增：使用者 CRUD
│   ├── RoleService.cs              # 新增：角色 CRUD + 權限管理
│   ├── JwtService.cs               # 修改：改用自訂 User 實體
│   ├── RegistrationService.cs      # 修改：改用 AuthService 建立帳號
│   └── ExternalLoginService.cs     # 修改：改用自訂實體
└── Repositories/                   # 如已有 GenericRepository 則直接使用

Library.Server/
├── Data/
│   └── ApplicationDbContext.cs     # 修改：移除 IdentityDbContext 繼承，改用 DbContext
├── Controllers/
│   ├── AccountController.cs        # 修改：改注入 IAuthService
│   ├── UserController.cs           # 修改：改注入 IUserService
│   └── RoleController.cs           # 修改：改注入 IRoleService
├── Filters/
│   └── PermissionAuthorizationHandler.cs  # 修改：改從自訂資料表查詢
├── Services/
│   └── CurrentUser.cs              # 不變（讀取 JWT Claims）
└── Migrations/
    └── *_CustomIdentityTables.cs   # 新增 Migration

Library.Server.Tests/
└── Services/
    ├── AuthServiceTests.cs         # 新增
    ├── UserServiceTests.cs         # 新增
    └── RoleServiceTests.cs         # 新增
```

**Structure Decision**: 沿用現有多專案分層架構。新實體放在 `Zheng.Infrastructure/Models/`，新 Service 放在 `Zheng.Bll/`，Controller 層僅替換注入依賴。`ApplicationDbContext` 從 `IdentityDbContext<IdentityUser>` 改為一般 `DbContext`，統一管理自訂實體。

## Complexity Tracking

> 無 Constitution 違規項目。本次改寫嚴格遵循既有分層架構，僅替換資料存取來源。
