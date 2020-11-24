using System.Collections.Generic;

namespace GameOfLife
{
    public class GameLogic
    {
        public bool currentField = false;
        public int cycleNumber = 1;
        public int width = 30;
        public int height = 10;
        public List<bool[,]> fieldAB;
        public Status status;

        public GameLogic ()
        {

        }
        public void StartGame(int x, int y)
        {
            currentField = false;
            if (fieldAB != null) fieldAB.Clear();
            status = Status.Started;
            width = x;
            height = y;
            fieldAB = new List<bool[,]>() { new bool[y, x], new bool[y, x] };
        }
        public void NextCycle()
        {
            int livingNeighbours = 0;

            for (int y = 0; y < fieldAB[currentField ? 1 : 0].GetLength(0); y++)
            {
                for (int x = 0; x < fieldAB[currentField ? 1 : 0].GetLength(1); x++)
                {
                    livingNeighbours = 0;
                    for (int y1 = -1; y1 < 2; y1++)
                    {
                        for (int x1 = -1; x1 < 2; x1++)
                        {
                            if (!(x1 == 0 && y1 == 0))
                            {
                                if (GetPosition(x + x1, y + y1))
                                    livingNeighbours++;
                            }
                        }
                    }
                    if (fieldAB[currentField ? 1 : 0][y, x]) // Zelle mit Leben
                    {
                        if (livingNeighbours == 2 || livingNeighbours == 3)
                            LifeToPosition(x, y);
                        else
                            DiePosition(x, y);
                    }
                    else // Zelle ohne Leben
                    {
                        if (livingNeighbours == 3)
                            LifeToPosition(x, y);
                        else
                            DiePosition(x, y);
                    }
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
            fieldAB[currentField ? 1 : 0][y, x] = !fieldAB[currentField ? 1 : 0][y, x];
        }
        public void LifeToPosition(int x, int y)
        {
            fieldAB[!currentField ? 1 : 0][y, x] = true;
        }
        public void DiePosition(int x, int y)
        {
            fieldAB[!currentField ? 1 : 0][y, x] = false;
        }
    }
}
