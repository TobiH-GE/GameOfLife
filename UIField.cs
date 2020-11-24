using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UIField : UIObject
    {
        int width = 0;
        int height = 0;
        public UIField(string name, string text, int x, int y, bool[,] field, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            this.field = field;
            this.width = width;
            this.height = height;
            selectable = false;
        }
        public override void Draw()
        {
            string fieldString; // wir bauen uns ein String -> schneller bei der Ausgabe

            for (int y1 = 0; y1 < height; y1++)
            {
                fieldString = "";
                for (int x1 = 0; x1 < width; x1++)
                {
                    if (field[y1, x1] == false)
                        fieldString = fieldString + "-";
                    else
                        fieldString = fieldString + "X";
                }
                Console.SetCursorPosition(x, y + y1);
                Console.Write(fieldString);
            }
        }
    }
}
