using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Zheng.Bll.Interfaces;

namespace Library.Server.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                emailSettings["FromName"] ?? "圖書管理系統",
                emailSettings["FromEmail"] ?? "noreply@library.local"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();

            try
            {
                var host = emailSettings["SmtpHost"] ?? "localhost";
                var port = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var useSsl = bool.Parse(emailSettings["UseSsl"] ?? "false");

                await client.ConnectAsync(host, port, useSsl);

                var user = emailSettings["SmtpUser"];
                if (!string.IsNullOrEmpty(user))
                {
                    await client.AuthenticateAsync(user, emailSettings["SmtpPass"]);
                }

                await client.SendAsync(message);
                _logger.LogInformation("驗證信已發送至 {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送郵件至 {Email} 失敗", toEmail);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
