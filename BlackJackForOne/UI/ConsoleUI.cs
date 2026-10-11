using BlackJackForOne.Application;
using BlackJackForOne.Game;
using BlackJackForOne.Models;


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

        // initialize game/session
        //_game = new BlackjackGame();
        BlackJackRules rules = new BlackJackRules();
        
        var session = new GameSession();
        _game = session.Game;

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
                //if (_game.ResetForNextRound()) Console.WriteLine("Deck has been reset and shuffled");
                if (session.BeginRound()) Console.WriteLine("Deck has been reset and shuffled");

                Console.WriteLine("Your balance is: {0} $", _game.Player.Balance);

                
                while(session.Phase is GamePhases.AwaitingBet)
                {
                    var bet = PlayerOptions("Minimum bet is 5 $ \nPlace your bet: ", _game.Player.Balance, true);
                    
                    if (session.TryStartRound(bet))
                    {
                        Console.WriteLine($"\nYour bet is: {bet}");
                    }
                    else if (bet < rules.MinBet)
                    {
                        Console.WriteLine("Bet invalid");
                    }
                    else
                    {
                        Console.WriteLine($"Not enough funds for that bet, your balance is {_game.Player.Balance}");
                    }
                }
                
                while (session.Phase is GamePhases.DealAllHands)
                {
                    
                    switch (session.TryDealAllHands())
                    {
                        case Replies.InstantBlackjack:
                            Console.WriteLine("Blackjack!!!  Let's see what the dealer has...");
                            break;
                        case Replies.Empty:
                            Console.WriteLine("Hands have been dealt.");
                            break;
                    }
                }
                
                while (session.Phase is GamePhases.PlayerTurn)
                {
                    Console.WriteLine();                
                    Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                    
                    // check up on thiiiiiis
                    var currentHand = session.GetActiveHand();
                    var currentCards = String.Join(" ", currentHand.Cards);
                    Console.WriteLine($"Current hand is: {currentCards} | Value: {currentHand.Value}");

                    Console.WriteLine();
                    var options = _game.Player.HasSplit ?
                        PlayerOptions("Press 1 for Hit | Press 2 for Stay | Press 3 for Double Down: ", 3, true):
                        PlayerOptions("Press 1 for Hit | Press 2 for Stay | Press 3 for Double Down | Press 4 for Split: ", 4, true);
                    
                    //PlayerOptions("Press 1 for Hit | Press 2 for Stay | Press 3 for Double Down | Press 4 for Split: ", 4, true);var options = PlayerOptions("Press 1 for Hit | Press 2 for Stay | Press 3 for Double Down | Press 4 for Split: ", 4, true);
                    var result = session.PlayerChoice(options);
                    var states = result.HandStates;
                    switch (result.Reply)
                    {
                        case Replies.Hit:
                            Console.WriteLine("Hit");
                            Console.WriteLine($"New hand: {String.Join(" ", states.First().Cards)} | Value: {states.First().Value}");
                            if (states.First().Busted) Console.WriteLine("You busted! Lets move on.");
                            if (states.First().Value == 21) Console.WriteLine("21! Lets move on.");
                            Console.WriteLine();
                            break;
                        case Replies.Stay:
                            Console.WriteLine("You chose to Stay");
                            break;
                        case Replies.DoubleDown:
                            Console.WriteLine("Double Down");
                            Console.WriteLine($"New bet is: {_game.Player.Bet}");
                            Console.WriteLine($"New Hand is: {String.Join(" ", states.First().Cards)} | Value: {states.First().Value}");
                            break;
                        case Replies.Split:
                            Console.WriteLine("You split your hand, here are your new hands:");
                            var hand1 = states.First();
                            var hand2 = states.Last();
                            var print1 = String.Join(" ", hand1.Cards);
                            var print2 = String.Join(" ", hand2.Cards);
                            Console.WriteLine($"First hand: {print1} | Value: {hand1.Value}");
                            Console.WriteLine($"Second hand: {print2} | Value: {hand2.Value}");
                            Console.WriteLine("Start playing the first hand then move onto the next one");
                            break;
                        case Replies.InvalidSplit:
                            Console.WriteLine("Can't split this hand");
                            break;
                        case Replies.InvalidDoubleDown:
                            Console.WriteLine("Can't double down on this hand");
                            break;
                    }
                    if (result.BlackjackInfo != Replies.Empty)
                    {
                        switch (result.BlackjackInfo)
                        {
                            case Replies.SplitFirstHandBlackjack:
                                Console.WriteLine("First hand is Blackjack! Let's move onto the next one.");
                                break;
                            case Replies.SplitSecondHandBlackjack:
                                Console.WriteLine("Second hand is Blackjack! Let's see what the Dealer has.");
                            // send second hand state
                                break;
                            case Replies.SplitBothBlackjack:
                                Console.WriteLine("Whoa! double Blackjack! Let's see what the Dealer has.");
                                break; 
                        }
                    }

                }
                Console.WriteLine("Choice is done");

                // Dealer phase
                while(session.Phase is GamePhases.DealerTurn)
                {
                    
                }








                break;
                // Hit or Stay, skip if blackjack
                bool hitting = _game.Player.HandValue < 21;
                if (!hitting) Console.WriteLine("2 card Blackjack!");
                while (hitting)
                {
                    if (_game.Player.HandValue == 21)
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
                        Console.WriteLine();
                        Console.WriteLine("You split your hand, here are your new hands:");
                        Console.WriteLine("First hand: {0}", _game.Player.Hands.First().ShowCards());
                        Console.WriteLine("Second hand: {0}", _game.Player.Hands.Last().ShowCards());
                        Console.WriteLine("Start playing the first hand then move onto the next one");
                        Console.WriteLine();
                        foreach (var hand in _game.Player.Hands)
                        {
                            bool playing = hand.CurrentValue() < 21;
                            if (!playing) Console.WriteLine("Blackjack!");
                            while (playing)
                            {
                                Console.WriteLine("Dealers face up card is: {0}", _game.Dealer.ShowCards());
                                Console.WriteLine("Your hand is: {0}", hand.ShowCards());
                                Console.WriteLine("Hand value is: {0}", hand.CurrentValue());
                                Console.WriteLine();
                                var options = PlayerOptions("Press 1 for Hit | Press 2 for Double Down | Press 3 for Stay: ", 3);
                                Console.WriteLine();
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
                                    /* case 2:
                                        if (!_game.CanDoubleDown())
                                        {
                                            Console.WriteLine("Not enough funds to Double Down");
                                            Console.WriteLine();
                                            break;
                                        }
                                        Console.WriteLine("Double Down");
                                        _game.DoubleDown(hand);
                                        Console.WriteLine("You doubled down \nNew bet is: {0} \nHand is: {1} \nHand value is: {2}", hand.Bet, hand.ShowCards(), hand.CurrentValue());
                                        playing = false;
                                        hitting = false;
                                        break; */
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
                        Console.WriteLine();
                        switch (options)
                        {
                            case 1:
                                Console.WriteLine("Hit");
                                _game.NewHit(_game.Player.Hands.First());
                                if (_game.IsBusted(_game.Player.Hands.First()))
                                {
                                    Console.WriteLine();
                                    //Console.WriteLine("You Busted with: {0} \nYour final value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
                                    hitting = false;
                                    Thread.Sleep(3000);
                                }
                                else
                                {
                                    //Console.WriteLine("Your new hand is: {0} \nYour current value is: {1}", _game.Player.ShowCards(), _game.Player.CurrentValue());
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
                                    _game.Split(_game.Player);
                                    Console.WriteLine("Split");
                                }
                                break;
                            /* case 3:
                                if (!_game.CanDoubleDown())
                                {
                                    Console.WriteLine("Not enough funds to Double Down");
                                    Console.WriteLine();
                                    break;
                                }
                                Console.WriteLine("Double Down");
                                //_game.DoubleDown(_game.Player);
                                //Console.WriteLine("You doubled down \nNew bet is: {0} \nHand is: {1} \nHand value is: {2}", _game.Player.Bet, _game.Player.ShowCards(), _game.Player.CurrentValue());
                                hitting = false;
                                break; */
                            case 4:
                                Console.WriteLine("Stay");
                                hitting = false;
                                break;
                        }
                    }
                } 

                Console.WriteLine();
                
                if (!rules.IsBusted(_game.Player.Hands.First()))
                {
                    var playerBestScore = 20;// _game.Player.CurrentValue();
                    if (_game.Player.HasSplit && !_game.Player.Hands.First().HasBusted() && !_game.Player.Hands.Last().HasBusted())
                    {
                        var hands = _game.Player.Hands;
                        var hand1 = hands.First();
                        var hand2 = hands.Last();
                        Console.WriteLine("First hand is: {0}, with value {1}", hand1.ShowCards(), hand1.CurrentValue());
                        Console.WriteLine("Second hand is: {0}, with value {1}", hand2.ShowCards(), hand2.CurrentValue());
                        Console.WriteLine();
                        switch ((hand1.HasBusted(), hand2.HasBusted()))
                        {
                            case (true, true):
                                Console.WriteLine("You shouldnt be here!!!!");
                                break;
                            case (true, false):
                                playerBestScore = hand2.CurrentValue();
                                break;
                            case (false, true):
                                playerBestScore = hand1.CurrentValue();
                                break;
                            case (false, false):
                                playerBestScore = hand1.CurrentValue() > hand2.CurrentValue()
                                    ? hand1.CurrentValue()
                                    : hand2.CurrentValue();
                                break;
                        }
                        Console.WriteLine("best score is: {0}", playerBestScore);

                    }
                    //else Console.WriteLine("Your hand value is: {0}", _game.Player.ScoringResult().Item1);

                    Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.Dealer.ShowCards(true), _game.Dealer.CurrentValue());
                    Console.WriteLine();
                    var dealerScore = _game.Dealer.CurrentValue();
                    while (dealerScore <= playerBestScore && dealerScore <= 17)
                    {
                        Thread.Sleep(5000);
                        Console.WriteLine("Dealer chose to Hit");
                        // deal a card, checks if busted
                        if (_game.CurrentHandValue(_game.Dealer) > 21)
                        {
                            Console.WriteLine("Dealer Busted with: {0}", _game.Dealer.ShowCards(true));
                            break;
                        }
                        dealerScore = _game.Dealer.CurrentValue();
                        Console.WriteLine("Dealers hand is: {0} \nDealers value is: {1}", _game.Dealer.ShowCards(true), _game.Dealer.CurrentValue());
                        Console.WriteLine();
                    } 
                    Thread.Sleep(5000);
                }

                Console.WriteLine("Dealers final hand value is: {0} ", _game.Dealer.CurrentValue());
                if (_game.Player.HasSplit)
                {
                    Console.WriteLine("Result for both hands, because you split");
                    foreach (var hand in _game.Player.Hands)
                    {
                        var result = hand.ScoringResult();
                        Console.WriteLine();
                        Console.WriteLine("Result for hand: {0}, of value: {1}", hand.ShowCards(), result.Item1);
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
                    Console.WriteLine("Your final hand value: {0}", _game.CurrentHandValue(_game.Player.Hands.First()));
                    Console.WriteLine();
                    switch (rules.WhoWon(_game.Dealer, _game.Player.Hands.First()))
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
                session.EndRound();
                if (PlayerOptions("Would you like to play again? press (1) yes, (2) no: ", 2, true) == 2) break;
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
        Console.WriteLine("Your balance is: {0} $", _game?.Player.Balance);
    }

    private int PlayerOptions(string output, int max, bool numInput = false)
    {
        int i;
        if (!numInput) Console.WriteLine("What would you like to do?");
        Console.Write(output);
        do
        {
            if (int.TryParse(Console.ReadLine(), out i) && Enumerable.Range(1, max).Contains(i)) break;
            if (output.Contains("bet"))
            {
                Console.WriteLine("Not enough funds on your account");
                if (PlayerOptions("Would you like to deposit more funds? press (1) yes, (2) no: ", 2, true) == 1)
                {
                    DepositFunds();
                    Console.Write("Place your bet: ");
                }
            }
            else Console.Write("Please enter a valid number: ");
        } while (true);

        return i;
    }
    
    
}
