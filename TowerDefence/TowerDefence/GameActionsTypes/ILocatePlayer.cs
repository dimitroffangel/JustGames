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
            foreach(var position in this.Variables.Battleground)
            {
                if (position.Uniq_X - additionX == this.Variables.PlayerCol &&
                    position.Uniq_Y - additionY == this.Variables.PlayerRow)
                    return true;
            }

            return false;
        }

        public TurretPlacement DeterminePlayerPosition() // find the position to spawn the turret
        {
            if (IsPlayerNearBattlefield(2, 0)) // from Left
                return TurretPlacement.Left;

            else if (IsPlayerNearBattlefield(-2, 0)) //from Right
                return TurretPlacement.Right;

            else if (IsPlayerNearBattlefield(0, 2)) // from Up
                return TurretPlacement.Top;

            else if (IsPlayerNearBattlefield(0, -2)) // from down
                return TurretPlacement.Down;

            else
                return TurretPlacement.Not_Allowed;
        }
    }
}
