using System;


namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Infernal : FlameThrower
    {
        public Infernal(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }

        protected override void SetStats()
        {
            this.Ammo = 5;
            this.FireRate = 1f;

            if (this.LevelDamage == 1)
            {
                this.Damage = 70;
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
