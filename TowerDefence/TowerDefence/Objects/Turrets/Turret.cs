using System;
using System.Collections.Generic;
using TowerDefence.Objects;
using TowerDefence.Objects.Turrets.FlameTurrets;

namespace TowerDefence
{
    abstract class Turret : Position
    {
        public TurretType m_Type;
        public TurretPlacement m_Placement;
        
        private int m_Ammo;
        private int m_LevelDamage;
        private float m_Damage;
        private float m_BonusDamage;
        private float m_FireRate;
        private int m_FireEffect;
        private DateTime m_LastTimeShot;

        private SetUpVariables internal_Variables;
        private Enemy m_target;

        public Turret(int x, int y, TurretType turretType, TurretPlacement placement,
            ref SetUpVariables variables) : base(x, y)
        {
            m_Type = turretType;
            m_Placement = placement;

            m_LevelDamage = 1;
            
            internal_Variables = variables;
            m_LastTimeShot = DateTime.Now; // loading the turret it must have time to prepare it self this is the time;
            this.SetStats();
        }

        protected abstract void SetStats();

        private void SetAmmo()
        {
            if (TurretType.Cannon == m_Type)
                m_Ammo = 15;

            else if (TurretType.FireBunker_Basic == m_Type)
                m_Ammo = 60;

            else if (TurretType.LaserAnnihilator == m_Type)
                m_Ammo = 1;

            else if (TurretType.Tardus == m_Type)
                m_Ammo = 40;
        }

        private void SetDamage()
        {
            if(m_Type == TurretType.Cannon)
            {
                if (LevelDamage == 1)
                    m_Damage = SetUpVariables.Level_1_Cannon_Damage;

                else if (LevelDamage == 2)                      
                    m_Damage = SetUpVariables.Level_2_Cannon_Damage;
                                                               
                else if (LevelDamage == 3)                      
                    m_Damage = SetUpVariables.Level_3_Cannon_Damage;
            }

            else if(m_Type == TurretType.Tardus)
            {
                if (LevelDamage == 1)
                    m_Damage = SetUpVariables.Level_1_MissleBunker_Damage;

                else if (LevelDamage == 2)
                    m_Damage = SetUpVariables.Level_2_MissleBunker_Damage;

                else if (LevelDamage == 3)
                    m_Damage = SetUpVariables.Level_3_MissleBunker_Damage;
            }
        }

        public void TryFiring()
        {
            var curTime = DateTime.Now;

            if ((curTime - this.LastTimeShot).TotalSeconds < m_FireRate)
                return;

            m_LastTimeShot = curTime;

            if (TurretType.Cannon == m_Type)
                FireOn(0);

            else if (TurretType.FireBunker_Basic == m_Type)
                FireOn(0);

            else if (TurretType.FireBunker_Demoman == m_Type) // add the grenade
                FireOn(0);

            else if (TurretType.FireBunker_Infernal == m_Type)
                FireOn(0);

            else if (TurretType.FireBunker_Hellion == m_Type)
                FireOn(0);

            else if (TurretType.Tardus == m_Type)
            {
                for (int i = 0; i < 2; i++)
                    FireOn(i);
            }

            else if (TurretType.LaserAnnihilator == m_Type)
            {
                internal_Variables.EnemiesCurrentlyKilled += internal_Variables.EnemyPositions.Count;

                foreach (var enemy in internal_Variables.EnemyPositions)
                {
                    Console.SetCursorPosition(enemy.Uniq_X, enemy.Uniq_Y);
                    Console.Write(" ");
                }

                internal_Variables.EnemyPositions.Clear();

                m_Ammo--;
            }
        }

