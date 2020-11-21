using System;
using System.Collections.Generic;

namespace GameOfLife
{
    class UIConsole : UI
    {
        public List<UIObject> UIElements = new List<UIObject>();
        
        FPS fpsCounter = new FPS();
        Point input = new Point();
        ConsoleKeyInfo UserInput = new ConsoleKeyInfo();
        ConsoleColor[] pColor = new ConsoleColor[2] { ConsoleColor.Red, ConsoleColor.Blue };
        private int activeElement;
        public int ActiveElement
        {
            get
            {
                return activeElement;
            }
            set
            {
                if (value >= UIElements.Count) value = 0;
                if (value < 0) value = UIElements.Count - 1;

                UIElements[activeElement].selected = false;
                activeElement = value;
                UIElements[activeElement].selected = true;
            }
        }

        public override void PrintStatus(ref Game game)
        {
            UIElements[GetUIElementByName("Status")] = (new UIText("Status", $"cylce {game.cycleNumber}!\n", 10, 2, true));
        }
        public override void PrintError(string str)
        {
            UIElements[GetUIElementByName("Error")].text = str;
        }
        public override void PrintInfo(string str)
        {
            UIElements[GetUIElementByName("Hint")].text = str;
        }
        public override void Start()
        {
            Console.Clear();
            Console.CursorVisible = false;
            game.StartGame(game.xsize, game.ysize);

            UIElements.Add(new UILogo("Logo", "Logo.txt", 5, 1, 96, 7));
            UIElements.Add(new UIText("Titel", "(c) by TobiH ", 99, 7));
            UIElements.Add(new UIText("Status", $"cylce #: {game.cycleNumber}", 5, 22, true));
            

            UIElements.Add(new UIText("Info", "[SPACE] toggle [C] cycle [A] auto [<>^v] nav [R] restart [ESC] exit", 5, 23));
            UIElements.Add(new UIText("Error", "", 20, 24));

            UIElements.Add(new UIInput("X", "X", 5, 26, true, Next));
            UIElements.Add(new UIInput("Y", "Y", 5, 27, true, Restart));

            UIElements.Add(new UIButton("Ok", "OK", 20, 28, true, Ok));
            UIElements.Add(new UIButton("Exit", "Exit", 30, 28, true, Exit));
            

            UIElements.Add(new UIField("Field", "GameOfLife", 10, 10, game.fieldAB[game.currentField ? 1 : 0], game.xsize, game.ysize));

            for (byte y = 0; y < game.ysize; y++)
            {
                for (byte x = 0; x < game.xsize; x++)
                {
                    UIElements.Add(new UIButton($"Button {x},{y}", "", 10 + x, 10 + y, true, Toggle));
                }
            }

            activeElement = 4;
        }
        public override void WaitForInput()
        {
            Draw();

            // Debug - Ausgabe von Infos
            Console.SetCursorPosition(50, 24);
            Console.WriteLine(" " + FindNextUIElement(Direction.Up).ToString() + " ");
            Console.SetCursorPosition(50, 26);
            Console.WriteLine(" " + FindNextUIElement(Direction.Down).ToString() + " ");
            Console.SetCursorPosition(47, 25);
            Console.WriteLine(" " + FindNextUIElement(Direction.Left).ToString() + " ");
            Console.SetCursorPosition(53, 25);
            Console.WriteLine(" " + FindNextUIElement(Direction.Right).ToString() + " ");
            //

            if (Console.KeyAvailable)
            {
                UserInput = Console.ReadKey(true);

                switch (UserInput.Key)
                {
                    case ConsoleKey.UpArrow:
                        ActiveElement = FindNextUIElement(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        ActiveElement = FindNextUIElement(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        ActiveElement = FindNextUIElement(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        ActiveElement = FindNextUIElement(Direction.Right);
                        break;
                    case ConsoleKey.C:
                        Ok();
                        break;
                    case ConsoleKey.Enter:
                        UIElements[ActiveElement].Action();
                        break;
                    case ConsoleKey.Spacebar:
                        UIElements[ActiveElement].Action();
                        break;
                    case ConsoleKey.R:
                        Restart();
                        break;
                    case ConsoleKey.Escape:
                        game.status = Status.Stopped;
                        break;
                    case ConsoleKey.Backspace:
                        UIElements[ActiveElement].input = UIElements[ActiveElement].input.Remove(UIElements[ActiveElement].input.Length - 1);
                        break;
                    default:
                        if (Char.IsNumber(UserInput.KeyChar))
                        {
                            UIElements[ActiveElement].input += UserInput.KeyChar;
                        }
                        break;
                }
            }
        }
        public int FindNextUIElement(Direction direction)
        {
            UIObject active = UIElements[activeElement];
            int found = -1;
            double foundDistance = 9999;

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
            return Math.Sqrt(Math.Pow(uobject.x - UIElements[activeElement].x, 2) + Math.Pow((uobject.y - UIElements[activeElement].y) * 2, 2)); // * 2 Hack, vertikale Distanz optisch grösser als rechnerische Distanz
        }
        public bool Toggle()
        {
            game.TogglePosition((UIElements[activeElement].x - 10), (UIElements[activeElement].y - 10));
            return true;
        }
        public bool Next()
        {
            ActiveElement = FindNextUIElement(Direction.Down);
            return true;
        }
        public bool Ok()
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
            int.TryParse(UIElements[GetUIElementByName("X")].input, out game.xsize);
            int.TryParse(UIElements[GetUIElementByName("Y")].input, out game.ysize);
            UIElements.Clear();
            game.ResetGame();
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
