using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GameOfLife
{
    class GameScene : Scene // TODO: allow window resize
    {
        Random rnd = new Random();
        public GameLogic gameLogic;
        DateTime lastUpdate = DateTime.Now;
        DateTime keyDown; int addX, addY;
        bool autoCycleMode = false;
        int _cycleDelay = 500;
        int randomCells = 50;
        public GameScene(GameLogic gameLogic = null)
        {
            if (gameLogic == null) this.gameLogic = new GameLogic();
            else this.gameLogic = gameLogic;
            Start();
        }
        public int cycleDelay
        {
            get
            {
                return _cycleDelay;
            }
            set
            {
                if (value >= 0 && value <= 5000)
                    _cycleDelay = value;
            }
        }
        public override void Start()
        {
            UIElements.Clear();
            ConsoleClear();

            logo = new UILogo("Logo", "Logo.txt", 5, 1, 88, 3);
            UIElements.Add(logo);

            UIElements.Add(new UIText("Status", $"cycle #: {gameLogic.cycleNumber}", 10, 0, true, ConsoleColor.DarkGray));

            UIElements.Add(new UIInput("X", "X-Size", 5, gameLogic.height + 6, gameLogic.width.ToString(), true, () => { activeElement = GetUIElementIDByName("Y"); }));
            UIElements.Add(new UIInput("Y", "Y-Size", 20, gameLogic.height + 6, gameLogic.height.ToString(), true, Restart));
            UIElements.Add(new UIInput("Random", "Random %", 35, gameLogic.height + 6, randomCells.ToString(), true, () => Random()));

            UIElements.Add(new UIButton("Empty",    "[  E Empty  ]", 5, gameLogic.height + 8, true, Restart));
            UIElements.Add(new UIButton("Cycle",    "[  C Cycle  ]", 19, gameLogic.height + 8, true, Cycle));
            UIElements.Add(new UIButton("Auto",     "[  A Auto   ]", 33, gameLogic.height + 8, true, () => { autoCycleMode = !autoCycleMode;}));
            UIElements.Add(new UIButton("Random",   "[ R Random  ]", 47, gameLogic.height + 8, true, Random));
            UIElements.Add(new UIButton("LoadSave", "[D Load/Save]", 61, gameLogic.height + 8, true, () => { Program.Scenes.Push(new LoadAndSaveScene(ref gameLogic)); }));
            UIElements.Add(new UIButton("Load",     "[L QuickLoad]", 75, gameLogic.height + 8, true, Load));
            UIElements.Add(new UIButton("Save",     "[S QuickSave]", 89, gameLogic.height + 8, true, Save));
            UIElements.Add(new UIButton("Quit",     "[ ESC Quit  ]", 103, gameLogic.height + 8, true, Quit));

            field = new UIField("Field", "GameOfLife", 5, 5, gameLogic.fieldAB[gameLogic.currentField ? 1 : 0], gameLogic.width, gameLogic.height);
            UIElements.Add(field);

            cursor = new UICursor("Cursor", " ", 5, 5, gameLogic.width, gameLogic.height, true, () => { Click(cursor.fieldX, cursor.fieldY); }, () => { field.DrawPosition(cursor.fieldX, cursor.fieldY); });
            UIElements.Add(cursor);

            UIElements.Add(new UIText("Info", $"use cursor keys and press/ hold space to drop some cells", 60, gameLogic.height + 6, true));

            DrawUIElements();

            activeElement = 2;
        }
        public override void Update()
        {
            Draw();
            field.Draw();
            logo.DrawEffect();
            CursorBlink();
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
                        if (cursor.cursorMode && cursor.fieldY == cursor.fieldMaxY - 1) { activeElement = 3; field.DrawPosition(cursor.fieldX, cursor.fieldY); }
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
                    case ConsoleKey.D:
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
                    case ConsoleKey.T:
                        field.colorMode++;
                        break;
                    case ConsoleKey.PageUp:
                        field.effectDelay += 10;
                        break;
                    case ConsoleKey.PageDown:
                        field.effectDelay -= 10;
                        break;
                    case ConsoleKey.Add:
                        cycleDelay += 50;
                        break;
                    case ConsoleKey.Subtract:
                        cycleDelay -= 50;
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
                        if (UserInput.KeyChar == '-') cycleDelay -= 50;
                        if (UserInput.KeyChar == '+') cycleDelay += 50;
                        break;
                }
            }
        }
        public void Click(int x, int y)
        {
            gameLogic.TogglePosition(x, y);
            if ((DateTime.Now - keyDown).TotalMilliseconds <= 50)
            {
                gameLogic.TogglePosition(x - addX++ / 40 + rnd.Next(0, addX++/20), y - addY++ / 40 + rnd.Next(0, addY++/20));
            }
            else
            {
                addX = 0; addY = 0;
            }
            keyDown = DateTime.Now;
        }
        public void Load()
        {
            LoadGame("_quicksave.xml");
        }
        public void Save()
        {
            SaveGame("_quicksave.xml");
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
                if ((DateTime.Now - lastUpdate).TotalMilliseconds >= cycleDelay)
                {
                    Cycle();
                    lastUpdate = DateTime.Now;
                }
            }
        }
        public void CursorBlink()
        {
            if (!cursor.cursorMode)
            {
                //if (System.DateTime.Now.Second % 2 == 0) cursor.visible = true;
                //else cursor.visible = false;
                //Program.DrawUpdates.Add(cursor);
            }
        }
        public void Random()
        {
            Random rnd = new Random();

            if (int.TryParse(GetUIElementByName("Random").input, out randomCells)) { }
            else { randomCells = 50; field.input = randomCells.ToString(); }

            for (byte y = 0; y < gameLogic.height; y++)
            {
                for (byte x = 0; x < gameLogic.width; x++)
                {
                    if (rnd.Next(0, 101) < randomCells)
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
            field.Set(gameLogic.fieldAB[gameLogic.currentField ? 1 : 0]);
            GetUIElementByName("Status").text = $"cycle #: {gameLogic.cycleNumber}  cycleDelay +|-: {cycleDelay}  effectDelay PGU|PGD: {field.effectDelay}  colorMode T: {field.colorMode}";
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
