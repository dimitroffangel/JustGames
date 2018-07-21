namespace TowerDefence
{
    abstract class ILocatePlayer : ICollide
    {
        public ILocatePlayer(ref SetUpVariables variables)
            :base(ref variables)
        {

        }

        private bool IsPlayerNearBattlefield(int additionX, int additionY)
        {
            foreach(var position in internal_Variables.Battleground)
            {
                if (position.X - additionX == internal_Variables.PlayerCol &&
                    position.Y - additionY == internal_Variables.PlayerRow)
                    return true;
            }

            return false;
        }

        public TurretPlacement DeterminePlayerPosition() // find the position to spawn the turret
        {
            if (IsPlayerNearBattlefield(2, 0) || IsPlayerNearBattlefield(3, 0)) // from Left
                return TurretPlacement.Left;

            else if (IsPlayerNearBattlefield(-2, 0) || IsPlayerNearBattlefield(-3, 0)) //from Right
                return TurretPlacement.Right;

            else if (IsPlayerNearBattlefield(0, 2) || IsPlayerNearBattlefield(0, 3)) // from Up
                return TurretPlacement.Top;

            else if (IsPlayerNearBattlefield(0, -2) || IsPlayerNearBattlefield(0, -3) || 
                IsPlayerNearBattlefield(0, -1)) // from down
                return TurretPlacement.Down;

            else
                return TurretPlacement.Not_Allowed;
        }
    }
}
