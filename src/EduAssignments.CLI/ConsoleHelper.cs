using EduAssignments.Core.Models;

namespace EduAssignments.CLI.Helpers;

public static class Prompt
{
    public static string GetRequiredString(string message)
    {
        while (true)
        {
            Console.Write($"{message}: ");
            string input = Console.ReadLine()?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(input))
                return input;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Invalid input. Input must not be null or empty");
            Console.ResetColor();
        }
    }

    public static bool GetYesNo(string message)
    {
        string resp = "";

        while (true)
        {
            Console.WriteLine($"{message} (Y/N)");
            resp = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";

            if (resp == "y" || resp == "yes" || resp == "n" || resp == "no")
            {
                break;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Invalid input. Please enter Y or N.");
            Console.ResetColor();
        }

        return resp == "y" || resp == "yes";
    }
}

public static class Graphics
{
    public static async Task Spinner(CancellationToken token)
    {
        var frames = new[] { '|', '/', '-', '\\' };
        var i = 0;

        while (!token.IsCancellationRequested)
        {
            Console.Write($"\rFetching assignments... {frames[i++ % frames.Length]}");
            try { await Task.Delay(100, token); }
            catch {}
        }

        Console.Write("\rFetching assignments... ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("done");
        Console.ResetColor();
    }

    public static void PrintAssignmentsTable(IEnumerable<AssignmentItem> assignments)
    {
        const int classWidth = 25;
        const int nameWidth = 40;
        const int statusWidth = 12;
        const int dateWidth = 20;

        Console.WriteLine(
            $"{ "Class", -classWidth } " +
            $"{ "Assignment", -nameWidth } " +
            $"{ "Status", -statusWidth } " +
            $"{ "DueDate", -dateWidth }"
        );

        Console.WriteLine(new string('-', classWidth + nameWidth + statusWidth + dateWidth + 3));

        foreach (var a in assignments)
        {
            Console.WriteLine(
                $"{ Truncate(a.ClassName, classWidth), -classWidth } " +
                $"{ Truncate(a.DisplayName, nameWidth), -nameWidth } " +
                $"{ a.Status, -statusWidth } " +
                $"{ a.DueDateTime:yyyy-MM-dd HH:mm, -dateWidth}"
            );
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value.Length <= maxLength
            ? value
            : value[..(maxLength - 3)] + "...";
    }
}