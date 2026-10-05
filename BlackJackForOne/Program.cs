using BlackJackForOne.UI;
using Spectre.Console;

namespace BlackJackForOne;

internal class Program
{
    static void Main ()
    {

        /*
                var image = new CanvasImage(_game.Dealer.ShowHand().First().ImagePath).NearestNeighborResampler().MaxWidth(20);
                Console.WriteLine(image);
                AnsiConsole.Write(image); */

        var left = new Panel("Your Account")
            .Header("[yellow]Notice[/]")
            .BorderColor(Color.Blue);
        var right = new Panel("Blackjack Table")
            .Header("[black]Table[/]")
            .BorderColor(Color.Red);

        AnsiConsole.Write(new Columns(left, right));



        Thread.Sleep(50000);
        
        ConsoleUI console = new ConsoleUI();
        console.Run();

        Console.WriteLine();
        Console.WriteLine("     Thank you for playing, GoodBye ♣ ♠ ♥ ♦      ");


    }

};



