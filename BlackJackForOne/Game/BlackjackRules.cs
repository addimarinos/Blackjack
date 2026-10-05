using BlackJackForOne.Models;

namespace BlackJackForOne.Game;
public class BlackJackRules
{

    public int MinBet = 5;
    public string WhoWon(Hand dealer, Hand player)
    {
        if (IsBusted(player)) return "dealer";
        if (IsBusted(dealer)) return "player";
        if (dealer.CurrentValue() == player.CurrentValue()) return "tie";
        return dealer.CurrentValue() > player.CurrentValue() ? "dealer" : "player";
    }

    public bool IsBusted(Hand hand)
    {
        return hand.CurrentValue() > 21;
    }

    public bool checkBalance(int balance)
    {
        return balance > MinBet;
    }
}