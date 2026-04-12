using Microsoft.Extensions.Caching.Memory;
using Moq;
using Zheng.Bll.Interfaces;
using Zheng.Bll.Services;
using Zheng.Infra.Data.Models;
using Zheng.Mdl.Models;

namespace Library.Server.Tests.Services
{
    public class RegistrationServiceTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly IMemoryCache _memoryCache;
        private readonly RegistrationService _service;

        public RegistrationServiceTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _authServiceMock = new Mock<IAuthService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _service = new RegistrationService(
                _userServiceMock.Object,
                _authServiceMock.Object,
                _emailSenderMock.Object,
                _memoryCache);
        }

        #region RegisterAsync

        [Fact]
        public async Task RegisterAsync_Success_CreatesUserAndSendsEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            dynamic data = new System.Dynamic.ExpandoObject();
            data.Id = userId;
            data.UserName = "testuser";
            _userServiceMock
                .Setup(x => x.CreateUserAsync(It.IsAny<CreateUserInput>()))
                .ReturnsAsync(Result.Success("使用者建立成功", (object)data));
            _authServiceMock
                .Setup(x => x.GenerateEmailVerificationTokenAsync(userId))
                .ReturnsAsync("test-token");

            // Act
            var result = await _service.RegisterAsync("testuser", "test@example.com", "Password123", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.True(result.Succeeded);
            _userServiceMock.Verify(x => x.CreateUserAsync(
                It.Is<CreateUserInput>(i => i.UserName == "testuser" && i.Email == "test@example.com" && !i.EmailConfirmed)), Times.Once);
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                "test@example.com",
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains("驗證電子郵件"))), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateUserName_ReturnsFailure()
        {
            // Arrange
            _userServiceMock
                .Setup(x => x.CreateUserAsync(It.IsAny<CreateUserInput>()))
                .ReturnsAsync(Result.Failure(new[] { "使用者名稱已存在" }));

            // Act
            var result = await _service.RegisterAsync("testuser", "test@example.com", "Password123", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("使用者名稱已存在", result.Errors);
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_ReturnsFailure()
        {
            // Arrange
            _userServiceMock
                .Setup(x => x.CreateUserAsync(It.IsAny<CreateUserInput>()))
                .ReturnsAsync(Result.Failure(new[] { "Email 已被使用" }));

            // Act
            var result = await _service.RegisterAsync("testuser", "test@example.com", "Password123", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Email 已被使用", result.Errors);
        }

        #endregion

        #region ConfirmEmailAsync

        [Fact]
        public async Task ConfirmEmailAsync_ValidToken_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, EmailConfirmed = false, IsActive = true };
            _userServiceMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var encodedToken = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes("decoded-token"));
            _authServiceMock.Setup(x => x.VerifyEmailTokenAsync(userId, "decoded-token")).ReturnsAsync(true);

            // Act
            var result = await _service.ConfirmEmailAsync(userId.ToString(), encodedToken);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ConfirmEmailAsync_InvalidUser_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.ConfirmEmailAsync(userId.ToString(), "some-token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmEmailAsync_AlreadyConfirmed_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, EmailConfirmed = true, IsActive = true };
            _userServiceMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.ConfirmEmailAsync(userId.ToString(), "any-token");

            // Assert
            Assert.True(result);
            _authServiceMock.Verify(x => x.VerifyEmailTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ResendVerificationAsync

        [Fact]
        public async Task ResendVerificationAsync_Success_SendsEmailAndReturnsTrue()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", EmailConfirmed = false, IsActive = true };
            _userServiceMock.Setup(x => x.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
            _authServiceMock.Setup(x => x.GenerateEmailVerificationTokenAsync(user.Id)).ReturnsAsync("new-token");

            // Act
            var result = await _service.ResendVerificationAsync("test@example.com", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.True(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                "test@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResendVerificationAsync_RateLimited_ReturnsFalse()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", EmailConfirmed = false, IsActive = true };
            _userServiceMock.Setup(x => x.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
            _authServiceMock.Setup(x => x.GenerateEmailVerificationTokenAsync(user.Id)).ReturnsAsync("token");

            // First call should succeed
            await _service.ResendVerificationAsync("test@example.com", "https://localhost/api/Account/ConfirmEmail");

            // Act - second call within rate limit window
            var result = await _service.ResendVerificationAsync("test@example.com", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ResendVerificationAsync_NonExistentEmail_ReturnsTrueWithoutLeakingInfo()
        {
            // Arrange
            _userServiceMock.Setup(x => x.FindByEmailAsync("unknown@example.com")).ReturnsAsync((User?)null);

            // Act
            var result = await _service.ResendVerificationAsync("unknown@example.com", "https://localhost/api/Account/ConfirmEmail");

            // Assert - should return true to prevent account enumeration
            Assert.True(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResendVerificationAsync_AlreadyVerified_ReturnsTrueWithoutSendingEmail()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", EmailConfirmed = true, IsActive = true };
            _userServiceMock.Setup(x => x.FindByEmailAsync("test@example.com")).ReturnsAsync(user);

            // Act
            var result = await _service.ResendVerificationAsync("test@example.com", "https://localhost/api/Account/ConfirmEmail");

            // Assert
            Assert.True(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
