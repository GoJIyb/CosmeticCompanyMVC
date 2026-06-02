namespace CosmeticCompanyMVC.Models;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public bool UseSSL { get; set; }
    public string FromName { get; set; } = "CosmeticCompany";
    public string FromEmail { get; set; } = "mrx3xxxx@gmail.com";
}