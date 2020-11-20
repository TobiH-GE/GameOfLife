using System.Collections.Generic;

namespace GameOfLife
{
    partial class Game
    {
        public bool[,] fieldA;
        public bool[,] fieldB;
        public List<bool[,]> fieldAB = new List<bool[,]>();
        public byte cycleNumber = 1;
        public UI UIGame;               // UserInterface des Spiels
        public Status status;
        public int currentField = 0;
        public int xsize = 6;
        public int ysize = 6;

        public Game (UI uigame)
        {
            UIGame = uigame;
            UIGame.game = this;
            UIGame.Start();         // UserInterface starten
        }
        public bool[,] GetField() // noch keine Verwendung
        {
            return fieldA;
        }
        public void StartGame()
        {
            status = Status.Started;

            SetFieldSize(xsize, ysize);

            fieldAB.Add(fieldA);
            fieldAB.Add(fieldB);
        }
        public void NextCycle()
        {
            int livingNeighbours = 0;

            for (int y = 0; y < fieldAB[0].GetLength(0); y++)
            {
                for (int x = 0; x < fieldAB[0].GetLength(1); x++)
                {
                    livingNeighbours = 0;
                    for (int y1 = -1; y1 < 2; y1++)
                    {
                        for (int x1 = -1; x1 < 2; x1++)
                        {
                            if (!(x + x1 == x && y + y1 == y))
                            {
                                if (GetPosition(x + x1, y + y1))
                                    livingNeighbours++;
                            }
                        }
                    }
                    if (fieldAB[0][y, x]) // Zelle mit Leben
                    {
                        if (livingNeighbours < 2)
                            DiePosition(x, y);
                        else if (livingNeighbours == 2 || livingNeighbours == 3)
                            LiveToPosition(x, y);
                        else
                            DiePosition(x, y);
                    }
                    else
                    {
                        if (livingNeighbours == 3)
                            LiveToPosition(x, y);
                        else
                            DiePosition(x, y);
                    }
                }
            }
            for (int y = 0; y < fieldAB[0].GetLength(0); y++)
            {
                for (int x = 0; x < fieldAB[0].GetLength(1); x++)
                {
                    fieldA[y, x] = fieldB[y, x];
                    fieldB[y, x] = false;
                }
            }
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

            else return fieldA[y, x];
        }
        public void TogglePosition(int x, int y)
        {
            fieldAB[0][y, x] = !fieldAB[0][y, x];
        }
        public void LiveToPosition(int x, int y)
        {
            fieldAB[1][y, x] = true;
        }
        public void DiePosition(int x, int y)
        {
            fieldAB[1][y, x] = false;
        }

        public void ResetGame()
        {
            
        }   
    }
}
