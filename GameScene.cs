using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GameOfLife
{
    class GameScene : Scene
    {
        public List<UIObject> UIElements = new List<UIObject>();
        public UICursor cursor;
        public GameLogic gameLogic = new GameLogic();
        FPS fpsCounter = new FPS();
        DateTime lastUpdate = DateTime.Now;
        bool autoCycleMode = false;
        bool cursorMode = false;
        int _activeElement = 0;
        ConsoleKeyInfo UserInput = new ConsoleKeyInfo();
        ConsoleColor[] pColor = new ConsoleColor[2] { ConsoleColor.Red, ConsoleColor.Blue };

        public GameScene()
        {
            Start();
        }
        public int activeElement
        {
            get
            {
                return _activeElement;
            }
            set
            {
                if (value >= UIElements.Count) value = 0;
                if (value < 0) value = UIElements.Count - 1;

                UIElements[_activeElement].selected = false;
                _activeElement = value;
                UIElements[_activeElement].selected = true;
            }
        }

        public override void PrintStatus(ref GameLogic game)
        {
            UIElements[GetUIElementByName("Status")] = (new UIText("Status", $"cylce {game.cycleNumber}!\n", 10, 2, true));
        }
        public override void Start(int x = 30, int y = 10)
        {
            UIElements.Clear();
            Console.Clear();
            Console.CursorVisible = false;

            autoCycleMode = false;
            if (x > 0 && x < System.Console.WindowWidth - 10 & y > 0 && y < System.Console.WindowHeight - 10) gameLogic.StartGame(x, y);
            else gameLogic.StartGame(30, 10);

            UIElements.Add(new UILogo("Logo", "Logo.txt", 5, 1, 88, 3));
            UIElements.Add(new UIText("Titel", "(c) by TobiH ", 99, 3));
            UIElements.Add(new UIText("Status", $"cylce #: {gameLogic.cycleNumber}", 10, 0, true));
            
            UIElements.Add(new UIInput("X", "X", 5, gameLogic.height + 6, true, Next));
            UIElements.Add(new UIInput("Y", "Y", 15, gameLogic.height + 6, true, Restart));

            UIElements.Add(new UIButton("New",    "[R] Restart", 5, gameLogic.height + 8, true, Restart));
            UIElements.Add(new UIButton("Toggle", "[ ] Toggle ", 19, gameLogic.height + 8, true, Toggle));
            UIElements.Add(new UIButton("Cycle",  "[C] Cycle  ", 33, gameLogic.height + 8, true, Cycle));
            UIElements.Add(new UIButton("Auto",   "[A] Auto   ", 47, gameLogic.height + 8, true, () => { autoCycleMode = !autoCycleMode;}));
            UIElements.Add(new UIButton("Messy",  "[M] Messy  ", 61, gameLogic.height + 8, true, Messy));
            UIElements.Add(new UIButton("Load",  "[L] Load  ", 75, gameLogic.height + 8, true, Load));
            UIElements.Add(new UIButton("Save",  "[S] Save  ", 89, gameLogic.height + 8, true, Save));
            UIElements.Add(new UIButton("Exit",   "[ESC] Exit ", 103, gameLogic.height + 8, true, Escape));

            UIElements.Add(new UIField("Field", "GameOfLife", 5,5, gameLogic.fieldAB[gameLogic.currentField ? 1 : 0], gameLogic.width, gameLogic.height));

            for (byte yb = 0; yb < gameLogic.height; yb++)
            {
                for (byte xb = 0; xb < gameLogic.width; xb++)
                {
                    UIElements.Add(new UIButton($"Button {xb},{yb}", "", 5 + xb, 5 + yb, true, Toggle));
                }
            }

            cursor = new UICursor("Cursor", " ", 5, 5, true, () => { Click(cursor.x, cursor.y); });

            activeElement = 3;
        }
        public override void Update()
        {
            Draw();
            cursor.Draw();
            AutoCycle();
            // Debug - Ausgabe von Infos
            //Console.SetCursorPosition(50, 24);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Up).ToString() + " ");
            //Console.SetCursorPosition(50, 26);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Down).ToString() + " ");
            //Console.SetCursorPosition(47, 25);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Left).ToString() + " ");
            //Console.SetCursorPosition(53, 25);
            //Console.WriteLine(" " + FindNextUIElement(Direction.Right).ToString() + " ");
            //

            if (Console.KeyAvailable)
            {
                UserInput = Console.ReadKey(true);

                switch (UserInput.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursorMode) cursor.Move(Direction.Up);
                        else activeElement = FindNextUIElement(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorMode) cursor.Move(Direction.Down);
                        else activeElement = FindNextUIElement(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorMode) cursor.Move(Direction.Left);
                        else activeElement = FindNextUIElement(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorMode) cursor.Move(Direction.Right);
                        else activeElement = FindNextUIElement(Direction.Right);
                        break;
                    case ConsoleKey.Enter:
                        if (cursorMode) cursor.Action();
                        else UIElements[activeElement].Action();
                        break;
                    case ConsoleKey.Spacebar:
                        if (cursorMode) cursor.Action();
                        else UIElements[activeElement].Action();
                        break;
                    case ConsoleKey.Tab:
                        cursorMode = !cursorMode;
                        break;
                    case ConsoleKey.A:
                        autoCycleMode = !autoCycleMode;
                        break;
                    case ConsoleKey.C:
                        Cycle();
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
                        Program.Scenes.Pop();
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
        public void Click(int x, int y) // TODO: einbauen
        {
            gameLogic.TogglePosition(x - 5, y - 5);
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

            gameLogic.cycleNumber = gobject.cycle;
            Start(gobject.width, gobject.height);

            for (int y = 0; y < gobject.height; y++)
            {
                for (int x = 0; x < gobject.width; x++)
                {
                    gameLogic.fieldAB[0][y, x] = gobject.field[i];
                    i++;
                }
            }
            
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
        public int FindNextUIElement(Direction direction) // TODO: optimize
        {
            UIObject active = UIElements[_activeElement];
            int found = -1;
            double foundDistance = 9999;

            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].visible && UIElements[i].selectable)
                {
                    if (direction == Direction.Up && UIElements[i].y < active.y && UIElements[i].x == active.x && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Down && UIElements[i].y > active.y && UIElements[i].x == active.x && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Left && UIElements[i].x < active.x && UIElements[i].y == active.y && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Right && UIElements[i].x > active.x && UIElements[i].y == active.y && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                }
            }
            if (found != -1) return found;

            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].visible && UIElements[i].selectable)
                {
                    if (direction == Direction.Up && UIElements[i].y < active.y && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Down && UIElements[i].y > active.y && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Left && UIElements[i].x < active.x && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                    else if (direction == Direction.Right && UIElements[i].x > active.x && DistanceTo(UIElements[i]) < foundDistance)
                    {
                        found = i;
                        foundDistance = DistanceTo(UIElements[i]);
                    }
                }
            }
            return found;
        }
        public int GetUIElementByName(string name)
        {
            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].name == name) return i;
            }
            return -1;
        }
        public double DistanceTo(UIObject uobject)
        {
            return Math.Sqrt(Math.Pow(uobject.x - UIElements[_activeElement].x, 2) + Math.Pow((uobject.y - UIElements[_activeElement].y) * 2, 2)); // * 2 Hack, vertikale Distanz optisch grösser als rechnerische Distanz
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
            gameLogic.status = Status.Stopped;
        }
        public void Restart()
        {
            gameLogic.cycleNumber = 1;
            int x;
            int y;
            if (int.TryParse(UIElements[GetUIElementByName("X")].input, out x) &&
                int.TryParse(UIElements[GetUIElementByName("Y")].input, out y))
                Start(x, y);
            else
                Start();
        }

        public override void Draw()
        {
            fpsCounter.Draw();

            for (int i = 0; i < UIElements.Count; i++)
            {
                UIElements[i].Draw();
            }
        }
    }
}
