namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Demoman : FlameThrower
    {
        public Demoman(int x, int y, TurretType turretType, TurretPlacement placement,
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }

        protected override void SetStats()
        {
            this.Ammo = 15;
            this.FireRate = 2.5f;

            if (this.LevelDamage == 1)
            {
                this.Damage = SetUpVariables.Level_1_FireBunker_Damage;
                this.FireEffect = 45;
            }

            else if (this.LevelDamage == 2)
            {
                this.Damage = SetUpVariables.Level_2_FireBunker_Damage;
                this.FireEffect = 5;
            }

            else if (this.LevelDamage == 3)
            {
                this.Damage = SetUpVariables.Level_3_FireBunker_Damage;
                this.FireEffect = 5;
            }
        }
    }
}
