namespace EduAssignments.Core.Models;

public class AppConfig
{
    public string TenantId { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string DefaultDomain { get; set; } = string.Empty;
}