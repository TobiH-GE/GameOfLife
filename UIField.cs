using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UIField : UIObject
    {
        sbyte[,] backupField;
        int width = 0;
        int height = 0;
        public UIField(string name, string text, int x, int y, bool[,] field, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            this.field = field;
            this.width = width;
            this.height = height;
            selectable = false;
            backupField = new sbyte[height,width];
        }
        public override void Draw()
        {
            //string fieldString; // wir bauen uns ein String -> schneller bei der Ausgabe

            for (int y1 = 0; y1 < height; y1++)
            {
                //fieldString = "";
                for (int x1 = 0; x1 < width; x1++)
                {
                    Console.SetCursorPosition(x + x1, y + y1);
                    if (field[y1, x1] == false)
                    {
                        if (backupField[y1, x1] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            backupField[y1, x1] = -128;
                        }
                        else if (backupField[y1, x1] > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.Write("-");
                    }
                        //fieldString = fieldString + "-";
                    else
                    {
                        if (backupField[y1, x1] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            backupField[y1, x1] = 127;
                        }
                        else if (backupField[y1, x1] < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.Write("+");
                        //fieldString = fieldString + "X";
                    }
                    if (backupField[y1, x1] > 0) backupField[y1, x1]--;
                    else if (backupField[y1, x1] < 0) backupField[y1, x1]++;
                }
                //Console.SetCursorPosition(x, y + y1);
                //Console.Write(fieldString);
            }
        }
    }
}
