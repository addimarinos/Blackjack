using BlackJackForOne.UI;
using Spectre.Console;

namespace BlackJackForOne;

internal class Program
{
    static void Main ()
    {
        
        ConsoleUi console = new ConsoleUi();
        console.Run();

        Console.WriteLine();
        Console.Clear();
        Console.WriteLine("Thank you for playing, GoodBye ♣ ♠ ♥ ♦");


    }

};



