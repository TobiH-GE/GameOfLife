﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UICursor : UIObject // TODO: Cursor-Bereich an Feld-Bereich anpassen
    {
        readonly private Action _execute; 

        public UICursor(string name, string text, int x, int y, bool visible = true, Action execute = null, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _execute = execute;
            selectable = true;
        }
        public override void Move(Direction direction)
        {
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
    }
}
