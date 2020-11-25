using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace GameOfLife
{
    class LoadAndSaveScene : Scene
    {
        public GameLogic gameLogic;
        public LoadAndSaveScene(ref GameLogic gameLogic)
        {
            this.gameLogic = gameLogic;
            Start();
        }
        public override void Start()
        {
            UIElements.Clear();
            Console.Clear();
            Console.CursorVisible = false;

            UIElements.Add(new UIText("LoadAndSave", $"select a game to load or enter filename for new savegame", 10, 5, true));
            UIElements.Add(new UIInput("Filename", "Filename", 10, 7, true, () => { }));
            UIElements.Add(new UIButton("Load", "Load", 10, 9, true, () => { LoadGame(UIElements[1].input); }));
            UIElements.Add(new UIButton("Save", "Save", 10, 10, true, () => { SaveGame(UIElements[1].input); }));

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
        public void LoadGame(string file)
        {
            SaveGame gobject = new SaveGame();
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            int i = 0;

            using (Stream load = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                gobject = (SaveGame)serializer.Deserialize(load);
            }
            bool[,] fieldA = new bool[gobject.height, gobject.width];
            for (int y = 0; y < gobject.height; y++)
            {
                for (int x = 0; x < gobject.width; x++)
                {
                    fieldA[y, x] = gobject.field[i];
                    i++;
                }
            }
            Program.Scenes.Pop();
            Program.Scenes.Pop();
            Console.Clear();
            Program.Scenes.Push(new GameScene(new GameLogic(gobject.width, gobject.height, fieldA, gobject.cycle)));
        }
        public void SaveGame(string file)
        {
            SaveGame gobject = new SaveGame();
            gobject.name = "Test";
            gobject.cycle = gameLogic.cycleNumber;
            gobject.width = gameLogic.width;
            gobject.height = gameLogic.height;
            gobject.datetime = DateTime.Now;
            gobject.field = new bool[gameLogic.height * gameLogic.width];
            int i = 0;
            int c = gameLogic.currentField ? 1 : 0;
            for (int y = 0; y < gameLogic.height; y++)
            {
                for (int x = 0; x < gameLogic.width; x++)
                {
                    gobject.field[i] = gameLogic.fieldAB[c][y, x];
                    i++;
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

            using (Stream save = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(save, gobject);
            }
            Program.Scenes.Pop();
        }
        public void Escape()
        {
            Program.Scenes.Pop();
        }
    }
}
