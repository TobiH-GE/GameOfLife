using System;

namespace GameOfLife
{
    struct Point
    {
        public sbyte x;
        public sbyte y;
    }
    class Program                               // *** GameOfLife von TobiH ***
    {
        static void Main(string[] args)
        {
            UI UIGame = new UIConsole();
            Game game = new Game(UIGame);

            do
            {
                UIGame.WaitForInput();
            } while (game.status != Status.Stopped);

        Console.Clear();
        }
    }
}
