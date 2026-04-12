# Feature Specification: 自訂身分驗證資料表結構

**Feature Branch**: `003-custom-identity-tables`  
**Created**: 2026-04-11  
**Status**: Draft  
**Input**: User description: "不使用 ASP.NET Core Identity 所需的資料表結構，改寫自己建立的資料表結構"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - 使用者帳號管理（自訂資料表） (Priority: P1)

系統管理員可以透過後台管理介面，使用自訂資料表結構（取代 ASP.NET Core Identity 預設的 AspNet* 資料表）來建立、查詢、編輯和刪除使用者帳號。使用者帳號的所有資料（密碼雜湊、角色、權限等）均儲存於自訂資料表中，不再依賴 Identity 框架的 UserManager/RoleManager。

**Why this priority**: 這是整個改寫的基礎，所有其他功能（登入、角色、權限）都依賴使用者帳號資料表結構的建立。

**Independent Test**: 可透過直接操作自訂使用者資料表，驗證 CRUD 操作能正確執行並回傳預期結果。

**Acceptance Scenarios**:

1. **Given** 系統管理員已登入，**When** 建立新使用者並填入帳號、Email、密碼，**Then** 系統在自訂使用者資料表中成功建立記錄，密碼以安全雜湊儲存
2. **Given** 自訂使用者資料表中已有使用者資料，**When** 查詢使用者列表，**Then** 系統從自訂資料表讀取並回傳分頁結果
3. **Given** 已存在的使用者，**When** 管理員修改使用者的 Email 或狀態，**Then** 自訂資料表中的對應欄位被更新，且更新時間自動記錄
4. **Given** 已存在的使用者，**When** 管理員刪除使用者，**Then** 該使用者從自訂資料表中被移除（或標記為停用）

---

### User Story 2 - 使用者登入與 JWT 令牌核發（脫離 Identity） (Priority: P1)

使用者可以透過帳號和密碼登入系統，系統使用自訂的驗證邏輯（不依賴 SignInManager）驗證身分，並核發 JWT Access Token 和 Refresh Token。

**Why this priority**: 登入是系統最核心的功能，與 User Story 1 同為 P1，因為沒有登入就無法使用任何受保護的功能。

**Independent Test**: 可透過 API 呼叫登入端點，驗證正確的帳密回傳 JWT Token，錯誤的帳密回傳對應錯誤訊息。

**Acceptance Scenarios**:

1. **Given** 使用者帳號已存在且 Email 已驗證，**When** 使用正確帳密呼叫登入 API，**Then** 系統回傳有效的 JWT Access Token 及 Refresh Token
2. **Given** 使用者輸入錯誤密碼，**When** 呼叫登入 API，**Then** 系統回傳登入失敗訊息，且帳號登入失敗次數遞增
3. **Given** 使用者帳號被鎖定，**When** 嘗試登入，**Then** 系統拒絕登入並回傳鎖定狀態資訊
4. **Given** 使用者 Email 尚未驗證，**When** 嘗試登入，**Then** 系統拒絕登入並提示需先完成 Email 驗證

---

### User Story 3 - 角色與權限管理（自訂資料表） (Priority: P2)

系統管理員可以建立、查詢、編輯和刪除角色，並為角色指派權限（基於 SysMenu 的 ModuleNo）。角色與權限資料儲存於自訂資料表中，取代 AspNetRoles、AspNetRoleClaims 及 AspNetUserRoles。

**Why this priority**: 角色與權限是系統授權的核心，但可在基本登入功能完成後實作。

**Independent Test**: 可透過建立角色、指派權限、將角色指派給使用者，然後驗證該使用者能存取對應的受保護功能。

**Acceptance Scenarios**:

