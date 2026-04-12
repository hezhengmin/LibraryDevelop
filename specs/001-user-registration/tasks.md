# Tasks: 使用者註冊服務（含電子郵件驗證與密碼加密）

**Input**: Design documents from `/specs/001-user-registration/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api.md, quickstart.md

**Tests**: Included — spec NFR explicitly requires testing standards compliance.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `Library.Server/`, `Zheng.Bll/`, `Zheng.Mdl/`
- **Frontend**: `library.client/src/`
- **Tests**: `Library.Server.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 安裝新套件、建立測試專案、設定 Email 組態

- [x] T001 Add MailKit NuGet package to `Library.Server/Library.Server.csproj`
- [x] T002 [P] Create xUnit test project `Library.Server.Tests/Library.Server.Tests.csproj` and add to `Library.sln`
- [x] T003 [P] Add EmailSettings configuration section to `Library.Server/appsettings.json` and `Library.Server/appsettings.Development.json`
- [x] T004 [P] Install Vitest and Vue Test Utils dev dependencies in `library.client/package.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 建立所有 User Story 共用的介面、模型與 DI 註冊

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T005 Extend `RegisterInputModel` with Email and ConfirmPassword properties in `Zheng.Mdl/Models/InputModel/RegisterInputModel.cs`
- [x] T006 [P] Create `IEmailSender` interface in `Zheng.Bll/Interfaces/IEmailSender.cs`
- [x] T007 [P] Implement `SmtpEmailSender` using MailKit in `Library.Server/Services/SmtpEmailSender.cs`
- [x] T008 Create `IRegistrationService` interface in `Zheng.Bll/Interfaces/IRegistrationService.cs`
- [x] T009 Configure Identity options (RequireConfirmedEmail, token lifespan 24h) and register EmailSender DI in `Library.Server/Program.cs`

**Checkpoint**: Foundation ready — user story implementation can now begin

---

## Phase 3: User Story 1 — 使用者自助註冊帳號 (Priority: P1) 🎯 MVP

**Goal**: 使用者可透過前端表單填寫帳號/Email/密碼完成註冊，帳號成功寫入 DB

**Independent Test**: 填寫註冊表單 → 提交 → 看到「註冊成功」訊息 → DB 中可查到帳號

- [x] T010 [US1] Implement `RegistrationService.RegisterAsync` (create user with email, generate confirmation token, send email) in `Zheng.Bll/Services/RegistrationService.cs`
- [x] T011 [US1] Refactor `AccountController.Register` to use `IRegistrationService` and accept new RegisterInputModel in `Library.Server/Controllers/AccountController.cs`
- [x] T012 [P] [US1] Update frontend `register` API function to include email and confirmPassword in `library.client/src/api/account.ts`
- [x] T013 [US1] Create `RegisterView.vue` with el-form (userName, email, password, confirmPassword) in `library.client/src/views/RegisterView.vue`
- [x] T014 [P] [US1] Add `/register` static route to Vue Router in `library.client/src/router/index.ts`
- [x] T015 [US1] Wire LoginView "立即註冊" link to navigate to `/register` in `library.client/src/views/LoginView.vue`
- [x] T016 [US1] Write RegistrationService unit tests (success, duplicate username, duplicate email, password validation) in `Library.Server.Tests/Services/RegistrationServiceTests.cs`

---

## Phase 4: User Story 2 — 電子郵件驗證 (Priority: P2)

**Goal**: 使用者點擊驗證信連結後帳號標記為已驗證，未驗證帳號無法登入

**Independent Test**: 註冊 → 收到驗證信 → 點擊連結 → 帳號變為已驗證 → 可成功登入

- [x] T017 [US2] Implement `RegistrationService.ConfirmEmailAsync` (validate token, mark email confirmed) in `Zheng.Bll/Services/RegistrationService.cs`
- [x] T018 [US2] Add `ConfirmEmail` GET endpoint to `AccountController` in `Library.Server/Controllers/AccountController.cs`
- [x] T019 [US2] Modify `AccountController.Login` to handle `IsNotAllowed` (unverified email) with specific error response in `Library.Server/Controllers/AccountController.cs`
- [x] T020 [P] [US2] Create `VerifyEmailView.vue` (display verification result, link to login) in `library.client/src/views/VerifyEmailView.vue`
- [x] T021 [P] [US2] Add `/verify-email` static route to Vue Router in `library.client/src/router/index.ts`
- [x] T022 [US2] Update LoginView to show "請先完成電子郵件驗證" message and resend link when login returns requiresEmailVerification in `library.client/src/views/LoginView.vue`
- [x] T023 [US2] Write ConfirmEmail unit tests (valid token, expired token, already confirmed, invalid user) in `Library.Server.Tests/Services/RegistrationServiceTests.cs`

---

## Phase 5: User Story 3 — 重新發送驗證信 (Priority: P3)

**Goal**: 使用者可請求重新發送驗證信，系統實施頻率限制避免濫用

**Independent Test**: 請求重發 → 收到新驗證信 → 1 分鐘內再次請求被拒 → 等待後可再次發送

- [x] T024 [US3] Implement `RegistrationService.ResendVerificationAsync` with IMemoryCache rate limiting (1 req/min per email) in `Zheng.Bll/Services/RegistrationService.cs`
- [x] T025 [US3] Add `ResendVerification` POST endpoint to `AccountController` (no account enumeration) in `Library.Server/Controllers/AccountController.cs`
- [x] T026 [P] [US3] Add `resendVerification` API function in `library.client/src/api/account.ts`
- [x] T027 [US3] Add resend verification UI to LoginView (email input + resend button, shown when login returns unverified) in `library.client/src/views/LoginView.vue`
- [x] T028 [US3] Write ResendVerification unit tests (success, rate limited, non-existent email, already verified) in `Library.Server.Tests/Services/RegistrationServiceTests.cs`

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 最終整合、安全性驗證、建置確認

- [x] T029 [P] Verify all frontend form validations (email format, password min length, confirm match) work with Element Plus rules in `library.client/src/views/RegisterView.vue`
- [x] T030 [P] Verify backend model validation annotations (Required, EmailAddress, StringLength, Compare) match frontend rules in `Zheng.Mdl/Models/InputModel/RegisterInputModel.cs`
- [x] T031 Run `dotnet build` for entire solution and fix any compilation errors
- [x] T032 Run `npm run build` and `npm run type-check` in `library.client/` and fix any TypeScript errors
- [x] T033 Run `dotnet test` in `Library.Server.Tests/` and verify all tests pass

---

## Dependencies

```text
Phase 1 (Setup)
  └── Phase 2 (Foundational)
        ├── Phase 3 (US1: 自助註冊) 🎯 MVP
        │     └── Phase 4 (US2: 電子郵件驗證)
        │           └── Phase 5 (US3: 重新發送驗證信)
        └── Phase 6 (Polish) — after all user stories
