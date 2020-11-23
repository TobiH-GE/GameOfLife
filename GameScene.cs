using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GameOfLife
{
    class GameScene : Scene
    {
        public List<UIObject> UIElements = new List<UIObject>();
        public GameLogic game = new GameLogic();
        FPS fpsCounter = new FPS();
        DateTime lastUpdate = DateTime.Now;
        bool autoCycleMode = false;
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
            
            game.StartGame(x, y);

            UIElements.Add(new UILogo("Logo", "Logo.txt", 5, 1, 88, 3));
            UIElements.Add(new UIText("Titel", "(c) by TobiH ", 99, 3));
            UIElements.Add(new UIText("Status", $"cylce #: {game.cycleNumber}", 10, 0, true));
            
            UIElements.Add(new UIInput("X", "X", 5, game.height + 6, true, Next));
            UIElements.Add(new UIInput("Y", "Y", 15, game.height + 6, true, Restart));

            UIElements.Add(new UIButton("New",    "[R] Restart", 5, game.height + 8, true, Restart));
            UIElements.Add(new UIButton("Toggle", "[ ] Toggle ", 20, game.height + 8, true, Toggle));
            UIElements.Add(new UIButton("Cycle",  "[C] Cycle  ", 35, game.height + 8, true, Cycle));
            UIElements.Add(new UIButton("Auto",   "[A] Auto   ", 50, game.height + 8, true, () => { autoCycleMode=!autoCycleMode; return true; }));
            UIElements.Add(new UIButton("Messy",  "[M] Messy  ", 65, game.height + 8, true, Messy));
            UIElements.Add(new UIButton("Load",  "[L] Load  ", 80, game.height + 8, true, Load));
            UIElements.Add(new UIButton("Save",  "[S] Save  ", 95, game.height + 8, true, Save));
            UIElements.Add(new UIButton("Exit",   "[ESC] Exit ", 110, game.height + 8, true, Exit));
            
            UIElements.Add(new UIField("Field", "GameOfLife", 5,5, game.fieldAB[game.currentField ? 1 : 0], game.width, game.height));

            for (byte yb = 0; yb < game.height; yb++)
            {
                for (byte xb = 0; xb < game.width; xb++)
                {
                    UIElements.Add(new UIButton($"Button {xb},{yb}", "", 5 + xb, 5 + yb, true, Toggle));
                }
            }

            activeElement = 3;
        }
        public override void Update()
        {
            Draw();
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
                    case ConsoleKey.Spacebar:
                        UIElements[activeElement].Action();
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
        public bool Load()
        {
            LoadGame("savegame.xml");
            return true;
        }
        public bool Save()
        {
            SaveGame("savegame.xml");
            return true;
        }
        public void LoadGame(string file) //TODO: Load / Save / Restart mit dem richtigen, aktiven Feld (Bug)
        {
            GameObject gobject = new GameObject();

            XmlSerializer serializer = new XmlSerializer(typeof(GameObject));
            int i = 0;

            using (Stream load = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                gobject = (GameObject)serializer.Deserialize(load);
            }

            autoCycleMode = false;
            game.currentField = false;
            game.fieldAB.Clear();
            game.cycleNumber = gobject.cycle;
            Start(gobject.width, gobject.height);

            for (int y = 0; y < gobject.height; y++)
            {
                for (int x = 0; x < gobject.width; x++)
                {
                    game.fieldAB[0][y, x] = gobject.field[i];
                    i++;
                }
            }
            
        }
        public void SaveGame(string file)
        {
            GameObject gobject = new GameObject();
            gobject.name = "Test";
            gobject.cycle = game.cycleNumber;
            gobject.width = game.width;
            gobject.height = game.height;
            gobject.datetime = DateTime.Now;
            gobject.field = new bool[game.height * game.width];
            int i = 0;
            for (int y = 0; y < game.height; y++)
            {
                for (int x = 0; x < game.width; x++)
                {
                    gobject.field[i] = game.fieldAB[game.currentField ? 1 : 0][y, x];
                    i++;
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(GameObject));

            using (Stream save = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(save, gobject);
            }
        }
        public void AutoCycle()
        {
            if (autoCycleMode)
            {
                if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 1000)
                {
                    Cycle();
                    lastUpdate = DateTime.Now;
                }
            }
        }
        public int FindNextUIElement(Direction direction)
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
        public bool Toggle()
        {
            game.TogglePosition((UIElements[_activeElement].x - 5), (UIElements[_activeElement].y - 5));
            return true;
        }
        public bool Messy()
        {
            Random rnd = new Random();
            for (byte y = 0; y < game.height; y++)
            {
                for (byte x = 0; x < game.width; x++)
                {
                    if (rnd.Next(0, 2) == 1)
                        game.TogglePosition(x, y);
                }
            }
            return true;
        }
        public bool Next()
        {
            activeElement = FindNextUIElement(Direction.Down);
            return true;
        }
        public bool Cycle()
        {
            game.NextCycle();
            UIElements[GetUIElementByName("Field")].field = game.fieldAB[game.currentField ? 1 : 0];
            UIElements[GetUIElementByName("Status")].text = $"cylce #: {game.cycleNumber}";
            return true;
        }
        public bool Exit()
        {
            game.status = Status.Stopped;
            return true;
        }
        public bool Restart()
        {
            int x;
            int y;
            game.ResetGame();
            if (int.TryParse(UIElements[GetUIElementByName("X")].input, out x) &&
                int.TryParse(UIElements[GetUIElementByName("Y")].input, out y))
                Start(x, y);
            else
                Start();
            return true;
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
