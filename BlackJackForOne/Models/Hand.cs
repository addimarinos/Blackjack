namespace BlackJackForOne.Models
{
    public class Hand(bool isDealer = false)
    {
        public readonly List<Card> Cards = [];

        public List<Hand> MultHands = [];
        public bool IsDealer = isDealer;
        public bool HasSplit = false;
        public int Bet { get; set; }
        public int Balance { get; set; }
        
        public void AddCard(Card? card)
        {
            if (card != null) Cards.Add(card);
        }


        public int CurrentValue()
        {
            if (Cards.Count == 0) return 0;
            var val = 0;
            int aceCounter = 0;
            foreach (Card c in Cards)
            {
                val += c.CardValue;
                if (c.Rank == Rank.Ace) aceCounter++;
                while (aceCounter > 0 && val > 21)
                {
                    val -= 10;
                    aceCounter--;
                }
            }
            return val;
        }

        public string ShowCards(bool showDealerHand = false)
        {
            if (IsDealer && !showDealerHand) return Cards.First().Rank.ToString();
            string allCards = "";
            foreach (Card c in Cards)
            {
                allCards += " " + c.Rank.ToString();
            }
            return allCards;
        }

        public bool HasBusted()
        {
            return CurrentValue() > 21; 
        }
        public (int, int) ScoringResult() // check and return value of hand and number of cards
        {
            return (CurrentValue(), Cards.Count);

            /* var total = CurrentValue();
            if (total > 21) return ("Bust", Cards.Count);
            if (total == 21 && Cards.Count == 2) return ("BlackJack", 2);

            return "House Wins"; */
        }

        public bool CanSplit()
        {
            return Cards.Count == 2 && Cards.First().Rank == Cards.Last().Rank;
        }

        public (Card, Card) SplitHand()
        {
            return (Cards.First(), Cards.Last());
        }
    }
}