```

### User Story Dependencies

| Story | Depends On | Can Start After |
|-------|-----------|-----------------|
| US1 (P1) | Phase 2 | Phase 2 complete |
| US2 (P2) | US1 (needs Register to create users) | T010, T011 complete |
| US3 (P3) | US2 (needs ConfirmEmail flow) | T017, T018 complete |

### Parallel Execution Opportunities

**Within Phase 1**:
- T002 + T003 + T004 can run in parallel (independent projects/configs)

**Within Phase 2**:
- T006 + T007 can run in parallel with T005 (different files)

**Within Phase 3 (US1)**:
- T012 + T014 can run in parallel (different frontend files)
- Frontend tasks (T012-T015) can start after T005 is done (model known)
- Backend (T010-T011) and frontend (T012-T015) can partially overlap

**Within Phase 4 (US2)**:
- T020 + T021 can run in parallel (different frontend files)

**Within Phase 5 (US3)**:
- T026 can run in parallel with T024 (different layers)

## Implementation Strategy

### MVP Scope (Phase 1–3)

Deliver **User Story 1** first as the MVP:
- Users can register with username + email + password
- Registration form with validation
- Account created in DB with email confirmation token generated
- Verification email sent (requires SMTP config)

### Incremental Delivery

1. **Increment 1** (Phase 1–3): Registration works end-to-end → Demo: user registers, sees success message
2. **Increment 2** (Phase 4): Email verification works → Demo: click link, account verified, can login
3. **Increment 3** (Phase 5): Resend flow works → Demo: request resend, receive new email, rate limited
4. **Increment 4** (Phase 6): All polish, validation alignment, build verification
