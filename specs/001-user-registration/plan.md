# Implementation Plan: 使用者註冊服務（含電子郵件驗證與密碼加密）

**Branch**: `001-user-registration` | **Date**: 2026-03-20 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-user-registration/spec.md`

## Summary

擴充現有的註冊功能，新增電子郵件欄位、電子郵件
驗證流程與密碼確認機制。後端利用 ASP.NET Core Identity
內建的 email confirmation token 與 PBKDF2 密碼雜湊；
前端新增獨立的 RegisterView 頁面，使用 Element Plus
表單元件，風格沿用現有 LoginView。

## Technical Context

**Language/Version**: C# (.NET 8) + TypeScript 5.8 (Vue 3)
**Primary Dependencies**: ASP.NET Core Identity, Entity Framework Core 9, Element Plus, Pinia, Vue Router 4
**Storage**: SQL Server (via IdentityDbContext<IdentityUser>)
**Testing**: 無既有測試專案，需新建 xUnit 後端測試專案 + 前端 Vitest 設定
**Target Platform**: Windows / Linux server + 桌面瀏覽器 (≥ 1024px)
**Project Type**: Web application (SPA + REST API)
**Performance Goals**: 註冊 API p95 ≤ 200ms，驗證信 30s 內送達
**Constraints**: 初始 bundle ≤ 300KB gzip，無 N+1 查詢
**Scale/Scope**: 內部圖書管理系統，預計 < 1000 使用者

## Constitution Check (Pre-Design)

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **程式品質**: 註冊邏輯放在新建的 `IRegistrationService` /
  `RegistrationService`（Service 層），Controller 只處理 HTTP。
  Email 發送抽象為 `IEmailSender` 介面。TypeScript strict mode
  已啟用，C# nullable 已啟用。
- [x] **測試標準**: 將新建 xUnit 測試專案 `Library.Server.Tests`，
  針對 RegistrationService 撰寫單元測試（mock UserManager），
  覆蓋率目標 ≥ 80%。AccountController Register endpoint 需
  正向 + 錯誤情境整合測試。前端 RegisterView 需 Vitest 元件測試。
- [x] **使用者體驗一致性**: RegisterView 使用 Element Plus
  el-form / el-input / el-button，樣式沿用 LoginView SCSS。
  載入狀態用 el-button :loading，錯誤用 ElMessage。
- [x] **效能需求**: 註冊為簡單 CRUD，p95 < 200ms 可達成。
  Email 發送為非同步背景作業，不阻塞 API 回應。
  RegisterView lazy load，不影響首屏 bundle。

## Project Structure

### Documentation (this feature)

```text
specs/001-user-registration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
Library.Server/
├── Controllers/
│   └── AccountController.cs       # 擴充 Register endpoint（加 email）
├── Services/
│   ├── IEmailSender.cs            # 新增：Email 發送介面
│   └── SmtpEmailSender.cs         # 新增：SMTP 實作
├── Data/
│   └── ApplicationDbContext.cs    # 無需變更（Identity 已含 Email）
└── Program.cs                     # 新增 EmailSender DI 註冊 + Identity 設定

Zheng.Bll/
├── Interfaces/
│   └── IRegistrationService.cs    # 新增：註冊服務介面
└── Services/
    └── RegistrationService.cs     # 新增：註冊業務邏輯

Zheng.Mdl/
└── Models/
    └── InputModel/
        └── RegisterInputModel.cs  # 擴充：加 Email + ConfirmPassword

library.client/
└── src/
    ├── views/
    │   └── RegisterView.vue       # 新增：註冊頁面
    ├── api/
    │   └── account.ts             # 擴充：register 加 email 參數
    ├── router/
    │   └── index.ts               # 新增 /register 靜態路由
    └── stores/
        └── auth.ts                # 無需變更（註冊不涉及 token）

Library.Server.Tests/              # 新增：xUnit 測試專案
├── Library.Server.Tests.csproj
└── Services/
    └── RegistrationServiceTests.cs
```

**Structure Decision**: 沿用現有 Web application 結構。後端分為
Library.Server（API 層）、Zheng.Bll（業務邏輯層）、Zheng.Mdl（模型層）。
新增 Library.Server.Tests 測試專案。前端為 library.client Vue 3 SPA。

## Complexity Tracking

> No Constitution Check violations. All four principles are met by design.

## Constitution Check (Post-Design Re-evaluation)

- [x] **程式品質**: PASS — `IRegistrationService` 在 Zheng.Bll，Controller 只處理
  HTTP。`IEmailSender` 介面抽象 email 發送。無 Controller 直存 DbContext。
- [x] **測試標準**: PASS — 新建 `Library.Server.Tests` xUnit 專案。
  RegistrationService 單元測試 mock UserManager/IEmailSender。
  AccountController 整合測試覆蓋 Register / ConfirmEmail / ResendVerification。
- [x] **使用者體驗一致性**: PASS — RegisterView 使用 Element Plus el-form，
  風格繼承 LoginView。VerifyEmailView 同風格。載入/錯誤處理符合規範。
- [x] **效能需求**: PASS — 註冊為簡單 INSERT，p95 < 200ms。Email 非阻塞。
  RegisterView lazy load 不影響初始 bundle。Rate limit 用 IMemoryCache（O(1)）。
