
namespace TowerDefence.Objects.Turrets
{
    class Tardus : Turret // Tarus means slow from latin
    {
        private float slowDuration;

        public Tardus(int x, int y, TurretType turretType, TurretPlacement placement, ref SetUpVariables variables)
           : base(x, y, turretType, placement, ref variables)
        {

        }

        public float SlowDuration { get => slowDuration; set => slowDuration = value; }

        protected override void SetStats()
        {
            this.Ammo = 15;
            this.FireRate = 0.6f;
            this.slowDuration = 0.5f;

            this.Damage = 20f;
        }
    }
}
