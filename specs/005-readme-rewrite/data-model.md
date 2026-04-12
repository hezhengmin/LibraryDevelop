# Data Model: README.md 內容結構

**Feature**: 005-readme-rewrite  
**Date**: 2026-04-12

> 本功能不涉及資料庫實體。此文件定義 README.md 的內容結構（即「文件資料模型」）。

## Entity: README Section（README 章節）

README.md 由以下區塊組成，順序固定：

| 區塊 | 必須 | 內容說明 |
|------|------|----------|
| 標題與簡介 | ✅ | 系統名稱、一段話描述核心功能 |
| 技術棧 | ✅ | 前後端框架與主要套件版本 |
| 快速啟動 | ✅ | 前端 `npm install && npm run dev`、後端 `dotnet run` |
| 功能模組概覽 | ✅ | 7 個 API Controller 對照表 |
| 文件索引 | ✅ | 9 大分類，每個文件一行（含 ⭐ 推薦標示） |
| 專案目錄結構 | ✅ | 6 個頂層專案資料夾用途說明 |
| 推薦閱讀順序 | ✅ | 按角色（新手 / 前端 / 後端）的閱讀路線 |
| 最後更新 | ✅ | 日期 |

## Entity: Document Entry（文件條目）

每個文件在索引中以固定格式呈現：

| 欄位 | 規則 |
|------|------|
| 連結文字 | 文件標題（正體中文） |
| 路徑 | 相對於 docs/ 的路徑（如 `QUICK_START_GUIDE.md`、`frontend/SCSS_GUIDE.md`）|
| 推薦標示 | 系列中最完整的文件加 ⭐ |
| 簡述 | 一行描述文件內容 |

格式範例：
```markdown
- [JWT 實作總結](JWT_IMPLEMENTATION_SUMMARY.md) ⭐ — JWT Token 認證機制完整說明
```

## Entity: Document Category（文件分類）

| 分類 ID | 分類名稱 | docs/ 數量 | frontend/ 數量 |
|---------|----------|------------|----------------|
| CAT-01 | 快速入門與系統總覽 | 3 | 0 |
| CAT-02 | 身份認證與授權 | 5 | 10 |
| CAT-03 | 使用者管理 | 0 | 3 |
| CAT-04 | 角色與權限管理 | 0 | 7 |
| CAT-05 | 書籍與分頁管理 | 2 | 0 |
| CAT-06 | 系統選單管理 | 6 | 0 |
| CAT-07 | 動態路由 | 6 | 0 |
| CAT-08 | 前端架構與樣式 | 0 | 8 |
| CAT-09 | 環境設定與代理 | 1 | 3 |
| CAT-10 | 問題修復 | 5 | 2 |
| CAT-11 | 維護記錄 | 3 | 2 |

## Entity: API Controller Mapping（API 控制器對照）

| Controller | 路由前綴 | 功能說明 |
|------------|----------|----------|
| AccountController | /api/Account | 登入、註冊、Email 驗證、Token 刷新 |
| UserController | /api/User | 使用者 CRUD、分頁查詢 |
| RoleController | /api/Role | 角色 CRUD、權限指派、分頁查詢 |
| BookController | /api/Book | 書籍 CRUD、搜尋、分頁、照片上傳 |
| SysMenuController | /api/SysMenu | 系統選單 CRUD、樹狀結構、導覽選單 |
| ExternalLoginController | /api/ExternalLogin | Google/Facebook OAuth 登入 |
| APIController | /api/API | 通用 API（下拉選單資料等） |

## Validation Rules

- 所有連結路徑必須通過 `Test-Path` 驗證
- 每個 docs/*.md 和 docs/frontend/*.md 檔案（README.md 除外）都必須在索引中出現
- ⭐ 標示每個系列最多 1-2 個
- 分類內檔案按「推薦優先 → 其餘字母順序」排列
