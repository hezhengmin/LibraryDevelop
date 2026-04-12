using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zheng.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "系統編號"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "書名標題"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "狀態(0有庫存1無庫存)"),
                    NumberOfCopies = table.Column<int>(type: "int", nullable: false, comment: "庫存數量"),
                    ISBN = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    ISSN = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: true),
                    GPN = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, comment: "GPN(政府出版品統一編號)"),
                    Publisher = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "出版單位"),
                    RightCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "出版情況"),
                    Creator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "作者資訊"),
                    PublishDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "出版日期"),
                    Edition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "版次"),
                    Cover = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "書封連結"),
                    Classify = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "書籍分類"),
                    GPNType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "出版品分類"),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "主題分類"),
                    Governance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "施政分類"),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "級別"),
                    Pages = table.Column<int>(type: "int", nullable: false, comment: "頁數"),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "開數"),
                    Binding = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "裝訂"),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "語言"),
                    Introduction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "abstract(書籍介紹)"),
                    Catalog = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "目次"),
                    Price = table.Column<int>(type: "int", nullable: false, comment: "價格"),
                    TargetPeople = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "適用對象"),
                    Types = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "資料類型"),
                    Attachment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "附件"),
                    Url = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true, comment: "出版品網址-線上版或試閱版"),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "播放時間長度"),
                    Numbers = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "字號"),
                    Restriction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "權利範圍"),
                    CeasedDate = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停刊註記"),
                    Authority = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "授權資訊"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "新增時間"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "新增者"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "修改時間"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "系統編號"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "名稱"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "新增時間"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "新增者"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "修改時間"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysMenu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentModuleId = table.Column<long>(type: "bigint", nullable: false, comment: "父選單編號"),
                    ModuleNo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "選單編號"),
                    Component = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "元件設定"),
                    Hidden = table.Column<bool>(type: "bit", nullable: true, comment: "是否隱藏（1：是，0：否）"),
                    Icon = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "圖示"),
                    IsMenu = table.Column<bool>(type: "bit", nullable: false, comment: "是否為選單（1：選單，0：按鈕）"),
                    IsOpen = table.Column<bool>(type: "bit", nullable: true, comment: "是否預設展開（1：是，0：否）"),
                    Levels = table.Column<int>(type: "int", nullable: false, comment: "層級"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "名稱"),
                    SortOrder = table.Column<int>(type: "int", nullable: false, comment: "排序順序"),
                    Status = table.Column<bool>(type: "bit", nullable: false, comment: "狀態（1：啟用，0：停用）"),
                    Tips = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "滑鼠懸停提示文字"),
                    Url = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "連結網址"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "新增時間"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "新增者"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改時間"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "系統編號"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "名稱"),
                    Extension = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "副檔名"),
                    ContentType = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false, comment: "檔案類型"),
                    Length = table.Column<long>(type: "bigint", nullable: false, comment: "檔案大小"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "新增時間"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "新增者"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "修改時間"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PasswordHashVersion = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    SecurityStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "系統編號"),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "書籍Id"),
                    UploadFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "檔案Id"),
                    SystemDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "系統日期")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookPhoto_Book",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "系統編號"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "借出開始日期"),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "借出結束日期"),
                    ReturnDate = table.Column<DateTime>(type: "datetime", nullable: true, comment: "書籍歸還日期"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "新增時間"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "新增者"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, comment: "修改時間"),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loan_Book",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Loan_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserExternalLogin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExternalLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExternalLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[] { new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650"), "00000000-0000-0000-0000-000000000002", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), "系統管理員", "Admin", "ADMIN", null });

            migrationBuilder.InsertData(
                table: "SysMenu",
                columns: new[] { "Id", "Component", "CreatedAt", "CreatedBy", "Hidden", "Icon", "IsMenu", "IsOpen", "Levels", "ModuleNo", "Name", "ParentModuleId", "SortOrder", "Status", "Tips", "UpdatedAt", "UpdatedBy", "Url" },
                values: new object[,]
                {
                    { 1L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 1, "Home", "首頁", 0L, 1, true, null, null, null, "/" },
                    { 2L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, true, 1, "Management", "主資料管理", 0L, 2, true, null, null, null, null },
                    { 3L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, true, 1, "System", "系統管理", 0L, 3, true, null, null, null, null },
                    { 4L, "/views/AboutView.vue", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 2, "reader", "讀者資料", 2L, 1, false, null, null, null, "/reader" },
                    { 5L, "/views/BooksView.vue", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 2, "Books", "書籍資料", 2L, 0, true, null, null, null, "/books" },
                    { 6L, "/views/UserManagementView.vue", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 2, "Users", "帳號管理", 3L, 2, true, null, null, null, "/users" },
                    { 7L, "/views/RoleManagementView.vue", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 3, "Roles", "角色管理", 3L, 0, true, null, null, null, "/roles" },
                    { 8L, "/views/SysMenuManagementView.vue", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, true, false, 2, "Menu", "系統選單", 3L, 0, true, null, null, null, "/menu" },
                    { 9L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 3, "Users.Create", "新增", 6L, 1, true, null, null, null, null },
                    { 10L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 3, "Users.Edit", "編輯", 6L, 2, true, null, null, null, null },
                    { 11L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 3, "Users.View", "檢視", 6L, 3, true, null, null, null, null },
                    { 12L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Users.Delete", "刪除", 6L, 0, true, null, null, null, null },
                    { 13L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Users.ResetPassword", "重設密碼", 6L, 5, true, null, null, null, null },
                    { 14L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Roles.View", "檢視", 7L, 1, true, null, null, null, null },
                    { 15L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Roles.Create", "新增", 7L, 2, true, null, null, null, null },
                    { 16L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Roles.Edit", "編輯", 7L, 3, true, null, null, null, null },
                    { 17L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, false, 4, "Roles.Delete", "刪除", 7L, 4, true, null, null, null, null },
                    { 18L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "Books.View", "檢視", 5L, 1, true, null, null, null, null },
                    { 19L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "Books.Create", "新增", 5L, 2, true, null, null, null, null },
                    { 20L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "Books.Edit", "編輯", 5L, 3, true, null, null, null, null },
                    { 21L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "Books.Delete", "刪除", 5L, 4, true, null, null, null, null },
                    { 22L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "System.Menu", "檢視", 8L, 1, true, null, null, null, null },
                    { 23L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "System.Menu.Create", "新增", 8L, 2, true, null, null, null, null },
                    { 24L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "System.Menu.Update", "編輯", 8L, 3, true, null, null, null, null },
                    { 25L, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), false, null, false, null, 3, "System.Menu.Delete", "刪除", 8L, 4, true, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PasswordHashVersion", "PhoneNumber", "SecurityStamp", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), "00000000-0000-0000-0000-000000000001", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), "admin@library.com", true, true, true, null, "ADMIN@LIBRARY.COM", "ADMIN", "$2a$12$i0MquD3jv8B8kXLNajHZ7OlndZIXRObDL5Ac1inOez0yLPH56R.g2", 2, null, "STATIC_SECURITY_STAMP_ADMIN_001", null, "admin" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "Permission", "RoleId" },
                values: new object[,]
                {
                    { 1, "Home", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 2, "Management", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 3, "Books", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 4, "Books.View", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 5, "Books.Create", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 6, "Books.Edit", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 7, "Books.Delete", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 8, "reader", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 9, "System", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 10, "Menu", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 11, "System.Menu", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 12, "System.Menu.Create", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 13, "System.Menu.Update", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 14, "System.Menu.Delete", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 15, "Users", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 16, "Users.Create", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 17, "Users.Edit", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 18, "Users.View", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 19, "Users.Delete", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 20, "Users.ResetPassword", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 21, "Roles", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 22, "Roles.View", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 23, "Roles.Create", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 24, "Roles.Edit", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") },
                    { 25, "Roles.Delete", new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650") }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[] { new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650"), new Guid("7101a225-07a0-460c-85c2-9291b0845d81"), new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_BookPhoto_BookId",
                table: "BookPhoto",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loan_BookId",
                table: "Loan",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loan_UserId",
                table: "Loan",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_NormalizedName",
                table: "Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_Permission",
                table: "RolePermission",
                columns: new[] { "RoleId", "Permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_NormalizedEmail",
                table: "User",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_NormalizedUserName",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExternalLogin_LoginProvider_ProviderKey",
                table: "UserExternalLogin",
                columns: new[] { "LoginProvider", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExternalLogin_UserId",
                table: "UserExternalLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_TokenValue",
                table: "UserToken",
                column: "TokenValue");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId",
                table: "UserToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId_TokenType",
                table: "UserToken",
                columns: new[] { "UserId", "TokenType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookPhoto");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Loan");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "SysMenu");

            migrationBuilder.DropTable(
                name: "UploadFile");

            migrationBuilder.DropTable(
                name: "UserExternalLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
