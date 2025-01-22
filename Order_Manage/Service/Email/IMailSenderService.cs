namespace Order_Manage.Service.Email
{
    public interface IMailSenderService
    {
        Task<bool> SendEmailAsync(MailData mailData);
    }
}
