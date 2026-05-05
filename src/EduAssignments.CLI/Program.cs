using Microsoft.Extensions.DependencyInjection;
using EduAssignments.CLI.Helpers;
using EduAssignments.Core.Interfaces;
using EduAssignments.Services;

string? upnArg = null;
string? csvArg = null;

for (int i = 0; i < args.Length; ++i)
{
    if (args[i] == "--upn" && (i + 1 < args.Length))
        upnArg = args[i + 1];
    if (args[i] == "--csv" && (i + 1 < args.Length))
        csvArg = args[i + 1];
}

var services = new ServiceCollection();

services.AddSingleton<IConfigService, JsonConfigService>();
services.AddSingleton<IUserService, UserService>();
services.AddSingleton<IExportService, CsvExportService>();

var tempConfig = new JsonConfigService().GetConfig();
services.AddSingleton<IGraphService>(new GraphService(
    tempConfig.TenantId,
    tempConfig.AppId,
    tempConfig.ClientSecret
));

var serviceProvider = services.BuildServiceProvider();

var graphService = serviceProvider.GetRequiredService<IGraphService>();
var userService = serviceProvider.GetRequiredService<IUserService>();
var exportService = serviceProvider.GetRequiredService<IExportService>();
var config = serviceProvider.GetRequiredService<IConfigService>().GetConfig(); 

string? upn = upnArg;
if (string.IsNullOrWhiteSpace(upn) && Prompt.GetYesNo("Do you want to filter by student?"))
{
    string inputUpn = Prompt.GetRequiredString("Student UPN");
    upn = userService.FormatUserPrincipalName(inputUpn, config.DefaultDomain);
}
else if (!string.IsNullOrWhiteSpace(upn))
{
    upn = userService.FormatUserPrincipalName(upn, config.DefaultDomain);
}

Console.WriteLine("Fetching assignments...");
var assignments = await graphService.GetAssignmentsAsync(upn);

Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine("Complete.");
Console.ResetColor();

foreach (var a in assignments)
{
    Console.WriteLine($"[{a.Status}] {a.ClassName} - {a.DisplayName} (Due: {a.DueDateTime})");
}

if (!string.IsNullOrWhiteSpace(csvArg))
{
    if (!Directory.Exists(csvArg))
        Directory.CreateDirectory(csvArg);
    var fileName = $"{DateTime.Now:yyyy.MM.dd}-assignments.csv";
    var fullPath = Path.Combine(csvArg, fileName);

    exportService.ExportToCsv(assignments, fullPath);
}
else if (Prompt.GetYesNo("Would you like this as a CSV?"))
{
    var path = Prompt.GetRequiredString("Enter output directory path");
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    var fileName = $"{DateTime.Now:yyyy.MM.dd}-assignments.csv";
    var fullPath = Path.Combine(path, fileName);

    exportService.ExportToCsv(assignments, fullPath);

    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine($"CSV exported to {fullPath}");
    Console.ResetColor();
}