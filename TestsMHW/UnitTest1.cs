using HelloConsole.Exceptions;
using HelloConsole.Models;
using HelloConsole.Services;
using Telerik.JustMock;

namespace TestsMHW;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public void GetMonsterByIndex_ShouldReturnMonster()
    {
        // Arrange
        var mockApi =
            Mock.Create<IApiClient>();

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
                mockApi.GetAsync(
                    Arg.AnyString))
            .Returns(Task.FromResult(json));

        var service =
            new MonsterService(mockApi);

        // Act
        Monster? monster =
            service.GetMonsterByIndex(1)
                .GetAwaiter()
                .GetResult();

        // Assert
        Assert.That(
            monster,
            Is.Not.Null);

        Assert.That(
            monster!.Name,
            Is.EqualTo("Zoh Shia"));
    }
    
    [Test]
    public void GetMonsterByName_ShouldReturnMonster()
    {
        // Arrange
        var mockApi =
            Mock.Create<IApiClient>();

        var json =
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

        Mock.Arrange(() =>
                mockApi.GetAsync(
                    "fr/monsters"))
            .Returns(Task.FromResult(json));

        var service =
            new MonsterService(mockApi);

        // Act
        Monster? monster =
            service.GetMonsterByName("Arkveld")
                .GetAwaiter()
                .GetResult();

        // Assert
        Assert.That(
            monster,
            Is.Not.Null);

        Assert.That(
            monster!.Species,
            Is.EqualTo("flying-wyvern"));
    }
    
    [Test]
    public void GetMonsterByName_UnknownMonster_ShouldThrowMonsterNotFoundException()
    {
        // Arrange
        var mockApi =
            Mock.Create<IApiClient>();

        var json =
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

        Mock.Arrange(() =>
                mockApi.GetAsync(
                    "fr/monsters"))
            .Returns(Task.FromResult(json));

        var service =
            new MonsterService(mockApi);

        // Act + Assert
        var exception =
            Assert.Throws<
                MonsterNotFoundException>(
                () =>
                    service
                        .GetMonsterByName(
                            "Toto l'asticot")
                        .GetAwaiter()
                        .GetResult());

        Assert.That(
            exception!.Message,
            Is.EqualTo(
                "Monstre non trouvé."));
    }
}