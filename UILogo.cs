using System;
using System.IO;

namespace GameOfLife
{
    public class UILogo : UIObject, IDrawable
    {
        DateTime lastUpdate;
        DateTime nextRandomEffectUpdate;
        bool playEffect = true;
        public string[] logo;
        public int height;
        public int effectPosition = 3;

        Random rnd = new Random();
        public UILogo(string name, string text, int x, int y, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            this.height = height;
            selectable = false;
            nextRandomEffectUpdate = DateTime.Now;

            logo = new string[height];
            Load(text);
        }
        public void Load(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                int i = 0;
                string line;

                while ((line = reader.ReadLine()) != null && i < height)
                {
                    logo[i] = new string(line);
                    i++;
                }
            }
        }
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            if (DateTime.Now >= nextRandomEffectUpdate)
            {
                playEffect = true;
                nextRandomEffectUpdate = DateTime.Now.AddMilliseconds(rnd.Next(5000, 15000));
            }

            if ((DateTime.Now - lastUpdate).TotalMilliseconds > 25 && playEffect)
            {
                // Line 2
                Console.SetCursorPosition(x + effectPosition, y + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(logo[1].Substring(effectPosition, 1));

                // Line 3
                Console.SetCursorPosition(x + effectPosition - 1, y + 2);
                Console.Write(logo[2].Substring(effectPosition - 1, 1));

                Console.SetCursorPosition(x + effectPosition - 1, y + 1);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(logo[1].Substring(effectPosition - 1, 1));

                Console.SetCursorPosition(x + effectPosition - 2, y + 2);
                Console.Write(logo[2].Substring(effectPosition - 2, 1));

                Console.SetCursorPosition(x + effectPosition, y + 2);
                Console.Write(logo[2].Substring(effectPosition, 1));

                Console.SetCursorPosition(x + effectPosition - 3, y + 2);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(logo[2].Substring(effectPosition - 3, 1));

                effectPosition++;
                if (effectPosition >= logo[1].Length)
                {
                    playEffect = false;
                    effectPosition = 3;
                }
                lastUpdate = DateTime.Now;
            }   

            if (drawUpdate)
            {
                int i = 0;

                Console.ForegroundColor = fColor;
                foreach (string line in logo)
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write(line);
                    i++;
                    if (i == 1) Console.ForegroundColor = ConsoleColor.Gray;
                    if (i >= 2) Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.ResetColor();
                drawUpdate = false; // Logo nur einmal zeichnen
            }
        }
    }
}
