using HelloConsole.Exceptions;
using HelloConsole.Models;
using Newtonsoft.Json;

namespace HelloConsole.Services;

public class MonsterService
{
    private readonly IApiClient _apiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterService"/> class.
    /// </summary>
    /// <param name="apiClient">
    /// HTTP client used to communicate with the Monster Hunter Wilds API.
    /// </param>
    /// <param name="memoryCache">
    /// Local cache used to store monster data and reduce API calls.
    /// </param>
    public MonsterService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    /// <summary>
    /// Retrieves a monster from the API using its unique identifier.
    /// </summary>
    /// <param name="index">
    /// The monster identifier used by the API.
    /// </param>
    /// <returns>
    /// A <see cref="Monster"/> corresponding to the specified identifier.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown when the API request fails.
    /// </exception>
    /// <exception cref="MonsterNotFoundException">
    /// Thrown when the API response cannot be deserialized into a valid monster.
    /// </exception>
    public async Task<Monster> GetMonsterByIndex (int index)
    {
        //No cache due to low data received
        ApiResponse response =
            await _apiClient.GetAsync($"fr/monsters/{index}");

        if (!response.IsSuccess)
        {
            throw new InvalidApiRequestException();
        }
        
        if (response.Content is null)
        {
            throw new MonsterNotFoundException();
        }
        
        Monster? monster = JsonConvert.DeserializeObject<Monster>(response.Content);

        if (monster is null)
        {
            throw new MonsterNotFoundException();
        }
        return monster;
        
    }
    
    /// <summary>
    /// Retrieves a monster by its name.
    /// </summary>
    /// <param name="name">
    /// The monster name to search for.
    /// </param>
    /// <returns>
    /// The first monster whose name matches the specified value,
    /// ignoring character casing.
    /// </returns>
    /// <exception cref="MonsterNotFoundException">
    /// Thrown when no monster with the specified name exists.
    /// </exception>
    public async Task<Monster> GetMonsterByName (string name)
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
        Directory.CreateDirectory("cache");
        
        string cacheFile =
            "cache/monsters.json";
        
        if (File.Exists(cacheFile))
        {
            DateTime lastWriteTime = File.GetLastWriteTimeUtc(cacheFile);

            //If the cache file is less than 60-minutes-old, the data comes from this
            if (DateTime.UtcNow - lastWriteTime < TimeSpan.FromMinutes(60))
            {
                string json = await File.ReadAllTextAsync(cacheFile);
                
                Console.WriteLine("Utilisation du fichier en cache");
                
                return DeserializeMonsters(json);
            }
            // If the file is older than 60 minutes
            else
            {
                ApiResponse response =
                    await _apiClient.GetAsync($"fr/monsters", lastWriteTime);
                
                if (!response.IsSuccess)
                {
                    throw new InvalidApiRequestException();
                }
        
                if (response.Content is null)
                {
                    throw new MonsterNotFoundException();
                }
                
                //If there's no updates on the serveur
                if (response.IsNotModified)
                {
                    File.SetLastWriteTimeUtc(cacheFile,  DateTime.UtcNow);
                    
                    Console.WriteLine("Utilisation du fichier en cache");
                    
                    string json = await File.ReadAllTextAsync(cacheFile);
                
                    return DeserializeMonsters(json);
                }
                //If there's updates, the file is updated too
                else
                {
                    Console.WriteLine("Mise à jour du fichier en cache");
                    
                    await File.WriteAllTextAsync(
                        cacheFile,
                        response.Content);
                    
                    return DeserializeMonsters(response.Content);
                }
            }
        }
        //If there's no file in cache
        else
        {
            ApiResponse response =
                await _apiClient.GetAsync($"fr/monsters");
            
            if (!response.IsSuccess)
            {
                throw new InvalidApiRequestException();
            }
        
            if (response.Content is null)
            {
                throw new MonsterNotFoundException();
            }
            
                    
            await File.WriteAllTextAsync(
                cacheFile,
                response.Content);
                    
            Console.WriteLine("Ecriture du fichier en cache");
            
            return DeserializeMonsters(response.Content);
        }
    }
    
    private static Monster[] DeserializeMonsters (string json)
    {
        Monster[] monsters = JsonConvert.DeserializeObject<Monster[]>(json) ?? [];

        return monsters;
    }
}