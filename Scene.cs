using System;
using System.Collections.Generic;

namespace GameOfLife
{
    abstract class Scene
    {
        public ConsoleKeyInfo UserInput = new ConsoleKeyInfo();
        FPS fpsCounter = new FPS();
        public List<UIObject> UIElements = new List<UIObject>();
        public UILogo logo;
        public UICursor cursor;
        public int _activeElement = 0;
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

                Program.DrawUpdates.Add(UIElements[_activeElement]);
                UIElements[_activeElement].selected = false;
                if (UIElements[value].selectable)
                {
                    _activeElement = value;
                    UIElements[_activeElement].selected = true;
                }
                Program.DrawUpdates.Add(UIElements[_activeElement]);
                if (activeElement == GetUIElementIDByName("Cursor")) cursor.cursorMode = true;
                else cursor.cursorMode = false;
            }
        }
        public void ConsoleClear()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.CursorVisible = false;
        }
        public int GetUIElementIDByName(string name)
        {
            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].name == name) return i;
            }
            return -1;
        }
        public UIObject GetUIElementByName(string name)
        {
            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].name == name) return UIElements[i];
            }
            return null;
        }
        public UIObject GetUIElementByID(int id)
        {
            return UIElements[id];
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
        public double DistanceTo(UIObject uobject)
        {
            return Math.Sqrt(Math.Pow(uobject.x - UIElements[_activeElement].x, 2) + Math.Pow((uobject.y - UIElements[_activeElement].y) * 2, 2)); // * 2 Hack, vertikale Distanz optisch grösser als rechnerische Distanz
        }
        public abstract void Start();
        public abstract void Update();
        public void Draw()
        {
            fpsCounter.Draw();
        }
    }
}
