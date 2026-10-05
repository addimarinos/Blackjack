using BlackJackForOne.Models;

namespace BlackJackForOne.Game;

public class BlackjackGame
{
    private Deck _currentDeck;
    public int InitDeckSize;
    public Hand Dealer;
    public Hand Player;
    public int currentBet;
    public int playerBalance;
    public int minimumBet = 5;

    public BlackjackGame()
    {
        _currentDeck = new Deck();
        InitDeckSize = _currentDeck.Cards.Count;
        Dealer = new Hand(true);
        Player = new Hand();
    }

    public bool PlaceYourBet(int bet)
    {
        if (bet <= playerBalance && bet >= minimumBet)
        {
            currentBet = bet;
            UpdateBalance("Bet");
            return true;
        }

        return false;

    }
    public void DealHands()
    {
        Dealer = new Hand(true);
        Player = new Hand();
        //Card ace = new Card(Rank.Ace);
        //Card king = new Card(Rank.King);

        Player.AddCard(_currentDeck.DealCard());
        Dealer.AddCard(_currentDeck.DealCard());
        Player.AddCard(_currentDeck.DealCard());
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

    public void UpdateBalance(string cond, int deposit = 0)
    {
        switch (cond)
        {
            case "Win":
                playerBalance += currentBet * 2;
                break;
            case "Tie":
                playerBalance += currentBet;
                break;
            case "Bet":
                playerBalance -= currentBet;
                break;
            case "Deposit":
                playerBalance += deposit;
                break;
        }
    }

    public void ResetForNextRound()
    {
        currentBet = 0;

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