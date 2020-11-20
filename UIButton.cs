using System;

namespace GameOfLife
{
    class UIButton : UIObject
    {
        Func<bool> methodName;

        public UIButton(string name, string text, int x, int y, bool visible = true, Func<bool> methodName = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base (name, text, x, y, visible, fColor, bColor, selected)
        {
            this.methodName = methodName;
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
            methodName();
        }
    }
}
