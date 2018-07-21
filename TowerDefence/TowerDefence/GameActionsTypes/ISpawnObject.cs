using System;
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
            for (int i = 0; i < internal_Variables.EnemyPositions.Count; i++)
            {
                if (internal_Variables.EnemyPositions[i].X == x && 
                    internal_Variables.EnemyPositions[i].Y == y)
                    return true;
            }
            return false;
        }

        private int GetBattlefieldIndex(int x, int y)
        {
            for(int i = 0; i < internal_Variables.Battleground.Count; i++)
            {
                var currentBattlefield = internal_Variables.Battleground[i];

                if (currentBattlefield.X == x && currentBattlefield.Y == y)
                    return i+1;
            }

            throw new ArgumentNullException("Searching for the battlefield index there was none found");
        }

        private void MoveEnemies()
        {
            for (int i = 0; i < internal_Variables.EnemyPositions.Count; i++)
            {
                var enemy = internal_Variables.EnemyPositions[i];
                int initialEnemyX = enemy.X;
                int initialEnemyY = enemy.Y;
                var curTime = DateTime.Now;
                // check move rate

                if ((curTime - enemy.TimeSinceLastMove).TotalSeconds >= enemy.MoveRate)
                {
                    enemy.TimeSinceLastMove = DateTime.Now;
                    var nextEnemyMoveIndex = GetBattlefieldIndex(enemy.X, enemy.Y);
                    
                    if (nextEnemyMoveIndex == internal_Variables.Battleground.Count)
                    {
                        // the enemy has reached its final destination time to kill him
                        Console.SetCursorPosition(enemy.X, enemy.Y);
                        Console.Write(" ");
                        // increment the missed enemies variable and remove the references to the object
                        internal_Variables.EnemiesCurrentlyMissed++;

                        internal_Variables.EnemyPositions.Remove(enemy);
                        i--;
                        continue;
                    }

                    var nextEnemyMove = internal_Variables.Battleground[nextEnemyMoveIndex];

                    if (AreEnemiesColliding(nextEnemyMove.X, nextEnemyMove.Y))
                        continue;

                    enemy.X = nextEnemyMove.X;
                    enemy.Y = nextEnemyMove.Y;


                    if (enemy.Y != initialEnemyY || enemy.X != initialEnemyX)
                    {
                        Console.SetCursorPosition(initialEnemyX, initialEnemyY);
                        Console.Write(" ");
                    }

                    // review if the enemy is colliding with a fire trap
                    for(int j = 0; j < internal_Variables.FireTrappers.Count; j++)
                    {
                        var curTrap = internal_Variables.FireTrappers[j];

                        if(curTrap.X == enemy.X && curTrap.Y == enemy.Y)
                        {
                            // deal damage to the enemy
                            // check if the enemy is alive 

                            enemy.TakeDamage(curTrap.Damage);
                            enemy.TryKilling();

                           if(enemy.GetHealthStatus() <= 0)
                            {
                                i--; // ensure the for cycle is not dumbed
                                internal_Variables.EnemyPositions.Remove(enemy);
                                internal_Variables.EnemiesCurrentlyKilled++;
                            }

                            // remove the trap
                            internal_Variables.FireTrappers.Remove(curTrap);
                            j--;

                        }
                    }
                }

                Console.SetCursorPosition(enemy.X, enemy.Y);
                Console.Write("$");
            }
        }

        public void SpawnEnemies()
        {
            if (internal_Variables.EnemiesCount > 0)
            {
                // check if there is someone on this position if there is then do not spawn an enemy

                if (!AreEnemiesColliding(SetUpVariables.InitialCol + 1, SetUpVariables.InitialRow))
                {
                    internal_Variables.EnemyPositions
                        .Add(new Imp(internal_Variables.Battleground[0].X, 
                        internal_Variables.Battleground[0].Y, ref internal_Variables));
                    internal_Variables.EnemiesCount--;
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
            Console.SetCursorPosition(obstaclePosition.X, obstaclePosition.Y);
            Console.Write("Y");

            // tests if the position is on the player if so, you are prompted to die
            if (obstaclePosition.X == internal_Variables.PlayerCol && // 'is dead Jim ba baba kungala!
                obstaclePosition.Y == internal_Variables.PlayerRow)
            {
                internal_Variables.IsGameOver = true;
                internal_Variables.Rocks_Felt_On_You++;
            }

            if (IsRockOnTurret(obstaclePosition)) // if the rock is on a turret then in the function the turret is annihilated
                internal_Variables.Rocks_Felt_On_Turret++;

            // add the obstacle to the list of obstacles
            internal_Variables.ObstaclePositions.Add(obstaclePosition);
        }
    }
}
