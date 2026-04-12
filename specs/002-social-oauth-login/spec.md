# Feature Specification: 第三方社群登入（Google OAuth、Facebook OAuth）

**Feature Branch**: `002-social-oauth-login`
**Created**: 2026-03-22
**Status**: Draft
**Input**: User description: "建立第三方社群登入功能（Google OAuth、Facebook OAuth），讓使用者可透過 Google 或 Facebook 帳号直接登入圖書管理系統"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - 使用者透過 Google 帳號登入 (Priority: P1)

一位使用者進入圖書管理系統的登入頁面，在傳統帳密登入
表單下方看到「使用 Google 登入」按鈕。點擊後系統導向
Google 授權頁面，使用者選擇或登入其 Google 帳號並同意
授權。授權完成後使用者被導回圖書管理系統，系統自動建立
或關聯帳號，使用者直接進入系統首頁，無需額外填寫資料。

**Why this priority**: Google 是全球最廣泛使用的登入方式之一，
整合 Google 登入可大幅降低註冊門檻，讓使用者快速開始使用
系統。

**Independent Test**: 可透過點擊 Google 登入按鈕、完成 Google
授權流程、確認使用者成功進入系統並取得有效登入狀態來獨立
驗證。

**Acceptance Scenarios**:

1. **Given** 使用者在登入頁面，**When** 點擊「使用 Google
   登入」按鈕，**Then** 系統導向 Google 授權頁面。
2. **Given** 使用者在 Google 授權頁面完成授權，**When**
   Google 將使用者導回系統，**Then** 系統建立或關聯帳號、
   產生登入 token，並將使用者導向首頁。
3. **Given** 使用者已曾用 Google 登入建立帳號，**When**
   再次使用相同 Google 帳號登入，**Then** 系統辨識為同一
   使用者，直接登入而不會建立重複帳號。
4. **Given** 使用者在 Google 授權頁面，**When** 使用者
   點擊「取消」或拒絕授權，**Then** 系統導回登入頁面並
   顯示「授權已取消」提示訊息。

---

### User Story 2 - 使用者透過 Facebook 帳號登入 (Priority: P2)

一位使用者在登入頁面看到「使用 Facebook 登入」按鈕。
點擊後系統導向 Facebook 授權頁面，使用者登入 Facebook
帳號並同意授權。完成後使用者被導回圖書管理系統，系統
自動建立或關聯帳號，使用者直接進入系統首頁。

**Why this priority**: Facebook 是另一主流社群平台，提供
Facebook 登入可覆蓋更廣泛的使用者群體。優先級低於
Google 因為整體架構與 Google 相同，第二個供應商的整合
相對容易。

**Independent Test**: 可透過點擊 Facebook 登入按鈕、完成
Facebook 授權流程、確認使用者成功進入系統來獨立驗證。

**Acceptance Scenarios**:

1. **Given** 使用者在登入頁面，**When** 點擊「使用 Facebook
   登入」按鈕，**Then** 系統導向 Facebook 授權頁面。
2. **Given** 使用者在 Facebook 授權頁面完成授權，**When**
   Facebook 將使用者導回系統，**Then** 系統建立或關聯帳號、
   產生登入 token，並將使用者導向首頁。
3. **Given** 使用者已曾用 Facebook 登入建立帳號，**When**
   再次使用相同 Facebook 帳號登入，**Then** 系統辨識為同一
   使用者，直接登入而不會建立重複帳號。
4. **Given** 使用者在 Facebook 授權頁面，**When** 使用者
   拒絕授權，**Then** 系統導回登入頁面並顯示「授權已取消」
   提示訊息。

---

### User Story 3 - 社群帳號與既有帳號關聯 (Priority: P3)

一位使用者先前已透過傳統帳密方式註冊帳號，現在希望也能
用 Google 或 Facebook 登入。當使用者用社群帳號登入時，
若該社群帳號的電子郵件地址與系統中已存在的帳號吻合，
系統自動將社群登入關聯至既有帳號，而非建立新帳號。

