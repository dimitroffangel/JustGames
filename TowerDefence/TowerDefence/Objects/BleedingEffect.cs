using System;
using TowerDefence.Enums;

namespace TowerDefence.Objects
{
    class BleedingEffect
    {
        private Enemy m_Target;
        private float m_Damage;
        private int m_Duration;
        private int m_CurDuration;
        private float m_Interval;
        private BleedingTypes m_BleedingType;
        private DateTime m_StartTime;
        private int uniqDamage;
        private BleedingTypes fireBleed;

        private SetUpVariables m_Variables;

        public BleedingEffect(ref Enemy enemy, int damage, int duration, BleedingTypes type, ref SetUpVariables variables)
        {
            m_Target = enemy;
            m_Damage = damage;
            m_Duration = duration;
            m_CurDuration = 0;
            m_BleedingType = type;
            m_StartTime = DateTime.Now;

            m_Variables = variables;
        }

        public void ActivateEffect()
        {
            if (m_CurDuration == m_Duration || m_Target == null)
                return;

            if(m_Target.GetHealthStatus() <= 0) // the target is already killed
            {
                m_Target = null;
                return;
            }

            if(m_BleedingType == BleedingTypes.FireBleed)
            {
                DateTime curTime = DateTime.Now;

                // check if the time is the appropriate one, if so do damage, TryKillingTheEnemy(), if succeeeds nullify the target 
                // increase the number of kills 
                if((curTime - m_StartTime).TotalSeconds >= 1)
                {
                    m_StartTime = curTime;
                    m_Target.TakeDamage(m_Damage);
                    if (m_Target.GetHealthStatus() <= 0)
                    {
                        m_Target.TryKilling();
                        this.m_Variables.EnemyPositions.Remove(m_Target);
                        m_Target = null;
                        this.Variables.EnemiesCurrentlyKilled++;
                    }
                    m_CurDuration++;
                }
            }

            if (m_CurDuration == m_Duration)
            {
                m_Target = null;
            }
        }


        internal Enemy Target { get => m_Target; set => m_Target = value; }
        internal SetUpVariables Variables { get => m_Variables; set => m_Variables = value; }
    }
}
