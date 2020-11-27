using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    interface IDrawable
    {
        public void Draw();
    }
    interface ISelectable
    {
        bool selected { get; set; }
    }
}
