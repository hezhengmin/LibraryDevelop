# 圖書管理系統（Library Management System）

全端圖書管理系統，提供書籍管理、使用者與角色權限控管、系統選單管理、JWT 認證與 Google / Facebook OAuth 社群登入。前端採用 Vue 3 + Element Plus 建構管理後台 SPA，後端以 ASP.NET Core Web API 搭配 Entity Framework Core (Code First) 存取 SQL Server。

---

## 技術棧

| 層級 | 技術 |
|------|------|
| **前端框架** | Vue 3.5 (Composition API)、Vue Router 4.6、Pinia 3.0 |
| **UI 元件庫** | Element Plus 2.13、@element-plus/icons-vue |
| **樣式** | Tailwind CSS 4.1、Sass (SCSS) 1.97 |
| **建置工具** | Vite 6.2、TypeScript 5.8 (strict mode) |
| **前端測試** | Vitest 4.1、@vue/test-utils 2.4 |
| **後端框架** | .NET 8.0、ASP.NET Core |
| **ORM** | Entity Framework Core 9.0 (Code First + Migrations) |
| **資料庫** | SQL Server |
| **認證** | JWT Bearer (HS256)、Google OAuth、Facebook OAuth |
| **API 文件** | Swashbuckle (Swagger / OpenAPI) 6.6 |
| **郵件** | MailKit 4.15 (SMTP) |
| **後端測試** | xUnit (InMemory DbContext) |

---

## 快速啟動

### 前置需求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQL Server（LocalDB 或完整版本）

### 資料庫設定

1. 修改 `Library.Server/appsettings.json` 中的 `ConnectionStrings:DefaultConnection`
2. 執行 Migration：

```bash
cd Library.Server
dotnet ef database update
```

### 啟動後端

```bash
cd Library.Server
dotnet run
# API: https://localhost:7019
# Swagger UI: https://localhost:7019/swagger
```

### 啟動前端

```bash
cd library.client
npm install
npm run dev
# 開發伺服器: https://localhost:5173
# Vite proxy 自動轉發 /api → https://localhost:7019
```

### 其他指令

| 指令 | 說明 |
|------|------|
| `npm run build` | 正式建置（含 TypeScript 檢查） |
| `npm run type-check` | 僅 TypeScript 型別檢查 |
| `npm run lint` | ESLint 自動修正 |
| `dotnet build` | 後端建置 |
| `dotnet test` | 執行後端測試 |

---

## 系統架構

```
┌─────────────────────────────────────────────────────────────┐
│                     Vue 3 SPA (前端)                         │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────────┐  │
│  │  Views   │ │  Stores  │ │  Router  │ │ API Modules   │  │
│  │ (11頁面) │ │ (Pinia)  │ │ (動態路由)│ │ (Axios)       │  │
│  └──────────┘ └──────────┘ └──────────┘ └───────┬───────┘  │
└────────────────────────────────────────────────────┼─────────┘
                                                     │ HTTPS /api
┌────────────────────────────────────────────────────┼─────────┐
│                ASP.NET Core Web API (後端)          │         │
│  ┌──────────────────────────────────────────────────┐       │
│  │              Controllers (7個)                    │       │
│  │  Account │ User │ Role │ Book │ SysMenu │ ...    │       │
│  └─────────────────────┬────────────────────────────┘       │
│  ┌─────────────────────┴────────────────────────────┐       │
│  │              Services (10個)                       │       │
│  │  Auth │ JWT │ User │ Role │ Book │ Permission │...│       │
│  └─────────────────────┬────────────────────────────┘       │
│  ┌─────────────────────┴────────────────────────────┐       │
│  │         GenericRepository + UnitOfWork             │       │
│  │              Entity Framework Core                 │       │
│  └─────────────────────┬────────────────────────────┘       │
└────────────────────────────────────────────────────┼─────────┘
                                                     │
                                              ┌──────┴──────┐
                                              │  SQL Server  │
                                              └─────────────┘
```

### 分層架構

嚴格遵循 **Controller → Service → Repository** 分層模式：

