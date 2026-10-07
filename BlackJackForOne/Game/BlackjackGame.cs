using BlackJackForOne.Models;

namespace BlackJackForOne.Game;

public class BlackjackGame
{
    private Deck _currentDeck;
    public int InitDeckSize;
    public Hand Dealer;
    public Hand Player;
    public int splitBet;
    public int CurrentBet;
    public int PlayerBalance;
    public int MinimumBet = 5;
    public List<Hand> PlayerSplit;


    public BlackjackGame()
    {
        _currentDeck = new Deck();
        InitDeckSize = _currentDeck.Cards.Count;
        Dealer = new Hand(true);
        Player = new Hand();
        PlayerSplit = [];
    }

    public bool PlaceYourBet(int bet)
    {
        if (bet > PlayerBalance || bet < MinimumBet) return false;
        
        Player.Bet = bet;
        CurrentBet = bet;
        Console.WriteLine("Place your bet, Player.Bet is: {0}", Player.Bet);
        UpdateBalance("Bet");
        return true;
    }


    public void DealHands()
    {
        Dealer.Cards.Clear();
        Player.Cards.Clear();
        Card eight = new Card(Rank.Eight, Suit.Clubs);
        //Card king = new Card(Rank.King);
        Player.AddCard(eight);
        Player.AddCard(eight);
        
        //Player.AddCard(_currentDeck.DealCard());
        Dealer.AddCard(_currentDeck.DealCard());
        //Player.AddCard(_currentDeck.DealCard());
        Dealer.AddCard(_currentDeck.DealCard());
    }

    public bool Hit(bool isdealer)
    {
        if (isdealer)
        {
            Dealer.AddCard(_currentDeck.DealCard());
            return Dealer.HasBusted();
        }
        
        Player.AddCard(_currentDeck.DealCard());
        return Player.HasBusted();
    }

    public void NewHit(Hand hand)
    {
        hand.AddCard(_currentDeck.DealCard());
    }

    public void Split()
    {
        
        var oldHand = Player.SplitHand();
        var hand1 = new Hand();
        var hand2 = new Hand();

        hand1.AddCard(oldHand.Item1);
        hand2.AddCard(oldHand.Item2);

        hand1.AddCard(_currentDeck.DealCard());
        hand2.AddCard(_currentDeck.DealCard());

        hand1.Bet = Player.Bet;
        hand2.Bet = Player.Bet;

        Player.MultHands.Add(hand1);
        Player.MultHands.Add(hand2);
        PlayerBalance -= Player.Bet;
        
        Player.HasSplit = true;
    }

    public void DoubleDown(Hand hand)
    {
        hand.AddCard(_currentDeck.DealCard());
        Console.WriteLine("hand.bet is: {0}", hand.Bet);
        PlayerBalance -= hand.Bet;
        hand.Bet += hand.Bet;
    }

    public void UpdateBalance(string cond, int deposit = 0)
    {
        switch (cond)
        {
            case "Win":
                PlayerBalance += deposit*2;
                break;
            case "Tie":
                PlayerBalance += deposit;
                break;
            case "Bet":
                PlayerBalance -= CurrentBet;
                break;
            case "Deposit":
                PlayerBalance += deposit;
                break;
        }
    }

    public int CurrentHandValue(Hand hand)
    {
        if (hand.Cards.Count == 0) return 0;
        var val = 0;
        int aceCounter = 0;
        foreach (Card c in hand.Cards)
        {
            val += c.CardValue;
            if (c.Rank == Rank.Ace) aceCounter++;
            while (aceCounter > 0 && val > 21)
            {
                val -= 10;
                aceCounter--;
            }
        }
        return val;
    }

    public bool IsBusted(Hand hand)
    {
        return CurrentHandValue(hand) > 21;
    }

    public void ResetForNextRound()
    {
        CurrentBet = 0;
        Player.HasSplit = false;
        Player.MultHands.Clear();
        // shuffle new deck if 50% of cards have been used
        if(CurrentDeckSize() <= InitDeckSize/2)
        {
            _currentDeck = new Deck();
            Console.WriteLine("Deck has been reset and shuffled");
        }


    }

    public int CurrentDeckSize()
    {
        return _currentDeck.CurrentDeckSize();
    }
}