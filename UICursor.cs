using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UICursor : UIObject, IDrawable // TODO: Cursor-Bereich an Feld-Bereich anpassen
    {
        readonly private Action _execute;
        readonly private Action _executeAfterMove;
        public int fieldX;
        public int fieldY;
        public bool _cursorMode;
        public int fieldMaxX = 100;
        public int fieldMaxY = 20;

        public UICursor(string name, string text, int x, int y, int fieldMaxX, int fieldMaxY, bool visible = true, Action execute = null, Action executeAfterMove = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            _executeAfterMove = executeAfterMove;
            selectable = true;
            this.fieldMaxX = fieldMaxX;
            this.fieldMaxY = fieldMaxY;
        }
        public bool cursorMode
        {
            get
            {
                return _cursorMode;
            }
            set
            {
                _cursorMode = value;
                if (cursorMode == true) visible = true;
                else visible = false;
            }
        }
        public void Move(Direction direction)
        {
            ActionAfterMove();
            switch (direction)
            {
                case Direction.Up:
                    if (fieldY > 0) fieldY--;
                    break;
                case Direction.Down:
                    if (fieldY < fieldMaxY - 1) fieldY++;
                    break;
                case Direction.Left:
                    if (fieldX > 0) fieldX--;
                    break;
                case Direction.Right:
                    if (fieldX < fieldMaxX - 1) fieldX++;
                    break;
                default:
                    break;
            }
            Program.DrawUpdates.Add(this);
        }
        public override void Draw()
        {
            if (visible)
            {
                Console.SetCursorPosition(x + fieldX, y + fieldY);
                Console.ForegroundColor = fColor;
                if (selected) Console.BackgroundColor = ConsoleColor.Green;
                else Console.BackgroundColor = bColor;
                Console.Write(text);
                Console.ResetColor();
            }
        }
        public override void Action()
        {
            _execute();
        }
        public void ActionAfterMove()
        {
            _executeAfterMove();
        }
    }
}
