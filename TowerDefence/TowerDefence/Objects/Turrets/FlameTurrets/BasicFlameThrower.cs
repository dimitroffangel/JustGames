namespace TowerDefence.Objects.Turrets
{
    class BasicFlameThrower : FlameThrower
    {

        public BasicFlameThrower(int x, int y, TurretType turretType, 
            TurretPlacement placement, ref SetUpVariables variables) 
            : base(x, y, turretType, placement, ref variables)
        {

        }

        protected override void SetStats()
        {
            this.Ammo = 15;
            this.FireRate = 0.1f;

            if (this.LevelDamage == 1)
            {
                this.Damage = 0;
                this.FireEffect = 20;
            }

            else if (this.LevelDamage == 2)
            {
                //test
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
