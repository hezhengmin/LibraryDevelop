<!--
  Sync Impact Report
  ===========================================================
  Version change: 1.1.1 → 1.1.2 (PATCH — 分支命名規範)
  Modified principles: N/A
  Added sections: N/A
  Removed sections: N/A
  Modified rules:
    - Development Workflow > 分支策略：新增 `feats/<功能簡稱>` 命名規範
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ verified (無需修改)
    - .specify/templates/spec-template.md ✅ verified (無需修改)
    - .specify/templates/tasks-template.md ✅ verified (無需修改)
    - .specify/templates/commands/ ✅ verified (空目錄)
  Follow-up TODOs: N/A
  ===========================================================
-->

# 圖書管理系統 (Library) Constitution

## Core Principles

### I. 程式品質 (Code Quality)

所有程式碼 MUST 遵守以下品質標準：

- **分層架構**：嚴格遵循 Controller → Service → Repository
  分層模式。Controller 不得直接存取 DbContext，
  Service 不得回傳 HTTP 狀態碼。
- **Code First 資料存取**：所有資料模型 MUST 以 C# class
  定義為唯一事實來源（Single Source of Truth），禁止使用
  Database First scaffold 產生模型。資料庫結構變更 MUST
  透過 EF Core Migrations 管理（`dotnet ef migrations add`
  / `dotnet ef database update`）。每個 Entity MUST 使用
  Fluent API（`IEntityTypeConfiguration<T>`）配置對應關係
  與約束，禁止僅依賴 Data Annotations 處理複雜映射。
  Migration 檔案 MUST 納入版本控制，禁止手動修改已套用
  至共用環境的 Migration。
- **型別安全**：前端 TypeScript MUST 啟用 strict mode，
  禁止使用 `any` 型別；後端 C# MUST 啟用 nullable
  reference types。
- **命名規範**：後端遵循 .NET 命名慣例（PascalCase
  for public members），前端遵循 Vue 3 Composition API
  慣例（camelCase for variables, PascalCase for
  components）。
- **程式碼審查**：每次合併 MUST 經過至少一人審查，
  確認符合架構分層與命名規範。
- **靜態分析**：前端 MUST 通過 ESLint 檢查且零錯誤；
  後端 MUST 通過編譯且零警告（已排除者除外）。
- **單一職責**：每個 class / composable / component
  MUST 只負責一項職責，避免 God Object。

**Rationale**: 分層架構與型別安全是大型系統可維護性的
基石。Code First 讓資料模型與應用程式碼同步演進，
Migration 提供可追蹤、可回溯的結構變更歷史。統一的
命名規範降低認知負擔，靜態分析在編譯期攔截錯誤，
減少執行期故障。

### II. 測試標準 (Testing Standards)

所有功能變更 MUST 具備對應的測試覆蓋：

- **後端單元測試**：Service 層與 Repository 層的商業
  邏輯 MUST 有對應的 xUnit / NUnit 測試，覆蓋率
  目標 ≥ 80%。
- **API 整合測試**：每個 Controller endpoint MUST 有
  至少一組正向情境與一組錯誤情境的整合測試。
- **前端元件測試**：可複用元件 MUST 使用 Vitest +
  Vue Test Utils 編寫測試，涵蓋 props 驗證、事件
  觸發、條件渲染。
- **迴歸防護**：Bug 修復 MUST 附帶重現該 bug 的測試
  案例，確保不再復發。
- **測試命名**：測試方法名稱 MUST 採用
  `MethodName_Scenario_ExpectedResult` 格式，
  清楚描述測試意圖。

**Rationale**: 測試是品質的安全網。沒有測試的程式碼
等同於未被驗證的假設。統一的測試命名讓失敗報告
一目了然，迴歸測試防止已修復的問題再次出現。

### III. 使用者體驗一致性 (UX Consistency)

所有使用者介面 MUST 維持一致的視覺與互動體驗：

- **設計系統**：MUST 使用 Element Plus 作為唯一 UI
  元件庫，禁止混用其他第三方 UI 框架。自訂元件
  MUST 遵循 Element Plus 的設計語言。
- **樣式規範**：MUST 使用 SCSS + Tailwind CSS
  進行樣式開發。全域主題色、間距、字型 MUST 定義
  於共用 SCSS 變數檔，禁止行內硬編碼色碼。
- **響應式設計**：所有頁面 MUST 支援 ≥ 1024px 寬度
  的桌面瀏覽器。管理後台無需支援行動裝置，但
  SHOULD 在 1280px 以上提供最佳體驗。
- **載入狀態**：所有非同步操作 MUST 顯示明確的載入
  指示器（skeleton / spinner / progress bar），
  禁止讓使用者面對空白畫面等待。
