using System;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Hellion : FlameThrower
    {
        private SetUpVariables internal_Variables;
        private DateTime m_timeDeparture;
        public const float m_MoveRate = 0.5f;

        public Hellion(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {
            internal_Variables = variables;
            m_timeDeparture = DateTime.Now;
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
            var currentPosition = new Position(this.Uniq_X, this.Uniq_Y);

            // check every position that surrounds the current position 
            // order them from closest to the target to farthest
            
            // calculate the speed with which the unit will reach the target or consecutive positions near it
            // 
        }

        private void FindShortestPath()
        {

        }

        private bool ContainsTurret(int x, int y)
        {
            foreach(var turret in internal_Variables.TurretsPosition)
            {
                if(turret.Uniq_X == x && turret.Uniq_Y == y)
                    return true;
            }

            return false;
        }

        public void Move()
        {
            DateTime currentTime = DateTime.Now;

            if ((currentTime - m_timeDeparture).Seconds < m_MoveRate)
                return;

            m_timeDeparture = currentTime;

            if (this.Target == null)
                this.FindTarget();

            int initialX = this.Uniq_X;
            int initialY = this.Uniq_Y;

            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write(" ");

            if (this.Uniq_X > this.Target.Uniq_X + 1)
                initialX--;
            else if (this.Uniq_X < this.Target.Uniq_X - 1)
                initialX++;

            if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                initialY--;
            else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                initialY++;

            // check if the new position does not overlap with a turret
            // if it does TODOO
            if(ContainsTurret(initialX, initialY))
            {
                //  move x towards it
                initialY = this.Uniq_Y;
                
                if(ContainsTurret(initialX, initialY))
                {
                    // move y towards it
                    initialX = this.Uniq_X;
                    if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                        initialY--;
                    else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                        initialY++;
                    
                    if(ContainsTurret(initialX, initialY))
                    {
                        // move x towards it, but y reversed towards it
                        initialX = this.Uniq_X;
                        initialY = this.Uniq_Y;

                        if (this.Uniq_X > this.Target.Uniq_X + 1)
                            initialX--;
                        else if (this.Uniq_X < this.Target.Uniq_X - 1)
                            initialX++;

                        if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                            initialY++;
                        else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                            initialY--;

                        if(ContainsTurret(initialX, initialY))
                        {
                            // move y towards it, but x reversed towards it
                            initialX = this.Uniq_X;
                            initialY = this.Uniq_Y;

                            if (this.Uniq_X > this.Target.Uniq_X + 1)
                                initialX++;
                            else if (this.Uniq_X < this.Target.Uniq_X - 1)
                                initialX--;

                            if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                                initialY--;
                            else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                                initialY++;

                            if (ContainsTurret(initialX, initialY))
                            {
                                // move both reversed towards it
                                initialX = this.Uniq_X;
                                initialY = this.Uniq_Y;

                                if (this.Uniq_X > this.Target.Uniq_X + 1)
                                    initialX++;
                                else if (this.Uniq_X < this.Target.Uniq_X - 1)
                                    initialX--;

                                if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                                    initialY++;
                                else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                                    initialY--;

                            }
                        }
                    }
                }
            }

            this.Uniq_X = initialX;
            this.Uniq_Y = initialY;

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
