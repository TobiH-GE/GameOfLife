using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UIField : UIObject
    {
        int width = 0;
        int height = 0;
        bool[,] field;
        public UIField(string name, string text, int x, int y, bool[,] field, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            this.field = field;
            this.width = width;
            this.height = height;
        }
        public override void Draw()
        {
            for (int y1 = 0; y1 < width; y1++)
            {
                for (int x1 = 0; x1 < height; x1++)
                {
                    Console.SetCursorPosition(x+x1, y+y1);
                    if (field[y1, x1] == false)
                        Console.Write(".");
                    else
                        Console.Write("X");
                }
            }
        }
    }
}
