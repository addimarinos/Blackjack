using BlackJackForOne.Models;

namespace BlackJackForOne.Game;
public class BlackJackRules
{

    public int minBet = 5;
    private  string WhoWon(Hand dealer, Hand player)
    {
        if (dealer.CurrentValue() == player.CurrentValue()) return "tie";
        return dealer.CurrentValue() > player.CurrentValue() ? "dealer" : "player";
    }

    public bool IsBusted(Hand hand)
    {
        return hand.CurrentValue() > 21;
    }

    public bool checkBalance(int balance)
    {
        return balance > minBet;
    }
}