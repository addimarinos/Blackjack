namespace BlackJackForOne.Models;

public sealed record Card(Rank Rank)
{
    public int CardValue => Rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => 10,
        Rank.Ace => 11,
        _ => (int)Rank
    };
    public override string ToString() => $"{Rank}";
}

public enum Rank
{
    Ace = 1,
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}