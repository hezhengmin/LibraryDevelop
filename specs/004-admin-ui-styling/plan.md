# Implementation Plan: 管理後台 UI 樣式統一

**Branch**: `004-admin-ui-styling` | **Date**: 2026-04-11 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/004-admin-ui-styling/spec.md`

## Summary

將圖書館管理系統的前端 UI（登入頁面、左側導覽選單、頂部導航欄、主佈局）全面對齊 vue3-element-admin 的視覺風格。此為純前端樣式重構，不涉及後端 API 或資料庫變更。透過更新 SCSS 變數、Layout 元件結構、登入頁面樣式，以及新增 Sidebar Logo、Navbar (hamburger + breadcrumb)、行動裝置響應式遮罩等元件，達到企業級後台管理系統的視覺一致性。

## Technical Context

**Language/Version**: TypeScript 5.x (Vue 3.5 Composition API)  
**Primary Dependencies**: Vue 3, Element Plus 2.13, Pinia 3, Vue Router 4, Axios  
**Storage**: N/A（純前端樣式變更，不涉及資料存取）  
**Testing**: Vitest + Vue Test Utils（前端元件測試）  
**Target Platform**: 桌面瀏覽器（≥ 1024px），響應式支援至行動裝置  
**Project Type**: Web application（ASP.NET Core 8 後端 + Vue 3 前端）  
**Performance Goals**: LCP ≤ 2.5s，初始 bundle ≤ 300KB gzip  
**Constraints**: 僅修改前端樣式與佈局元件，不得變更任何 API 行為或後端程式碼  
**Scale/Scope**: 約 5 個 Vue 元件 + 3 個 SCSS 檔案需修改/新增  
**CSS Stack**: SCSS + Tailwind CSS 4（via PostCSS）  
**Build Tool**: Vite（已配置 SCSS 自動匯入 variables.scss / mixins.scss）

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **程式品質**: 本功能僅涉及前端 Vue 元件與 SCSS 樣式，不觸及後端。
  前端程式碼使用 TypeScript strict mode，遵循 Vue 3 Composition API 慣例。
  不涉及資料模型或 EF Core Migrations。分層架構不受影響。
- [x] **測試標準**: 新增/修改的可複用元件（SidebarLogo、Navbar、Breadcrumb）
  將有 Vitest + Vue Test Utils 測試，涵蓋 props 驗證、事件觸發、條件渲染。
  登入頁面與佈局的視覺驗證以手動目視測試為主。
- [x] **使用者體驗一致性**: 使用 Element Plus 元件（el-menu、el-scrollbar、
  el-breadcrumb、el-dropdown），樣式以 SCSS 變數 + Tailwind CSS 實作。
  側邊欄收合/展開帶載入動畫，錯誤處理維持現有行為。
- [x] **效能需求**: 不新增 API 呼叫。新增 SCSS/CSS 預計增量極小（< 5KB gzip）。
  側邊欄過渡動畫使用 CSS transition（GPU 加速），不影響 LCP。
  不引入新的 JavaScript 依賴。

## Project Structure

### Documentation (this feature)

```text
specs/004-admin-ui-styling/
├── spec.md              # Feature specification
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output (UI component model)
├── quickstart.md        # Phase 1 output
└── contracts/           # Phase 1 output (component interface contracts)
```

### Source Code (affected files)

```text
library.client/src/
├── styles/
│   ├── variables.scss           # UPDATE: 新增/修改側邊欄、登入頁配色變數
│   ├── mixins.scss              # KEEP: 已有 flex/responsive mixins
│   └── sidebar.scss             # NEW: 側邊欄全域樣式（參考 vue3-element-admin）
├── layout/
│   ├── Layout.vue               # UPDATE: 重構為 sidebar + navbar + main 三段式佈局
│   └── components/
│       ├── Sidebar/
│       │   ├── index.vue        # NEW: 側邊欄容器（深色背景、el-scrollbar）
│       │   ├── SidebarItem.vue  # NEW: 遞迴選單項（支援嵌套子選單）
│       │   └── Logo.vue         # NEW: 側邊欄 Logo（展開/收合兩種狀態）
│       ├── Navbar.vue           # NEW: 頂部導航欄（漢堡按鈕 + 麵包屑 + 使用者下拉）
│       └── Breadcrumb.vue       # NEW: 麵包屑元件（從 route.matched 產生）
├── views/
│   └── LoginView.vue            # UPDATE: 樣式改為深色背景、暗色輸入框風格
├── stores/
│   └── app.ts                   # NEW: 應用狀態（sidebar opened/collapsed、device）
└── assets/
    └── base.css                 # KEEP: 不修改
```

**Structure Decision**: 採用 Web application 前端結構。將 Layout.vue 的單一檔案
拆分為 layout/components/ 子元件目錄，與 vue3-element-admin 的佈局元件結構對齊。
新增 stores/app.ts 管理側邊欄收合狀態與裝置偵測。

## Complexity Tracking

> No Constitution violations detected. All gates passed.
