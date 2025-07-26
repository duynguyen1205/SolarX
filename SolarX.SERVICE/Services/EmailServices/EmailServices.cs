using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SolarX.SERVICE.Abstractions.IEmailServices;

namespace SolarX.SERVICE.Services.EmailServices;

public class EmailServices : IEmailServices
{
    private readonly EmailOption _emailOption = new();

    public EmailServices(IConfiguration configuration)
    {
        configuration.GetSection("MailOption").Bind(_emailOption);
    }

    public async Task<bool> SendEmailAsync(MailContent content, string displayName)
    {
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.Sender = new MailboxAddress(displayName, _emailOption.Mail);
            mimeMessage.From.Add(new MailboxAddress(displayName, _emailOption.Mail));
            mimeMessage.To.Add(MailboxAddress.Parse(content.To));
            mimeMessage.Subject = content.Subject;


            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = content.Body
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            // dùng SmtpClient của MailKit

            using MailKit.Net.Smtp.SmtpClient client = new();
            await client.ConnectAsync(_emailOption.Host, _emailOption.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailOption.Mail, _emailOption.Password);
            await client.SendAsync(mimeMessage);

            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending email: {e.Message}");
            return false;
        }
    }
}