using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Models;
using Order_Manage.Repository;
using Microsoft.AspNetCore.Identity;

namespace Order_Manage.Service.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IConfiguration _configuration;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public int generateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999);
        }
        public ApiResponse<string> handleSendCodeToMail(ViaCodeRequest request)
        {
            try
            {
                var acc = _accountRepository.FindByEmail(request.email);
                if (acc != null)
                {
                    return ApiResponse<string>.Error(202, "Email này đã tồn tại");
                }
                var code = generateCode();
                //var viaCode = insertCode(code, request.email);
                int emailSent = sendCodeToEmail(request.email, "Verification Code", $"Your verification code is: {code}");
                if (emailSent == 1)
                {
                    insertCode(code, request.email);
                    return ApiResponse<string>.Success(null, "Mã xác minh đã được gửi qua email");
                }
                else
                {
                    return ApiResponse<string>.Error(500, "Không thể gửi email. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in handleSendCodeToMail");
                return ApiResponse<string>.Error(500, $"An error occurred: {ex.Message}");
            }
        }
        public ViaCode insertCode(int code, string email)
        {
            var viaCode = new ViaCode
            {
                viaCode = code,
                email = email,
                createAt = DateTime.UtcNow
            };

            var result = _accountRepository.InsertViaCode(viaCode);
            if (result <= 0)
            {
                throw new Exception("Failed to insert verification code into database");
            }

            return viaCode;
        }
        public int sendCodeToEmail(string to, string subject, string content)
        {
            var smtpConfig = _configuration.GetSection("SmtpConfig");
            var smtpServer = smtpConfig["Server"];
            var smtpPort = int.Parse(smtpConfig["Port"]);
            var smtpUser = smtpConfig["Username"];
            var smtpPass = smtpConfig["Password"];

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Order Management", smtpUser));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;

                email.Body = new TextPart("plain")
                {
                    Text = content
                };

                using var client = new SmtpClient();
                client.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(smtpUser, smtpPass);
                client.Send(email);
                client.Disconnect(true);

                _logger.LogInformation($"Email sent successfully to {to}");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while sending email to {to}");
                return 0;
            }
        }
    }
}
