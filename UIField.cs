using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    class UIField : UIObject, IDrawable // TODO: Random-Effekt: zappelnde Zelle
    {
        List<ConsoleColor[]> colorTheme = new List<ConsoleColor[]>
        {
            new ConsoleColor[]{ConsoleColor.White, ConsoleColor.Green, ConsoleColor.DarkGreen, ConsoleColor.DarkGreen, ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.Gray, ConsoleColor.DarkGray},
            new ConsoleColor[]{ConsoleColor.White, ConsoleColor.Magenta, ConsoleColor.DarkMagenta,ConsoleColor.DarkMagenta, ConsoleColor.Green, ConsoleColor.DarkGreen , ConsoleColor.Gray, ConsoleColor.DarkGray},
            new ConsoleColor[]{ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.DarkYellow, ConsoleColor.DarkYellow, ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.Gray, ConsoleColor.DarkGray},     
            new ConsoleColor[]{ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.DarkRed, ConsoleColor.Gray, ConsoleColor.DarkGray}     
        };
        Random rnd = new Random();
        DateTime lastUpdate; // FPS limiter
        public bool[,] _field;
        sbyte[,] backupField;
        int width = 0;
        int height = 0;
        public int _effectDelay = 50;
        int _colorMode = 0;
        int prevX = -1;
        int prevY = -1;

        public UIField(string name, string text, int x, int y, bool[,] field, int width = 0, int height = 0, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false) : base(name, text, x, y, visible, fColor, bColor, selected)
        {
            _field = field;
            this.width = width;
            this.height = height;
            selectable = false;
            backupField = new sbyte[height,width];
            Program.DrawUpdates.Add(this);
            for (int y1 = 0; y1 < height; y1++)
            {
                for (int x1 = 0; x1 < width; x1++)
                {
                    backupField[y1, x1] = -2;
                }
            }
        }
        public int colorMode
        {
            get
            {
                return _colorMode;
            }
            set
            {
                if (value < 0) _colorMode = colorTheme.Count - 1;
                else if (value >= colorTheme.Count) _colorMode = 0;
                else _colorMode = value;
            }
        }
        public int effectDelay
        {
            get
            {
                return _effectDelay;
            }
            set
            {
                if (value >= 0 && value <= 500)
                    _effectDelay = value;
            }
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
        public void Set(bool[,] field)
        {
            _field = field;
        }
        public void DrawPosition(int x1, int y1)
        {
            if (backupField[y1, x1] >= -128 && backupField[y1, x1] < -100)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][4], '·');
            }
            else if (backupField[y1, x1] >= -100 && backupField[y1, x1] < -50)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][5], '·');
            }
            else if (backupField[y1, x1] >= -50 && backupField[y1, x1] < -2)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][6], '·');
            }
            else if (backupField[y1, x1] == -2 || backupField[y1, x1] == -1)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][7], '·');
            }
            else if (backupField[y1, x1] >= 0 && backupField[y1, x1] < 115)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][1], 'O');
            }
            else if (backupField[y1, x1] >= 115 && backupField[y1, x1] < 126)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][3], 'O');
            }
            else if (backupField[y1, x1] == 126 || backupField[y1, x1] == 127)
            {
                SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][2], 'o');
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
                        if (backupField[y1, x1] >= 0)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][4], '·');
                            backupField[y1, x1] = -128;
                        }
                        else if (backupField[y1, x1] == -120)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][5], '·');
                        }
                        else if (backupField[y1, x1] == -100)
                        {
                            backupField[y1, x1] -= (sbyte)rnd.Next(0, 5);
                        }
                        else if (backupField[y1, x1] == -50)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][6], '·');
                        }
                        else if(backupField[y1, x1] == -2)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][7], '·');
                        }
                    }
                    else
                    {
                        if (backupField[y1, x1] < 0)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][0], 'o');
                            backupField[y1, x1] = 127;
                        }
                        else if (backupField[y1, x1] == 126)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][2], 'o');
                        }
                        else if (backupField[y1, x1] == 116)
                        {
                            backupField[y1, x1] += (sbyte)rnd.Next(0, 3);
                        }
                        else if (backupField[y1, x1] == 115)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][3], 'O');
                        }
                        else if (backupField[y1, x1] == 100)
                        {
                            SetColorAndDraw(x + x1, y + y1, colorTheme[_colorMode][1], 'O');
                        }
                    }
                    if (backupField[y1, x1] > 0) backupField[y1, x1]--;
                    else if (backupField[y1, x1] < -1) backupField[y1, x1]++;
                }
            }
        }
        public void SetColorAndDraw(int x, int y, ConsoleColor color, char pchar)
        {
            Console.ForegroundColor = color;
            if (x != prevX + 1 || y != prevY) Console.SetCursorPosition(x, y);
            prevX = x;  // xy merken und evtl. im nächsten Durchlauf SetCursorPosition überspringen, da Write die x-Position eh um 1 erhöht -> mehr Speed?
            prevY = y;
            Console.Write(pchar);
        }
    }
}
