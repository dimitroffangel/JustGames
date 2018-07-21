using System;
using TowerDefence.Objects.Turrets;
using TowerDefence.Objects.Turrets.FlameTurrets;

namespace TowerDefence
{
    abstract class HumanInterface : ISpawnObject
    {
       public HumanInterface(ref SetUpVariables variables)
            :base(ref variables)
        {

        }


        public void MovePlayer(ConsoleKeyInfo userInput)
        {
            Console.SetCursorPosition(internal_Variables.PlayerCol, internal_Variables.PlayerRow);
            Console.Write(" ");


            char symbol = '^';
            int currentPlayerX = internal_Variables.PlayerCol;
            int currentPlayerY = internal_Variables.PlayerRow;

            if (userInput.Key == ConsoleKey.W)
            {
                internal_Variables.PlayerRow--;
                // do not change let it be default symbol
            }

            else if (userInput.Key == ConsoleKey.A)
            {
                internal_Variables.PlayerCol--;
                symbol = '<';
            }

            else if (userInput.Key == ConsoleKey.D)
            {
                internal_Variables.PlayerCol++;
                symbol = '>';
            }

            else if (userInput.Key == ConsoleKey.S)
            {
                internal_Variables.PlayerRow++;
                symbol = 'v';
            }

            if (IsPlayerCollidingWithEnemy(0, 0))
                internal_Variables.IsGameOver = true;

            else if (internal_Variables.PlayerCol >= Console.WindowWidth)
                internal_Variables.PlayerCol--;

            else if (internal_Variables.PlayerCol < 0)
                internal_Variables.PlayerCol++;

            else if (internal_Variables.PlayerRow >= Console.WindowHeight)
                internal_Variables.PlayerRow--;

            else if (internal_Variables.PlayerRow < 0)
                internal_Variables.PlayerRow++;

            // supervises if the player is colliding with a turret, if so, the movement must be abandoned 
            foreach(var turretPosition in internal_Variables.TurretsPosition)
            {
                if(turretPosition.X == internal_Variables.PlayerCol && turretPosition.Y == internal_Variables.PlayerRow)
                {
                    // return the player x and y to the initial values
                    internal_Variables.PlayerCol = currentPlayerX;
                    internal_Variables.PlayerRow = currentPlayerY;
                    break;
                }
            }

            // same if the player is colliding with an obstacle
            foreach(var obstaclePosition in internal_Variables.ObstaclePositions)
            {
                if(obstaclePosition.X == internal_Variables.PlayerCol && obstaclePosition.Y == internal_Variables.PlayerRow)
                {
                    internal_Variables.PlayerCol = currentPlayerX;
                    internal_Variables.PlayerRow = currentPlayerY;
                    break;
                }
            }

            // obstacles are spawned every time a player moves, this is the reason to call the function here
            this.SpawnObstacles();

            Console.SetCursorPosition(internal_Variables.PlayerCol, internal_Variables.PlayerRow);
            Console.Write(symbol);

            // change the player view
            internal_Variables.PlayerView = symbol;
        }

        public void ReadInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey(true);

                if (userInput.Key == ConsoleKey.A || userInput.Key == ConsoleKey.D ||
                   userInput.Key == ConsoleKey.W || userInput.Key == ConsoleKey.S)
                    MovePlayer(userInput);

                else if (userInput.Key == ConsoleKey.P || userInput.Key == ConsoleKey.O ||
                         userInput.Key == ConsoleKey.I || userInput.Key == ConsoleKey.U)
                {
                    char turretSymbol = ' ';
                    TurretType newTurretType = TurretType.Cannon;

                    // I want to build the turret in front of the player so I need to oversee his cursor
                    int newTurretX = internal_Variables.PlayerCol;
                    int newTurretY = internal_Variables.PlayerRow;
                    
                    if (internal_Variables.PlayerView == 'v')
                        newTurretY++;
                    else if (internal_Variables.PlayerView == '^')
                        newTurretY--;
                    else if (internal_Variables.PlayerView == '<')
                        newTurretX--;
                    else if (internal_Variables.PlayerView == '>')
                        newTurretX++;

                    if (newTurretX < 0 || newTurretX >= Console.WindowWidth ||
                        newTurretY < 0 || newTurretY >= Console.WindowHeight)
                        return;

                    TurretPlacement placeOn = DeterminePlayerPosition();

                    if (internal_Variables.PocketMoney >= 50 && userInput.Key == ConsoleKey.P)
                    {
                        turretSymbol = '*';
                        // the turret type is the already defined one
                        internal_Variables.PocketMoney -= 50;
                    }

                    else if (internal_Variables.PocketMoney >= 340 && userInput.Key == ConsoleKey.O)
                    {
                        turretSymbol = '&';
                        newTurretType = TurretType.FireBunker_Hellion;
                        internal_Variables.PocketMoney -= 340;
                    }

                    else if (internal_Variables.PocketMoney >= 165 && userInput.Key == ConsoleKey.I)
                    {
                        turretSymbol = '%';
                        newTurretType = TurretType.Tardus;
                        internal_Variables.PocketMoney -= 165;
                    }

                    else if (internal_Variables.PocketMoney >= 500 && userInput.Key == ConsoleKey.U)
                    {
                        turretSymbol = '#';
                        newTurretType = TurretType.LaserAnnihilator;
                        internal_Variables.PocketMoney -= 500;
                    }

                    foreach (var obstaclePosition in internal_Variables.ObstaclePositions)
                    {
                        if (obstaclePosition.X == newTurretX && obstaclePosition.Y == newTurretY)
                           return;
                    }

                    Console.SetCursorPosition(newTurretX, newTurretY);
                    Console.Write(turretSymbol);

                    if (userInput.Key == ConsoleKey.P || userInput.Key == ConsoleKey.U)
                        internal_Variables.TurretsPosition.Add(new Cannon(newTurretX, newTurretY, newTurretType, placeOn, ref internal_Variables));
                    else if (userInput.Key == ConsoleKey.I)
                        internal_Variables.TurretsPosition.Add(new Tardus(newTurretX, newTurretY, newTurretType, placeOn, ref internal_Variables));
                    else if (userInput.Key == ConsoleKey.O)
                    {
                        var turret = new Hellion(newTurretX, newTurretY, newTurretType, placeOn, ref internal_Variables);

                       /* // else no sense to care for the newly created gadget 
                        for (int j = 0; j < internal_Variables.Battleground.Count; j++)
                        {
                            var curPosition = internal_Variables.Battleground[j];
                            if (curPosition.Uniq_X == newTurretX && curPosition.Uniq_Y == newTurretY)
                            {
                                internal_Variables.FireTrappers.Add(trap);
                                break;
                            }
                        }
                        */
                        internal_Variables.Hellions.Add(turret);
                        internal_Variables.TurretsPosition.Add(turret);
                    }
                }
            }
        }

    }
}
