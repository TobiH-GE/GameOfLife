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
        bool autoCycleMode;
        public GameScene(GameLogic gameLogic = null)
        {
            if (gameLogic == null) this.gameLogic = new GameLogic();
            else this.gameLogic = gameLogic;
            Start();
        }
        public override void Start()
        {
            UIElements.Clear();
            Console.Clear();
            Console.CursorVisible = false;
            autoCycleMode = false;

            UIElements.Add(new UILogo("Logo", "Logo.txt", 5, 1, 88, 3));
            UIElements.Add(new UIText("Titel", "(c) by TobiH ", 99, 3));
            UIElements.Add(new UIText("Status", $"cycle #: {gameLogic.cycleNumber}", 10, 0, true));
            
            UIElements.Add(new UIInput("X", "X", 5, gameLogic.height + 6, true, Next));
            UIElements.Add(new UIInput("Y", "Y", 15, gameLogic.height + 6, true, Restart));

            UIElements.Add(new UIButton("New",    "[R] Restart", 5, gameLogic.height + 8, true, Restart));
            UIElements.Add(new UIButton("Toggle", "[ ] Toggle ", 19, gameLogic.height + 8, true, () => { })); // TODO: macht nichts
            UIElements.Add(new UIButton("Cycle",  "[C] Cycle  ", 33, gameLogic.height + 8, true, Cycle));
            UIElements.Add(new UIButton("Auto",   "[A] Auto   ", 47, gameLogic.height + 8, true, () => { autoCycleMode = !autoCycleMode;}));
            UIElements.Add(new UIButton("Messy",  "[M] Messy  ", 61, gameLogic.height + 8, true, Messy));
            UIElements.Add(new UIButton("Load",  "[L] Load  ", 75, gameLogic.height + 8, true, Load));
            UIElements.Add(new UIButton("Save",  "[S] Save  ", 89, gameLogic.height + 8, true, Save));
            UIElements.Add(new UIButton("Exit",   "[ESC] Exit ", 103, gameLogic.height + 8, true, Escape));

            UIElements.Add(new UIField("Field", "GameOfLife", 5,5, gameLogic.fieldAB[gameLogic.currentField ? 1 : 0], gameLogic.width, gameLogic.height));

            #region entfernte Buttons
            //for (byte yb = 0; yb < gameLogic.height; yb++) // alle Buttons auf dem Feld entfernt, dafür ein Cursor im Bereich aktiv wenn cursorMode = true
            //{
            //    for (byte xb = 0; xb < gameLogic.width; xb++)
            //    {
            //        UIElements.Add(new UIButton($"Button {xb},{yb}", "", 5 + xb, 5 + yb, true, Toggle));
            //    }
            //}
            #endregion
            cursor = new UICursor("Cursor", " ", 5, 5, true, () => { Click(cursor.fieldX, cursor.fieldY); });
            UIElements.Add(cursor);

            activeElement = 3;
        }
        public override void Update()
        {
            Draw();
            AutoCycle();
            #region Debug - Ausgabe von Infos
            //Console.SetCursorPosition(50, 24);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Up).ToString() + " ");
            //Console.SetCursorPosition(50, 26);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Down).ToString() + " ");
            //Console.SetCursorPosition(47, 25);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Left).ToString() + " ");
            //Console.SetCursorPosition(53, 25);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Right).ToString() + " ");
            #endregion

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
                        if (cursor.cursorMode && cursor.fieldY == cursor.fieldMaxY - 1) activeElement = 3;
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
                        if (cursor.cursorMode) activeElement = GetUIElementByName("Cursor");
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
                    case ConsoleKey.M:
                        Messy();
                        break;
                    case ConsoleKey.R:
                        Restart();
                        break;
                    case ConsoleKey.S:
                        Save();
                        break;
                    case ConsoleKey.Escape:
                        Escape();
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
        public void Messy()
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
        }
        public void Next()
        {
            activeElement = FindNextUIElement(Direction.Down);
        }
        public void Cycle()
        {
            gameLogic.NextCycle();
            UIElements[GetUIElementByName("Field")].field = gameLogic.fieldAB[gameLogic.currentField ? 1 : 0];
            UIElements[GetUIElementByName("Status")].text = $"cylce #: {gameLogic.cycleNumber}";
        }
        public void Escape()
        {
            Program.Scenes.Pop();
        }
        public void Restart()
        {
            int x;
            int y;
            if (int.TryParse(UIElements[GetUIElementByName("X")].input, out x) &&
                int.TryParse(UIElements[GetUIElementByName("Y")].input, out y))
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
