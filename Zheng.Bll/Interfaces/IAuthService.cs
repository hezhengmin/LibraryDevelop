using System;
using System.Threading.Tasks;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Interfaces
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public User? User { get; set; }
        public string? Error { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResult> ValidatePasswordAsync(string userName, string password);
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, int hashVersion, string providedPassword);
        Task<string> GenerateEmailVerificationTokenAsync(Guid userId);
        Task<bool> VerifyEmailTokenAsync(Guid userId, string token);
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<bool> VerifyPasswordResetTokenAsync(Guid userId, string token);
    }
}
