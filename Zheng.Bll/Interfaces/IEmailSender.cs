using System.Threading.Tasks;

namespace Zheng.Bll.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
