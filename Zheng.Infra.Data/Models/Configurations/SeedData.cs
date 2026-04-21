using System;
using Microsoft.EntityFrameworkCore;

namespace Zheng.Infra.Data.Models.Configurations;

public static class SeedData
{
    // 固定 GUID，確保 seed data 可重複執行
    public static readonly Guid AdminUserId = new("7101a225-07a0-460c-85c2-9291b0845d81");
    public static readonly Guid AdminRoleId = new("c69cd53c-0ced-4aae-a120-1a7097e43650");

    private static readonly DateTime SeedDate = new(2026, 4, 11, 0, 0, 0, DateTimeKind.Utc);

    // BCrypt hash of "Admin@123" with workFactor 12
    private const string AdminPasswordHash = "$2a$12$i0MquD3jv8B8kXLNajHZ7OlndZIXRObDL5Ac1inOez0yLPH56R.g2";

    public static void Apply(ModelBuilder modelBuilder)
    {
        SeedAdminUser(modelBuilder);
        SeedAdminRole(modelBuilder);
        SeedUserRole(modelBuilder);
        SeedRolePermissions(modelBuilder);
        SeedSysMenus(modelBuilder);
    }

    private static void SeedAdminUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = AdminUserId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@library.com",
            NormalizedEmail = "ADMIN@LIBRARY.COM",
            EmailConfirmed = true,
            PasswordHash = AdminPasswordHash,
            PasswordHashVersion = 2,
            SecurityStamp = "STATIC_SECURITY_STAMP_ADMIN_001",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            IsActive = true,
            CreatedAt = SeedDate,
            ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
        });
    }

    private static void SeedAdminRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(new Role
        {
            Id = AdminRoleId,
            Name = "系統管理員",
            NormalizedName = "系統管理員",
            Description = "系統管理員",
            CreatedAt = SeedDate,
            ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
        });
    }

    private static void SeedUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(new UserRole
        {
            UserId = AdminUserId,
            RoleId = AdminRoleId,
            AssignedAt = SeedDate
        });
    }

    private static void SeedRolePermissions(ModelBuilder modelBuilder)
    {
        var permissions = new[]
        {
            "Home", "Management", "Books", "Books.View", "Books.Create", "Books.Edit", "Books.Delete",
            "reader", "System", "Menu",
            "System.Menu", "System.Menu.Create", "System.Menu.Update", "System.Menu.Delete",
            "Users", "Users.Create", "Users.Edit", "Users.View", "Users.Delete", "Users.ResetPassword",
            "Roles", "Roles.View", "Roles.Create", "Roles.Edit", "Roles.Delete"
        };

        var data = new RolePermission[permissions.Length];
        for (int i = 0; i < permissions.Length; i++)
        {
            data[i] = new RolePermission
            {
                Id = i + 1,
                RoleId = AdminRoleId,
                Permission = permissions[i]
            };
        }

        modelBuilder.Entity<RolePermission>().HasData(data);
    }

    private static void SeedSysMenus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SysMenu>().HasData(
            // ── 層級 1：頂層選單容器 ──
            new SysMenu
            {
                Id = 1, ParentModuleId = 0, ModuleNo = "Home",
                Name = "首頁", Url = "/", Icon = "HomeFilled",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 1, SortOrder = 1, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 2, ParentModuleId = 0, ModuleNo = "Management",
                Name = "主資料管理", Icon = "Document",
                IsMenu = true, IsOpen = true, Hidden = false,
                Levels = 1, SortOrder = 2, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 3, ParentModuleId = 0, ModuleNo = "System",
                Name = "系統管理", Icon = "Setting",
                IsMenu = true, IsOpen = true, Hidden = false,
                Levels = 1, SortOrder = 3, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },

            // ── 層級 2：頁面選單 ──
            new SysMenu
            {
                Id = 4, ParentModuleId = 2, ModuleNo = "reader",
                Name = "讀者資料", Url = "/reader", Component = "/views/AboutView.vue",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 2, SortOrder = 1, Status = false,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 5, ParentModuleId = 2, ModuleNo = "Books",
                Name = "書籍資料", Url = "/books", Component = "/views/BooksView.vue",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 2, SortOrder = 0, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 6, ParentModuleId = 3, ModuleNo = "Users",
                Name = "帳號管理", Url = "/users", Component = "/views/UserManagementView.vue",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 2, SortOrder = 2, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 7, ParentModuleId = 3, ModuleNo = "Roles",
                Name = "角色管理", Url = "/roles", Component = "/views/RoleManagementView.vue",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 3, SortOrder = 0, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },
            new SysMenu
            {
                Id = 8, ParentModuleId = 3, ModuleNo = "Menu",
                Name = "系統選單", Url = "/menu", Component = "/views/SysMenuManagementView.vue",
                IsMenu = true, IsOpen = false, Hidden = false,
                Levels = 2, SortOrder = 0, Status = true,
                CreatedAt = SeedDate, CreatedBy = AdminUserId
            },

            // ── 按鈕權限（IsMenu=false） ──
            // Users (parent=6)
            new SysMenu { Id = 9, ParentModuleId = 6, ModuleNo = "Users.Create", Name = "新增", IsMenu = false, IsOpen = false, Hidden = false, Levels = 3, SortOrder = 1, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 10, ParentModuleId = 6, ModuleNo = "Users.Edit", Name = "編輯", IsMenu = false, IsOpen = false, Hidden = false, Levels = 3, SortOrder = 2, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 11, ParentModuleId = 6, ModuleNo = "Users.View", Name = "檢視", IsMenu = false, IsOpen = false, Hidden = false, Levels = 3, SortOrder = 3, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 12, ParentModuleId = 6, ModuleNo = "Users.Delete", Name = "刪除", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 0, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 13, ParentModuleId = 6, ModuleNo = "Users.ResetPassword", Name = "重設密碼", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 5, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            // Roles (parent=7)
            new SysMenu { Id = 14, ParentModuleId = 7, ModuleNo = "Roles.View", Name = "檢視", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 1, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 15, ParentModuleId = 7, ModuleNo = "Roles.Create", Name = "新增", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 2, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 16, ParentModuleId = 7, ModuleNo = "Roles.Edit", Name = "編輯", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 3, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 17, ParentModuleId = 7, ModuleNo = "Roles.Delete", Name = "刪除", IsMenu = false, IsOpen = false, Hidden = false, Levels = 4, SortOrder = 4, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            // Books (parent=5)
            new SysMenu { Id = 18, ParentModuleId = 5, ModuleNo = "Books.View", Name = "檢視", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 1, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 19, ParentModuleId = 5, ModuleNo = "Books.Create", Name = "新增", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 2, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 20, ParentModuleId = 5, ModuleNo = "Books.Edit", Name = "編輯", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 3, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 21, ParentModuleId = 5, ModuleNo = "Books.Delete", Name = "刪除", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 4, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            // System.Menu (parent=8)
            new SysMenu { Id = 22, ParentModuleId = 8, ModuleNo = "System.Menu", Name = "檢視", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 1, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 23, ParentModuleId = 8, ModuleNo = "System.Menu.Create", Name = "新增", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 2, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 24, ParentModuleId = 8, ModuleNo = "System.Menu.Update", Name = "編輯", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 3, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId },
            new SysMenu { Id = 25, ParentModuleId = 8, ModuleNo = "System.Menu.Delete", Name = "刪除", IsMenu = false, Hidden = false, Levels = 3, SortOrder = 4, Status = true, CreatedAt = SeedDate, CreatedBy = AdminUserId }
        );
    }
}
