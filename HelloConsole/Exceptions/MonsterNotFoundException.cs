namespace HelloConsole.Exceptions;

public class MonsterNotFoundException : Exception
{
    public MonsterNotFoundException()
        : base(
            "Monstre non trouvé.")
    {
    }
}