1. **Given** 系統管理員已登入，**When** 建立角色並指派多項權限，**Then** 自訂角色資料表新增記錄，且角色權限對應表正確插入權限項目
2. **Given** 已存在角色，**When** 將角色指派給使用者，**Then** 自訂使用者角色關聯表正確建立對應關係
3. **Given** 使用者已被指派角色，**When** 存取需要該角色權限的 API，**Then** 授權系統從自訂資料表查詢使用者權限並允許存取
4. **Given** 使用者未擁有特定權限，**When** 嘗試存取受保護的 API，**Then** 系統拒絕存取並回傳 403 狀態碼

---

### User Story 4 - 使用者註冊與 Email 驗證（自訂實作） (Priority: P2)

新使用者可以自行註冊帳號，系統使用自訂邏輯建立帳號並發送驗證信件。使用者點擊驗證連結後完成 Email 驗證。

**Why this priority**: 註冊功能重要但相對獨立，可在登入功能穩定後實作。

**Independent Test**: 可透過呼叫註冊 API 建立帳號，確認驗證信發送，再模擬點擊驗證連結完成驗證。

**Acceptance Scenarios**:

1. **Given** 使用者填入帳號、Email、密碼，**When** 呼叫註冊 API，**Then** 自訂資料表建立新使用者，Email 驗證狀態為未驗證，系統發送驗證信
2. **Given** 使用者已收到驗證信，**When** 點擊驗證連結，**Then** 系統更新自訂資料表中的 Email 驗證狀態為已驗證
3. **Given** 使用者重複以同一 Email 註冊，**When** 呼叫註冊 API，**Then** 系統回傳 Email 已被使用的錯誤訊息

---

### User Story 5 - 社群帳號（OAuth）登入整合（自訂資料表） (Priority: P3)

使用者可以使用 Google 或 Facebook 帳號登入系統，系統透過自訂外部登入資料表儲存 OAuth 提供者資訊，取代 AspNetUserLogins。

**Why this priority**: 社群登入是便利功能但非核心，可在主要驗證流程完成後實作。

**Independent Test**: 可透過 OAuth 流程登入，驗證外部登入資訊正確儲存在自訂資料表，且使用者能正常獲取 JWT Token。

**Acceptance Scenarios**:

1. **Given** 使用者選擇 Google 登入，**When** 完成 OAuth 授權，**Then** 系統在自訂外部登入資料表儲存提供者資訊，並核發 JWT Token
2. **Given** 使用者已有本地帳號且 Email 相同，**When** 使用社群帳號登入，**Then** 系統自動連結至既有帳號
3. **Given** 使用者為新用戶，**When** 使用社群帳號首次登入，**Then** 系統自動建立帳號並儲存外部登入資訊

---

### Edge Cases

- 資料遷移期間若系統同時有舊 Identity 資料與新自訂資料表，如何確保資料一致性？
- 密碼重設 Token 在自訂實作中如何安全生成與驗證（不依賴 Identity 的 DataProtection Token Provider）？
- 多個外部帳號連結至同一使用者帳號時，如何處理衝突？
- 同時多裝置登入時，Refresh Token 的管理與撤銷策略
- 帳號鎖定解鎖的時間邏輯（自動解鎖 vs 手動解鎖）

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: 系統必須以自訂資料表取代所有 ASP.NET Core Identity 預設資料表（AspNetUsers、AspNetRoles、AspNetUserRoles、AspNetRoleClaims、AspNetUserClaims、AspNetUserLogins、AspNetUserTokens）
- **FR-002**: 系統必須提供自訂的密碼雜湊機制（使用安全的雜湊演算法，如 BCrypt 或 PBKDF2），取代 Identity 的 PasswordHasher
- **FR-003**: 系統必須實作自訂的使用者驗證邏輯，取代 SignInManager 的 PasswordSignInAsync
- **FR-004**: 系統必須維持現有的 JWT Token 核發與驗證流程（Access Token + Refresh Token）
- **FR-005**: 系統必須實作自訂的角色管理功能，包含角色 CRUD 及角色權限（Permission）指派
- **FR-006**: 系統必須實作自訂的使用者-角色關聯管理，支援一個使用者擁有多個角色
- **FR-007**: 系統必須實作自訂的 Email 驗證機制，包含驗證 Token 的生成、發送與驗證
- **FR-008**: 系統必須實作帳號鎖定機制，在登入失敗達到指定次數後自動鎖定帳號
- **FR-009**: 系統必須實作自訂的外部登入（OAuth）資訊儲存，取代 AspNetUserLogins
- **FR-010**: 系統必須提供密碼重設功能，使用自訂的安全 Token 機制
- **FR-011**: 系統必須保留現有的權限驗證架構（PermissionAuthorizationHandler），改為從自訂資料表查詢權限
- **FR-012**: 系統必須提供資料遷移方案，將現有 Identity 資料表中的資料遷移至新的自訂資料表
- **FR-013**: 系統必須將所有 Identity 相關的業務邏輯封裝於 Service 層，Controllers 不直接操作 UserManager/RoleManager

