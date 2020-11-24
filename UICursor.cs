using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UICursor : UIObject
    {
        readonly private Action _execute;
        int maxX;
        int maxY;
        int minX;
        int minY;

        public UICursor(string name, string text, int x, int y, bool visible = true, Action execute = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            selectable = true;
            selected = true;
        }
        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    y--;
                    break;
                case Direction.Down:
                    y++;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Right:
                    x++;
                    break;
                default:
                    break;
            }
        }
        public override void Draw()
        {
            if (true)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = fColor;
                if (true)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    if (text == "") text = " ";
                }
                else
                {
                    Console.BackgroundColor = bColor;
                    if (text == " ") text = "";
                }

                Console.Write(text);
                Console.ResetColor();
            }
        }
        public void Action()
        {
            _execute();
        }
    }
}
