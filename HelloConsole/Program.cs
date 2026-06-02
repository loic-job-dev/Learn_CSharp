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

        Console.WriteLine(
            "Choisissez un mode de consultation :\n" +
            "1 - Par index\n" +
            "2 - Par nom de monstre"
        );

        if (!int.TryParse(
                Console.ReadLine(),
                out int choice))
        {
            Console.WriteLine("Nombre invalide");
            return;
        }

        if (choice == 1)
        {
            Console.WriteLine(
                "Choisissez un index de monstre à consulter :"
            );

            if (!int.TryParse(
                    Console.ReadLine(),
                    out int index))
            {
                Console.WriteLine("Nombre invalide");
                return;
            }

            try
            {
                Monster monster = service.GetMonsterByIndex(index)
                    .GetAwaiter()
                    .GetResult();
                monster.DisplayInfos();
            }
            catch (HttpRequestException ex)
            {
                ConsoleHelper.WriteLineColor("Index inconnu", ConsoleColor.DarkRed);
            }
        }

        if (choice == 2)
        {
            Console.WriteLine(
                "\nChoisissez un nom de monstre à consulter :"
            );

            string name = Console.ReadLine();

            try
            {
                Monster monsterByName = service.GetMonsterByName(name)
                    .GetAwaiter()
                    .GetResult();

                monsterByName.DisplayInfos();
            }
            catch (MonsterNotFoundException ex)
            {
                ConsoleHelper.WriteLineColor(ex.Message, ConsoleColor.DarkRed);
            }
        }
    }
}