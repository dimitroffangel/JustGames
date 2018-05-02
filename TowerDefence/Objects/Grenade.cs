using System;
using System.Collections.Generic;

namespace TowerDefence.Objects
{
    class Grenade : Position
    {
        private DateTime m_StartTime;
        private const int m_TimeBeforeExplosion = 2;
        private bool m_HasExploded;
        private float m_Damage;
        private float m_SideDamage;

        private SetUpVariables internal_Variables;

        public Grenade(int x, int y, float damage, float sideDamage, ref SetUpVariables variables) : base(x, y)
        {
            internal_Variables = variables;
            m_Damage = damage;
            m_SideDamage = sideDamage;
        }

        public void TryExploding()
        {
            if (m_HasExploded)
                return;

            DateTime curTime = DateTime.Now;

            // check if the time for explosion is right
            if((curTime - m_StartTime).Seconds >= m_TimeBeforeExplosion)
            {
                this.ExplodeNow();   
            }
        }

        internal SetUpVariables Variables { get => internal_Variables; set => internal_Variables = value; }
        public DateTime StartTime { get => m_StartTime; set => m_StartTime = value; }
        public bool HasExploded { get => m_HasExploded; set => m_HasExploded = value; }
        public float Damage { get => m_Damage; set => m_Damage = value; }
        public float SideDamage { get => m_SideDamage; set => m_SideDamage = value; }

        public void ExplodeNow()
        {
            m_HasExploded = true;

            // do tremendous damage to the main position and round damage to the neighbouring locations
            // check if there are enemies near the target and if there is even a target

            for (int i = 0; i < internal_Variables.EnemyPositions.Count; i++)
            {
                var enemy = internal_Variables.EnemyPositions[i];

                // the big damage
                if (this.Uniq_X == enemy.Uniq_X && this.Uniq_Y == enemy.Uniq_Y)
                {
                    enemy.TakeDamage(m_Damage);
                    if (enemy.GetHealthStatus() <= 0)
                    {
                        enemy.TryKilling();
                        internal_Variables.EnemyPositions.Remove(enemy);
                        internal_Variables.EnemiesCurrentlyKilled++;
                        // decrement the i so that the cycle do not miss an enemy
                        i--;
                        continue;
                    }
                }
                // side damage
                if ((this.Uniq_X == enemy.Uniq_X && this.Uniq_Y + 1 == enemy.Uniq_Y) ||
                    (this.Uniq_X == enemy.Uniq_X && this.Uniq_Y - 1 == enemy.Uniq_Y) ||
                    (this.Uniq_X + 1 == enemy.Uniq_X && this.Uniq_Y == enemy.Uniq_Y) ||
                    (this.Uniq_X - 1 == enemy.Uniq_X && this.Uniq_Y == enemy.Uniq_Y))
                {
                    enemy.TakeDamage(m_SideDamage);

                    if (enemy.GetHealthStatus() <= 0)
                    {
                        enemy.TryKilling();
                        internal_Variables.EnemyPositions.Remove(enemy);
                        internal_Variables.EnemiesCurrentlyKilled++;
                        // decrement the i so that the cycle do not miss an enemy
                        i--;
                        continue;
                    }
                }
            }
        }
    }
}
