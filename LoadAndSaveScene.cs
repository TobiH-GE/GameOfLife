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
        string[] fileNames;
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
            UIElements.Add(new UIButton("Load", "Load", 10, 11, true, () => { LoadGame(UIElements[1].input); }));
            UIElements.Add(new UIButton("Save", "Save", 10, 12, true, () => { SaveGame(UIElements[1].input + ".xml"); }));

            UIElements.Add(new UIText("Found", $"found savegames:", 30, 9, true));
            fileNames = Directory.GetFiles(@".\", "*.xml");

            for (int i = 0; i < 10; i++)
            {
                //int e = new int();
                //e = i;
                //UIElements.Add(new UIButton($"File {i}", $"{fileNames[i]}", 30, 11 + i, true, () => { if (UIElements[1].input == fileNames[e]) LoadGame(UIElements[1].input); else UIElements[1].input = fileNames[e]; }));
                UIElements.Add(new UIButton($"File {i}", $"{i}.", 30, 11 + i, true, () => { }));
            }

            cursor = new UICursor("Cursor", " ", 12, 7, 0, 0, true, () => { }); // TODO: set cursor at input position
            UIElements.Add(cursor);
            activeElement = 1;
            ListFiles();

            DrawUIElements();
        }
        public void ListFiles()
        {
            for (int i = 0; i < 10 && i < fileNames.Length; i++)
            {
                if (fileNames[i] != "")
                {
                    int e = new int();
                    e = i;
                    UIElements[GetUIElementIDByName($"File {i}")] = new UIButton($"File {i}", $"{fileNames[i]}", 30, 11 + i, true, () => { if (UIElements[1].input == fileNames[e]) LoadGame(UIElements[1].input); else UIElements[1].input = fileNames[e]; });
                }
                else
                    UIElements[GetUIElementIDByName($"File {i}")].text = $"{i}.";
            }
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
                    case ConsoleKey.Backspace:
                        UIElements[activeElement].input = UIElements[activeElement].input.Remove(UIElements[activeElement].input.Length - 1);
                        break;
                    default:
                        if (Char.IsLetterOrDigit(UserInput.KeyChar))
                        {
                            UIElements[activeElement].input += UserInput.KeyChar;
                        }
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
            Console.Clear();
            Program.Scenes.Pop();
            Program.Scenes.Peek().Start();
        }
    }
}
