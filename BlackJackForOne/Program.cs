
using BlackJackForOne.Models;
using BlackJackForOne.Game;

namespace BlackJackForOne;

internal class Program
{
    static void Main ()
    {
        Console.WriteLine("Welcome to BlackJack for One!");

        var deck = new Deck();
        Console.WriteLine(deck.Cards.Count);
        foreach (Card c in deck.Cards)
        {
            Console.Write("{0} ", c.Rank);
        }
        Console.WriteLine();


        Hand dealer = new Hand(true);
        Hand player = new Hand();

        dealer.AddCard(deck.DealCard());
        player.AddCard(deck.DealCard());
        dealer.AddCard(deck.DealCard());
        player.AddCard(deck.DealCard());

        Console.WriteLine("Dealer Cards: {0}", dealer.ShowCards(true));
        Console.WriteLine("Player Cards: {0}", player.ShowCards());

        Console.WriteLine("Dealer final result {0}", dealer.ScoringResult());
        Console.WriteLine("Player final result {0}", player.ScoringResult());

        foreach (Card c in deck.Cards)
        {
            Console.Write("{0} ", c.Rank);
        }
        Console.WriteLine();
    }

};



