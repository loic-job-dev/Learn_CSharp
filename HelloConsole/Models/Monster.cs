using HelloConsole.Helpers;

namespace HelloConsole.Models;

public class Monster
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Description { get; set; }

    public Reward[] Rewards { get; set; } = [];

    public void DisplayInfos()
    {
        Console.WriteLine();

        // Nom
        Console.ForegroundColor =
            ConsoleColor.Cyan;
        Console.Write(Name);

        // Espèce
        Console.ForegroundColor =
            ConsoleColor.DarkBlue;
        Console.Write($" ({Species})");

        Console.ResetColor();
        Console.Write(" : ");

        // Description
        Console.ForegroundColor =
            ConsoleColor.Blue;
        Console.WriteLine(Description);

        Console.ResetColor();

        if (Rewards.Length > 0)
        {
            Console.WriteLine();

            ConsoleHelper.WriteLineColor(
                "On peut trouver sur ce monstre les items suivants :",
                ConsoleColor.Yellow);

            for (int i = 0; i < Rewards.Length; i++)
            {
                var reward = Rewards[i];

                ConsoleColor color =
                    i % 2 == 0
                        ? ConsoleColor.DarkYellow
                        : ConsoleColor.Magenta;

                ConsoleHelper.WriteLineColor(
                    $"- {reward.Item.Name} : {reward.Item.Description}",
                    color);
            }
        }
    }
}