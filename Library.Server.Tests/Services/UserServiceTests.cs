using Microsoft.EntityFrameworkCore;
using Moq;
using Zheng.Bll.Interfaces;
using Zheng.Bll.Services;
using Zheng.Infra.Data.Models;

namespace Library.Server.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);
            _authServiceMock = new Mock<IAuthService>();
            _authServiceMock
                .Setup(a => a.HashPassword(It.IsAny<string>()))
                .Returns((string p) => $"HASHED_{p}");

            _service = new UserService(_context, _authServiceMock.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region CreateUserAsync

        [Fact]
        public async Task CreateUser_ValidInput_CreatesUserWithBCryptHash()
        {
            // Arrange
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var input = new CreateUserInput
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                EmailConfirmed = true,
                Roles = new List<string> { "Admin" }
            };

            // Act
            var result = await _service.CreateUserAsync(input);

            // Assert
            Assert.True(result.Succeeded);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
            Assert.NotNull(user);
            Assert.Equal("TESTUSER", user.NormalizedUserName);
            Assert.Equal("TEST@EXAMPLE.COM", user.NormalizedEmail);
            Assert.Equal("HASHED_Password123!", user.PasswordHash);
            Assert.Equal(2, user.PasswordHashVersion);
            Assert.True(user.IsActive);
            _authServiceMock.Verify(a => a.HashPassword("Password123!"), Times.Once);

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
            Assert.NotNull(userRole);
            Assert.Equal(role.Id, userRole.RoleId);
        }

        [Fact]
        public async Task CreateUser_DuplicateUserName_ReturnsFailure()
        {
            // Arrange
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "existing",
                NormalizedUserName = "EXISTING",
                Email = "existing@example.com",
                NormalizedEmail = "EXISTING@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            var input = new CreateUserInput
            {
                UserName = "existing",
                Email = "new@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _service.CreateUserAsync(input);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("使用者名稱已存在", result.Errors);
        }

        [Fact]
        public async Task CreateUser_DuplicateEmail_ReturnsFailure()
        {
            // Arrange
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "existing",
                NormalizedUserName = "EXISTING",
                Email = "dup@example.com",
                NormalizedEmail = "DUP@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            var input = new CreateUserInput
            {
                UserName = "newuser",
                Email = "dup@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _service.CreateUserAsync(input);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Email 已被使用", result.Errors);
        }

        #endregion

        #region GetUsersAsync

        [Fact]
        public async Task GetUsers_WithPagination_ReturnsPagedResult()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                _context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    UserName = $"user{i:D2}",
                    NormalizedUserName = $"USER{i:D2}",
                    Email = $"user{i}@example.com",
                    NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                    PasswordHash = "hash",
                    PasswordHashVersion = 2,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUsersAsync(page: 2, pageSize: 5);

            // Assert
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Items.Count());
            Assert.Equal(2, result.PageNumber);
        }

        [Fact]
        public async Task GetUsers_WithKeyword_FiltersResults()
        {
            // Arrange
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "alice",
                NormalizedUserName = "ALICE",
                Email = "alice@example.com",
                NormalizedEmail = "ALICE@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                NormalizedUserName = "BOB",
                Email = "bob@example.com",
                NormalizedEmail = "BOB@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUsersAsync(1, 10, "alice");

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("alice", result.Items.First().UserName);
        }

        [Fact]
        public async Task GetUsers_ExcludesInactiveUsers()
        {
            // Arrange
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "active",
                NormalizedUserName = "ACTIVE",
                Email = "active@example.com",
                NormalizedEmail = "ACTIVE@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "inactive",
                NormalizedUserName = "INACTIVE",
                Email = "inactive@example.com",
                NormalizedEmail = "INACTIVE@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUsersAsync(1, 10);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("active", result.Items.First().UserName);
        }

        #endregion

        #region DeleteUserAsync

        [Fact]
        public async Task DeleteUser_SetsIsActiveFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = userId,
                UserName = "todelete",
                NormalizedUserName = "TODELETE",
                Email = "delete@example.com",
                NormalizedEmail = "DELETE@EXAMPLE.COM",
                PasswordHash = "hash",
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(user);
            Assert.False(user.IsActive);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public async Task DeleteUser_NotFound_ReturnsFailure()
        {
            // Act
            var result = await _service.DeleteUserAsync(Guid.NewGuid());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("找不到指定的使用者", result.Errors);
        }

        #endregion

        #region ResetPasswordAsync

        [Fact]
        public async Task ResetPassword_UpdatesHashAndVersion()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = userId,
                UserName = "resetme",
                NormalizedUserName = "RESETME",
                Email = "reset@example.com",
                NormalizedEmail = "RESET@EXAMPLE.COM",
                PasswordHash = "old_hash",
                PasswordHashVersion = 1,
                SecurityStamp = "old-stamp",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ResetPasswordAsync(userId, "NewPassword123!");

            // Assert
            Assert.True(result.Succeeded);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(user);
            Assert.Equal("HASHED_NewPassword123!", user.PasswordHash);
            Assert.Equal(2, user.PasswordHashVersion);
            Assert.NotEqual("old-stamp", user.SecurityStamp);
            Assert.NotNull(user.UpdatedAt);
        }

        [Fact]
        public async Task ResetPassword_NotFound_ReturnsFailure()
        {
            // Act
            var result = await _service.ResetPasswordAsync(Guid.NewGuid(), "NewPassword123!");

            // Assert
            Assert.False(result.Succeeded);
        }

        #endregion
    }
}
