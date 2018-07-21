using System;

namespace TowerDefence
{
    abstract class ICollide
    {
        public SetUpVariables internal_Variables;

        public ICollide(ref SetUpVariables variables)
        {
            internal_Variables = variables;
        }

        public bool IsPlayerCollidingWithEnemy(int additionX, int additionY)
        {
            Position playerPosition = new Position(internal_Variables.PlayerCol + additionX, internal_Variables.PlayerRow + additionY);

            foreach (var enemy in internal_Variables.EnemyPositions)
            {
                if (playerPosition.X == enemy.X)
                {
                    if (playerPosition.Y == enemy.Y)
                        return true;
                }
            }

            return false;
        }

        public bool IsRockOnTurret(Position obstaclePosition)
        {
            for (int i = 0; i < internal_Variables.TurretsPosition.Count; i++)
            {
                var turret = internal_Variables.TurretsPosition[i];

                if (turret.X == obstaclePosition.X &&
                   turret.Y == obstaclePosition.Y)
                {
                    // draw the animation
                    Console.SetCursorPosition(turret.X, turret.Y);
                    Console.Write(" ");
                    // remove the turret
                    internal_Variables.TurretsPosition.Remove(turret);
                    return true;
                }
            }

            return false;
        }

    }
}
