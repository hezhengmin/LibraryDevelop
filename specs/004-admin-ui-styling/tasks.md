# Tasks: 管理後台 UI 樣式統一

**Input**: Design documents from `/specs/004-admin-ui-styling/`
**Prerequisites**: plan.md (required), spec.md (required for user stories)

**Tests**: 本功能以純視覺 / 樣式變更為主，spec 未要求自動化測試。不生成測試任務。

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 建立全域 CSS 變數、SCSS 設定與應用狀態管理

- [X] T001 Define CSS custom properties for sidebar theming in library.client/src/styles/variables.scss
- [X] T002 [P] Create sidebar global styles in library.client/src/styles/sidebar.scss (sidebar-container, menu hover, collapse transition, scrollbar)
- [X] T003 [P] Create app store for sidebar state and device detection in library.client/src/stores/app.ts
- [X] T004 Import sidebar.scss into global styles entry in library.client/src/styles/index.scss or library.client/src/main.ts

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 拆分 Layout.vue 為子元件目錄結構，建立共用佈局骨架

**⚠️ CRITICAL**: 此階段完成後各 User Story 的元件才能各自獨立開發

- [X] T005 Create layout components directory structure at library.client/src/layout/components/
- [X] T006 Refactor library.client/src/layout/Layout.vue to use sidebar + navbar + main-container three-section structure with CSS class bindings (hideSidebar, openSidebar, mobile, withoutAnimation)
- [X] T007 Add resize mixin/composable for device detection (mobile < 768px) in library.client/src/layout/composables/useResizeHandler.ts

**Checkpoint**: Layout skeleton ready — sidebar, navbar, and main-container slots defined. User story phases can now begin.

---

## Phase 3: User Story 1 — 登入頁面視覺升級 (Priority: P1) 🎯 MVP

**Goal**: 登入頁面改為深色全屏背景、暗色輸入框、白色文字，與 vue3-element-admin 登入頁視覺一致

