using Microsoft.Extensions.DependencyInjection;
using EduAssignments.CLI.Helpers;
using EduAssignments.Core.Interfaces;
using EduAssignments.Services;

string? upnArg = null;
string? csvArg = null;
bool automate = false;

for (int i = 0; i < args.Length; ++i)
{
    if (args[i] == "--upn" && (i + 1 < args.Length))
        upnArg = args[i + 1];
    if (args[i] == "--csv" && (i + 1 < args.Length))
        csvArg = args[i + 1];
    if ((args[i] == "--auto") && (i + 1 < args.Length))
        automate = true;
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

if (!automate && string.IsNullOrWhiteSpace(upn) && Prompt.GetYesNo("Do you want to filter by student?"))
{
    string inputUpn = Prompt.GetRequiredString("Student UPN");
    upn = userService.FormatUserPrincipalName(inputUpn, config.DefaultDomain);
}
else if (!string.IsNullOrWhiteSpace(upn))
{
    upn = userService.FormatUserPrincipalName(upn, config.DefaultDomain);
}

using var cts = new CancellationTokenSource();
var spinner = Task.Run(() => Graphics.Spinner(cts.Token));

var assignments = await graphService.GetAssignmentsAsync(upn);

cts.Cancel();
await spinner;

Console.WriteLine($"Fetched {assignments.Count()} active assignments");

string path = string.Empty;
if (!string.IsNullOrWhiteSpace(csvArg))
{
    path = csvArg;
}
else if (automate)
{
    if (string.IsNullOrWhiteSpace(config.OutputFolder))
        throw new InvalidOperationException("Automation mode required app_config.json OutputFolder to be configured. Or --csv to be used");
    path = config.OutputFolder;
}
else if (Prompt.GetYesNo("Would you like this as a CSV?"))
{
    path = Prompt.GetRequiredString("Enter output directory path");
}
else
{
    Graphics.PrintAssignmentsTable(assignments);
    return;
}

if (!Directory.Exists(path))
    Directory.CreateDirectory(path);

var fileName = $"{DateTime.Now:yyyy.MM.dd}-assignments.csv";
var fullPath = Path.Combine(path, fileName);

exportService.ExportToCsv(assignments, fullPath);

Console.ForegroundColor = ConsoleColor.DarkGreen;
Console.WriteLine($"CSV exported to {fullPath}");
Console.ResetColor();