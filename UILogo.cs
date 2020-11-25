using System;
using System.IO;

namespace GameOfLife
{
    public class UILogo : UIObject
    {
        public string[] logo;
        public int height;
        public UILogo(string name, string text, int x, int y, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            this.height = height;
            selectable = false;

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
            if (drawUpdate && visible)
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