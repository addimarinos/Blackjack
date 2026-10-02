using BlackJackForOne.Models;

namespace BlackJackForOne.Game;
public static class BlackJackRules
{

    private static string WhoWon(Hand dealer, Hand player)
    {
        if (dealer.CurrentValue() == player.CurrentValue()) return "tie";
        return dealer.CurrentValue() > player.CurrentValue() ? "dealer" : "player";
    }

    
}