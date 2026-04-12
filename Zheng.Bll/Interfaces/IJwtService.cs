using System;
using System.Threading.Tasks;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateAccessTokenAsync(User user);
        string GenerateRefreshToken();
        Task<bool> ValidateTokenAsync(string token);
        Task SaveTokenAsync(Guid userId, string accessToken, string refreshToken);
    }
}