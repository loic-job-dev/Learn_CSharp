namespace HelloConsole.Models;

public class Reward
{
    public int Id { get; set; }

    public Item Item { get; set; } = new();
}