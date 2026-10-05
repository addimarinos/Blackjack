namespace BlackJackForOne.Models
{
    public class Deck
    {
        public readonly List<Card> Cards;

        public Deck() // simple improvement - param for number of decks: int num decks then loop that in construct
        {
            Cards = [];
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                // Add four cards of each rank and suitto the deck
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    Cards.Add(new Card(rank, suit));
                    Cards.Add(new Card(rank, suit));
                    Cards.Add(new Card(rank, suit));
                    Cards.Add(new Card(rank, suit));
                }
            }
            // shuffle the deck after creating it, using Fischer-Yates shuffle
            this.Shuffle();
        }

        public Card DealCard()
        {
            // if deck is empty return null
            //if (Cards.Count == 0) return 0;

            // otherwise deal top card
            Card card = Cards.First();
            Cards.RemoveAt(0);
            return card;
        }

        public void Shuffle()
        {
            Random r = new Random();

            for (int i = 0; i < Cards.Count - 1; i++)
            {
                int index = r.Next(i, Cards.Count);
                (Cards[i], Cards[index]) = (Cards[index], Cards[i]);
            }
        }

        public int CurrentDeckSize()
        {
            return Cards.Count;
        }
    }
}