
namespace BlackJackForOne.Models
{
    public class Player(string name)
    {
        public List<Hand> Hands { get; set; } = [];
        public string Name { get; } = name;
        public int Balance { get; set; }
        public int Bet { get; set; }
        public bool HasSplit { get; set; } = false;

        public void SplitHand(Card newcard1, Card newcard2)
        {
            var oldHand = Hands.First().SplitHand();
            Hands.Clear();

            var newHand1 = new Hand();
            var newHand2 = new Hand();

            newHand1.AddCard(oldHand.Item1);
            newHand1.AddCard(newcard1);
            newHand2.AddCard(oldHand.Item2);
            newHand2.AddCard(newcard2);

            Hands.Add(newHand1);
            Hands.Add(newHand2);

        }
    }
}