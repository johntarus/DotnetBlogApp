namespace BlogApp.Utils;

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty; 
    public string SmtpPassword { get; set; } = string.Empty;
    public string SmtpSenderEmail { get; set; } = string.Empty;
    public string SmtpSenderName { get; set; } = string.Empty;
}