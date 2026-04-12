# Feature Specification: 專案文件（README.md）重寫

**Feature Branch**: `005-readme-rewrite`  
**Created**: 2026-04-12  
**Status**: Draft  
**Input**: User description: "README.md 請幫我重寫文件，再重看一次專案裡面的內容"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - 新開發者瞭解專案全貌 (Priority: P1)

一位新加入的開發者打開專案，透過 docs/README.md 在 5 分鐘內理解系統包含哪些功能模組、技術架構，以及如何啟動開發環境。

**Why this priority**: 文件入口是所有開發者第一個接觸的資源。如果無法快速掌握專案全貌，後續所有開發工作都會延遲。

**Independent Test**: 將重寫後的 README.md 交給一位未接觸過此專案的開發者閱讀，確認其能在 5 分鐘內回答：系統有哪些主要功能？如何啟動前後端？API 端點在哪裡查詢？

**Acceptance Scenarios**:

1. **Given** 一位從未看過此專案的開發者，**When** 開啟 docs/README.md，**Then** 能在首段落看見系統簡介、技術棧說明、以及後端 7 個控制器對應的功能模組（帳號、使用者、角色、書籍、系統選單、外部登入、API 通用）
2. **Given** README.md 已正確列出所有功能模組，**When** 開發者點選任一功能模組對應的詳細文件連結，**Then** 連結指向的檔案確實存在且內容與連結描述相符
3. **Given** README.md 包含啟動指引區塊，**When** 開發者依照步驟執行，**Then** 能正確啟動前端（Vue 3 dev server）與後端（ASP.NET Core）

---

### User Story 2 - 開發者快速定位特定功能文件 (Priority: P2)

一位正在開發特定功能的開發者（例如：書籍管理、角色權限）需要快速找到對應的技術文件，而不需要瀏覽 30+ 個無明確分類的 Markdown 檔案。

**Why this priority**: 現有 docs/ 目錄包含超過 40 個文件，缺乏清晰分類。開發者需要一個分類索引來快速定位所需文件，減少搜尋時間。

**Independent Test**: 針對以下常見查詢場景，開發者能在 README 索引中 30 秒內找到正確文件連結：JWT 認證、書籍 CRUD、角色權限、前端路由設定、API 整合。

**Acceptance Scenarios**:

1. **Given** README.md 按功能模組分類所有文件，**When** 開發者搜尋「角色」相關文件，**Then** 在角色與權限分類下找到 ROLE_MANAGEMENT_DOCS.md、ROLE_QUICK_START.md、MULTI_PERMISSION_GUIDE.md 等相關文件
2. **Given** README.md 包含前端與後端文件分類，**When** 前端開發者只想看前端相關文件，**Then** 前端文件區塊集中列出所有 frontend/ 目錄下的相關文件

---

### User Story 3 - 文件索引與實際檔案一致 (Priority: P3)

專案維護者需要確認 README.md 中列出的所有文件連結都指向實際存在的檔案，且所有 docs/ 中的檔案都被索引收錄。

**Why this priority**: 目前 README.md 遺漏了多個已存在的文件（如 CURRENTUSER_IMPLEMENTATION.md、DYNAMIC_ROUTING 系列等），導致開發者不知道這些文件存在。

**Independent Test**: 逐一檢查 README.md 中的所有連結是否指向真實存在的檔案；反向檢查 docs/ 下所有 .md 檔案是否都被 README.md 索引到。

**Acceptance Scenarios**:

1. **Given** README.md 已列出所有文件，**When** 逐一驗證每個連結，**Then** 100% 連結指向存在的檔案
2. **Given** docs/ 包含 27 個根目錄文件與 frontend/ 下 34 個文件，**When** 與 README.md 索引交叉比對，**Then** 每個 .md 檔案都有對應的索引條目（README.md 本身除外）

---

### Edge Cases

