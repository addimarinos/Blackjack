using BlackJackForOne.Game;

namespace BlackJackForOne.UI;

public class ConsoleUI
{
    private BlackjackGame? _game;
    public bool IsPlaying = true;

    public void Run()
    {
        // Make the console
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Blackjack For One ♣ ♠ ♥ ♦";
        //Console.SetWindowSize(60, 140);

        // initialize game
        _game = new BlackjackGame();
        BlackJackRules rules = new BlackJackRules();

        Console.WriteLine("Welcome to BlackJack for One!");

        DepositFunds();
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


            while (IsPlaying)
            {
                Console.Clear();
                _game.ResetForNextRound();

                Console.WriteLine("Your balance is: {0}", _game.playerBalance);
                Console.Write("minimum bet is {0} $ \nPlace your bet: ", _game.minimumBet);
                bool validBet = false;
                while (!validBet)
                {
                    var input = ReadNumberInput();
                    if (_game.PlaceYourBet(input))
                    {
                        validBet = true;
                        Console.WriteLine("Your bet is {0} $", input);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("invalid bet, minimum bet is {0} $", _game.minimumBet);
                    }

                }

                // Bet is in, lets deal cards and play
                _game.DealHands();
                Console.WriteLine("Your hand is: {0} \nYour current value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                Console.WriteLine();

                // Hit or Stay, skip if blackjack
                bool hitting = _game.Player.CurrentValue() < 21;
                if (!hitting) Console.WriteLine("You got a Blackjack!");
                do
                {
                    var input = HitOrStayInput();
                    if (input is 1)
                    {
                        Console.WriteLine("You chose to Hit");
                        if (_game.Hit(false))
                        {
                            Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                            hitting = false;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                            Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("You chose to Stay");
                        hitting = false;
                    }
                } while (hitting);

                Console.WriteLine();
                
                if (!rules.IsBusted(_game.Player))
                {
                    var playerFinalHand = _game.Player.ScoringResult();
                    Console.WriteLine("Your hand value is: {0}", playerFinalHand.Item1);
                    Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.Dealer.ShowCards(true), _game.Dealer.CurrentValue());
                    Console.WriteLine();
                    var dealerScore = _game.Dealer.CurrentValue();
                    while (dealerScore <= playerFinalHand.Item1 && dealerScore <= 17)
                    {
                        Thread.Sleep(5000);
                        Console.WriteLine("Dealer chose to Hit");
                        // deal a card, checks if busted
                        if (_game.Hit(true))
                        {
                            Console.WriteLine("Dealer Busted");
                            break;
                        }
                        dealerScore = _game.Dealer.CurrentValue();
                        Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.Dealer.ShowCards(true), _game.Dealer.CurrentValue());
                        Console.WriteLine();
                    } 
                    Thread.Sleep(5000);
                }
                Console.WriteLine();
                Console.WriteLine("Dealers final hand value is: {0} \nYour final hand value: {1}", _game.Dealer.CurrentValue(), _game.Player.CurrentValue());
                switch (rules.WhoWon(_game.Dealer, _game.Player))
                {
                    case "player":
                        
                        Console.WriteLine("You Won!");
                        Console.WriteLine("Winnings are: {0} $", _game.currentBet*2);
                        Console.WriteLine("Previous balance {0} $", _game.playerBalance);
                        _game.UpdateBalance("Win");
                        break;
                    case "dealer":
                        Console.WriteLine("House Wins");
                        break;
                    case "tie":
                        Console.WriteLine("Result is a Tie");
                        Console.WriteLine("You get your bet back of value: {0}", _game.currentBet);
                        Console.WriteLine("Previous balance {0} $", _game.playerBalance);
                        _game.UpdateBalance("Tie");
                        break;
                }
                Console.WriteLine();
                Console.WriteLine("New balance is: {0}", _game.playerBalance);
                Console.WriteLine();
                Console.Write("Would you like to play again? press (1) yes, (2) no: ");

                if (ReadNumberInput() == 2) break;
                if (_game.playerBalance == 0) DepositFunds();
            }

            IsPlaying = false;
            Console.WriteLine("You have stopped playing");
            Console.WriteLine("Your balance is:{0}", _game.playerBalance);
            Console.WriteLine("Press <Esc> to exit");
            
        }
    }

    public void DepositFunds()
    {
        int b;
        Console.WriteLine("Please deposit $$$ to your account so that you can play (Minimum bet is 5$)");
        Console.Write("Your input: ");
        do
        {
            if (int.TryParse(Console.ReadLine(), out b) && b is > 0 and < 1000000) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        _game?.UpdateBalance("Deposit", b);

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