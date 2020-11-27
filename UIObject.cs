using System;

namespace GameOfLife
{
    public class UIObject : IDrawable // TODO: Interface IDrawable und Liste needsRedraw
    {
        public string name;
        public string _text;
        public string _input;
        public int x;
        public int y;
        public bool visible;
        protected ConsoleColor fColor;
        protected ConsoleColor bColor;
        public bool selected;
        public bool selectable;
        
        public string input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                Program.DrawUpdates.Add(this);
            }
        }
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Program.DrawUpdates.Add(this);
            }
        }
        public UIObject(string name, string text, int x, int y, bool visible = true, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black, bool selected = false)
        {
            this.name = name;
            this.text = text;
            this.x = x;
            this.y = y;
            this.visible = visible;
            this.fColor = fColor;
            this.bColor = bColor;
            this.selected = selected;
        }

        public virtual void Draw()
        {
            
        }
        public virtual void Set(bool [,] field)
        {

        }
        public virtual void Action()
        {

        }
        public virtual void ActionAfterMove()
        {

        }
        public virtual void Move(Direction up)
        {
            
        }
    }
}