- 如果文件已過時或內容與系統現狀不符，README.md 應標註該文件的最後更新日期或狀態標記（如「可能過時」）
- 如果某些文件內容高度重疊（如多個 LOGIN 系列文件），README.md 應標示推薦閱讀文件，避免開發者閱讀冗餘內容
- 如果未來新增功能文件，README.md 的分類結構應易於擴充，不需重新組織

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: README.md 必須包含系統簡介，說明這是一套圖書管理系統，涵蓋書籍管理、使用者管理、角色權限管理、系統選單管理、身份認證（JWT + OAuth）等核心功能
- **FR-002**: README.md 必須列出完整的技術棧概覽，包含前端（Vue 3、TypeScript、Element Plus、Pinia、Tailwind CSS / SCSS）與後端（ASP.NET Core、Entity Framework Core、JWT、Identity）
- **FR-003**: README.md 必須包含專案目錄結構說明，標示 Library.Server（後端）、library.client（前端）、Zheng.Bll（商業邏輯層）、Zheng.Mdl（資料模型層）、Library.Server.Tests（測試）、docs（文件）、specs（規格）的用途
- **FR-004**: README.md 必須按功能模組分類索引所有 docs/ 及 docs/frontend/ 下的文件，分類至少包含：身份認證與授權、使用者管理、角色與權限管理、書籍管理、系統選單管理、前端架構與樣式、動態路由、環境設定、問題修復與維護記錄
- **FR-005**: README.md 中所有文件連結必須使用正確的相對路徑，且指向實際存在的檔案
- **FR-006**: README.md 必須包含快速啟動指引，說明前後端啟動步驟
- **FR-007**: README.md 必須反映目前系統已實作的 API 控制器：AccountController（帳號登入/註冊）、UserController（使用者 CRUD）、RoleController（角色 CRUD）、BookController（書籍 CRUD）、SysMenuController（系統選單）、ExternalLoginController（OAuth 外部登入）、APIController（通用 API）
- **FR-008**: README.md 必須涵蓋目前未被索引但已存在的文件，包含 CURRENTUSER_IMPLEMENTATION.md、CURRENTUSER_MULTIPLE_ROLES.md、DB_MCP_COPILOT_CLI.md、DYNAMIC_ROUTING 系列文件、ROUTING_STRUCTURE.md、FIX 系列文件等
- **FR-009**: README.md 對於多篇內容重疊的文件（如 LOGIN 系列 8 篇、SYSMENU 系列 6 篇），必須標示推薦優先閱讀的文件
- **FR-010**: README.md 必須包含最後更新日期

### Non-Functional Requirements (Constitution Alignment)

- **程式品質**: 遵循 Controller → Service → Repository 分層，
  TypeScript strict / C# nullable enabled，通過靜態分析。
  資料模型採用 Code First，結構變更透過 EF Core Migrations 管理。
- **測試標準**: Service/Repository 單元測試覆蓋率 ≥ 80%，
  Controller endpoint 含正向與錯誤整合測試，bug fix 附迴歸測試。
- **使用者體驗一致性**: 使用 Element Plus 元件，SCSS + Tailwind CSS
  樣式，顯示載入狀態與使用者友善錯誤訊息。
- **效能需求**: API p95 ≤ 200ms (CRUD) / 500ms (複雜查詢)，
  LCP ≤ 2.5s，初始 bundle ≤ 300KB gzip，無 N+1 查詢。

## Assumptions

- 所有 docs/ 及 docs/frontend/ 下現有的 .md 檔案均為有效文件，不需刪除
- README.md 為文件索引入口，不包含功能實作細節，僅提供概覽與連結
- 文件語言維持正體中文（與現有文件一致）
- 專案結構與功能模組以目前程式碼為準（7 個 Controller、10 個 Service、11 個前端頁面）
- 啟動指引僅需涵蓋本地開發環境，不涉及部署至正式環境

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 新開發者閱讀 README.md 後，5 分鐘內能說出系統包含的所有功能模組
- **SC-002**: README.md 索引涵蓋 docs/ 下 100% 的 .md 檔案（README.md 本身除外），零遺漏
- **SC-003**: README.md 中 100% 的文件連結指向實際存在的檔案，零壞連結
- **SC-004**: 開發者在 30 秒內能透過分類索引定位到特定功能的文件
- **SC-005**: 有重疊內容的文件系列（LOGIN、SYSMENU 等）均標示推薦優先閱讀的文件，減少開發者閱讀冗餘內容的時間
