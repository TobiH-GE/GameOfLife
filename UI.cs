using System;

namespace GameOfLife
{
    abstract class UI
    {
        public Game game;
        public abstract void PrintStatus(ref Game game);
        public abstract void PrintError(string str);
        public abstract void PrintInfo(string str);
        public abstract void Start();
        public abstract void WaitForInput();
        public abstract void Draw();
    }
}
