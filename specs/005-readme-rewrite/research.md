# Research: 專案文件（README.md）重寫

**Feature**: 005-readme-rewrite  
**Date**: 2026-04-12

## Research Task 1: 現有檔案完整清單與分類

### Decision: 9 大功能分類

按功能模組將 docs/ 下 65 個 .md 檔案歸類為 9 大類別。

### Findings

#### docs/ 根目錄（32 個檔案）

| 分類 | 數量 | 檔案 |
|------|------|------|
| 身份認證與授權 | 6 | CURRENTUSER_IMPLEMENTATION.md, CURRENTUSER_MULTIPLE_ROLES.md, JWT_IMPLEMENTATION_SUMMARY.md, JWT_ERROR_FIX.md, Identity結構說明.md |
| 系統選單管理 | 6 | SYSMENU_COMPLETE_SUMMARY.md ⭐, SYSMENU_IMPLEMENTATION.md, SYSMENU_FRONTEND_IMPLEMENTATION.md, SYSMENU_QUICK_REFERENCE.md, SYSMENU_UI_IMPROVEMENTS.md, SYSMENU_UPDATE_SUMMARY.md |
| 動態路由 | 6 | DYNAMIC_ROUTING.md, DYNAMIC_ROUTING_GUIDE.md, DYNAMIC_ROUTING_QUICK_REF.md, DYNAMIC_ROUTING_SUMMARY.md, DYNAMIC_ROUTING_TEST.md, ROUTING_STRUCTURE.md |
| 分頁與資料管理 | 2 | PAGINATION_IMPLEMENTATION.md ⭐, PAGINATION_UPDATE_SUMMARY.md |
| 問題修復 | 5 | BUILD_FIX.md, ENCODING_FIX.md, FIX_ADDROUTE_CORRECT_USAGE.md, FIX_BOOKS_ROUTE.md, FIX_LAYOUT_MISSING.md, FIX_VITE_DYNAMIC_IMPORT.md |
| 快速入門與總覽 | 3 | QUICK_START_GUIDE.md ⭐, SYSTEM_FEATURES_SUMMARY.md, UPDATE_COMPLETION_REPORT.md |
| 開發工具 | 1 | DB_MCP_COPILOT_CLI.md |
| 維護記錄 | 3 | CHANGELOG.md, COMMIT_SUMMARY.md, GIT_COMMIT_REPORT.md |

#### docs/frontend/（33 個檔案）

| 分類 | 數量 | 檔案 |
|------|------|------|
| 登入功能 | 10 | LOGIN_API_INTEGRATION.md, LOGIN_ARCHITECTURE.md, LOGIN_COMPLETION.md ⭐, LOGIN_INTEGRATION_UPDATE.md, LOGIN_QUICK_REFERENCE.md, LOGIN_TEST_GUIDE.md, LOGIN_UPDATE_SUMMARY.md, LOGINVIEW_QUICK_REF.md, LOGINVIEW_STYLE_UPDATE.md |
| 角色與權限 | 7 | ROLE_MANAGEMENT_DOCS.md ⭐, ROLE_IMPLEMENTATION_SUMMARY.md, ROLE_QUICK_START.md, PERMISSION_SYSTEM_DOCS.md ⭐, PERMISSION_QUICK_START.md, MULTI_PERMISSION_GUIDE.md, MULTI_PERMISSION_QUICK.md |
| 使用者管理 | 3 | USER_MANAGEMENT_DOCS.md ⭐, USER_COMPLETION_SUMMARY.md, USER_QUICK_START.md |
| 前端架構 | 8 | ARCHITECTURE_DIAGRAM.md, IMPLEMENTATION_SUMMARY.md, LAYOUT_GUIDE.md, QUICK_REFERENCE.md ⭐, README.md, SCSS_GUIDE.md ⭐, SCSS_IMPLEMENTATION.md, API_INTEGRATION.md |
| 環境與設定 | 3 | ENV_CONFIG_SUMMARY.md, PROXY_CONFIG.md, PROXY_SETUP_SUMMARY.md |
| 問題修復 | 2 | BUGFIX_COMPLETION_REPORT.md, BUGFIX_USER_MANAGEMENT.md |
| 维護記錄 | 2 | CHANGELOG.md, COMPLETION_SUMMARY.md |

### Alternatives Considered
- 按字母排列：不利於開發者按功能查找
- 按建立日期排列：文件日期不一致，無法準確排序

---

## Research Task 2: 現有 README.md 的索引缺口

### Decision: 現有 README 遺漏 46 個檔案

現有 README.md 僅索引約 19 個檔案（約 29%），遺漏 46 個。

### Findings

**高優先補充**（核心功能文件）：
- CURRENTUSER_IMPLEMENTATION.md, CURRENTUSER_MULTIPLE_ROLES.md — ICurrentUser 服務
- DYNAMIC_ROUTING 系列 5 篇 — 動態路由核心功能
- ROUTING_STRUCTURE.md — Vue Router 結構
- MULTI_PERMISSION_GUIDE.md, MULTI_PERMISSION_QUICK.md — 多權限模式
- LOGIN_QUICK_REFERENCE.md, LOGIN_TEST_GUIDE.md — 登入參考
- FIX 系列 4 篇 — 重要問題修復模式

