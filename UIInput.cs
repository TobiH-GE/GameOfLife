using System;

namespace GameOfLife
{
    class UIInput : UIObject, IDrawable
    {
        readonly private Action _execute;
        public UIInput(string name, string text, int x, int y, string input = "", bool visible = true, Action execute = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base (name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            selectable = true;
            base.input = input;
        }

        public override void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fColor;
            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.BackgroundColor = bColor;
            }

            Console.Write(text + ":");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" " + input + " ");
            Console.ResetColor();
        }
        public override void Action()
        {
            _execute();
        }
    }
}