- **錯誤處理**：API 錯誤 MUST 以 Element Plus
  Message 或 Notification 元件呈現使用者友善的
  錯誤訊息，禁止暴露原始錯誤堆疊。
- **國際化準備**：所有使用者可見文字 SHOULD 集中管理
  於語系檔，為未來多語支援做準備。

**Rationale**: 一致的使用者體驗降低學習曲線，提升
使用者信任感。統一的設計系統避免風格碎片化，
明確的載入與錯誤處理是專業應用的基本要求。

### IV. 效能需求 (Performance Requirements)

系統 MUST 滿足以下效能基準：

- **API 回應時間**：一般 CRUD 操作 MUST 在 200ms
  內回應（p95）；複雜查詢（含分頁、篩選、排序）
  MUST 在 500ms 內回應（p95）。
- **前端首屏載入**：生產環境首屏 LCP（Largest
  Contentful Paint）MUST ≤ 2.5 秒。
- **打包體積**：前端生產建置的初始 JavaScript bundle
  MUST ≤ 300KB（gzip 後）。超過時 MUST 使用
  code splitting 與 lazy loading 拆分。
- **資料庫查詢**：禁止在迴圈中執行 N+1 查詢。
  所有列表查詢 MUST 使用分頁（預設每頁 ≤ 50 筆）。
  複雜查詢 MUST 建立適當的資料庫索引。
- **記憶體管理**：前端元件卸載時 MUST 清除所有
  訂閱、計時器與事件監聽器，避免記憶體洩漏。
- **快取策略**：不常變動的參考資料（如選單、角色
  列表）SHOULD 實施前端快取，減少重複 API 呼叫。

**Rationale**: 效能直接影響使用者滿意度與系統可擴展性。
明確的量化指標讓效能標準可測試、可追蹤，而非淪為
模糊的期望。

## Technology Stack (技術堆疊)

以下為本專案核心技術堆疊，任何變更 MUST 經過團隊
討論並更新本文件：

| Layer | Technology |
|-------|------------|
| Frontend Framework | Vue 3 + TypeScript (Composition API) |
| UI Library | Element Plus |
| State Management | Pinia |
| Routing | Vue Router 4 |
| Build Tool | Vite |
| CSS | SCSS + Tailwind CSS 4 |
| Backend Framework | ASP.NET Core 8 (.NET 8) |
| ORM | Entity Framework Core 9 (Code First + Migrations) |
| Database | SQL Server |
| Authentication | JWT + ASP.NET Core Identity |
| API Documentation | Swashbuckle (Swagger) |

## Development Workflow (開發流程)

- **分支策略**：功能開發 MUST 從 main 分支建立
  feature branch，分支命名 MUST 採用
  `feats/<功能簡稱>` 格式（例如
  `feats/user-registration`）。合併前 MUST
  通過所有 CI 檢查。
- **Commit 規範**：MUST 採用 Conventional Commits
  格式（`feat:`, `fix:`, `docs:`, `refactor:`,
  `test:`, `perf:`, `chore:`）。Commit 描述 MUST
  使用繁體中文撰寫（例如
  `feat: 新增使用者註冊功能`）。
- **建置驗證**：合併前 MUST 確認前後端均可成功建置
  （`npm run build` + `dotnet build`）且無型別錯誤。
- **Migration 流程**：資料模型變更 MUST 先建立
  Migration（`dotnet ef migrations add <Name>`），
  在本機驗證 `dotnet ef database update` 成功後
  才提交。Migration 名稱 MUST 使用描述性名稱
  （如 `AddBookIsbnColumn`），禁止使用流水號或
  日期作為唯一識別。合併衝突時 MUST 重新產生
  Migration，禁止手動合併 Migration 檔案。

## Governance

本 Constitution 為本專案最高層級的開發準則，
所有開發活動 MUST 遵循本文件所定義的原則。

### Amendment Procedure

- 任何原則修訂 MUST 以 PR 提出，附帶變更理由。
- 新增或刪除原則視為 MAJOR 版本變更。
- 原則內容的實質擴充視為 MINOR 版本變更。
- 措辭修正、筆誤修復視為 PATCH 版本變更。

### Versioning Policy

採用語意化版本 MAJOR.MINOR.PATCH：

- **MAJOR**: 原則的新增、刪除或根本性重新定義。
- **MINOR**: 現有原則的實質擴充或新章節的加入。
- **PATCH**: 措辭澄清、筆誤修正、非語意性調整。

### Compliance Review

- 每次 Code Review MUST 檢查是否符合四大核心原則。
- 若有原則衝突，MUST 在 PR 中明確說明豁免理由。

**Version**: 1.1.2 | **Ratified**: 2026-03-20 | **Last Amended**: 2026-04-11
