
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using BlackJackForOne.Game;

namespace BlackJackForOne.Application
{
    public enum GamePhases
    {
        RoundStart,
        AwaitingBet,
        DealAllHands,
        PlayerTurn,
        DealerTurn,
        Payout,
        RoundEnd
    }

    public enum Replies
    {
        Empty,
        Hit,
        Stay,
        DoubleDown,
        InvalidDoubleDown,
        Split,
        InvalidSplit,
        InstantBlackjack,
        SplitFirstHandBlackjack,
        SplitSecondHandBlackjack,
        SplitBothBlackjack,
        DealerBlackjack,
        DealerBusted,
        DealerWin,
        DealerLose,
        PlayerWin,
        Tie

    }
    public class GameSession
    {
        public BlackjackGame Game { get; } = new();

        public GamePhases Phase { get; private set; } = GamePhases.RoundStart;

        private int _activeHand;
        private int _playerBestScore;
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

            Phase = GamePhases.DealAllHands;
            return true;
        }

        public Replies TryDealAllHands()
        {
            if (Phase != GamePhases.DealAllHands)
            {
                throw new InvalidOperationException(
                    $"Cannot deal hands in the current phase: {Phase}"
                );
            }
            Game.DealHands();
            _activeHand = 0;
            
            if (Game.HasBlackjack(Game.Player.Hands.First()))
            {
                Phase = GamePhases.DealerTurn;
                return Replies.InstantBlackjack;
            }
            Phase = GamePhases.PlayerTurn;
            return Replies.Empty;
        }

        public Result PlayerChoice(int option)
        {
            if (Phase != GamePhases.PlayerTurn)
            {
                throw new InvalidOperationException(
                    $"Cannot make choice in the current phase: {Phase}"
                );
            }
            var isDone = false;
            var player = Game.Player;
            var hand = player.Hands[_activeHand];
            var reply = Replies.Empty;
            
            // Handstate fields
            List<HandState> states = [];
            HandState state;
            List<string> cards = [];
            var value = 0;
            var id = _activeHand;
            var bustedHand = false;
            

            switch (option)
            {
                case 1: // Hit
                    Game.NewHit(hand);
                    reply = Replies.Hit;

                    // create a snapshot of hand after hit
                    cards = hand.Cards  
                        .Select(card => card.Rank.ToString())
                        .ToList();
                    value = hand.CurrentValue();
                    id = _activeHand;
                    bustedHand = Game.IsBusted(hand);
                
                    state = new HandState(
                        Cards: new ReadOnlyCollection<string>(cards),
                        Value: value, 
                        IsActiveID: id,
                        Busted: bustedHand);
                    states.Add(state);
                
                    if (bustedHand || Game.HasBlackjack(hand)) 
                    {
                        if (player.HasSplit && _activeHand == 0) _activeHand = 1;
                        else isDone = true;
                    }
                    break;
                case 2: // Stay
                    if (player.HasSplit && _activeHand == 0) _activeHand = 1;
                    else isDone = true;
                    reply = Replies.Stay;
                    break;
                case 3: // Double Down
                    if (!Game.CanDoubleDown(player, hand))
                    {
                        reply = Replies.InvalidDoubleDown; 
                        break;
                    }
                    Game.DoubleDown(hand);
                    
                    cards = hand.Cards  
                        .Select(card => card.Rank.ToString())
                        .ToList();
                    value = hand.CurrentValue();
                    id = _activeHand;
                    bustedHand = Game.IsBusted(hand);
                    
                    state = new HandState(
                        Cards: new ReadOnlyCollection<string>(cards),
                        Value: value, 
                        IsActiveID: id,
                        Busted: bustedHand);
                    states.Add(state);
                    
                    if (player.HasSplit && _activeHand == 0) _activeHand = 1;
                    else isDone = true;
                    reply = Replies.DoubleDown;
                    break;
                case 4: // Split
                    if (!player.CanSplit()) 
                    {
                        reply = Replies.InvalidSplit;
                        break;
                    }
                    
                    Game.Split(player);
                    var hand1 = Game.Player.Hands.First();
                    var hand2 = Game.Player.Hands.Last();
                    var cards1 = hand1.Cards.Select(card => card.Rank.ToString()).ToList();
                    var cards2 = hand2.Cards.Select(card => card.Rank.ToString()).ToList();
                    var state1 = new HandState(
                        Cards: new ReadOnlyCollection<string>(cards1),
                        Value: hand1.CurrentValue(),
                        IsActiveID: 0,
                        Busted: Game.IsBusted(hand1)
                    );
                    var state2 = new HandState(
                        Cards: new ReadOnlyCollection<string>(cards2),
                        Value: hand2.CurrentValue(),
                        IsActiveID: 1,
                        Busted: Game.IsBusted(hand2)
                    );
                    states.Add(state1);
                    states.Add(state2);
                    reply = Replies.Split;
                    
                    break;
            }

            var blackjackInfo = Replies.Empty;
            if (player.HasSplit)
            {
                switch (CheckBlackJack())
                {
                    case Replies.SplitFirstHandBlackjack:
                        blackjackInfo = Replies.SplitFirstHandBlackjack;
                        _activeHand = 1;
                        break;
                    case Replies.SplitSecondHandBlackjack:
                        // send second hand state
                        if (_activeHand == 1)
                        {
                            blackjackInfo = Replies.SplitSecondHandBlackjack;
                            isDone = true;    
                        }
                        
                        break;
                    case Replies.SplitBothBlackjack:
                        blackjackInfo = Replies.SplitBothBlackjack;
                        // send both handstates
                        isDone = true;
                        break; 
                }
            }
            
            if (isDone)
            {
                Phase = GamePhases.DealerTurn;
                _activeHand = 0;
                var hands = Game.Player.Hands;
                // Check if player busted, if so skip dealer turn
                if (Game.Player.HasSplit)
                {
                    if (Game.IsBusted(hands[0]) && Game.IsBusted(hands[1]))
                    {
                        Phase = GamePhases.Payout;
                    }
                    _playerBestScore = hands[0].CurrentValue() >= hands[1].CurrentValue()
                        ? hands[0].CurrentValue()
                        : hands[1].CurrentValue();
                }
                else if (Game.IsBusted(hands[0]))
                {
                    Phase = GamePhases.Payout;
                }
                else
                {
                    _playerBestScore = hands[0].CurrentValue();
                }
            }
            
            var result = new Result(reply, blackjackInfo, states);
            
            return result;
        }

