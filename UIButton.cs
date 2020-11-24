using System;

namespace GameOfLife
{
    class UIButton : UIObject
    {
        readonly private Action _execute;

        public UIButton(string name, string text, int x, int y, bool visible = true, Action execute = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base (name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            selectable = true;
        }
        public override void Draw()
        {
            if (visible)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = fColor;
                if (selected)
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
        public override void Action()
        {
            _execute();
        }
    }
}