- **Controller**：接收 HTTP 請求、驗證權限、回傳結果。不直接存取 DbContext。
- **Service**：封裝商業邏輯。不回傳 HTTP 狀態碼。
- **Repository**：`GenericRepository<T>` 泛型資料存取 + `UnitOfWork` 交易管理。
- **Entity Framework Core**：Code First 模式，所有結構變更透過 Migration 管理。

---

## 專案結構

```
Library.sln
├── Library.Server/              # ASP.NET Core Web API
│   ├── Controllers/             # API 控制器（7 個）
│   │   ├── AccountController    #   登入、註冊、Email 驗證、Token 刷新
│   │   ├── UserController       #   使用者 CRUD（需權限）
│   │   ├── RoleController       #   角色 CRUD + 權限指派（需權限）
│   │   ├── BookController       #   書籍 CRUD + 搜尋（需權限）
│   │   ├── SysMenuController    #   系統選單 CRUD + 樹狀結構（需權限）
│   │   ├── ExternalLoginController # Google/Facebook OAuth
│   │   └── APIController        #   通用 API（下拉選單資料）
│   ├── Services/                # CurrentUser、SmtpEmailSender
│   ├── Filters/                 # ModelStateValidationFilter
│   ├── Program.cs               # DI、中介軟體、認證設定
│   └── appsettings.json         # 連線字串、JWT、OAuth、Email 設定
│
├── library.client/              # Vue 3 SPA
│   └── src/
│       ├── api/                 # Axios HTTP 模組
│       │   ├── axios.ts         #   攔截器（Token 注入、401/403 處理）
│       │   ├── account.ts       #   登入、註冊、Email 驗證
│       │   ├── user.ts          #   使用者 CRUD
│       │   ├── role.ts          #   角色 CRUD + 權限
│       │   ├── books.ts         #   書籍 CRUD
│       │   └── sysMenu.ts       #   系統選單 CRUD
│       ├── views/               # 頁面元件（11 個）
│       │   ├── LoginView        #   登入（含 OAuth 按鈕）
│       │   ├── RegisterView     #   註冊
│       │   ├── VerifyEmailView  #   Email 驗證
│       │   ├── OAuthCallbackView#   OAuth 回呼處理
│       │   ├── HomeView         #   首頁儀表板
│       │   ├── BooksView        #   書籍管理（CRUD + 搜尋 + 分頁）
│       │   ├── UserManagementView   # 使用者管理
│       │   ├── RoleManagementView   # 角色與權限管理
│       │   ├── SysMenuManagementView# 系統選單管理（樹狀結構）
│       │   ├── AboutView        #   關於
│       │   └── EmptyView        #   動態路由回退
│       ├── stores/              # Pinia 狀態管理
│       │   ├── auth.ts          #   Token、使用者資訊、登入/登出
│       │   ├── menu.ts          #   動態選單、路由載入
│       │   └── app.ts           #   側邊欄、裝置偵測
│       ├── router/              # Vue Router（靜態 + 動態路由）
│       ├── layout/              # 後台版面（Sidebar + Navbar + Main）
│       ├── components/          # 共用元件（Sidebar、Navbar）
│       └── styles/              # SCSS 全域樣式
│
├── Zheng.Bll/                   # 商業邏輯層
│   ├── Interfaces/              # 服務介面定義
│   ├── Services/                # 服務實作（10 個）
│   │   ├── AuthService          #   密碼驗證、Email Token 產生/驗證
│   │   ├── JwtService           #   JWT Access/Refresh Token
│   │   ├── UserService          #   使用者 CRUD + 分頁
│   │   ├── RoleService          #   角色 CRUD + 權限指派
│   │   ├── BookService          #   書籍 CRUD + 搜尋
│   │   ├── SysMenuService       #   系統選單 CRUD + 樹狀結構
│   │   ├── PermissionService    #   權限查詢（User → Roles → Permissions）
│   │   ├── RegistrationService  #   註冊 + Email 驗證
│   │   ├── ExternalLoginService #   OAuth 外部登入處理
│   │   └── CurrentUser          #   目前登入使用者資訊
│   ├── GenericRepository.cs     # 泛型 Repository（CRUD）
│   └── UnitOfWork.cs            # 交易管理
│
├── Zheng.Infra.Data/            # 資料存取層（EF Core）
│   ├── Models/                  # Entity 定義（14 個實體）
│   │   ├── LibraryContext.cs    #   DbContext
│   │   └── Configurations/     #   Fluent API 設定
│   └── Migrations/              # EF Core Migrations
│
├── Zheng.Mdl/                   # 共用模型層
│   ├── Models/
│   │   ├── Dto/                 #   BookDto、SysMenuDto
│   │   ├── InputModel/          #   API 輸入模型（9 個）
│   │   └── Attributes/          #   自訂驗證（OptionalEmail、OptionalPhone）
│   ├── Enum.cs                  #   StatusType（庫存）、TokenType
│   ├── PagedResult.cs           #   通用分頁結果
│   └── Result.cs                #   通用 API 回傳結果
│
├── Library.Server.Tests/        # xUnit 測試專案
│   └── Services/
│       ├── AuthServiceTests     #   認證邏輯測試
│       ├── UserServiceTests     #   使用者管理測試
│       ├── RoleServiceTests     #   角色管理測試
│       └── RegistrationServiceTests # 註冊流程測試
│
├── docs/                        # 技術文件
└── specs/                       # 功能規格（SpecKit）
```