**Why this priority**: 確保已註冊使用者不會因使用社群登入
而產生重複帳號，維持帳號資料的一致性。此功能在前兩個
故事完成後才有意義。

**Independent Test**: 可透過先用帳密方式建立帳號（使用
特定電子郵件），再用擁有相同電子郵件的 Google/Facebook
帳號登入，確認系統使用同一帳號而非建立新帳號來獨立驗證。

**Acceptance Scenarios**:

1. **Given** 系統已有一筆以 `user@example.com` 註冊的帳號，
   **When** 使用者以 Google 帳號（電子郵件為
   `user@example.com`）登入，**Then** 系統將 Google 登入
   關聯至既有帳號，使用者看到的是原有帳號資料。
2. **Given** 系統已有一筆以 `user@example.com` 註冊的帳號，
   **When** 使用者以 Facebook 帳號（電子郵件為
   `user@example.com`）登入，**Then** 系統將 Facebook 登入
   關聯至既有帳號。
3. **Given** 使用者已透過 Google 關聯帳號，**When**
   使用者使用帳密方式登入相同帳號，**Then** 帳密登入
   仍可正常使用，兩種登入方式共存。

---

### Edge Cases

- 社群帳號未提供電子郵件（如 Facebook 使用者未授權電子
  郵件權限），系統 MUST 提示使用者需授權電子郵件才能
  完成登入。
- 使用者用 Google 登入後，再用 Facebook 登入（兩者使用
  相同電子郵件），系統 MUST 將兩個社群登入都關聯至同一
  帳號。
- 社群登入供應商服務暫時無法連線，系統 MUST 顯示使用者
  友善的錯誤訊息，並提示可使用其他登入方式。
- 授權回呼中攜帶無效或過期的授權碼，系統 MUST 拒絕登入
  並顯示「授權失敗，請重新登入」提示。
- 使用者在授權流程進行中關閉瀏覽器視窗，再次訪問系統時
  MUST 回到正常的登入頁面狀態。
- 社群帳號對應的電子郵件與系統中已被驗證的帳號吻合時，
  該社群登入 MUST 視為已驗證，無需再次進行電子郵件驗證。

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: 登入頁面 MUST 在既有帳密登入表單下方顯示
  「使用 Google 登入」和「使用 Facebook 登入」按鈕。
- **FR-002**: 點擊社群登入按鈕後，系統 MUST 將使用者導向
  對應供應商的 OAuth 授權頁面。
- **FR-003**: 授權成功回呼時，系統 MUST 驗證授權碼的有效性
  並從供應商取得使用者基本資訊（電子郵件、姓名、頭像連結）。
- **FR-004**: 若系統中不存在該社群帳號對應的使用者，系統
  MUST 自動建立新帳號，並將社群登入資訊與新帳號關聯。
- **FR-005**: 若系統中已存在相同電子郵件的使用者帳號，系統
  MUST 將社群登入關聯至該既有帳號，而非建立新帳號。
- **FR-006**: 社群登入成功後，系統 MUST 產生與帳密登入相同
  格式的存取 token 與重新整理 token，確保後續 API 存取行為
  一致。
- **FR-007**: 透過社群登入建立的帳號 MUST 自動標記電子郵件
  為「已驗證」（因社群供應商已驗證該電子郵件）。
- **FR-008**: 使用者在授權頁面取消或拒絕授權時，系統 MUST
  將使用者導回登入頁面並顯示適當提示訊息。
- **FR-009**: 社群登入按鈕在載入授權頁面期間 MUST 顯示
  載入狀態，防止使用者重複點擊。
- **FR-010**: 社群登入供應商無法連線時，系統 MUST 顯示
  使用者友善的錯誤訊息。
- **FR-011**: 系統 MUST 向社群供應商僅請求必要的最小權限
  範圍（電子郵件、基本個人資訊），不請求多餘的權限。
