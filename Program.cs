using System;
using System.Collections.Generic;

namespace GameOfLife // TODO: catch wrong input
{
    class Program                               // *** GameOfLife von TobiH ***
    {
        public static Stack<Scene> Scenes = new Stack<Scene>();
        public static List<IDrawable> DrawUpdates = new List<IDrawable>();
        static void Main(string[] args)
        {
            Scenes.Push(new GameScene());

            do
            {
                if (DrawUpdates.Count > 0)
                {
                    foreach (var item in DrawUpdates)
                    {
                        item.Draw();
                    }
                    DrawUpdates.Clear();
                }
                Scenes.Peek().Update();
            } while (Scenes.Count > 0);

            Console.Clear();
        }
    }
}
