using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameOfLife
{
    class LoadAndSaveScene : Scene
    {
        public LoadAndSaveScene()
        {
            Start();
        }
        public override void Start()
        {
            UIElements.Clear();
            Console.Clear();
            Console.CursorVisible = false;

            UIElements.Add(new UIText("LoadAndSave", $"select a game to load or enter filename for new savegame", 10, 5, true));
            UIElements.Add(new UIInput("Filename", "Filename", 10, 7, true, () => { }));
            UIElements.Add(new UIButton("Load", "Load", 10, 9, true, () => { }));
            UIElements.Add(new UIButton("Save", "Save", 10, 10, true, () => { }));

            string[] fileNames = Directory.GetFiles(@".\", "*.xml");

            for (int i = 0; i < fileNames.Length; i++)
            {
                int e = new int();
                e = i;
                UIElements.Add(new UIButton($"File {i}", $"{fileNames[i]}", 10, 12 + i, true, () => { UIElements[1].input = fileNames[e];}));
            }

            cursor = new UICursor("Cursor", " ", 5, 5, true, () => { });
            UIElements.Add(cursor);
            activeElement = 1;
        }
        public override void Update()
        {
            Draw();

            if (Console.KeyAvailable)
            {
                UserInput = Console.ReadKey(true);

                switch (UserInput.Key)
                {
                    case ConsoleKey.UpArrow:
                        activeElement = FindNextUIElement(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        activeElement = FindNextUIElement(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        activeElement = FindNextUIElement(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        activeElement = FindNextUIElement(Direction.Right);
                        break;
                    case ConsoleKey.Enter:
                        UIElements[activeElement].Action();
                        break;
                    case ConsoleKey.Escape:
                        Escape();
                        break;
                    default:
                        break;
                }
            }
        }

        public void Escape()
        {
            Program.Scenes.Pop();
        }
    }
}
