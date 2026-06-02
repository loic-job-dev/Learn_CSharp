namespace HelloConsole.Helpers;

public static class ConsoleHelper
{
    public static void WriteLineColor(
        string text,
        ConsoleColor color)
    {
        Console.ForegroundColor =
            color;

        Console.WriteLine(text);

        Console.ResetColor();
    }
}