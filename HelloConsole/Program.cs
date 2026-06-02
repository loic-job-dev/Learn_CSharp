using HelloConsole.Exceptions;
using HelloConsole.Helpers;
using HelloConsole.Models;
using HelloConsole.Services;
using Microsoft.Extensions.Caching.Memory;

namespace HelloConsole;

class Program
{
    static void Main(string[] args)
    {
        ApiClient client = new ApiClient();
        
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        
        MonsterService service = new MonsterService(client, cache);

        int choice = 0;
        
        while (choice != 9)
        {
            Console.WriteLine(
                "\nChoisissez un mode de consultation :\n" +
                "1 - Par index\n" +
                "2 - Par nom de monstre\n" +
                "9 - Quitter"
            );

            if (!int.TryParse(
                    Console.ReadLine(),
                    out choice))
            {
                Console.WriteLine("Nombre invalide");
                return;
            }

            switch (choice)
            {
                case 1:
                {
                    Console.WriteLine("Choisissez un index de monstre à consulter :");

                    if (!int.TryParse(
                            Console.ReadLine(),
                            out int index))
                    {
                        Console.WriteLine(
                            "Nombre invalide");
                        break;
                    }

                    try
                    {
                        Monster monster =
                            service
                                .GetMonsterByIndex(index)
                                .GetAwaiter()
                                .GetResult();

                        monster.DisplayInfos();
                    }
                    catch (HttpRequestException)
                    {
                        ConsoleHelper.WriteLineColor(
                            "Index inconnu",
                            ConsoleColor.DarkRed);
                    }

                    break;
                }

                case 2:
                {
                    Console.WriteLine("\nChoisissez un nom de monstre à consulter :");

                    string name = Console.ReadLine() ?? "";

                    try
                    {
                        Monster monster =
                            service
                                .GetMonsterByName(name)
                                .GetAwaiter()
                                .GetResult();

                        monster.DisplayInfos();
                    }
                    catch (MonsterNotFoundException ex)
                    {
                        ConsoleHelper.WriteLineColor(
                            ex.Message,
                            ConsoleColor.DarkRed);
                    }

                    break;
                }

                case 9:
                    break;

                default:
                    Console.WriteLine("Choix invalide");
                    break;
            }
        }
    }
}