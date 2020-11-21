using System.Collections.Generic;

namespace GameOfLife
{
    partial class Game
    {
        public bool[,] fieldA;
        public bool[,] fieldB;
        public List<bool[,]> fieldAB = new List<bool[,]>();
        public int cycleNumber = 1;
        public UI UIGame;               // UserInterface des Spiels
        public Status status;
        public bool currentField = false;
        public int xsize = 30;
        public int ysize = 10;

        public Game (UI uigame)
        {
            UIGame = uigame;
            UIGame.game = this;
            UIGame.Start();         // UserInterface starten
        }
        public bool[,] GetField() // noch keine Verwendung
        {
            return fieldAB[currentField ? 1 : 0];
        }
        public void StartGame(int x, int y)
        {
            status = Status.Started;

            SetFieldSize(x, y); // Feldgrösse

            fieldAB.Add(fieldA);
            fieldAB.Add(fieldB);
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
                        if (livingNeighbours < 2)
                            DiePosition(x, y);
                        else if (livingNeighbours == 2 || livingNeighbours == 3)
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
        public void SetFieldSize(int x, int y)
        {
            fieldA = new bool[y, x];
            fieldB = new bool[y, x];
        }
        public bool GetPosition(int x, int y)
        {
            if (x < 0 || x >= fieldA.GetLength(1) ||
                y < 0 || y >= fieldA.GetLength(0))
                
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

        public void ResetGame()
        {
            cycleNumber = 1;
            currentField = false;
            if (xsize > 0 && xsize < 100 & ysize > 0 && ysize < 100)
                SetFieldSize(xsize, ysize);
            else
            {
                xsize = 30;
                ysize = 10;
            }
            fieldAB.Clear();
        }   
    }
}
