using System;

namespace GameOfLife
{
    abstract class Scene
    {
        public GameLogic game;
        public abstract void PrintStatus(ref GameLogic game);
        public abstract void Start(int x, int y);
        public abstract void Update();
        public abstract void Draw();
    }
}
