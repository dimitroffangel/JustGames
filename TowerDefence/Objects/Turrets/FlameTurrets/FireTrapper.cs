using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class FireTrapper : FlameThrower
    {
        public FireTrapper(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }

        protected override void SetStats()
        {
            this.Ammo = 10;
            this.FireRate = 0.7f;

            if (this.LevelDamage == 1)
            {
                this.Damage = 0;
                this.FireEffect = 0;
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
