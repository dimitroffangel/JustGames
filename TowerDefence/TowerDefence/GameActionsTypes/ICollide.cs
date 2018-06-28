using System;

namespace TowerDefence
{
    abstract class ICollide
    {
        public SetUpVariables Variables;

        public ICollide(ref SetUpVariables variables)
        {
            this.Variables = variables;
        }

        public bool IsPlayerCollidingWithEnemy(int additionX, int additionY)
        {
            Position playerPosition = new Position(Variables.PlayerCol + additionX, Variables.PlayerRow + additionY);

            foreach (var enemy in Variables.EnemyPositions)
            {
                if (playerPosition.Uniq_X == enemy.Uniq_X)
                {
                    if (playerPosition.Uniq_Y == enemy.Uniq_Y)
                        return true;
                }
            }

            return false;
        }

        public bool IsRockOnTurret(Position obstaclePosition)
        {
            for (int i = 0; i < this.Variables.TurretsPosition.Count; i++)
            {
                var turret = this.Variables.TurretsPosition[i];

                if (turret.Uniq_X == obstaclePosition.Uniq_X &&
                   turret.Uniq_Y == obstaclePosition.Uniq_Y)
                {
                    // draw the animation
                    Console.SetCursorPosition(turret.Uniq_X, turret.Uniq_Y);
                    Console.Write(" ");
                    // remove the turret
                    this.Variables.TurretsPosition.Remove(turret);
                    return true;
                }
            }

            return false;
        }

    }
}
