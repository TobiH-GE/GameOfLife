using System;
using System.Collections.Generic;

namespace GameOfLife
{
    class Program                               // *** GameOfLife von TobiH ***
    {
        public static Stack<Scene> Scenes = new Stack<Scene>();
        static void Main(string[] args)
        {
            Scenes.Push(new GameScene());

            do
            {
                Scenes.Peek().Update();
            } while (Scenes.Count > 0);

            Console.Clear();
        }
    }
}
