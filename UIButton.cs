using System;

namespace GameOfLife
{
    class UIButton : UIObject, IDrawable
    {
        readonly private Action _execute;

        public UIButton(string name, string text, int x, int y, bool visible = true, Action execute = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base (name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            selectable = true;
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
                Program.DrawUpdates.Add(this);
            }
        }
        public override void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fColor;
            if (selected) Console.BackgroundColor = ConsoleColor.Green;
            else Console.BackgroundColor = bColor;
            Console.Write(text);
            Console.ResetColor(); 
        }
        public override void Action()
        {
            _execute();
        }
    }
}
