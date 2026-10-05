namespace BlackJackForOne.Models;

public sealed record Card(Rank Rank, Suit Suit)
{
    public int CardValue => Rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => 10,
        Rank.Ace => 11,
        _ => (int)Rank
    };
    public override string ToString() => $"{Rank}";

    public string ImagePath =>
        Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Images",
            $"{Rank switch
            {
                Rank.Ace => "A",
                Rank.Ten => "10",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                _ => ((int)Rank).ToString()
            }}{(char)Suit}.png");
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

public enum Suit
{
    Hearts = 'H',
    Spades = 'S',
    Diamonds = 'D',
    Clubs = 'C'
}
