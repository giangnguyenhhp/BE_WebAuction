namespace EmailService.SendMailServices;

public interface IEmailSender
{
    void SendEmail(Message message);
    Task SendEmailAsync(Message message);
    Task SendSmsAsync(string number,string message);
}