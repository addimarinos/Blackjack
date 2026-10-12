using BlackJackForOne.Models;

namespace BlackJackForOne.Game;

public class BlackjackGame
{
    private Deck _currentDeck;
    public int InitDeckSize;
    public readonly Hand Dealer;
    public Player Player;
    public int splitBet;
    public int CurrentBet;
    public int PlayerBalance;
    public int MinimumBet = 5;


    public BlackjackGame()
    {
        _currentDeck = new Deck();
        InitDeckSize = _currentDeck.Cards.Count;
        Dealer = new Hand(true);
        Player = new Player("Bob");
    }

    public bool PlaceYourBet(int bet)
    {
        if (bet > Player.Balance || bet < MinimumBet) return false;
        
        Player.Bet = bet;
        CurrentBet = bet;
        UpdateBalance("Bet");
        return true;
    }


    public void DealHands()
    {
        //var eight = new Card(Rank.Eight, Suit.Clubs);
        //Card king = new Card(Rank.King);
        var playerHand = new Hand();

        // Testing split
        // playerHand.AddCard(eight);
        // playerHand.AddCard(eight);

        // Multiplayer: loop through table size, add cards 
        playerHand.AddCard(_currentDeck.DealCard());
        Dealer.AddCard(_currentDeck.DealCard());
        playerHand.AddCard(_currentDeck.DealCard());
        Dealer.AddCard(_currentDeck.DealCard());

        playerHand.Bet = Player.Bet;
        Player.Hands.Add(playerHand);
    }

    public bool Hit(Hand hand, bool isdealer)
    {
        if (isdealer)
        {
            Dealer.AddCard(_currentDeck.DealCard());
            return Dealer.HasBusted();
        }
        hand.AddCard(_currentDeck.DealCard());

        return IsBusted(hand);
    }

    public void NewHit(Hand hand)
    {
        hand.AddCard(_currentDeck.DealCard());
    }

    public void Split(Player player)
    {
        var oldHand = player.Hands.First().SplitHand();
        var hand1 = new Hand();
        var hand2 = new Hand();

        hand1.AddCard(oldHand.Item1);
        hand2.AddCard(oldHand.Item2);

        hand1.AddCard(_currentDeck.DealCard());
        hand2.AddCard(_currentDeck.DealCard());

        hand1.Bet = Player.Bet;
        hand2.Bet = Player.Bet;
        Player.Balance -= Player.Bet;
        Player.Bet += Player.Bet;
        

        Player.Hands.Clear();
        Player.Hands.Add(hand1);
        Player.Hands.Add(hand2);
        
        Player.HasSplit = true;
    }

    public bool CanDoubleDown(Player player, Hand hand)
    {
        if (hand.Cards.Count > 2) return false;
        if (player.Balance < hand.Bet) return false;
        
        return true;
    }
    public void DoubleDown(Hand hand)
    {
        hand.AddCard(_currentDeck.DealCard());
        Player.Balance -= hand.Bet;
        Player.Bet += hand.Bet;
        hand.Bet += hand.Bet;
    }

    public void UpdateBalance(string cond, int deposit = 0)
    {
        switch (cond)
        {
            case "Win":
                Player.Balance += deposit*2;
                break;
            case "Tie":
                Player.Balance += deposit;
                break;
            case "Bet":
                Player.Balance -= Player.Bet;
                break;
            case "Deposit":
                Player.Balance += deposit;
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
    public bool HasBlackjack(Hand hand)
    {
        return CurrentHandValue(hand) == 21;// && hand.Cards.Count == 2;
    }

    public bool ResetForNextRound()
    {
        CurrentBet = 0;
        Dealer.Cards.Clear();
        Player.Hands.Clear();
        Player.Bet = 0;
        Player.HasSplit = false;
        
        // shuffle new deck if 50% of cards have been used
        if(CurrentDeckSize() <= InitDeckSize/2)
        {
            _currentDeck = new Deck();
            return true; 
        }
        return false;
    }

    public int CurrentDeckSize()
    {
        return _currentDeck.CurrentDeckSize();
    }
}