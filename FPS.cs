using System;

namespace GameOfLife
{
    class FPS
    {
        DateTime lastUpdate;
        uint framesSinceLastUpdate;

        public FPS()
        {
            lastUpdate = DateTime.Now;
            framesSinceLastUpdate = 0;
        }
        public void Draw()
        {
            framesSinceLastUpdate++;
            if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 1000)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(0, 0);
                Console.Write("{0,0} fps ", framesSinceLastUpdate / 5);
                framesSinceLastUpdate = 0;
                lastUpdate = DateTime.Now;
            }
        }
    }
}
