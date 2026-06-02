using System.Net;
using HelloConsole.Exceptions;
using HelloConsole.Models;
using HelloConsole.Services;
using Microsoft.Extensions.Caching.Memory;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace TestsMHW;

public class Tests
{
    
    private IApiClient _mockApi;
    private IMemoryCache _memoryCache;
    private MonsterService _service;

    private const string MonstersJson =
        """
        [
          {
            "id": 1,
            "name": "Zoh Shia",
            "species": "construct",
            "description": "Guardian"
          },
          {
            "id": 2,
            "name": "Arkveld",
            "species": "flying-wyvern",
            "description": "Guardian"
          }
        ]
        """;

    [SetUp]
    public void Setup()
    {
        _mockApi = Mock.Create<IApiClient>();
        
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _service = new MonsterService(_mockApi, _memoryCache);
    }
    
    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }
    
    [Test]
    public void
        GetMonsterByIndex_ShouldReturnMonster()
    {
        // Arrange
        var json =
            """
            {
                "id": 1,
                "name": "Zoh Shia",
                "species": "construct",
                "description": "Guardian"
            }
            """;

        Mock.Arrange(() =>
                _mockApi.GetAsync(
                    Arg.AnyString,
                    Arg.IsAny<DateTimeOffset?>()))
            .Returns(
                Task.FromResult(
                    CreateResponse(
                        json)));

        // Act
        Monster? monster =
            _service
                .GetMonsterByIndex(1)
                .GetAwaiter()
                .GetResult();

        // Assert
        Assert.That(
            monster,
            Is.Not.Null);

        Assert.That(
            monster!.Name,
            Is.EqualTo(
                "Zoh Shia"));
    }

    [Test]
    public void
        GetMonsterByName_ShouldReturnMonster()
    {
        // Arrange
        ArrangeMonsterList();

        // Act
        Monster? monster =
            _service
                .GetMonsterByName(
                    "Arkveld")
                .GetAwaiter()
                .GetResult();

        // Assert
        Assert.That(
            monster,
            Is.Not.Null);

        Assert.That(
            monster.Species,
            Is.EqualTo(
                "flying-wyvern"));
    }

    [Test]
    public void
        GetMonsterByName_UnknownMonster_ShouldThrowMonsterNotFoundException()
    {
        // Arrange
        ArrangeMonsterList();

        // Act + Assert
        var exception =
            Assert.Throws<
                MonsterNotFoundException>(
                () =>
                    _service
                        .GetMonsterByName(
                            "Toto l'asticot")
                        .GetAwaiter()
                        .GetResult());

        Assert.That(
            exception!.Message,
            Is.EqualTo(
                "Monstre non trouvé."));
    }

    private void ArrangeMonsterList()
    {
        Mock.Arrange(() =>
                _mockApi.GetAsync(
                    "fr/monsters",
                    Arg.IsAny<
                        DateTimeOffset?>()))
            .Returns(
                Task.FromResult(
                    CreateResponse(
                        MonstersJson)));
    }

    private HttpResponseMessage CreateResponse (string json)
    {
        return new HttpResponseMessage(
            HttpStatusCode.OK)
        {
            Content =
                new StringContent(
                    json)
        };
    }
}