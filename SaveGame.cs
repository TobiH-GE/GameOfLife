﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife
{
    public class SaveGame
    {
        public string name;
        public int cycle;
        public int width;
        public int height;
        public DateTime datetime;
        public bool[] field;
    }
}
