# Quickstart: 自訂身分驗證資料表結構

**Feature**: 003-custom-identity-tables  
**Date**: 2026-04-11

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB 或 SQLEXPRESS)
- Node.js 18+
- 現有 Library 專案可正常編譯與執行

## 開發順序

### Phase 1: 資料模型與 Migration（先做）

1. 安裝 BCrypt.Net-Next NuGet 套件至 Zheng.Bll 專案
   ```bash
   cd Zheng.Bll
   dotnet add package BCrypt.Net-Next
   ```

2. 在 `Zheng.Infrastructure/Models/` 建立新實體：
   - `User.cs`, `Role.cs`, `UserRole.cs`, `RolePermission.cs`, `UserExternalLogin.cs`, `UserToken.cs`

3. 在 `LibraryContext` 新增 DbSet 並移除舊 DbSet：
   ```csharp
   // 移除
   public DbSet<AspNetUser> AspNetUsers { get; set; }
   public DbSet<AspNetRole> AspNetRoles { get; set; }
   // ...其他 AspNet* DbSets

   // 新增
   public DbSet<User> Users { get; set; }
   public DbSet<Role> Roles { get; set; }
   public DbSet<UserRole> UserRoles { get; set; }
   public DbSet<RolePermission> RolePermissions { get; set; }
   public DbSet<UserExternalLogin> UserExternalLogins { get; set; }
   public DbSet<UserToken> UserTokens { get; set; }
   ```

4. 建立 EF Core Migration：
   ```bash
   cd Library.Server
   dotnet ef migrations add AddCustomIdentityTables
   dotnet ef database update
   ```

5. 建立資料遷移 Migration（SQL 腳本搬移既有資料）：
   ```bash
   dotnet ef migrations add MigrateIdentityData
   dotnet ef database update
   ```

### Phase 2: Service 層改寫

6. 建立新 Service 介面與實作：
   - `IAuthService` / `AuthService` — 密碼驗證、雜湊、Token 生成
   - `IUserService` / `UserService` — 使用者 CRUD
   - `IRoleService` / `RoleService` — 角色 CRUD + 權限管理

7. 修改現有 Service：
   - `JwtService` — 移除 `UserManager<IdentityUser>` 依賴，改接收 `User` 實體
   - `RegistrationService` — 改用 `AuthService` 與 `IUserService`
   - `ExternalLoginService` — 改用 `LibraryContext` 直接查詢 UserExternalLogin

### Phase 3: Controller 層改寫

8. 修改 Controller 注入：
   - `AccountController` — 移除 `UserManager`, `SignInManager`，改注入 `IAuthService`, `IUserService`
   - `UserController` — 移除 `UserManager`, `RoleManager`，改注入 `IUserService`
   - `RoleController` — 移除 `RoleManager`，改注入 `IRoleService`
   - `ExternalLoginController` — 移除 `UserManager`，改注入 `IExternalLoginService`

9. 修改 `PermissionAuthorizationHandler` — 改從 `LibraryContext` 直接查詢權限

### Phase 4: 移除 Identity 依賴

10. 修改 `Program.cs`：
    - 移除 `AddIdentityApiEndpoints<IdentityUser>()`
    - 移除 `ApplicationDbContext` 註冊
    - 註冊新 Service（IAuthService, IUserService, IRoleService）
    - 保留 JWT Bearer 驗證與 Google/Facebook OAuth

11. 刪除 `ApplicationDbContext` 檔案

12. 移除 NuGet 套件：
    ```bash
    cd Library.Server
    dotnet remove package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    ```

### Phase 5: 測試

13. 撰寫單元測試：
    - `AuthServiceTests.cs` — 密碼雜湊、驗證、鎖定邏輯
    - `UserServiceTests.cs` — 使用者 CRUD
    - `RoleServiceTests.cs` — 角色 CRUD + 權限管理

14. 驗證所有 API 端點行為與改寫前一致

## 驗證清單

- [ ] `dotnet build` 成功，零錯誤
- [ ] `dotnet ef database update` 成功
- [ ] 登入 API 回傳正確 JWT Token
- [ ] 舊密碼可正常登入（PBKDF2 相容）
- [ ] 新密碼以 BCrypt 儲存
- [ ] 角色與權限功能正常
- [ ] 社群登入正常
- [ ] Email 驗證正常
- [ ] `Library.Server.csproj` 不再引用 `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- [ ] 前端功能無需修改