        private void FireOn(int addition) // turret's position + these arguments
        {
            int targetX = Uniq_X;
            int targetY = Uniq_Y;

            if (m_Placement == TurretPlacement.Top)
            {
                targetX += addition;
                targetY += 2;
            }

            if (m_Placement == TurretPlacement.Down)
            {
                targetX += addition;
                targetY -= 2;
            }

            if (m_Placement == TurretPlacement.Right)
            {
                targetX -= 2;
                targetY += addition;
            }
            if (m_Placement == TurretPlacement.Left)
            {
                targetX += 2;
                targetY += addition;
            }

            if(this.m_Type == TurretType.FireBunker_Demoman) // the demoman shot is different from the others shootings
               this.internal_Variables.Grenades.Add(new Grenade(targetX, targetY, this.m_Damage, this.m_Damage / 4, ref this.internal_Variables));

            if (IsShotOnEnemy(targetX, targetY))
            {

            }

            m_Ammo--;
            internal_Variables.AmmoLost++;
        }

        private bool IsShotOnEnemy(int x, int y)
        {
            for(int i = 0; i< internal_Variables.EnemyPositions.Count; i++)
            {
                var enemy = internal_Variables.EnemyPositions[i];

                if (TurretType.FireBunker_Hellion == m_Type && this.Target != null)
                {
                    if((x + 1 == this.Target.Uniq_X && y - 1 == this.Target.Uniq_Y) ||
                        (x + 1 == this.Target.Uniq_X && y + 1 == this.Target.Uniq_Y) ||
                       (x - 1 == this.Target.Uniq_X && y - 1 == this.Target.Uniq_Y) ||
                       (x - 1 == this.Target.Uniq_X && y + 1 == this.Target.Uniq_Y) || 
                       (x == this.Target.Uniq_X && y + 1 == this.Target.Uniq_Y) ||
                       (x == this.Target.Uniq_X && y - 1 == this.Target.Uniq_Y) ||
                       (x + 1 == this.Target.Uniq_X && y == this.Target.Uniq_Y) ||
                       (x - 1 == this.Target.Uniq_X && y == this.Target.Uniq_Y))
                    {
                        this.Target.TakeDamage(m_Damage);

                        if (this.Target.GetHealthStatus() > 0)
                            return false;

                        // destroy the enemy if the health is <= 0
                        this.Target.TryKilling();
                        internal_Variables.EnemyPositions.Remove(this.Target);
                        this.Target = null;

                        internal_Variables.EnemiesCurrentlyKilled++;
                        
                        return true;
                    }
                }

                else if (enemy.Uniq_X == x && enemy.Uniq_Y == y)
                {
                    enemy.TakeDamage(m_Damage);
                    // add the burning effect 

                    if (this.m_Type == TurretType.FireBunker_Basic)
                    {
                        for (int j = 0; j < this.internal_Variables.BleedingEnemies.Count; j++)
                        {
                            var bleeding = this.internal_Variables.BleedingEnemies[j];

                            if (bleeding.Target == enemy)
                            {
                                this.internal_Variables.BleedingEnemies.Remove(bleeding);
                                break;
                            }
                        }

                        this.internal_Variables.BleedingEnemies.Add(new BleedingEffect(ref enemy, this.FireEffect, 3, 
                            Enums.BleedingTypes.FireBleed, ref this.internal_Variables));
                        enemy.BleedingEffects.Add(Enums.BleedingTypes.FireBleed);
                    }

                    if(this.m_Type == TurretType.FireBunker_Demoman) // if the grenade is right on target
                    {
                        Random random = new Random();
                        int randomDigit = random.Next(0, 100);

                        if (randomDigit < 50) // explode now
                            this.internal_Variables.Grenades[this.internal_Variables.Grenades.Count-1].ExplodeNow();

                        return true;
                        //explode now
                    }

                    if(this.m_Type == TurretType.FireBunker_Infernal)
                    {
                        int curX = x;
                        int curY = y;
                        int counter = 1;
                        var curEnemy = enemy;
                        Enemy dummy1 = curEnemy;
                        Enemy dummy2 = curEnemy;
                        Enemy dummy3 = curEnemy;
                        Enemy dummy4 = curEnemy;
                        List<Position> circledPositions = new List<Position>();
                        
                        // side damage
                        while((dummy1 != null && internal_Variables.EnemyHasPosition(curX, curY + 1) && 
                            !circledPositions.Contains(new Position(curX, curY + 1)) && dummy1.BleedingEffects.Contains(Enums.BleedingTypes.FireBleed)) ||
                            (dummy2 != null  && internal_Variables.EnemyHasPosition(curX, curY - 1) && 
                            !circledPositions.Contains(new Position(curX, curY-1)) && dummy2.BleedingEffects.Contains(Enums.BleedingTypes.FireBleed)) ||
                            (dummy3 != null && internal_Variables.EnemyHasPosition(curX+1, curY) && 
                            !circledPositions.Contains(new Position(curX+1, curY)) && dummy3.BleedingEffects.Contains(Enums.BleedingTypes.FireBleed)) ||
                            (dummy4 != null && internal_Variables.EnemyHasPosition(curX-1, curY) && 
                            !circledPositions.Contains(new Position(curX-1, curY)) && dummy4.BleedingEffects.Contains(Enums.BleedingTypes.FireBleed)))
                        {
                            if(internal_Variables.EnemyHasPosition(curX, curY + 1))
                                curY = curY + 1;
                            else if (internal_Variables.EnemyHasPosition(curX, curY - 1))
                                curY = curY - 1;
                            else if (internal_Variables.EnemyHasPosition(curX + 1, curY))
                                curX++;
                            else if (internal_Variables.EnemyHasPosition(curX - 1, curY))
                                curX--;

                            dummy1 = internal_Variables.GetEnemy(curX, curY + 1);
                            dummy2 = internal_Variables.GetEnemy(curX, curY - 1);
                            dummy3 = internal_Variables.GetEnemy(curX + 1, curY);
                            dummy4 = internal_Variables.GetEnemy(curX - 1, curY);

                            curEnemy = internal_Variables.GetEnemy(curX, curY);
                            circledPositions.Add(new Position(curX, curY));

                            curEnemy.TakeDamage(m_Damage* counter);

                            // destroy the enemy if the health is <= 0
                            if (curEnemy.GetHealthStatus() <= 0)
                            {
                                curEnemy.TryKilling();
                                internal_Variables.EnemyPositions.Remove(curEnemy);
                                internal_Variables.EnemiesCurrentlyKilled++;
                            }
                            counter++;
                        }


                    }

                    if (this.m_Type == TurretType.Tardus)
                    {
                        enemy.MoveRate *= 2;
                        this.internal_Variables.SlowedEnemies.Add(enemy);
                        enemy.State = Enums.EnemyState.Slowed;
                        enemy.SlowedTime = DateTime.Now;
                        enemy.SlowedDuration = SetUpVariables.Level_1_Tardus_Slow_Duration;
                    }

                    if (enemy.GetHealthStatus() > 0)
                        continue;

                    // destroy the enemy if the health is <= 0
                    enemy.TryKilling();
                    internal_Variables.EnemyPositions.Remove(enemy);
                    internal_Variables.EnemiesCurrentlyKilled++;
                    return true;
                }
            }

            return false;
        }

        public int Ammo { get => m_Ammo; set => m_Ammo = value; }
        public int LevelDamage { get => m_LevelDamage; set => m_LevelDamage = value; }
        public float Damage { get => m_Damage; set => m_Damage = value; }
        public float BonusDamage { get => m_BonusDamage; set => m_BonusDamage = value; }
        public float FireRate { get => m_FireRate; set => m_FireRate = value; }
        public DateTime LastTimeShot { get => m_LastTimeShot; set => m_LastTimeShot = value; }
        internal SetUpVariables Variables { get => internal_Variables; set => internal_Variables = value; }
        public int FireEffect { get => m_FireEffect; set => m_FireEffect = value; }
        internal Enemy Target { get => m_target; set => m_target = value; }
    }
}