**中優先補充**（輔助文件）：
- DB_MCP_COPILOT_CLI.md — 開發工具設定
- LOGINVIEW_QUICK_REF.md, LOGINVIEW_STYLE_UPDATE.md — 前端樣式
- BUGFIX_COMPLETION_REPORT.md, BUGFIX_USER_MANAGEMENT.md — 問題修復

**低優先補充**（可能過時）：
- CHANGELOG.md, COMMIT_SUMMARY.md, GIT_COMMIT_REPORT.md — 單次記錄
- frontend/CHANGELOG.md, frontend/COMPLETION_SUMMARY.md — Vite 自動產生

---

## Research Task 3: 重疊文件的推薦優先閱讀

### Decision: 為每個系列標示 ⭐ 推薦文件

| 系列 | 推薦文件 | 理由 |
|------|----------|------|
| LOGIN（10 篇） | LOGIN_COMPLETION.md | 最完整的登入功能總結 |
| SYSMENU（6 篇） | SYSMENU_COMPLETE_SUMMARY.md | 已標示為最完整說明 |
| DYNAMIC_ROUTING（6 篇） | DYNAMIC_ROUTING_GUIDE.md | 實作步驟指南，最實用 |
| ROLE/PERMISSION（7 篇） | ROLE_MANAGEMENT_DOCS.md + PERMISSION_SYSTEM_DOCS.md | 各自涵蓋不同面向 |
| USER（3 篇） | USER_MANAGEMENT_DOCS.md | 最完整的使用者管理文件 |
| SCSS（2 篇） | SCSS_GUIDE.md | 概念完整，SCSS_IMPLEMENTATION 為補充 |

---

## Research Task 4: 技術棧與啟動指引

### Decision: 記錄精確的版本與啟動方式

### Frontend
- **框架**: Vue 3.5.13, Vue Router 4.6.4, Pinia 3.0.4
- **UI**: Element Plus 2.13.0, @element-plus/icons-vue 2.3.2
- **樣式**: Tailwind CSS 4.1.18, Sass 1.97.1
- **建置**: Vite 6.2.4, TypeScript 5.8.0
- **測試**: Vitest 4.1.0, @vue/test-utils 2.4.6
- **Lint**: ESLint 9.22.0

**啟動指令**:
```bash
cd library.client
npm install
npm run dev          # https://localhost:5173
npm run build        # 正式建置
npm run type-check   # TypeScript 檢查
npm run lint         # ESLint 修正
```

### Backend
- **框架**: .NET 8.0, ASP.NET Core
- **ORM**: Entity Framework Core 9.0.6 (SQL Server)
- **認證**: JWT Bearer 8.0.11, Google/Facebook OAuth 8.0.11
- **API 文件**: Swashbuckle (Swagger) 6.6.2
- **郵件**: MailKit 4.15.1

**啟動指令**:
```bash
cd Library.Server
dotnet run           # https://localhost:7019
```

### 專案結構
```
Library.sln
├── Library.Server/       # ASP.NET Core 後端 API
│   ├── Controllers/      # 7 個 API 控制器 + WeatherForecast
│   ├── Services/         # CurrentUser, SmtpEmailSender
│   └── Filters/          # 授權過濾器
├── library.client/       # Vue 3 前端 SPA
│   └── src/
│       ├── api/          # Axios API 模組 (account, books, role, sysMenu, user)
│       ├── views/        # 11 個頁面
│       ├── stores/       # Pinia stores (app, auth, menu)
│       ├── router/       # 動態路由
│       ├── layout/       # 管理後台版面
│       ├── components/   # 共用元件
│       └── styles/       # SCSS 樣式
├── Zheng.Bll/            # 商業邏輯層
│   ├── Services/         # 10 個服務 (Auth, Book, JWT, Role, User, SysMenu, Permission, Registration, ExternalLogin, CurrentUser)
│   ├── Interfaces/       # 服務介面
│   ├── GenericRepository.cs
│   └── UnitOfWork.cs
├── Zheng.Mdl/            # 資料模型層
│   ├── Models/           # DTO, InputModel, Attributes
│   ├── Enum.cs           # StatusType, TokenType
│   ├── PagedResult.cs
│   └── Result.cs
├── Library.Server.Tests/ # xUnit 測試
│   └── Services/         # AuthService, UserService, RoleService, RegistrationService 測試
├── docs/                 # 技術文件（65 個 .md）
└── specs/                # 功能規格
```

---

## Research Task 5: README.md 結構設計

### Decision: 採用「概覽 → 快速啟動 → 分類索引 → 專案結構」結構

### Rationale
- 開發者首先需要知道「這是什麼」→ 系統簡介
- 其次是「如何運行」→ 快速啟動
- 然後是「深入特定功能」→ 按模組分類的文件索引
- 最後是「專案怎麼組織」→ 目錄結構

### Structure
1. 標題與系統簡介
2. 技術棧概覽
3. 快速啟動（前後端啟動步驟）
4. 功能模組與 API 控制器對照
5. 文件索引（9 大分類，含 ⭐ 推薦標示）
   - 5.1 快速入門與系統總覽
   - 5.2 身份認證與授權
   - 5.3 使用者管理
   - 5.4 角色與權限管理
   - 5.5 書籍管理
   - 5.6 系統選單管理
   - 5.7 動態路由
   - 5.8 前端架構與樣式
   - 5.9 環境設定與代理
   - 5.10 問題修復
   - 5.11 維護記錄
6. 專案目錄結構
7. 推薦閱讀順序（按角色）
8. 最後更新日期
