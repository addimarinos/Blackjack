using BlackJackForOne.Models;

namespace BlackJackForOne.Game;

public class BlackjackGame
{
    private Deck? _currentDeck;
    public Hand? dealer;
    public Hand? player;
    public int currentBet;
    public int playerBalance;
    public int minimumBet = 5;

    public BlackjackGame()
    {
        _currentDeck = new Deck();
        dealer = new Hand(true);
        player = new Hand();
    }

    public void PlayRound()
    {
        
    }

    public bool PlaceYourBet(int bet)
    {
        if (bet < playerBalance && bet >= minimumBet)
        {
            currentBet = bet;
            playerBalance -= bet;
            return true;
        }

        return false;

    }
    public void DealHands()
    {
        dealer = new Hand(true);
        player = new Hand();

        player.AddCard(_currentDeck?.DealCard());
        dealer.AddCard(_currentDeck?.DealCard());
        player.AddCard(_currentDeck?.DealCard());
        dealer.AddCard(_currentDeck?.DealCard());
    }

    public bool Hit(bool isdealer)
    {
        if (isdealer)
        {
            dealer?.AddCard(_currentDeck?.DealCard());
            return dealer.HasBusted();
        }
        
        player?.AddCard(_currentDeck?.DealCard());
        return player.HasBusted();
        
    }

    public void Stay()
    {
        
    }

    public void UpdateBalance(int b)
    {
        playerBalance = b;
    }
}