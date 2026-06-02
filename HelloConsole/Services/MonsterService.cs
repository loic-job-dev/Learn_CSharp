using HelloConsole.Exceptions;
using HelloConsole.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace HelloConsole.Services;

public class MonsterService
{
    private readonly IApiClient _apiClient;
    
    private readonly IMemoryCache _memoryCache;

    private DateTimeOffset? _lastModified;

    public MonsterService(IApiClient apiClient, IMemoryCache memoryCache)
    {
        _apiClient = apiClient;
        _memoryCache = memoryCache;
    }

    public async Task<Monster> GetMonsterByIndex(int index)
    {
        //No cache due to low data received
        using HttpResponseMessage response =
            await _apiClient.GetAsync($"fr/monsters/{index}");
        
        response.EnsureSuccessStatusCode();

        var jsonResponse =
            await response.Content
                .ReadAsStringAsync();
        

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

    private async Task<Monster[]>
        GetAllMonsters()
    {
        using HttpResponseMessage response =
            await _apiClient.GetAsync(
                "fr/monsters",
                _lastModified);

        //If there isn't new data on the API server
        if (response.StatusCode ==
            System.Net.HttpStatusCode
                .NotModified)
        {
            //If there's cached data
            if (_memoryCache.TryGetValue(
                    "monster_list",
                    out Monster[]?
                        cachedMonsters))
            {
                return cachedMonsters!;
            }
        }

        //Else, the HTTP response is serialized
        response
            .EnsureSuccessStatusCode();

        var jsonResponse =
            await response.Content
                .ReadAsStringAsync();

        Monster[] monsters =
            JsonConvert
                .DeserializeObject<
                    Monster[]>(jsonResponse)
            ?? [];

        //New value for lastModified
        _lastModified =
            response.Content
                .Headers
                .LastModified;

        //New value for the cache
        _memoryCache.Set(
            "monster_list",
            monsters,
            TimeSpan.FromMinutes(60));

        return monsters;
    }
}