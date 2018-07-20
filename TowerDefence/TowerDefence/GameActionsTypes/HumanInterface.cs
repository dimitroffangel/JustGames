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
            Console.SetCursorPosition(this.Variables.PlayerCol, this.Variables.PlayerRow);
            Console.Write(" ");


            char symbol = '^';
            int currentPlayerX = Variables.PlayerCol;
            int currentPlayerY = Variables.PlayerRow;

            if (userInput.Key == ConsoleKey.W)
            {
                Variables.PlayerRow--;
                // do not change let it be default symbol
            }

            else if (userInput.Key == ConsoleKey.A)
            {
                Variables.PlayerCol--;
                symbol = '<';
            }

            else if (userInput.Key == ConsoleKey.D)
            {
                Variables.PlayerCol++;
                symbol = '>';
            }

            else if (userInput.Key == ConsoleKey.S)
            {
                Variables.PlayerRow++;
                symbol = 'v';
            }

            if (IsPlayerCollidingWithEnemy(0, 0))
                Variables.IsGameOver = true;

            else if (Variables.PlayerCol >= Console.WindowWidth)
                Variables.PlayerCol--;

            else if (Variables.PlayerCol < 0)
                Variables.PlayerCol++;

            else if (Variables.PlayerRow >= Console.WindowHeight)
                Variables.PlayerRow--;

            else if (Variables.PlayerRow < 0)
                Variables.PlayerRow++;

            // supervises if the player is colliding with a turret, if so, the movement must be abandoned 
            foreach(var turretPosition in Variables.TurretsPosition)
            {
                if(turretPosition.Uniq_X == Variables.PlayerCol && turretPosition.Uniq_Y == Variables.PlayerRow)
                {
                    // return the player x and y to the initial values
                    Variables.PlayerCol = currentPlayerX;
                    Variables.PlayerRow = currentPlayerY;
                    break;
                }
            }

            // same if the player is colliding with an obstacle
            foreach(var obstaclePosition in Variables.ObstaclePositions)
            {
                if(obstaclePosition.Uniq_X == Variables.PlayerCol && obstaclePosition.Uniq_Y == Variables.PlayerRow)
                {
                    Variables.PlayerCol = currentPlayerX;
                    Variables.PlayerRow = currentPlayerY;
                    break;
                }
            }

            // obstacles are spawned every time a player moves, this is the reason to call the function here
            this.SpawnObstacles();

            Console.SetCursorPosition(this.Variables.PlayerCol, this.Variables.PlayerRow);
            Console.Write(symbol);

            // change the player view
            Variables.PlayerView = symbol;
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
                    int newTurretX = this.Variables.PlayerCol;
                    int newTurretY = this.Variables.PlayerRow;

                    if (Variables.PlayerView == 'v')
                        newTurretY++;
                    else if (Variables.PlayerView == '^')
                        newTurretY--;
                    else if (Variables.PlayerView == '<')
                        newTurretX--;
                    else if (Variables.PlayerView == '>')
                        newTurretX++;

                    if (newTurretX < 0 || newTurretX >= Console.WindowWidth ||
                        newTurretY < 0 || newTurretY >= Console.WindowHeight)
                        return;

                    TurretPlacement placeOn = DeterminePlayerPosition();

                    if (this.Variables.PocketMoney >= 50 && userInput.Key == ConsoleKey.P)
                    {
                        turretSymbol = '*';
                        // the turret type is the already defined one
                        this.Variables.PocketMoney -= 50;
                    }

                    else if (this.Variables.PocketMoney >= 340 && userInput.Key == ConsoleKey.O)
                    {
                        turretSymbol = '&';
                        newTurretType = TurretType.FireBunker_Hellion;
                        this.Variables.PocketMoney -= 340;
                    }

                    else if (this.Variables.PocketMoney >= 165 && userInput.Key == ConsoleKey.I)
                    {
                        turretSymbol = '%';
                        newTurretType = TurretType.Tardus;
                        this.Variables.PocketMoney -= 165;
                    }

                    else if (this.Variables.PocketMoney >= 500 && userInput.Key == ConsoleKey.U)
                    {
                        turretSymbol = '#';
                        newTurretType = TurretType.LaserAnnihilator;
                        this.Variables.PocketMoney -= 500;
                    }

                    foreach (var obstaclePosition in Variables.ObstaclePositions)
                    {
                        if (obstaclePosition.Uniq_X == newTurretX && obstaclePosition.Uniq_Y == newTurretY)
                           return;
                    }

                    Console.SetCursorPosition(newTurretX, newTurretY);
                    Console.Write(turretSymbol);

                    if (userInput.Key == ConsoleKey.P || userInput.Key == ConsoleKey.U)
                        this.Variables.TurretsPosition.Add(new Cannon(newTurretX, newTurretY, newTurretType, placeOn, ref this.Variables));
                    else if (userInput.Key == ConsoleKey.I)
                        this.Variables.TurretsPosition.Add(new Tardus(newTurretX, newTurretY, newTurretType, placeOn, ref this.Variables));
                    else if (userInput.Key == ConsoleKey.O)
                    {
                        var turret = new Hellion(newTurretX, newTurretY, newTurretType, placeOn, ref this.Variables);

                       /* // else no sense to care for the newly created gadget 
                        for (int j = 0; j < this.Variables.Battleground.Count; j++)
                        {
                            var curPosition = this.Variables.Battleground[j];
                            if (curPosition.Uniq_X == newTurretX && curPosition.Uniq_Y == newTurretY)
                            {
                                this.Variables.FireTrappers.Add(trap);
                                break;
                            }
                        }
                        */
                        this.Variables.Hellions.Add(turret);
                        this.Variables.TurretsPosition.Add(turret);
                    }
                }
            }
        }

    }
}
