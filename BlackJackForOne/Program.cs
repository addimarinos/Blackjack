using BlackJackForOne.UI;

namespace BlackJackForOne;

internal class Program
{
    static void Main ()
    {

        /*
                var image = new CanvasImage(_game.Dealer.ShowHand().First().ImagePath).NearestNeighborResampler().MaxWidth(20);
                Console.WriteLine(image);
                AnsiConsole.Write(image); */
        
        
        ConsoleUI console = new ConsoleUI();
        console.Run();

        Console.WriteLine();
        Console.WriteLine("     Thank you for playing, GoodBye ♣ ♠ ♥ ♦      ");


    }

};



