using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GameOfLife
{
    class GameScene : Scene
    {
        public GameLogic gameLogic;
        DateTime lastUpdate = DateTime.Now;
        bool autoCycleMode = false;
        public GameScene(GameLogic gameLogic = null)
        {
            if (gameLogic == null) this.gameLogic = new GameLogic();
            else this.gameLogic = gameLogic;
            Start();
        }
        public override void Start()
        {
            UIElements.Clear();
            ConsoleClear();

            logo = new UILogo("Logo", "Logo.txt", 5, 1, 88, 3);
            UIElements.Add(logo);

            UIElements.Add(new UIText("Status", $"cycle #: {gameLogic.cycleNumber}", 10, 0, true));

            UIElements.Add(new UIInput("X", "X-Size", 5, gameLogic.height + 6, true, () => { activeElement = GetUIElementIDByName("Y"); }));
            UIElements.Add(new UIInput("Y", "Y-Size", 20, gameLogic.height + 6, true, Restart));

            UIElements.Add(new UIButton("Empty",    "[  E Empty  ]", 5, gameLogic.height + 8, true, Restart));
            UIElements.Add(new UIButton("Cycle",    "[  C Cycle  ]", 19, gameLogic.height + 8, true, Cycle));
            UIElements.Add(new UIButton("LoadSave", "[D Load/Save]", 33, gameLogic.height + 8, true, () => { Program.Scenes.Push(new LoadAndSaveScene(ref gameLogic)); }));
            UIElements.Add(new UIButton("Auto",     "[  A Auto   ]", 47, gameLogic.height + 8, true, () => { autoCycleMode = !autoCycleMode;}));
            UIElements.Add(new UIButton("Random",   "[ R Random  ]", 61, gameLogic.height + 8, true, Random));
            UIElements.Add(new UIButton("Load",     "[L QuickLoad]", 75, gameLogic.height + 8, true, Load));
            UIElements.Add(new UIButton("Save",     "[S QuickSave]", 89, gameLogic.height + 8, true, Save));
            UIElements.Add(new UIButton("Quit",     "[ ESC Quit  ]", 103, gameLogic.height + 8, true, Quit));

            field = new UIField("Field", "GameOfLife", 5, 5, gameLogic.fieldAB[gameLogic.currentField ? 1 : 0], gameLogic.width, gameLogic.height);
            UIElements.Add(field);

            cursor = new UICursor("Cursor", " ", 5, 5, gameLogic.width, gameLogic.height, true, () => { Click(cursor.fieldX, cursor.fieldY); }, () => { UpdatePosition(cursor.fieldX, cursor.fieldY); });
            UIElements.Add(cursor);

            DrawUIElements();

            activeElement = 2;
        }

        public void UpdatePosition(int x, int y)
        {
            field.DrawPosition(x, y);
        }
        public override void Update()
        {
            Draw();
            GetUIElementByName("Field").Draw();
            logo.DrawEffect();
            AutoCycle();

            if (Console.KeyAvailable)
            {
                UserInput = Console.ReadKey(true);

                switch (UserInput.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursor.cursorMode) cursor.Move(Direction.Up);
                        else activeElement = FindNextUIElement(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursor.cursorMode && cursor.fieldY == cursor.fieldMaxY - 1) { activeElement = 3; UpdatePosition(cursor.fieldX, cursor.fieldY); }
                        else if (cursor.cursorMode) cursor.Move(Direction.Down);
                        else activeElement = FindNextUIElement(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursor.cursorMode) cursor.Move(Direction.Left);
                        else activeElement = FindNextUIElement(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursor.cursorMode) cursor.Move(Direction.Right);
                        else activeElement = FindNextUIElement(Direction.Right);
                        break;
                    case ConsoleKey.Enter:
                        if (cursor.cursorMode) cursor.Action();
                        else UIElements[activeElement].Action();
                        break;
                    case ConsoleKey.Spacebar:
                        if (cursor.cursorMode) cursor.Action();
                        else UIElements[activeElement].Action();
                        break;
                    case ConsoleKey.Tab:
                        cursor.cursorMode = !cursor.cursorMode;
                        if (cursor.cursorMode) activeElement = GetUIElementIDByName("Cursor");
                        else activeElement = 3;
                        break;
                    case ConsoleKey.A:
                        autoCycleMode = !autoCycleMode;
                        break;
                    case ConsoleKey.C:
                        Cycle();
                        break;
                    case ConsoleKey.D: // TODO: entfernen, nur zum Test
                        Program.Scenes.Push(new LoadAndSaveScene(ref gameLogic));
                        break;
                    case ConsoleKey.L:
                        Load();
                        break;
                    case ConsoleKey.R:
                        Random();
                        break;
                    case ConsoleKey.E:
                        Restart();
                        break;
                    case ConsoleKey.S:
                        Save();
                        break;
                    case ConsoleKey.Add:
                        field.effectDelay++;
                        break;
                    case ConsoleKey.Subtract:
                        field.effectDelay--;
                        break;
                    case ConsoleKey.Escape:
                        Quit();
                        break;
                    case ConsoleKey.Backspace:
                        UIElements[activeElement].input = UIElements[activeElement].input.Remove(UIElements[activeElement].input.Length - 1);
                        break;
                    default:
                        if (Char.IsNumber(UserInput.KeyChar))
                        {
                            UIElements[activeElement].input += UserInput.KeyChar;
                        }
                        break;
                }
            }
        }
        public void Click(int x, int y)
        {
            gameLogic.TogglePosition(x, y);
        }
        public void Load() // TODO: Eingabe im Userinterface
        {
            LoadGame("savegame.xml");
        }
        public void Save() // TODO: Eingabe im Userinterface
        {
            SaveGame("savegame.xml");
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
            gameLogic = new GameLogic(gobject.width, gobject.height, fieldA, gobject.cycle);
            Start();
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
        }
        public void AutoCycle()
        {
            if (autoCycleMode)
            {
                if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 500)
                {
                    Cycle();
                    lastUpdate = DateTime.Now;
                }
            }
        }
        public void Toggle()
        {
            gameLogic.TogglePosition((UIElements[_activeElement].x - 5), (UIElements[_activeElement].y - 5));
        }
        public void Random()
        {
            Random rnd = new Random();
            for (byte y = 0; y < gameLogic.height; y++)
            {
                for (byte x = 0; x < gameLogic.width; x++)
                {
                    if (rnd.Next(0, 2) == 1)
                        gameLogic.TogglePosition(x, y);
                }
            }
            Program.DrawUpdates.Add(GetUIElementByName("Field"));
        }
        public void Next()
        {
            activeElement = FindNextUIElement(Direction.Down);
        }
        public void Cycle()
        {
            gameLogic.NextCycle();
            GetUIElementByName("Field").Set(gameLogic.fieldAB[gameLogic.currentField ? 1 : 0]);
            GetUIElementByName("Status").text = $"cycle #: {gameLogic.cycleNumber}  effectDelay +/-: {field.effectDelay}";
        }
        public void Quit()
        {
            Program.Scenes.Pop();
        }
        public void Restart()
        {
            int x;
            int y;
            if (int.TryParse(UIElements[GetUIElementIDByName("X")].input, out x) &&
                int.TryParse(UIElements[GetUIElementIDByName("Y")].input, out y))
            {
                gameLogic = new GameLogic(x, y);
                Start();
            }
            else
            {
                gameLogic = new GameLogic();
                Start();
            }
        }
    }
}
