namespace SolarX.SERVICE.Abstractions.IEmailServices;

public interface IEmailServices
{
    Task<bool> SendEmailAsync(MailContent content, string displayName);
}

public class MailContent
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}