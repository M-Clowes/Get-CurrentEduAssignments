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