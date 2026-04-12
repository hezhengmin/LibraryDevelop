#nullable enable
using System.Security.Claims;
using System.Threading.Tasks;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Interfaces
{
    public class ExternalLoginResult
    {
        public bool Succeeded { get; set; }
        public User? User { get; set; }
        public string? Error { get; set; }
        public bool IsNewUser { get; set; }
    }

    public interface IExternalLoginService
    {
        Task<ExternalLoginResult> ProcessExternalLoginAsync(
            string loginProvider,
            string providerKey,
            string? email,
            string? name);
    }
}
