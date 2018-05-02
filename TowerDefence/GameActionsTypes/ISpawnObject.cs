using System;
using TowerDefence.Objects;
using TowerDefence.Objects.Enemies;

namespace TowerDefence
{
    abstract class ISpawnObject : ILocatePlayer
    {
        public ISpawnObject(ref SetUpVariables variables)
            :base(ref variables)
        {
                
        }

        private bool AreEnemiesColliding(int x, int y)
        {
            for (int i = 0; i < this.Variables.EnemyPositions.Count; i++)
            {
                if (this.Variables.EnemyPositions[i].Uniq_X == x && this.Variables.EnemyPositions[i].Uniq_Y == y)
                    return true;
            }

            return false;
        }

        private void MoveEnemies()
        {
            for (int i = 0; i < this.Variables.EnemyPositions.Count; i++)
            {
                var enemy = this.Variables.EnemyPositions[i];
                int initialEnemyX = enemy.Uniq_X;
                int initialEnemyY = enemy.Uniq_Y;
                var curTime = DateTime.Now;
                // check move rate

                if ((curTime - enemy.TimeSinceLastMove).TotalSeconds >= enemy.MoveRate)
                {
                    enemy.TimeSinceLastMove = DateTime.Now;

                    if (enemy.Uniq_Y <= Console.WindowHeight - 5 && enemy.Uniq_X < Console.WindowWidth - 31 &&
                        !AreEnemiesColliding(enemy.Uniq_X, enemy.Uniq_Y + 1))
                        enemy.Uniq_Y++;

                    else
                    {
                        if (enemy.Uniq_X < Console.WindowWidth - 31 && !AreEnemiesColliding(enemy.Uniq_X + 1, enemy.Uniq_Y))
                            enemy.Uniq_X++;

                        else if (!AreEnemiesColliding(enemy.Uniq_X, enemy.Uniq_Y - 1))
                        {
                            // the enemy has reached its final destination time to kill him
                            if (enemy.Uniq_Y <= SetUpVariables.InitialRow)
                            {
                                enemy.Uniq_Y = SetUpVariables.InitialRow;
                                enemy.Uniq_X = SetUpVariables.InitialCol + 1;
                                // increment the missed enemies variable and remove the references to the object
                                this.Variables.EnemiesCurrentlyMissed++;
                                this.Variables.EnemyPositions.Remove(enemy);
                                i--;
                                continue;
                            }

                            enemy.Uniq_Y--;
                        }
                    }

                    if (enemy.Uniq_Y != initialEnemyY || enemy.Uniq_X != initialEnemyX)
                    {
                        Console.SetCursorPosition(initialEnemyX, initialEnemyY);
                        Console.Write(" ");
                    }
                }

                Console.SetCursorPosition(enemy.Uniq_X, enemy.Uniq_Y);
                Console.Write("$");
            }

        }

        public void SpawnEnemies()
        {
            if (this.Variables.EnemiesCount > 0)
            {
                // check if there is someone on this position if there is then do not spawn an enemy

                if (!AreEnemiesColliding(SetUpVariables.InitialCol + 1, SetUpVariables.InitialRow))
                {
                    this.Variables.EnemyPositions.Add(new Imp(SetUpVariables.InitialCol + 1, SetUpVariables.InitialRow, ref this.Variables));
                    this.Variables.EnemiesCount--;
                }
            }

            // move the enemies
            MoveEnemies();
        }

        public void SpawnObstacles()
        {
            Random random = new Random();

            // generate randomly the positions

            int randX = random.Next(0, Console.WindowWidth - 1);
            int randY = random.Next(0, Console.WindowHeight - 1);
            Position obstaclePosition = new Position(randX, randY);

            // draw it
            Console.SetCursorPosition(obstaclePosition.Uniq_X, obstaclePosition.Uniq_Y);
            Console.Write("Y");

            // tests if the position is on the player if so, you are prompted to die
            if (obstaclePosition.Uniq_X == this.Variables.PlayerCol && // 'is dead Jim ba baba kungala!
                obstaclePosition.Uniq_Y == this.Variables.PlayerRow)
            {
                this.Variables.IsGameOver = true;
                this.Variables.Rocks_Felt_On_You++;
            }

            if (IsRockOnTurret(obstaclePosition)) // if the rock is on a turret then in the function the turret is annihilated
                this.Variables.Rocks_Felt_On_Turret++;

            // add the obstacle to the list of obstacles
            this.Variables.ObstaclePositions.Add(obstaclePosition);
        }

    }
}
