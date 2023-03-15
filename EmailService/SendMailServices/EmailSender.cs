using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace EmailService.SendMailServices;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailConfiguration> emailConfiguration, ILogger<EmailSender> logger)
    {
        _emailConfiguration = emailConfiguration.Value;
        _logger = logger;
        _logger.LogInformation("Create Send Email Service");
    }

    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        Send(emailMessage);
        _logger.LogInformation("Email sent");
    }

    public async Task SendEmailAsync(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        await SendAsync(emailMessage);
        _logger.LogInformation("Email sent");
    }

    public Task SendSmsAsync(string number, string message)
    {
        /* Dịch vụ send sms sau này sẽ cài đặt ở đây
         
         */

        //Sms gửi tạm thời sẽ lưu vào folder smsSave
        Directory.CreateDirectory("smsSave");
        var smsSaveFile = $@"smsSave/{number}-{Guid.NewGuid()}.txt";
        File.WriteAllTextAsync(smsSaveFile, message);
        _logger.LogInformation("Lỗi gửi sms, lưu tại - {SmsSaveFile}", smsSaveFile);
        return Task.FromResult(0);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_emailConfiguration.From, _emailConfiguration.UserName));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder()
        {
            HtmlBody = $"<h2 style='color:red;'>{message.Content}<h2>"
        };

        if (message.Attachments != null && message.Attachments.Any())
        {
            foreach (var attachment in message.Attachments)
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    attachment.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private async Task SendAsync(MimeMessage emailMessage)
    {
        using var client = new SmtpClient();
        {
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);

                await client.SendAsync(emailMessage);
            }
            catch (Exception e)
            {
                //Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailSave
                Directory.CreateDirectory("mailSave");
                var emailSaveFile = $@"mailSave/{Guid.NewGuid()}.eml";
                await emailMessage.WriteToAsync(emailSaveFile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - {EmailSaveFile}", emailSaveFile);
                _logger.LogError("Something went wrong inside Send action : {Message}", e.Message);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }

    private void Send(MimeMessage emailMessage)
    {
        using var client = new SmtpClient();
        {
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);

                client.Send(emailMessage);
            }
            catch (Exception e)
            {
                //Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailSave
                Directory.CreateDirectory("mailSave");
                var emailSaveFile = $@"mailSave/{Guid.NewGuid()}.eml";
                emailMessage.WriteTo(emailSaveFile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - {EmailSaveFile}", emailSaveFile);
                _logger.LogError("Something went wrong inside Send action : {Message}", e.Message);
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}