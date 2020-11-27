using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UICursor : UIObject, IDrawable // TODO: Cursor-Bereich an Feld-Bereich anpassen
    {
        readonly private Action _execute;
        readonly private Action _executeAfterMove;

        public UICursor(string name, string text, int x, int y, int fieldMaxX, int fieldMaxY, bool visible = true, Action execute = null, Action executeAfterMove = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            _executeAfterMove = executeAfterMove;
            selectable = true;
            base.fieldMaxX = fieldMaxX;
            base.fieldMaxY = fieldMaxY;
        }
        public override void Move(Direction direction)
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
        public override void ActionAfterMove()
        {
            _executeAfterMove();
        }
    }
}
