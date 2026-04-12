# Research: 使用者註冊服務

## R1: ASP.NET Core Identity Email Confirmation

### Decision
使用 ASP.NET Core Identity 內建的 `UserManager.GenerateEmailConfirmationTokenAsync()`
與 `UserManager.ConfirmEmailAsync()` 搭配 Data Protection token provider。

### Rationale
- Identity 已內建完整的 email confirmation 機制，無需自建 token 系統。
- `IdentityUser` 已有 `Email`、`EmailConfirmed` 欄位，不需額外 migration。
- Token 由 Data Protection API 產生，自帶防篡改與到期機制。
- `UserManager.GenerateEmailConfirmationTokenAsync()` 產生 URL-safe token。
- `UserManager.ConfirmEmailAsync()` 驗證 token 並自動更新 `EmailConfirmed`。

### Alternatives Considered
| Alternative | Rejected Because |
|-------------|-----------------|
| 自建 GUID token + DB 表 | 重新發明已有的 Identity 機制，維護成本高 |
| JWT-based verification token | 需額外處理到期與單次使用邏輯 |

### Implementation Notes
- 需設定 `options.SignIn.RequireConfirmedEmail = true` 在 IdentityOptions 中。
- Token 預設無到期限制，需設定 `DataProtectionTokenProviderOptions.TokenLifespan = TimeSpan.FromHours(24)`。
- 驗證連結格式：`{baseUrl}/api/Account/ConfirmEmail?userId={userId}&token={encodedToken}`。
- Token 需 URL encode（`WebEncoders.Base64UrlEncode`）以安全傳輸。

---

## R2: Email 發送方案

### Decision
使用 MailKit 套件透過 SMTP 發送郵件，實作 `IEmailSender` 介面。
開發環境使用 Ethereal（fake SMTP），生產環境可替換為任何 SMTP 服務。

### Rationale
- MailKit 是 .NET 生態最成熟的郵件套件，比 System.Net.Mail.SmtpClient 更現代（後者已標記 obsolete）。
- 介面抽象 `IEmailSender` 可讓測試時輕鬆 mock，也方便未來切換為 SendGrid / AWS SES。
- ASP.NET Core Identity 本身就有 `IEmailSender<TUser>` 介面可搭配。

### Alternatives Considered
| Alternative | Rejected Because |
|-------------|-----------------|
| System.Net.Mail.SmtpClient | 已被標記 obsolete，不建議新專案使用 |
| SendGrid / AWS SES SDK | 增加外部服務依賴，且此為內部系統不需高吞吐 |
| FluentEmail | 多一層抽象但底層仍需 MailKit，新增不必要複雜度 |

### Implementation Notes
- 新增 NuGet package: `MailKit` (含 MimeKit)。
- SMTP 設定放在 `appsettings.json` 的 `EmailSettings` 區段。
- 開發環境可使用 Ethereal (https://ethereal.email/) 或 MailHog（Docker）。
- Email 發送應在 Controller 層 fire-and-forget（實際為 await 但用背景佇列處理），
  短期可直接 await（內部系統流量低），長期可改 background service。

---

## R3: 重新發送驗證信的頻率限制

### Decision
使用 `IMemoryCache` 實作簡單的 per-email rate limiting（每分鐘 1 次）。

### Rationale
- 要求為「每分鐘最多 1 次」，屬於簡單的固定窗口限制。
- 使用 Memory Cache 足以應付內部系統規模（< 1000 使用者）。
- 無需引入 Redis 或 ASP.NET Core Rate Limiting Middleware（over-engineering）。

### Alternatives Considered
| Alternative | Rejected Because |
|-------------|-----------------|
| ASP.NET Core Rate Limiting Middleware | 針對整體 endpoint 限速，而非 per-email；設定複雜度不符需求 |
| Redis-based rate limiting | 內部系統規模不需分散式快取 |
| Database timestamp 紀錄 | 增加不必要的 DB 寫入，Memory Cache 更輕量 |

### Implementation Notes
- Key 格式：`resend_verify_{email_hash}`，Value: timestamp。
- TTL: 60 秒。
- 在 Service 層（RegistrationService）實作，Controller 不處理限速邏輯。

---

## R4: 登入流程修改（電子郵件驗證檢查）

### Decision
在現有 AccountController.Login 中增加 `EmailConfirmed` 檢查，
未驗證的帳號返回特定錯誤訊息（含「重新發送驗證信」提示）。

### Rationale
- Identity 已有 `RequireConfirmedEmail` 設定，但預設行為是
  `SignInResult.IsNotAllowed`，需轉換為使用者友善的中文訊息。
- 前端需能區分「密碼錯誤」與「未驗證」兩種失敗原因以顯示
  不同 UI 提示。

### Implementation Notes
- `SignInManager.PasswordSignInAsync` 在 `RequireConfirmedEmail = true` 時，
  對未驗證帳號回傳 `result.IsNotAllowed`。
- 需在 Login 中特別處理 `result.IsNotAllowed` 並回傳明確的錯誤碼。
- 前端根據錯誤碼決定是否顯示「重新發送驗證信」按鈕。

---

## R5: 安全性考量

### Decision
- **帳號列舉防護**：重新發送驗證信的 API 無論 email 是否存在，
  均回傳相同成功訊息。
- **Token 單次使用**：Identity `ConfirmEmailAsync` 內建設計已確保
  同一 token 無法重複使用（token 綁定 SecurityStamp，確認後 stamp 更新）。
- **密碼雜湊**：使用 Identity 預設的 PBKDF2-HMAC-SHA256 with 128-bit salt
  + 100,000 iterations（.NET 8 預設），無需額外配置。

### Implementation Notes
- 禁止在 API response 中洩露「email 已存在」的資訊（僅在 Register 時給出提示，
  Resend 時不洩露）。
- 驗證連結中的 token MUST 使用 URL encoding，避免特殊字元被截斷。
