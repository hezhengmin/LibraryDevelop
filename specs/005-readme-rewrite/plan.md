# Implementation Plan: 專案文件（README.md）重寫

**Branch**: `005-readme-rewrite` | **Date**: 2026-04-12 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/005-readme-rewrite/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

重寫 `docs/README.md`，基於完整的專案探索結果（7 個 Controller、10 個 Service、11 個前端頁面、65 個文件），建立一份準確的文件索引，涵蓋系統簡介、技術棧、快速啟動指引、按 9 大功能模組分類的文件索引（100% 檔案覆蓋）、專案目錄結構。

## Technical Context

**Language/Version**: Markdown（純文件，無程式碼變更）  
**Primary Dependencies**: N/A（僅修改 docs/README.md）  
**Storage**: N/A  
**Testing**: 手動驗證連結正確性 + 檔案覆蓋率  
**Target Platform**: GitHub / 本地 Markdown 閱讀器  
**Project Type**: 文件重寫（documentation-only）  
**Performance Goals**: N/A  
**Constraints**: 所有連結必須為相對路徑、指向實際存在的檔案  
**Scale/Scope**: 1 個檔案（docs/README.md），索引 65 個 .md 檔案

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **程式品質**: N/A — 本功能僅修改 Markdown 文件，不涉及程式碼變更。
  文件結構遵循清楚的分類邏輯，易於維護。
- [x] **測試標準**: 以手動交叉比對取代自動化測試 —
  驗證 100% 連結指向存在的檔案、100% docs/ 檔案被索引。
- [x] **使用者體驗一致性**: 文件使用正體中文、統一的 Markdown 格式，
  以 ⭐ 標示推薦閱讀文件，減少資訊過載。
- [x] **效能需求**: N/A — 純文件，無效能影響。

## Project Structure

### Documentation (this feature)

```text
specs/005-readme-rewrite/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output — 完整檔案稽核與分類結果
├── data-model.md        # Phase 1 output — README.md 內容結構定義
├── quickstart.md        # Phase 1 output — 實作快速參考
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
docs/
├── README.md            # ← 本功能唯一修改的檔案
├── *.md                 # 32 個根目錄文件（只讀，不修改）
└── frontend/
    └── *.md             # 33 個前端文件（只讀，不修改）
```

**Structure Decision**: 本功能為純文件重寫，僅修改 `docs/README.md`。
不新增、不刪除、不移動任何現有文件。

## Complexity Tracking

無 Constitution violation。本功能為純文件變更，不涉及架構、效能、測試等技術面。
