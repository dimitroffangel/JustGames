using System;
using System.Collections.Generic;
using TowerDefence.Enums;

namespace TowerDefence.Objects
{
    abstract class Enemy : Position
    {
        private EnemyType m_Type;
        private EnemyState m_State;
        
        private DateTime m_TimeSinceLastMove;
        private float m_MoveRate;
        private float m_InitialMoveRate;
        private float m_Health;
        private float m_SlowedDuration;
        private DateTime m_SlowedTime;
        private List<Enum> m_BleedingEffects;
        private List<Turret> m_TargetedBy;

        private SetUpVariables internal_Variables;

        public Enemy(int x, int y, ref SetUpVariables variables) : base(x, y)
        {
            m_Type = EnemyType.Imp;
            m_State = EnemyState.Normal;
            internal_Variables = variables;

            m_TimeSinceLastMove = DateTime.Now;
            // set the primary things of the enemies
            m_BleedingEffects = new List<Enum>();
            TargetedBy = new List<Turret>();
            this.SetStats();
        }

        protected abstract void SetStats();
        protected abstract void Foo();

        public void TakeDamage(float damage)
        {
            m_Health -= damage;
        }

        public void TryKilling()
        {
            if (m_Health <= 0)
            {
                Console.SetCursorPosition(this.X, this.Y);
                Console.Write(" ");
            }
        }

        public float GetHealthStatus()
        {
            return m_Health;
        }

        public float MoveRate
        {
            get => m_MoveRate; set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("MoveRate must be bigger than 0");

                m_MoveRate = value;
            }
        }
        public float InitialMoveRate { get => m_InitialMoveRate; set => m_InitialMoveRate = value; }
        public DateTime TimeSinceLastMove { get => m_TimeSinceLastMove; set => m_TimeSinceLastMove = value; }
        internal SetUpVariables Variables { get => internal_Variables; set => internal_Variables = value; }
        public EnemyType UniqType { get => m_Type; set => m_Type = value; }
        protected float Health { get => m_Health; set => m_Health = value; }
        public List<Enum> BleedingEffects { get => m_BleedingEffects; set => m_BleedingEffects = value; }
        internal EnemyState State { get => m_State; set => m_State = value; }
        public float SlowedDuration { get => m_SlowedDuration; set => m_SlowedDuration = value; }
        public DateTime SlowedTime { get => m_SlowedTime; set => m_SlowedTime = value; }
        internal List<Turret> TargetedBy { get => m_TargetedBy; set => m_TargetedBy = value; }
    }
}
