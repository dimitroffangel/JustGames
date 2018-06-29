namespace TowerDefence.Objects.Turrets
{
    class Cannon : Turret
    {
        public Cannon(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }

        protected override void SetStats()
        {
            this.Ammo = 15;
            this.FireRate = 0.6f;

            if (this.m_Type == TurretType.Cannon)
            {
                if (this.LevelDamage == 1)
                    this.Damage = SetUpVariables.Level_1_Cannon_Damage;

                else if (this.LevelDamage == 2)
                    this.Damage = SetUpVariables.Level_2_Cannon_Damage;

                else if (this.LevelDamage == 3)
                    this.Damage = SetUpVariables.Level_3_Cannon_Damage;
            }
        }
    }
}
