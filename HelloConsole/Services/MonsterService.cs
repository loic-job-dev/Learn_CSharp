using HelloConsole.Exceptions;
using HelloConsole.Models;
using Newtonsoft.Json;

namespace HelloConsole.Services;

public class MonsterService
{
    private readonly IApiClient _apiClient;

    public MonsterService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Monster> GetMonsterByIndex(int index)
    {
        var jsonResponse =
            await _apiClient.GetAsync(
                $"fr/monsters/{index}"
            );

        Monster? monster = JsonConvert.DeserializeObject<Monster>(
                jsonResponse
            );

        if (monster is null)
        {
            throw new MonsterNotFoundException();
        }
        return monster;
    }
    
    public async Task<Monster> GetMonsterByName(
        string name)
    {
        Monster[] monsters =
            await GetAllMonsters();

        Monster? monster = monsters.FirstOrDefault(
            monster =>
                monster.Name.Equals(
                    name,
                    StringComparison
                        .OrdinalIgnoreCase
                )
        );
        
        if (monster is null)
        {
            throw new MonsterNotFoundException();
        }
        return monster;
    }

    private async Task<Monster[]> GetAllMonsters()
    {
        var json =
            await _apiClient.GetAsync(
                "fr/monsters"
            );

        return JsonConvert
                   .DeserializeObject<
                       Monster[]>(json)
               ?? [];
    }
}