### Key Entities

- **User（使用者）**: 系統的核心身分實體，包含帳號、Email、密碼雜湊、Email 驗證狀態、鎖定狀態、登入失敗次數等屬性。取代 AspNetUsers。
- **Role（角色）**: 權限群組定義，包含角色名稱與描述。取代 AspNetRoles。
- **UserRole（使用者角色關聯）**: 多對多關聯，記錄使用者被指派的角色。取代 AspNetUserRoles。
- **RolePermission（角色權限）**: 記錄角色擁有的權限項目，權限值對應 SysMenu 的 ModuleNo。取代 AspNetRoleClaims。
- **UserExternalLogin（使用者外部登入）**: 儲存 OAuth 提供者資訊（LoginProvider、ProviderKey）。取代 AspNetUserLogins。
- **UserToken（使用者 Token）**: 儲存 Refresh Token、Email 驗證 Token、密碼重設 Token 等。整合原有 Token 表功能並取代 AspNetUserTokens。

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

- 現有系統已有完整的 JWT Token 核發機制（JwtService），改寫後將沿用此機制，僅替換 Token 內 Claims 的資料來源
- 密碼雜湊採用 BCrypt 演算法（業界標準，成本因子預設 12），除非有明確的合規需求指定其他演算法
- 帳號鎖定策略採用：連續失敗 5 次後鎖定 15 分鐘，之後自動解鎖（與 Identity 預設行為一致）
- 自訂資料表的主鍵統一使用 GUID（與現有 Token 表一致），而非 Identity 預設的字串型 ID
- 資料遷移為一次性操作，遷移完成後移除 Identity 相關的 NuGet 套件與程式碼
- 前端 API 介面保持不變，僅後端資料存取層與服務層改寫，對前端為透明升級
- Email 發送機制（SmtpEmailSender）維持不變，僅替換驗證 Token 的生成邏輯
- 現有的 PermissionAuthorizationHandler 授權流程架構不變，僅替換資料查詢來源

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 所有現有的使用者管理功能（CRUD、鎖定/解鎖、密碼重設）在改寫後能正常運作，功能回歸測試通過率 100%
- **SC-002**: 使用者登入流程在改寫前後的體驗完全一致，登入操作在 3 秒內完成
- **SC-003**: 角色與權限管理功能（CRUD、權限指派、權限驗證）在改寫後正常運作，所有權限檢查行為與改寫前一致
- **SC-004**: 社群帳號（Google / Facebook）登入在改寫後正常運作，現有已連結的社群帳號不受影響
- **SC-005**: 現有 Identity 資料表中的所有使用者、角色、權限資料完整遷移至自訂資料表，遷移後資料筆數與內容一致
- **SC-006**: 改寫後的系統不再依賴 Microsoft.AspNetCore.Identity.EntityFrameworkCore 套件（可從專案依賴中移除）
- **SC-007**: 所有新增的 Service 層單元測試覆蓋率達 80% 以上
- **SC-008**: 前端功能在後端改寫後無需任何修改即可正常運作（API 介面向下相容）
