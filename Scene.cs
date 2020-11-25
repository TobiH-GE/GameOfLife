using System;
using System.Collections.Generic;

namespace GameOfLife
{
    abstract class Scene
    {
        public List<UIObject> UIElements = new List<UIObject>();
        public UIObject cursor;
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

                UIElements[_activeElement].selected = false;
                if (UIElements[value].selectable)
                {
                    _activeElement = value;
                    UIElements[_activeElement].selected = true;
                }
                if (activeElement == GetUIElementByName("Cursor")) cursor.cursorMode = true;
                else cursor.cursorMode = false;
            }
        }
        public int GetUIElementByName(string name)
        {
            for (int i = 0; i < UIElements.Count; i++)
            {
                if (UIElements[i].name == name) return i;
            }
            return -1;
        }
        public abstract void PrintStatus(ref GameLogic game);
        public abstract void Start();
        public abstract void Update();
        public abstract void Draw();
    }
}
