using System.ComponentModel;
using BlackJackForOne.Game;

namespace BlackJackForOne.UI;

public class ConsoleUI
{
    private BlackjackGame _game;
    public bool isPlaying = true;
    public bool nextHand = false;

    public void Run()
    {
        // Make the console
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Blackjack For One ♣ ♠ ♥ ♦";
        //Console.SetWindowSize(60, 140);

        // initialize game
        _game = new BlackjackGame();

        Console.WriteLine("Welcome to BlackJack for One!");

        GameSettings();
        Console.WriteLine("Your balance is: {0}", _game.playerBalance);

        Console.WriteLine("When you are ready press <Enter> to start the game \nPress <Esc> to exit the game");
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
            while (Console.ReadKey().Key != ConsoleKey.Enter) {}
            Console.Clear();
            /* 1. place bet
             * 2. deal hands
             * 3. show hands and current value
             * 4. hit or stay
             * 5. if hit: loop and show hand each time and current value
             *      if bust, end round
             * 6. if stay: exit loop and continue
             * 7. show dealer second card.
             * 8. if lower than player: dealer hits until win or bust/value X(should he stop at 17?)
             * 9. collect winnings or remove bet
             * 10. Play again or exit...
             */

            Console.WriteLine("Lets begin!");
            Console.WriteLine();


            while (isPlaying)
            {
                Console.WriteLine("Your balance is: {0}", _game.playerBalance);
                Console.Write("minimum bet is {0}$ \nPlace your bet: ", _game.minimumBet);
                bool validBet = false;
                while (!validBet)
                {
                    var input = ReadNumberInput();
                    if (_game.PlaceYourBet(input))
                    {
                        validBet = true;
                        Console.WriteLine("Your bet is {0}$", input);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("invalid bet, minimum bet is {0}$", _game.minimumBet);
                    }

                }

                // Bet is in, lets deal cards and play
                _game.DealHands();
                Console.WriteLine("Your hand is: {0} \nYour current value is: {1}", _game.player?.ShowCards(), _game.player?.CurrentValue());
                Console.WriteLine("Dealers face up card is: {0}", _game.dealer?.ShowCards());
                Console.WriteLine();

                // Hit or Stay
                bool hitting = true;
                do
                {
                    var input = HitOrStayInput();
                    if (input is 1)
                    {
                        Console.WriteLine("You chose to Hit");
                        if (_game.Hit(false))
                        {
                            Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", _game.player?.ShowCards(), _game.player?.CurrentValue());
                            hitting = false;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", _game.player?.ShowCards(), _game.player?.CurrentValue());
                            Console.WriteLine("Dealers face up card is: {0}", _game.dealer?.ShowCards());
                        }
                    }
                    else
                    {
                        Console.WriteLine("You chose to Stay");
                        hitting = false;
                    }
                } while (hitting);

                if (!_game.player.HasBusted())
                {
                    var playerFinalHand = _game.player.ScoringResult();
                    Console.WriteLine("Your hand value is: {0}", playerFinalHand.Item1);
                    Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.dealer?.ShowCards(true), _game.dealer?.CurrentValue());
                    Console.WriteLine();
                    var dealerScore = _game.dealer.CurrentValue();
                    while (dealerScore < playerFinalHand.Item1)
                    {
                        _game.Hit(true);
                        dealerScore = _game.dealer.CurrentValue();
                    }

                    Console.WriteLine("Final score is:");
                    Console.WriteLine(" Dealers hand is: {0} \nDealers value is: {1}", _game.dealer?.ShowCards(true), _game.dealer?.CurrentValue());
                    Console.WriteLine(" Your hand is: {0} \nYour hand value is: {1}", _game.player?.ShowCards(), _game.player?.CurrentValue());
                    
                }
                /* 
                while (hitting)
                {
                    var input = HitOrStayInput();
                    if (input is 1)
                    {
                        Console.WriteLine("You chose to Hit");
                        _game.Hit(false);
                        Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", _game.player?.ShowCards(), _game.player?.CurrentValue());
                    }
                    else
                    {
                        Console.WriteLine("You chose to Stay");
                        _game.Stay();
                        hitting = false;
                    }

                } */

                Console.WriteLine();
                Console.Write("Would you like to play again? ");

            }
            

        }
    }

    public void GameSettings()
    {
        int b;
        Console.Write("Please deposit $$$ to your account so that you can play\nMinimum bet is 5$: ");
        do
        {
            if (int.TryParse(Console.ReadLine(), out b)) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        _game.UpdateBalance(b);

        // TODO: select number of decks to play with then display the win ratio

    }

    private int ReadNumberInput()
    {
        int b;
        do
        {
            if (int.TryParse(Console.ReadLine(), out b)) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        return b;

    }

    private int HitOrStayInput()
    {
        int i;
        Console.WriteLine("Would you like to Hit or Stay");
        Console.Write("Press 1 for Hit | Press 2 for Stay:");
        do
        {
            if (int.TryParse(Console.ReadLine(), out i) && i is 1 or 2) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        return i;
    }
}