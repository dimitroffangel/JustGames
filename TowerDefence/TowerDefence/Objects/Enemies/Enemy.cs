using System;
using System.Collections.Generic;
using TowerDefence.Enums;

namespace TowerDefence.Objects
{
    abstract class Enemy : Position
    {
        private EnemyType m_Type;
        
        private DateTime m_TimeSinceLastMove;
        private float m_MoveRate;
        private float m_Health;
        private List<Enum> m_BleedingEffects;

        private SetUpVariables internal_Variables;

        public Enemy(int x, int y, ref SetUpVariables variables) : base(x, y)
        {
            m_Type = EnemyType.Imp;
            internal_Variables = variables;

            m_TimeSinceLastMove = DateTime.Now;
            // set the primary things of the enemies
            m_BleedingEffects = new List<Enum>();
            this.SetStats();
        }

        protected abstract void SetStats();

        public void TakeDamage(float damage)
        {
            this.m_Health -= damage;
        }

        public void TryKilling()
        {
            if (this.m_Health <= 0)
            {
                Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
                Console.Write(" ");
            }
        }

        public float GetHealthStatus()
        {
            return m_Health;
        }

        public float MoveRate { get => m_MoveRate; set => m_MoveRate = value; }
        public DateTime TimeSinceLastMove { get => m_TimeSinceLastMove; set => m_TimeSinceLastMove = value; }
        internal SetUpVariables Variables { get => internal_Variables; set => internal_Variables = value; }
        public EnemyType UniqType { get => m_Type; set => m_Type = value; }
        protected float Health { get => m_Health; set => m_Health = value; }
        public List<Enum> BleedingEffects { get => m_BleedingEffects; set => m_BleedingEffects = value; }
    }
}
