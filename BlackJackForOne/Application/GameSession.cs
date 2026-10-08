
using BlackJackForOne.Game;

namespace BlackJackForOne.Application
{
    public enum GamePhases
    {
        RoundStart,
        AwaitingBet,
        PlayerTurn,
        DealerTurn,
        RoundEnd
    }
    public class GameSession
    {
        public BlackjackGame Game { get; } = new();

        public GamePhases Phase { get; private set; } = GamePhases.RoundStart;

        public bool BeginRound()
        {
            if (Phase is not (GamePhases.RoundStart or GamePhases.RoundEnd))
            {
                throw new InvalidOperationException(
                    $"Cannot begin round in the current phase: {Phase}."
                );
            }

            var deckShuffled = Game.ResetForNextRound();
            Phase = GamePhases.AwaitingBet;
            return deckShuffled;
        }

        public bool TryStartRound(int bet)
        {
            if (Phase != GamePhases.AwaitingBet)
            {
                throw new InvalidOperationException(
                    $"Cannot start round in the current phase: {Phase}."
                );
            }

            if (!Game.PlaceYourBet(bet)) return false;
            Game.DealHands();
            Phase = GamePhases.PlayerTurn;
            return true;
        }

        public (string, bool) PlayerChoice(int option)
        {
            if (Phase != GamePhases.PlayerTurn)
            {
                throw new InvalidOperationException(
                    $"Cannot make choice in the current phase: {Phase}"
                );
            }
            var isDone = false;
            var player = Game.Player;
            var reply = "Continue";

            switch (option)
            {
                case 1: // Hit
                    Game.NewHit(player);
                    reply = "Hit";
                    if (Game.IsBusted(player) || Game.HasBlackjack(player)) isDone = true;
                    break;
                case 2: // Stay
                    isDone = true;
                    reply = "Stay";
                    break;
                case 3: // Double Down
                    Game.DoubleDown(player);
                    isDone = true;
                    reply = "DoubleDown";
                    break;
                case 4: // Split
                    reply = "Split";
                    break;
            }
            if (isDone)
            {
                Phase = GamePhases.DealerTurn;
            }
            return (reply, isDone);
        }

        public void EndRound()
        {
            if (Phase != GamePhases.DealerTurn)
            {
                throw new InvalidOperationException(
                    $"Cannot End round in the current phase: {Phase}."
                );
            }
            Phase = GamePhases.RoundEnd;
        }
    }
}
