# Tasks: 專案文件（README.md）重寫

**Input**: Design documents from `/specs/005-readme-rewrite/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, quickstart.md

**Tests**: Not requested — no automated tests. Validation via PowerShell scripts in Phase 5.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different sections, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- All tasks target single file: `docs/README.md`

---

## Phase 1: Setup

**Purpose**: Backup existing file and prepare workspace

- [x] T001 Backup current docs/README.md to docs/README.md.bak
- [x] T002 Create empty docs/README.md with document skeleton (title, section headers only) per data-model.md structure

---

## Phase 2: User Story 1 — 新開發者瞭解專案全貌 (Priority: P1) 🎯 MVP

**Goal**: 新開發者透過 README.md 在 5 分鐘內理解系統功能模組、技術架構與啟動方式

**Independent Test**: 閱讀完成後能回答：系統有哪些功能？如何啟動前後端？API 端點在哪查？

### Implementation for User Story 1

- [x] T003 [US1] Write system introduction section in docs/README.md — include system name (圖書管理系統), core features (book management, user management, role/permission, system menu, JWT + OAuth authentication), one-paragraph description per FR-001
- [x] T004 [US1] Write tech stack overview section in docs/README.md — list frontend (Vue 3.5, TypeScript 5.8, Element Plus 2.13, Pinia 3.0, Tailwind CSS 4.1, Vite 6.2) and backend (.NET 8.0, ASP.NET Core, EF Core 9.0, JWT Bearer, Google/Facebook OAuth, MailKit, Swagger) per FR-002 and research.md Task 4
- [x] T005 [US1] Write quick start section in docs/README.md — frontend commands (cd library.client, npm install, npm run dev) and backend commands (cd Library.Server, dotnet run) with dev URLs per FR-006 and research.md Task 4
- [x] T006 [US1] Write feature modules and API controller mapping table in docs/README.md — 7 controllers (Account, User, Role, Book, SysMenu, ExternalLogin, API) with route prefixes and descriptions per FR-007 and data-model.md API Controller Mapping
- [x] T007 [US1] Write project directory structure section in docs/README.md — describe Library.Server (backend), library.client (frontend), Zheng.Bll (business logic), Zheng.Mdl (data model), Library.Server.Tests (tests), docs (documentation), specs (specifications) per FR-003 and research.md Task 4

**Checkpoint**: README.md now provides complete system overview - a new developer can understand the project scope, tech stack, and how to start it

---

## Phase 3: User Story 2 — 開發者快速定位特定功能文件 (Priority: P2)

**Goal**: 開發者透過分類索引在 30 秒內找到特定功能的技術文件

**Independent Test**: 搜尋「JWT 認證」「角色權限」「前端路由」等場景，能在 30 秒內定位正確文件連結

### Implementation for User Story 2

- [x] T008 [P] [US2] Write document index category: 快速入門與系統總覽 in docs/README.md — index QUICK_START_GUIDE.md ⭐, SYSTEM_FEATURES_SUMMARY.md, UPDATE_COMPLETION_REPORT.md per research.md CAT-01
- [x] T009 [P] [US2] Write document index category: 身份認證與授權 in docs/README.md — index JWT_IMPLEMENTATION_SUMMARY.md ⭐, JWT_ERROR_FIX.md, Identity結構說明.md, CURRENTUSER_IMPLEMENTATION.md, CURRENTUSER_MULTIPLE_ROLES.md from docs/; and LOGIN_COMPLETION.md ⭐, LOGIN_ARCHITECTURE.md, LOGIN_API_INTEGRATION.md, LOGIN_INTEGRATION_UPDATE.md, LOGIN_QUICK_REFERENCE.md, LOGIN_TEST_GUIDE.md, LOGIN_UPDATE_SUMMARY.md, LOGINVIEW_QUICK_REF.md, LOGINVIEW_STYLE_UPDATE.md from docs/frontend/ per research.md CAT-02; mark ⭐ recommended per FR-009
- [x] T010 [P] [US2] Write document index category: 使用者管理 in docs/README.md — index USER_MANAGEMENT_DOCS.md ⭐, USER_COMPLETION_SUMMARY.md, USER_QUICK_START.md from docs/frontend/ per research.md CAT-03
- [x] T011 [P] [US2] Write document index category: 角色與權限管理 in docs/README.md — index ROLE_MANAGEMENT_DOCS.md ⭐, ROLE_IMPLEMENTATION_SUMMARY.md, ROLE_QUICK_START.md, PERMISSION_SYSTEM_DOCS.md ⭐, PERMISSION_QUICK_START.md, MULTI_PERMISSION_GUIDE.md, MULTI_PERMISSION_QUICK.md from docs/frontend/ per research.md CAT-04
- [x] T012 [P] [US2] Write document index category: 書籍與分頁管理 in docs/README.md — index PAGINATION_IMPLEMENTATION.md ⭐, PAGINATION_UPDATE_SUMMARY.md from docs/ per research.md CAT-05
- [x] T013 [P] [US2] Write document index category: 系統選單管理 in docs/README.md — index SYSMENU_COMPLETE_SUMMARY.md ⭐, SYSMENU_IMPLEMENTATION.md, SYSMENU_FRONTEND_IMPLEMENTATION.md, SYSMENU_QUICK_REFERENCE.md, SYSMENU_UI_IMPROVEMENTS.md, SYSMENU_UPDATE_SUMMARY.md from docs/ per research.md CAT-06
- [x] T014 [P] [US2] Write document index category: 動態路由 in docs/README.md — index DYNAMIC_ROUTING_GUIDE.md ⭐, DYNAMIC_ROUTING.md, DYNAMIC_ROUTING_QUICK_REF.md, DYNAMIC_ROUTING_SUMMARY.md, DYNAMIC_ROUTING_TEST.md, ROUTING_STRUCTURE.md from docs/ per research.md CAT-07
- [x] T015 [P] [US2] Write document index category: 前端架構與樣式 in docs/README.md — index QUICK_REFERENCE.md ⭐, ARCHITECTURE_DIAGRAM.md, IMPLEMENTATION_SUMMARY.md, LAYOUT_GUIDE.md, SCSS_GUIDE.md ⭐, SCSS_IMPLEMENTATION.md, API_INTEGRATION.md, frontend/README.md from docs/frontend/ per research.md CAT-08
- [x] T016 [P] [US2] Write document index category: 環境設定與開發工具 in docs/README.md — index ENV_CONFIG_SUMMARY.md, PROXY_CONFIG.md, PROXY_SETUP_SUMMARY.md from docs/frontend/; and DB_MCP_COPILOT_CLI.md from docs/ per research.md CAT-09
- [x] T017 [P] [US2] Write document index category: 問題修復 in docs/README.md — index BUILD_FIX.md, ENCODING_FIX.md, FIX_ADDROUTE_CORRECT_USAGE.md, FIX_BOOKS_ROUTE.md, FIX_LAYOUT_MISSING.md, FIX_VITE_DYNAMIC_IMPORT.md from docs/; and BUGFIX_COMPLETION_REPORT.md, BUGFIX_USER_MANAGEMENT.md from docs/frontend/ per research.md CAT-10
- [x] T018 [P] [US2] Write document index category: 維護記錄 in docs/README.md — index CHANGELOG.md, COMMIT_SUMMARY.md, GIT_COMMIT_REPORT.md from docs/; and CHANGELOG.md, COMPLETION_SUMMARY.md from docs/frontend/ per research.md CAT-11
- [x] T019 [US2] Write recommended reading order section in docs/README.md — provide reading paths for three roles: newcomer (QUICK_START → SYSTEM_FEATURES → frontend/QUICK_REFERENCE), frontend developer (frontend/QUICK_REFERENCE → ARCHITECTURE_DIAGRAM → SCSS_GUIDE → API_INTEGRATION), backend developer (SYSTEM_FEATURES → JWT_IMPLEMENTATION → Identity結構說明 → QUICK_START_GUIDE API section) per data-model.md

**Checkpoint**: README.md now has complete categorized index covering all 11 categories - any developer can locate specific docs in 30 seconds

---

## Phase 4: User Story 3 — 文件索引與實際檔案一致 (Priority: P3)

**Goal**: 100% 連結正確、100% 檔案被索引、零壞連結、零遺漏

**Independent Test**: 執行 PowerShell 驗證腳本，確認所有連結存在且所有檔案被索引

### Implementation for User Story 3

- [x] T020 [US3] Verify all markdown links in docs/README.md point to existing files — run PowerShell link validation script from quickstart.md Step 3, fix any broken links
- [x] T021 [US3] Verify 100% file coverage in docs/README.md — run PowerShell coverage script from quickstart.md Step 4, ensure every .md file in docs/ and docs/frontend/ (except README.md files) is indexed
- [x] T022 [US3] Add last updated date footer to docs/README.md — include date 2026-04-12 per FR-010

**Checkpoint**: All links verified, all files indexed, zero broken links, zero missing files

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup

- [x] T023 Review complete docs/README.md for consistent formatting — verify all entries follow data-model.md Document Entry format (link + ⭐ + description), consistent heading levels, proper Markdown syntax
- [x] T024 Run quickstart.md full validation — execute both PowerShell scripts (link check + coverage check) and confirm zero errors
- [x] T025 Remove backup file docs/README.md.bak after final review

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **User Story 1 (Phase 2)**: Depends on T002 (skeleton) — creates document structure
- **User Story 2 (Phase 3)**: Depends on Phase 2 completion — fills in categorized index within existing structure
- **User Story 3 (Phase 4)**: Depends on Phase 3 completion — validates final content
- **Polish (Phase 5)**: Depends on Phase 4 completion — final review

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Setup — writes overview sections (intro, tech stack, quick start, controllers, project structure)
- **User Story 2 (P2)**: Can start after US1 — writes 11 category index sections + reading order
- **User Story 3 (P3)**: Can start after US2 — validates link correctness and file coverage

### Within Each User Story

- US1: T003–T007 can run in parallel (independent sections)
- US2: T008–T018 can ALL run in parallel (independent categories), T019 depends on categories being complete
- US3: T020 → T021 → T022 sequential (validate → fix → finalize)

### Parallel Opportunities

**User Story 1 (5 parallel tasks)**:
```
T003 (intro) | T004 (tech stack) | T005 (quick start) | T006 (controllers) | T007 (project structure)
```

**User Story 2 (11 parallel tasks)**:
```
T008 (快速入門) | T009 (認證) | T010 (使用者) | T011 (角色) | T012 (書籍) | T013 (選單) | T014 (路由) | T015 (前端) | T016 (環境) | T017 (修復) | T018 (維護)
→ then T019 (閱讀順序)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T002)
2. Complete Phase 2: User Story 1 (T003–T007)
3. **STOP and VALIDATE**: README.md has system overview — new dev can understand the project
4. This alone delivers significant value over the current README

### Incremental Delivery

1. Setup → US1 → System overview ready (MVP)
2. Add US2 → Categorized file index with 65 files (full navigation)
3. Add US3 → Validated links + coverage (production quality)
4. Polish → Final formatting + cleanup

---

## Notes

- All tasks target single file: `docs/README.md`
- No code changes, no new files (except temporary backup)
- All file paths in task descriptions reference actual existing files per research.md audit
- ⭐ marks in task descriptions indicate recommended reading per research.md Task 3
- [P] tasks within US2 are independent Markdown sections — safe to write in parallel
- Commit after each phase for easy rollback