**Independent Test**: 開啟 /login 路由，目視確認深色背景 (#2d3a4b)、置中表單、白色文字、暗色輸入框樣式

### Implementation for User Story 1

- [X] T008 [US1] Restyle login container background and form layout in library.client/src/views/LoginView.vue `<style>` section — full-screen dark background (#2d3a4b), centered form (max-width 520px, padding-top 160px)
- [X] T009 [US1] Override Element Plus input styles for dark login theme in library.client/src/views/LoginView.vue — transparent input background, white text/caret, dark form-item borders (rgba(255,255,255,0.1)), remove card glass effect
- [X] T010 [US1] Restyle login title, subtitle, and all text elements to light colors (white/#eee) in library.client/src/views/LoginView.vue
- [X] T011 [US1] Restyle social login buttons and register link for dark background consistency in library.client/src/views/LoginView.vue
- [X] T012 [US1] Add responsive styles for login form (max-width 100% on < 640px, adjust padding) in library.client/src/views/LoginView.vue

**Checkpoint**: 登入頁面應呈現深色後台風格，所有現有功能（登入、社群登入、註冊連結、email 驗證提醒）仍正常運作

---

## Phase 4: User Story 2 — 左側導覽選單樣式升級 (Priority: P1)

**Goal**: 左側固定深色側邊欄，含 Logo 區塊、深色選單、收合/展開動畫、滑鼠懸停高亮

**Independent Test**: 登入後檢視左側側邊欄，確認深色背景、Logo、收合/展開、滑鼠高亮

### Implementation for User Story 2

- [X] T013 [P] [US2] Create SidebarLogo component in library.client/src/layout/components/Sidebar/Logo.vue — show full title when expanded, icon/short text when collapsed, with fade transition
- [X] T014 [P] [US2] Create SidebarItem recursive component in library.client/src/layout/components/Sidebar/SidebarItem.vue — handle single/nested menu items, resolve paths, render el-menu-item and el-sub-menu
- [X] T015 [US2] Create Sidebar container component in library.client/src/layout/components/Sidebar/index.vue — dark background (#304156), el-scrollbar, el-menu with menuBg/menuText/menuActiveText colors, integrate Logo and SidebarItem
- [X] T016 [US2] Wire Sidebar into Layout.vue — replace current inline sidebar markup with Sidebar component, bind isCollapse from app store
- [X] T017 [US2] Add sidebar collapse toggle button in sidebar footer area in library.client/src/layout/components/Sidebar/index.vue
- [X] T018 [US2] Style active menu item highlight (--menu-active-text color) and hover states (--menu-hover background) via sidebar.scss variables in library.client/src/styles/sidebar.scss

**Checkpoint**: 側邊欄為深色風格，展開寬度 210px、收合 54px，Logo 顯示正確，滑鼠懸停高亮，收合/展開有 0.28s 過渡動畫

---

## Phase 5: User Story 3 — 頂部導航欄與主佈局升級 (Priority: P2)

**Goal**: 固定白色頂部導航欄，含漢堡按鈕、麵包屑導覽、使用者頭像下拉選單

**Independent Test**: 登入後檢視頂部導航欄，確認漢堡按鈕切換側邊欄、麵包屑顯示正確、右側使用者下拉正常

### Implementation for User Story 3

- [X] T019 [P] [US3] Create Breadcrumb component in library.client/src/layout/components/Breadcrumb.vue — generate breadcrumb items from $route.matched, use el-breadcrumb, with transition animation
- [X] T020 [P] [US3] Create Hamburger toggle component in library.client/src/layout/components/Hamburger.vue — SVG icon that rotates on toggle, emits click event
- [X] T021 [US3] Create Navbar component in library.client/src/layout/components/Navbar.vue — fixed white header, Hamburger (left), Breadcrumb (left), user avatar + el-dropdown (right, with logout action)
- [X] T022 [US3] Wire Navbar into Layout.vue — replace current inline header markup with Navbar component, connect hamburger toggle to app store sidebar state
- [X] T023 [US3] Style fixed header positioning in Layout.vue — width calc(100% - sidebarWidth), transition width 0.28s, adjust for hideSidebar (calc(100% - 54px))

**Checkpoint**: 頂部導航欄固定、白色背景，漢堡按鈕可切換側邊欄，麵包屑與使用者下拉選單正常

---

## Phase 6: User Story 4 — 行動裝置響應式佈局 (Priority: P3)

**Goal**: 行動裝置上側邊欄覆蓋式展開，搭配半透明遮罩

**Independent Test**: 將瀏覽器窗口縮至 < 768px，確認側邊欄覆蓋式展開並帶遮罩

### Implementation for User Story 4

- [X] T024 [US4] Add mobile overlay mask (drawer-bg) in library.client/src/layout/Layout.vue — semi-transparent black background, click to close sidebar, conditional rendering when device === 'mobile' && sidebar.opened
- [X] T025 [US4] Integrate useResizeHandler composable in Layout.vue — auto-detect screen width, toggle device state, auto-close sidebar on mobile route change
- [X] T026 [US4] Add mobile-specific CSS in library.client/src/layout/Layout.vue and library.client/src/styles/sidebar.scss — sidebar position fixed with z-index 1001, mobile .fixed-header width 100%, sidebar transition transform for mobile

**Checkpoint**: 行動裝置上側邊欄覆蓋式展開、點擊遮罩或導覽後自動收合

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: 跨 Story 改進與最終收尾

- [X] T027 [P] Add CSS transition for sidebar collapse animation (width 0.28s) in library.client/src/styles/sidebar.scss
- [X] T028 [P] Ensure el-menu collapse-transition is disabled (use CSS transition only) in library.client/src/layout/components/Sidebar/index.vue
- [X] T029 Verify all existing page views (HomeView, BooksView, UserManagementView, RoleManagementView, SysMenuManagementView) render correctly within new layout in library.client/src/layout/Layout.vue
- [X] T030 Run quickstart.md visual validation — compare login page and main layout screenshots against vue3-element-admin reference

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 (T001, T003 provide variables and store)
- **US1 (Phase 3)**: Depends on Phase 1 only (T001 for color variables) — can start in parallel with Phase 2
- **US2 (Phase 4)**: Depends on Phase 2 (T005, T006 for layout skeleton)
- **US3 (Phase 5)**: Depends on Phase 2 (T006 for layout skeleton)
- **US4 (Phase 6)**: Depends on Phase 2 (T007 for resize composable) and Phase 4 (T016 sidebar wiring)
- **Polish (Phase 7)**: Depends on Phases 3–6 completion

### User Story Dependencies

- **US1 (登入頁面)**: Independent — only needs Phase 1 color variables
- **US2 (側邊欄)**: Independent after Foundational — no other story dependencies
- **US3 (頂部導航欄)**: Independent after Foundational — works with or without US2 sidebar styling
- **US4 (響應式)**: Depends on US2 (sidebar component must exist to apply mobile overlay)

### Within Each User Story

- Parallel [P] tasks within same story can run simultaneously (different files)
- Non-[P] tasks must run sequentially within their story
- Each story's Checkpoint must be verified before proceeding

### Parallel Opportunities

- Phase 1: T002 and T003 can run in parallel (different files)
- Phase 3 (US1) can start in parallel with Phase 2 (Foundational)
- Phase 4: T013 and T014 can run in parallel (different files)
- Phase 5: T019 and T020 can run in parallel (different files)
- Phase 4 (US2) and Phase 5 (US3) can start in parallel once Phase 2 completes
- Phase 7: T027 and T028 can run in parallel (different files)

---

## Parallel Example: User Story 2

```bash
# Launch parallel tasks for US2:
Task T013: "Create SidebarLogo in layout/components/Sidebar/Logo.vue"
Task T014: "Create SidebarItem in layout/components/Sidebar/SidebarItem.vue"

# Then sequential tasks:
Task T015: "Create Sidebar container (depends on T013, T014)"
Task T016: "Wire Sidebar into Layout.vue (depends on T015)"
Task T017: "Add collapse toggle button (depends on T015)"
Task T018: "Style active/hover states (depends on T015)"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T004)
2. Complete Phase 3: US1 — 登入頁面 (T008–T012)
3. **STOP and VALIDATE**: 登入頁面視覺與 vue3-element-admin 一致
4. 可選：立即部署/展示 MVP

### Incremental Delivery

1. Setup + US1 → 登入頁面升級完成 → Demo (MVP!)
2. Foundational + US2 → 側邊欄升級完成 → Demo
3. US3 → 頂部導航欄完成 → Demo
4. US4 → 行動裝置支援完成 → Demo
5. Polish → 最終品質驗證

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup (Phase 1) together
2. Once Setup is done:
   - Developer A: US1 (登入頁面) — immediate start
   - Developer B: Foundational (Phase 2) — immediate start
3. Once Foundational completes:
   - Developer A (if free): US3 (導航欄)
   - Developer B: US2 (側邊欄)
4. US4 after US2 sidebar is wired

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- 本功能不涉及後端 API 或資料庫變更，所有任務均為前端 Vue/SCSS 修改
- 參考色值：登入背景 #2d3a4b、側邊欄背景 #304156、選單文字 #fff、active 文字 #409EFF
