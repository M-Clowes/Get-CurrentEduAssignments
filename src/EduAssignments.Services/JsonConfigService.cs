using Microsoft.Extensions.Configuration;
using EduAssignments.Core.Interfaces;
using EduAssignments.Core.Models;

namespace EduAssignments.Services;

public class JsonConfigService : IConfigService
{
    private readonly AppConfig _config;

    public JsonConfigService(string filePath = "app_config.json")
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(filePath, optional: false, reloadOnChange: true);
        IConfiguration config = builder.Build();

        _config = config.Get<AppConfig>() ?? throw new InvalidOperationException("Could not load configuration.");
    }

    public AppConfig GetConfig() => _config;
}