---

## 資料模型

### 實體關係

```
User ──┬── UserRole ──── Role ──── RolePermission
       ├── UserToken
       ├── UserExternalLogin
       └── Loan ──── Book ──── BookPhoto
                               Category
                               UploadFile
```

### 實體清單

| 實體 | 資料表 | 說明 |
|------|--------|------|
| **User** | User | 使用者帳號（Email、密碼雜湊、鎖定、2FA） |
| **Role** | Role | 角色（名稱、描述） |
| **UserRole** | UserRole | 使用者-角色多對多（含指派時間） |
| **RolePermission** | RolePermission | 角色-權限對應（字串型權限碼） |
| **UserToken** | UserToken | Token 儲存（RefreshToken / EmailVerification / PasswordReset） |
| **UserExternalLogin** | UserExternalLogin | OAuth 外部登入（Google / Facebook） |
| **Book** | Book | 書籍（標題、ISBN、ISSN、GPN、出版社、價格、庫存狀態） |
| **BookPhoto** | BookPhoto | 書籍封面照片 |
| **Category** | Category | 書籍分類（支援父子層級） |
| **Loan** | Loan | 借閱記錄（借出日、到期日、歸還日） |
| **SysMenu** | SysMenu | 系統選單（樹狀結構、元件路徑、排序、顯示/隱藏） |
| **UploadFile** | UploadFile | 上傳檔案 metadata |

---

## API 端點

### AccountController — `/api/Account`（公開）

| 方法 | 路徑 | 說明 |
|------|------|------|
| POST | `/Login` | 帳號密碼登入 → accessToken + refreshToken |
| POST | `/Register` | 註冊（自動發送 Email 驗證信） |
| GET | `/ConfirmEmail` | Email 驗證確認 |
| POST | `/ResendVerification` | 重發驗證信 |

### UserController — `/api/User`（需認證 + 權限）

| 方法 | 路徑 | 權限 | 說明 |
|------|------|------|------|
| GET | `/` | Users.View | 使用者列表（分頁 + 關鍵字搜尋） |
| GET | `/{id}` | Users.View | 使用者詳細（含角色） |
| POST | `/` | Users.Create | 建立使用者 |
| PUT | `/{id}` | Users.Edit | 更新使用者 |
| DELETE | `/{id}` | Users.Delete | 刪除使用者 |
| POST | `/{id}/ResetPassword` | Users.ResetPassword | 重設密碼 |

### RoleController — `/api/Role`（需認證 + 權限）

| 方法 | 路徑 | 權限 | 說明 |
|------|------|------|------|
| GET | `/` | Roles.View | 角色列表（分頁） |
| GET | `/{id}` | Roles.View | 角色詳細（含權限清單） |
| POST | `/` | Roles.Create | 建立角色 |
| PUT | `/{id}` | Roles.Edit | 更新角色 |
| DELETE | `/{id}` | Roles.Delete | 刪除角色 |

### BookController — `/api/Book`（需認證 + 權限）

