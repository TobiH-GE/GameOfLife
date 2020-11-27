using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UIField : UIObject, IDrawable // TODO: Random-Effekt: zappelnde Zelle
    {
        DateTime lastUpdate; // FPS limiter
        public bool[,] _field;
        sbyte[,] backupField;
        int width = 0;
        int height = 0;
        public int effectDelay = 50;
        public UIField(string name, string text, int x, int y, bool[,] field, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _field = field;
            this.width = width;
            this.height = height;
            selectable = false;
            backupField = new sbyte[height,width];
            Program.DrawUpdates.Add(this);
        }
        public bool[,] field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = field;
                Program.DrawUpdates.Add(this);
            }
        }
        public override void Set(bool[,] field)
        {
            _field = field;
        }
        public void DrawPosition(int x1, int y1)
        {
            if (backupField[y1, x1] == -2 || backupField[y1, x1] == -1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("·");
            }
            else if (backupField[y1, x1] >= -128 && backupField[y1, x1] < -100)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("·");
            }
            else if (backupField[y1, x1] >= -100 && backupField[y1, x1] < -50)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("·");
            }
            else if (backupField[y1, x1] >= -50 && backupField[y1, x1] < -2)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("·");
            }
            else if (backupField[y1, x1] >= 0 && backupField[y1, x1] < 115)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("O");
            }
            else if (backupField[y1, x1] >= 115 && backupField[y1, x1] < 126)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("O");
            }
            else if (backupField[y1, x1] == 126 || backupField[y1, x1] == 127)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.SetCursorPosition(x + x1, y + y1);
                Console.Write("o");
            }
        }
        public override void Draw()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            if ((DateTime.Now - lastUpdate).TotalMilliseconds <= effectDelay) return;
            lastUpdate = DateTime.Now;

            for (int y1 = 0; y1 < height; y1++)
            {
                for (int x1 = 0; x1 < width; x1++)
                {
                    if (field[y1, x1] == false)
                    {
                        if (backupField[y1, x1] == -2)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("·");
                        }
                        else if (backupField[y1, x1] >= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            backupField[y1, x1] = -128;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("·");
                        }
                        else if (backupField[y1, x1] == -100)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("·");
                        }
                        else if (backupField[y1, x1] == -50)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("·");
                        }
                    }
                    else
                    {
                        if (backupField[y1, x1] == 100)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("O");
                        }
                        else if (backupField[y1, x1] == 115)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("O");
                        }
                        else if (backupField[y1, x1] < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            backupField[y1, x1] = 127;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("o");
                        }
                        else if (backupField[y1, x1] == 126)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.SetCursorPosition(x + x1, y + y1);
                            Console.Write("o");
                        }
                    }
                    if (backupField[y1, x1] > 0) backupField[y1, x1]--;
                    else if (backupField[y1, x1] < -1) backupField[y1, x1]++;
                }
            }
        }
    }
}
