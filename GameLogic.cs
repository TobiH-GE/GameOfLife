using System.Collections.Generic;

namespace GameOfLife
{
    public class GameLogic
    {
        public bool currentField = false;
        public int cycleNumber = 1;
        public int width = 100;
        public int height = 20;
        public List<bool[,]> fieldAB;
        public Status status;

        public GameLogic (int width = 100, int height = 20, bool[,] fieldA = null, int cycleNumber = 1)
        {
            if (width > System.Console.WindowWidth - 11) width = System.Console.WindowWidth - 11;
            if (height > System.Console.WindowHeight - 9) height = System.Console.WindowHeight - 9;
            if (width > 0 && width < System.Console.WindowWidth - 10 & height > 0 && height < System.Console.WindowHeight - 8)
            {
                this.width = width;
                this.height = height;
            }

            status = Status.Started;
            this.cycleNumber = cycleNumber;
            fieldAB = new List<bool[,]>() { new bool[height, width], new bool[height, width] };
            if (fieldA != null) fieldAB[0] = fieldA;
        }
        public void NextCycle()
        {
            int livingNeighbours = 0;

            for (int y = 0; y < fieldAB[currentField ? 1 : 0].GetLength(0); y++)
            {
                for (int x = 0; x < fieldAB[currentField ? 1 : 0].GetLength(1); x++)
                {
                    livingNeighbours = 0;
                    for (int y1 = -1; y1 < 2; y1+=2) // alle Positionen ausser x, y
                    {
                        for (int x1 = -1; x1 < 2; x1++)
                        {
                            if (GetPosition(x + x1, y + y1)) livingNeighbours++;
                        }
                        if (GetPosition(x + y1, y)) livingNeighbours++;
                    }
                    if (livingNeighbours == 3) SetPosition(x, y);
                    else if (fieldAB[currentField ? 1 : 0][y, x] && livingNeighbours == 2) SetPosition(x, y);
                    else SetPosition(x, y, false);
                }
            }
            currentField = !currentField;
            cycleNumber++;
        }
        public bool GetPosition(int x, int y)
        {
            if (x < 0 || x >= fieldAB[0].GetLength(1) ||
                y < 0 || y >= fieldAB[0].GetLength(0))
                
                return false;

            else return fieldAB[currentField ? 1 : 0][y, x];
        }
        public void TogglePosition(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                fieldAB[currentField ? 1 : 0][y, x] = !fieldAB[currentField ? 1 : 0][y, x];
        }
        public void SetPosition(int x, int y, bool l = true)
        {
            fieldAB[!currentField ? 1 : 0][y, x] = l;
        }
    }
}
