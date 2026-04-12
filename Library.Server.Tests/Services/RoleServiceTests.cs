using Microsoft.EntityFrameworkCore;
using Zheng.Bll.Interfaces;
using Zheng.Bll.Services;
using Zheng.Infra.Data.Models;

namespace Library.Server.Tests.Services
{
    public class RoleServiceTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly RoleService _service;

        public RoleServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);
            _service = new RoleService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region CreateRoleAsync

        [Fact]
        public async Task CreateRole_WithPermissions_InsertsRoleAndPermissions()
        {
            // Arrange
            var input = new CreateRoleInput
            {
                Name = "Editor",
                Description = "Can edit content",
                Permissions = new List<string> { "Books.View", "Books.Edit" }
            };

            // Act
            var result = await _service.CreateRoleAsync(input);

            // Assert
            Assert.True(result.Succeeded);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Editor");
            Assert.NotNull(role);
            Assert.Equal("EDITOR", role.NormalizedName);

            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.Permission)
                .ToListAsync();
            Assert.Equal(2, permissions.Count);
            Assert.Contains("Books.View", permissions);
            Assert.Contains("Books.Edit", permissions);
        }

        [Fact]
        public async Task CreateRole_DuplicateName_ReturnsFailure()
        {
            // Arrange
            _context.Roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CreateRoleAsync(new CreateRoleInput { Name = "Admin" });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("角色名稱已存在", result.Errors);
        }

        #endregion

        #region UpdateRoleAsync

        [Fact]
        public async Task UpdateRole_ChangedPermissions_DiffUpdate()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            _context.Roles.Add(new Role
            {
                Id = roleId,
                Name = "Viewer",
                NormalizedName = "VIEWER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Books.View" });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Users.View" });
            await _context.SaveChangesAsync();

            var input = new UpdateRoleInput
            {
                Name = "Viewer",
                Permissions = new List<string> { "Books.View", "Roles.View" } // removed Users.View, added Roles.View
            };

            // Act
            var result = await _service.UpdateRoleAsync(roleId, input);

            // Assert
            Assert.True(result.Succeeded);
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();
            Assert.Equal(2, permissions.Count);
            Assert.Contains("Books.View", permissions);
            Assert.Contains("Roles.View", permissions);
            Assert.DoesNotContain("Users.View", permissions);
        }

        #endregion

        #region DeleteRoleAsync

        [Fact]
        public async Task DeleteRole_WithUsers_ReturnsError()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _context.Roles.Add(new Role
            {
                Id = roleId,
                Name = "InUse",
                NormalizedName = "INUSE",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            });
            _context.Users.Add(new User
            {
                Id = userId,
                UserName = "user1",
                NormalizedUserName = "USER1",
                Email = "user1@example.com",
                NormalizedEmail = "USER1@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId, AssignedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteRoleAsync(roleId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Contains("使用者指派"));
        }

        [Fact]
        public async Task DeleteRole_NoUsers_Succeeds()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            _context.Roles.Add(new Role
            {
                Id = roleId,
                Name = "Unused",
                NormalizedName = "UNUSED",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Some.Perm" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteRoleAsync(roleId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId));
            Assert.Empty(await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync());
        }

        #endregion

        #region GetRoleWithPermissionsAsync

        [Fact]
        public async Task GetRoleWithPermissions_ReturnsPermissionList()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            _context.Roles.Add(new Role
            {
                Id = roleId,
                Name = "Manager",
                NormalizedName = "MANAGER",
                Description = "Manager role",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Books.View" });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Books.Edit" });
            _context.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = "Users.View" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetRoleWithPermissionsAsync(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manager", result.Name);
            Assert.Equal("Manager role", result.Description);
            Assert.Equal(3, result.Permissions.Count);
            Assert.Contains("Books.View", result.Permissions);
        }

        #endregion
    }
}
