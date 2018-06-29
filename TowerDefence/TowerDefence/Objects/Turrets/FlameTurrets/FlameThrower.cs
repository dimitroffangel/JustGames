namespace TowerDefence.Objects.Turrets
{
    abstract class FlameThrower : Turret
    {
        public FlameThrower(int x, int y, TurretType turretType, TurretPlacement placement,
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }
    }
}
