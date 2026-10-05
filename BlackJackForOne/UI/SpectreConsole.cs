using Spectre.Console;

namespace BlackJackForOne.UI
{
    public class SpectreConsole
    {
        public void Run()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Blackjack[/]").RuleStyle("black"));

            //var dealerCards = new List<string> { $"[black]{}"}
        }
    }
}