        public DealerResult TryDealerTurn()
        {
            if (Phase != GamePhases.DealerTurn)
            {
                throw new InvalidOperationException(
                    $"Cannot start Dealers turn in the current phase: {Phase}."
                );
            }
            
            // check if dealer blackjackgit
            var dealer = Game.Dealer;
            var reply = Replies.Empty;

            if (Game.HasBlackjack(dealer))
            {
                reply = Replies.DealerBlackjack;
                Phase = GamePhases.Payout;
            }
            else if (Game.IsBusted(dealer))
            {
                reply = Replies.DealerBusted;
                Phase = GamePhases.Payout;

            }
            else if (dealer.CurrentValue() > _playerBestScore)
            {
                reply = Replies.DealerWin;
                Phase = GamePhases.Payout;

            }
            else if (dealer.CurrentValue() <= 17 && dealer.CurrentValue() < _playerBestScore) Game.NewHit(dealer);
            else
            {
                reply = Replies.DealerLose;
                Phase = GamePhases.Payout;
            }
            var hand = dealer.Cards.Select(card => card.Rank.ToString()).ToList();
            var result = new DealerResult(
                Cards: new ReadOnlyCollection<string>(hand),
                Reply: reply,
                Value: dealer.CurrentValue()
            );
            return result;
        }

        public WinnerResult TryPayout()
        {
            if (Phase != GamePhases.Payout)
            {
                throw new InvalidOperationException(
                    $"Cannot calculate winnings in the current phase: {Phase}."
                );
            }

            var reply = Replies.Empty;




            var player = Game.Player;
            var playerHands = player.Hands;
            var dealer = Game.Dealer;

            if (Game.IsBusted(playerHands[_activeHand])) reply = Replies.DealerWin;
            else if (Game.IsBusted(dealer)) reply = Replies.PlayerWin;
            else if (dealer.CurrentValue() == playerHands[_activeHand].CurrentValue()) reply = Replies.Tie;
            else reply = dealer.CurrentValue() > playerHands[_activeHand].CurrentValue() ? Replies.DealerWin : Replies.PlayerWin;

            if (player.HasSplit && _activeHand == 0)
            {
                _activeHand = 1;
            }
            else Phase = GamePhases.RoundEnd;

            var currentHand = playerHands[_activeHand];
            var playerCards = currentHand.Cards.Select(card => card.Rank.ToString()).ToList();
            var playerState = new HandState(
                Cards: new ReadOnlyCollection<string>(playerCards),
                Value: currentHand.CurrentValue(),
                IsActiveID: _activeHand
            );
            var hand = dealer.Cards.Select(card => card.Rank.ToString()).ToList();
            var dealerResult = new DealerResult(
                Cards: new ReadOnlyCollection<string>(hand),
                Reply: Replies.Empty,
                Value: dealer.CurrentValue()
            );

            var winner = new WinnerResult(
                Player: playerState,
                Dealer: dealerResult,
                WinCondition: reply
            );

            return winner;
        }

        public void EndRound()
        {
            if (Phase != GamePhases.RoundEnd)
            {
                throw new InvalidOperationException(
                    $"Cannot End round in the current phase: {Phase}."
                );
            }
            Phase = GamePhases.RoundEnd;
        }




        private Replies CheckBlackJack()
        {
            if (Phase != GamePhases.PlayerTurn) {
                throw new InvalidOperationException(
                    $"Cannot check Blackjack in the current phase: {Phase}."
                );
            }
            
            if (!Game.Player.HasSplit) return Replies.Empty;


            var hands = Game.Player.Hands;
            foreach (var hand in hands) hand.CheckBlackjack();
            if (hands[0].Blackjack && hands[1].Blackjack) 
            {
                return Replies.SplitBothBlackjack;
                
            }
            if (_activeHand == 0 && hands[0].Blackjack)
            {
                return Replies.SplitFirstHandBlackjack;
            }                
            if (_activeHand == 1 && hands[1].Blackjack)
            {
                return Replies.SplitSecondHandBlackjack;
            }

            
            return Replies.Empty;
        }

        public HandState GetActiveHand()
        {
            var hand = Game.Player.Hands[_activeHand];
            var cards = hand.Cards  
                .Select(card => card.Rank.ToString())
                .ToList();

            var state = new HandState(
                Cards: new ReadOnlyCollection<string>(cards),
                Value: hand.CurrentValue(), 
                IsActiveID: _activeHand);
            
            return state;
        }

        public DealerResult GetDealerHand()
        {

            var dealer = Game.Dealer;
            var hand = dealer.Cards.Select(card => card.Rank.ToString()).ToList();
            var result = new DealerResult(
                Cards: new ReadOnlyCollection<string>(hand),
                Reply: Replies.Empty,
                Value: dealer.CurrentValue()
            );

            return result;
        }
    }
}
