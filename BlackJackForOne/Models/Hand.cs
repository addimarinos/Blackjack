namespace BlackJackForOne.Models
{
    public class Hand(bool isDealer = false)
    {
        private readonly List<Card> _cards = new List<Card>();
        public bool IsDealer = isDealer;

        public void AddCard(Card? card)
        {
            if (card != null) _cards.Add(card);
        }

        public int CurrentValue()
        {
            if (_cards.Count == 0) return 0;
            var val = 0;
            int aceCounter = 0;
            foreach (Card c in _cards)
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
            if (IsDealer && !showDealerHand) return _cards.First().Rank.ToString();
            string allCards = "";
            foreach (Card c in _cards)
            {
                allCards += " " + c.Rank.ToString();
            }
            return allCards;
        }
        public (int, int) ScoringResult() // check and return value of hand and number of cards
        {
            return (CurrentValue(), _cards.Count);

            /* var total = CurrentValue();
            if (total > 21) return ("Bust", _cards.Count);
            if (total == 21 && _cards.Count == 2) return ("BlackJack", 2);

            return "House Wins"; */
        }
    }
}