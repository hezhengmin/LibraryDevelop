using System.Threading.Tasks;
using Zheng.Mdl.Models;

namespace Zheng.Bll.Interfaces
{
    public interface IRegistrationService
    {
        Task<Result> RegisterAsync(string userName, string email, string password, string confirmationUrlBase);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ResendVerificationAsync(string email, string confirmationUrlBase);
    }
}