- **FR-012**: 系統 MUST 安全儲存社群登入的關聯資訊
  （供應商名稱、供應商端使用者識別碼）。

### Key Entities

- **外部登入關聯 (External Login)**：代表使用者帳號與某個
  社群登入供應商的關聯。關鍵屬性包括關聯的使用者、供應商
  名稱（Google / Facebook）、供應商端使用者識別碼。一個
  使用者可關聯多個不同供應商的登入。
- **使用者帳號 (User)**：既有實體，新增屬性包括頭像連結
  （可選，來自社群帳號）。透過社群登入建立的帳號不設定
  密碼，僅能透過社群方式登入。

### Non-Functional Requirements (Constitution Alignment)

- **程式品質**: 遵循 Controller → Service → Repository 分層，
  TypeScript strict / C# nullable enabled，通過靜態分析。
  社群登入邏輯 MUST 放在 Service 層，Controller 只負責
  HTTP 請求處理與 OAuth 回呼處理。
- **測試標準**: Service/Repository 單元測試覆蓋率 ≥ 80%，
  Controller endpoint 含正向與錯誤整合測試，bug fix 附迴歸測試。
  至少測試：授權成功、授權取消、新帳號建立、既有帳號關聯、
  供應商無法連線等情境。
- **使用者體驗一致性**: 使用 Element Plus 元件，SCSS + Tailwind CSS
  樣式，顯示載入狀態與使用者友善錯誤訊息。社群登入按鈕
  風格 MUST 與現有登入頁面保持視覺一致，按鈕包含供應商
  品牌 icon。
- **效能需求**: 社群登入流程（從點擊按鈕到進入系統首頁，
  不含供應商授權頁面的時間）p95 ≤ 3 秒，初始 bundle ≤ 300KB
  gzip。

## Scope

### In Scope

- 登入頁面新增 Google 與 Facebook 社群登入按鈕
- Google OAuth 授權流程整合
- Facebook OAuth 授權流程整合
- 社群帳號與系統帳號的自動建立與關聯
- 社群登入帳號的電子郵件自動驗證
- 社群登入後產生與帳密登入一致的 JWT token
- 授權取消與失敗的錯誤處理

### Out of Scope

- 其他社群登入供應商（GitHub、LINE、Apple 等）
- 帳號設定頁面中管理已關聯的社群登入
- 解除社群登入關聯功能
- 使用社群帳號頭像作為系統預設頭像
- 社群帳號資料同步（如名稱、頭像變更時自動更新）

## Assumptions

- 系統已有 ASP.NET Core Identity 基礎設施（UserManager、
  SignInManager）可直接使用，Identity 原生支援外部登入
  關聯（UserLoginInfo）。
- 開發團隊已在 Google Cloud Console 和 Facebook Developer
  Portal 建立應用程式並取得 Client ID 與 Client Secret。
- Google 和 Facebook OAuth 供應商回傳的使用者資訊中包含
  已驗證的電子郵件地址。
- 前端使用重導向模式（redirect）而非彈出視窗模式進行
  OAuth 授權。
- JWT token 的產生邏輯沿用現有的 JwtService，社群登入
  成功後呼叫相同的 token 產生流程。

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 使用者可在 30 秒內完成社群登入流程（從點擊
  社群登入按鈕到進入系統首頁，含供應商授權頁面操作時間）。
- **SC-002**: 95% 的社群登入嘗試成功完成，使用者順利進入
  系統。
- **SC-003**: 100% 使用相同電子郵件的社群登入正確關聯至
  既有帳號，不產生重複帳號。
- **SC-004**: 透過社群登入建立的帳號 100% 自動完成電子郵件
  驗證，無需使用者額外操作。
- **SC-005**: 授權取消或失敗時，100% 的情境顯示使用者友善
  的提示訊息，不出現未處理的錯誤畫面。
