using BlackJackForOne.Game;


namespace BlackJackForOne.UI;

public class ConsoleUi
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

        // Clear console and start the game
        Console.Clear();
        Console.WriteLine("Welcome to BlackJack for One!");

        DepositFunds();
        Console.WriteLine();
        Console.WriteLine("Press <Esc> to exit the game - Press Any key to continue");
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {

            /* Console.Write("Loading...");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                Thread.Sleep(500);
            } */
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

                Console.WriteLine("Your balance is: {0}", _game.PlayerBalance);

                bool validBet = false;
                while (!validBet)
                {
                    Console.Write("minimum bet is {0} $ \nPlace your bet: ", _game.MinimumBet);
                    var input = ReadNumberInput();
                    if (_game.PlaceYourBet(input))
                    {
                        validBet = true;
                        Console.WriteLine("Your bet is {0} $", input);
                        Console.WriteLine();
                    }
                    else
                    {
                        if (input < _game.MinimumBet)
                            Console.WriteLine("invalid bet, minimum bet is {0} $", _game.MinimumBet);
                        else
                            Console.WriteLine("Not enough funds for that bet, your balance is: {0} $",
                                _game.PlayerBalance);
                        Console.Write("Would you like to deposit more funds? press (1) yes, (2) no: ");
                        if (ReadNumberInput() == 1) DepositFunds();
                    }

                }

                // Bet is in, lets deal cards and play
                _game.DealHands();
                Console.WriteLine("Your hand is: {0} \nYour current value is: {1}", _game.Player.ShowCards(),
                    _game.Player.CurrentValue());
                Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                Console.WriteLine();

                // Hit or Stay, skip if blackjack
                bool hitting = _game.Player.CurrentValue() < 21;
                if (!hitting) Console.WriteLine("2 card Blackjack!");
                while (hitting)
                {
                    if (_game.Player.CurrentValue() == 21)
                    {
                        Console.WriteLine("Blackjack!");
                        break;
                    }

                    /*      New logic
                        For each player in table
                            if hasSplit
                                for each hand in player
                                    options per hand
                            options per player
                    */
                    if (_game.Player.HasSplit)
                    {
                        Console.WriteLine("You split your hand, here are your new hands:");
                        Console.WriteLine("First hand: {0}", _game.Player.MultHands.First().ShowCards());
                        Console.WriteLine("Second hand: {0}", _game.Player.MultHands.Last().ShowCards());
                        Console.WriteLine("Start playing the first hand then move onto the next one");
                        Console.WriteLine();
                        foreach (var hand in _game.Player.MultHands)
                        {
                            bool playing = hand.CurrentValue() < 21;
                            if (!playing) Console.WriteLine("Blackjack!");
                            while (playing)
                            {
                                Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                                Console.WriteLine("Your hand is: {0}", hand.ShowCards());
                                Console.WriteLine("Hand value is: {0}", hand.CurrentValue());
                                var options = PlayerOptions("Press 1 for Hit | Press 2 for Double Down | Press 3 for Stay: ", 3);
                                switch (options)
                                {
                                    case 1:
                                        Console.WriteLine("Hit");
                                        _game.NewHit(hand);
                                        if (_game.IsBusted(hand))
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", hand.ShowCards(), hand.CurrentValue());
                                            playing = false;
                                            hitting = false;
                                            Thread.Sleep(3000);
                                        }
                                        else
                                        {
                                            Console.WriteLine();
                                            Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", hand.ShowCards(), hand.CurrentValue());
                                            Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                                            Console.WriteLine();
                                        }
                                        break;
                                    case 2:
                                        Console.WriteLine("Double Down");
                                        _game.DoubleDown(hand);
                                        Console.WriteLine("You doubled down \nNew bet is: {0}, Hand is: {1}, Hand value is: {2}", hand.Bet, hand.ShowCards(), hand.CurrentValue());
                                        playing = false;
                                        hitting = false;
                                        break;
                                    case 3:
                                        Console.WriteLine("Stay");
                                        playing = false;
                                        hitting = false;
                                        break;
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        var options = PlayerOptions("Press 1 for Hit | Press 2 for Split | Press 3 for Double Down | Press 4 for Stay: ", 4);
                        switch (options)
                        {
                            case 1:
                                Console.WriteLine("Hit");
                                _game.NewHit(_game.Player);
                                if (_game.IsBusted(_game.Player))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                                    hitting = false;
                                    Thread.Sleep(3000);
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                                    Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                                    Console.WriteLine();
                                }
                                break;
                            case 2:
                                if (!_game.Player.CanSplit())
                                {
                                    Console.WriteLine("Cards are different, you can't split this hand");
                                    Console.WriteLine();
                                }
                                else
                                {
                                    _game.Split();
                                    Console.WriteLine("Split");
                                }
                                break;
                            case 3:
                                Console.WriteLine("Double Down");
                                _game.DoubleDown(_game.Player);
                                Console.WriteLine("You doubled down \nNew bet is: {0}, Hand is: {1}, Hand value is: {2}", _game.Player.Bet, _game.Player.ShowCards(), _game.Player.CurrentValue());
                                hitting = false;
                                break;
                            case 4:
                                Console.WriteLine("Stay");
                                hitting = false;
                                break;
                        }
                    }
                    
                    /* var input = HitOrStayInput();
                    if (input is 1)
                    {
                        Console.WriteLine("You chose to Hit");
                        if (_game.Hit(false))
                        {
                            Console.WriteLine();
                            Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                            hitting = false;
                            Thread.Sleep(3000);
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
                    } */
                } 

                Console.WriteLine();
                
                if (!rules.IsBusted(_game.Player))
                {
                    var playerBestScore = _game.Player.CurrentValue();
                    if (_game.Player.HasSplit)
                    {
                        var hands = _game.Player.MultHands;
                        var hand1 = hands.First();
                        var hand2 = hands.Last();
                        Console.WriteLine("First hand is: {0}", hand1.ShowCards());
                        Console.WriteLine("Second hand is: {0}", hand2.ShowCards());

                        switch ((hand1.HasBusted(), hand2.HasBusted()))
                        {
                            case (true, true):
                                Console.WriteLine("You shouldnt be here!!!!");
                                break;
                            case (true, false):
                                playerBestScore = hand1.CurrentValue();
                                break;
                            case (false, true):
                                playerBestScore = hand2.CurrentValue();
                                break;
                            case (false, false):
                                playerBestScore = hand1.CurrentValue() > hand2.CurrentValue()
                                    ? hand1.CurrentValue()
                                    : hand2.CurrentValue();
                                break;
                        }
                        Console.WriteLine("best score is: {0}", playerBestScore);

                    }
                    else Console.WriteLine("Your hand value is: {0}", _game.Player.ScoringResult().Item1);

                    Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.Dealer.ShowCards(true), _game.Dealer.CurrentValue());
                    Console.WriteLine();
                    var dealerScore = _game.Dealer.CurrentValue();
                    while (dealerScore <= playerBestScore && dealerScore <= 17)
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


                Console.WriteLine("Dealers final hand value is: {0} ", _game.Dealer.CurrentValue());
                if (_game.Player.HasSplit)
                {
                    Console.WriteLine("Result for both hands, because you split");
                    foreach (var hand in _game.Player.MultHands)
                    {
                        var result = hand.ScoringResult();
                        Console.WriteLine();
                        Console.WriteLine("Result for hand: {0}, of value: {1}", hand.CurrentValue(), result.Item1);
                        switch (rules.WhoWon(_game.Dealer, hand))
                        {
                            case "player":
                                
                                Console.WriteLine("You Won!");
                                Console.WriteLine("Winnings are: {0} $", hand.Bet);
                                Console.WriteLine("Previous balance {0} $", _game.PlayerBalance);
                                _game.UpdateBalance("Win", hand.Bet);
                                break;
                            case "dealer":
                                Console.WriteLine("House Wins");
                                break;
                            case "tie":
                                Console.WriteLine("Result is a Tie");
                                Console.WriteLine("You get your bet back of value: {0} $", hand.Bet);
                                Console.WriteLine("Previous balance {0} $", _game.PlayerBalance);
                                _game.UpdateBalance("Tie", hand.Bet);
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Your final hand value: {0}", _game.Player.CurrentValue());
                    Console.WriteLine();
                    switch (rules.WhoWon(_game.Dealer, _game.Player))
                    {
                        case "player":
                            Console.WriteLine("You Won!");
                            Console.WriteLine("Winnings are: {0} $", _game.Player.Bet*2);
                            Console.WriteLine("Previous balance {0} $", _game.PlayerBalance);
                            _game.UpdateBalance("Win", _game.Player.Bet);
                            break;
                        case "dealer":
                            Console.WriteLine("House Wins");
                            break;
                        case "tie":
                            Console.WriteLine("Result is a Tie");
                            Console.WriteLine("You get your bet back of value: {0} $", _game.Player.Bet);
                            Console.WriteLine("Previous balance {0} $", _game.PlayerBalance);
                            _game.UpdateBalance("Tie", _game.Player.Bet);
                            break;
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("New balance is: {0}", _game.PlayerBalance);
                Console.WriteLine();
                Console.Write("Would you like to play again? press (1) yes, (2) no: ");

                if (ReadNumberInput() == 2) break;
                if (_game.PlayerBalance < _game.MinimumBet) DepositFunds();
            }

            IsPlaying = false;
            Console.WriteLine("You have decided to stop playing");
            Console.WriteLine("Your balance is: {0} $", _game.PlayerBalance);
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
            if (int.TryParse(Console.ReadLine(), out b) && b is > 0 and <= 1000000) break;
            Console.WriteLine("Minimum balance needed is 5 $, maximum deposit is 1.000.000 $");
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        _game?.UpdateBalance("Deposit", b);
        Console.WriteLine("Your balance is: {0} $", _game?.PlayerBalance);
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
        Console.Write("Press 1 for Hit | Press 2 for Stay: ");
        do
        {
            if (int.TryParse(Console.ReadLine(), out i) && i is 1 or 2) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        return i;
    }

    private int PlayerOptions(string output, int max)
    {
        int i;
        Console.WriteLine("What would you like to do?");
        Console.Write(output);
        do
        {
            if (int.TryParse(Console.ReadLine(), out i) && Enumerable.Range(1,max).Contains(i)) break;
            Console.WriteLine("Please enter a valid number: ");
        } while (true);

        return i;
    }
}