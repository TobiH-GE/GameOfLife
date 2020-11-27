using System;

namespace GameOfLife
{
    class UIText : UIObject, IDrawable
    {
        public UIText(string name, string text, int x, int y, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            selectable = false;
        }
        public new string text
        {
            get
            {
                return base.text;
            }
            set
            {
                base.text = text;
            }
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

            Console.Write(text);
            Console.ResetColor();
            drawUpdate = false;
        }
    }   
}