| 方法 | 路徑 | 權限 | 說明 |
|------|------|------|------|
| POST | `/GetBooks` | Books.View | 書籍列表（分頁 + 搜尋） |
| GET | `/{id}` | Books.View | 書籍詳細 |
| POST | `/` | Books.Create | 新增書籍 |
| PUT | `/{id}` | Books.Edit | 更新書籍 |
| DELETE | `/{id}` | Books.Delete | 刪除書籍 |

### SysMenuController — `/api/SysMenu`（需認證 + 權限）

| 方法 | 路徑 | 權限 | 說明 |
|------|------|------|------|
| GET | `/` | System.Menu | 選單樹狀結構 |
| GET | `/{id}` | System.Menu | 選單詳細 |
| POST | `/` | System.MenuCreate | 建立選單 |
| PUT | `/{id}` | System.MenuUpdate | 更新選單 |
| DELETE | `/{id}` | System.MenuDelete | 刪除選單 |

### ExternalLoginController — `/api/ExternalLogin`（公開）

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/Challenge/{provider}` | 導向 OAuth 登入頁（Google / Facebook） |
| GET | `/Callback/{provider}` | OAuth 回呼處理 |

---

## 認證與授權

### 認證流程

1. **帳號密碼登入**：`POST /api/Account/Login` → JWT Access Token (HS256) + Refresh Token
2. **OAuth 登入**：`/api/ExternalLogin/Challenge/Google` → Google 登入頁 → Callback → JWT
3. **Token 注入**：前端 Axios 攔截器自動在 Header 加入 `Authorization: Bearer {token}`
4. **Token 失效**：401 回應 → 前端自動導向登入頁

### 權限系統

```
使用者 → 擁有多個角色 → 每個角色擁有多個權限碼
```

**權限碼定義**：

| 模組 | 權限碼 |
|------|--------|
| Books | `Books.View`, `Books.Create`, `Books.Edit`, `Books.Delete` |
| Users | `Users.View`, `Users.Create`, `Users.Edit`, `Users.Delete`, `Users.ResetPassword` |
| Roles | `Roles.View`, `Roles.Create`, `Roles.Edit`, `Roles.Delete` |
| System | `System.Menu`, `System.Menu.Create`, `System.Menu.Update`, `System.Menu.Delete` |

Controller 方法透過 `[Permission("權限碼")]` 屬性進行授權檢查。

---

## 前端路由

### 靜態路由（無需驗證）

| 路徑 | 元件 | 說明 |
|------|------|------|
| `/login` | LoginView | 登入頁 |
| `/register` | RegisterView | 註冊頁 |
| `/verify-email` | VerifyEmailView | Email 驗證頁 |
| `/oauth/callback` | OAuthCallbackView | OAuth 回呼頁 |

### 動態路由（需驗證，從 API 載入）

登入後由 `useMenuStore` 呼叫 `/api/SysMenu` 取得選單結構，動態產生路由：

```
/ (Layout)
├── /home          → HomeView（首頁）
├── /books         → BooksView（書籍管理）
├── /users         → UserManagementView（使用者管理）
├── /roles         → RoleManagementView（角色管理）
├── /sysmenu       → SysMenuManagementView（選單管理）
└── /about         → AboutView（關於）
```

### 導航守衛

- 未驗證 → 導向 `/login`
- 已驗證訪問 `/login` → 導向 `/`
- 首次進入受保護頁面 → 載入動態路由

---

## 開發規範

詳見 [Constitution](.specify/memory/constitution.md)，摘要如下：

- **分層架構**：Controller → Service → Repository，嚴禁跨層存取
- **Code First**：Entity 以 Fluent API 設定，結構變更透過 Migration 管理
- **型別安全**：TypeScript strict mode（禁止 `any`）、C# nullable enabled
- **測試標準**：Service 覆蓋率 ≥ 80%、每個 endpoint 至少正向 + 錯誤測試
- **UI 規範**：Element Plus 為唯一 UI 庫、SCSS + Tailwind CSS、載入/錯誤狀態必須處理
- **效能**：API p95 ≤ 200ms (CRUD) / 500ms (複雜查詢)、LCP ≤ 2.5s、bundle ≤ 300KB gzip
- **Commit 規範**：Conventional Commits 格式（`feat:`, `fix:`, `docs:` ...）、繁體中文描述
- **分支策略**：`feats/<功能簡稱>` 命名

---

**最後更新**: 2026-04-12