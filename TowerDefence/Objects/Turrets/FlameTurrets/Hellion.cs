using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Hellion : FlameThrower
    {
        private SetUpVariables internal_Variables;

        public Hellion(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {
            internal_Variables = variables;
        }

        private void FindTarget()
        {
            float minDistance = int.MaxValue;
            int minIndex = -1;
            int index = -1;

            foreach(var enemy in internal_Variables.EnemyPositions)
            {
                index++;    
                float curDistance = this.DistanceBetween(enemy);

                if (minDistance > curDistance)
                {
                    minIndex = index;
                    minDistance = curDistance;
                }
            }

            this.Target = internal_Variables.EnemyPositions[minIndex];

            // find the moving direction of the target

        }

        private void SetDestination()
        {

        }

        public void Move()
        {
            if (this.Target == null)
                this.FindTarget();

            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write(" ");

            if (this.Uniq_X > this.Target.Uniq_X + 1)
                this.Uniq_X--;
            else if (this.Uniq_X < this.Target.Uniq_X - 1)
                this.Uniq_X++;

            if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                this.Uniq_Y--;
            else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                this.Uniq_Y++;


            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write("&");
        }

        protected override void SetStats()
        {
            this.Ammo = 30;
            this.FireRate = 0.1f;

            if (this.LevelDamage == 1)
            {
                this.Damage = 20;
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
