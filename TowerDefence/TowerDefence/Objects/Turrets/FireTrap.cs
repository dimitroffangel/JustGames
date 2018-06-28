using System;

namespace TowerDefence.Objects.Turrets
{
    class FireTrap : Position
    {
        private bool m_HasExploded;
        private float m_Damage;
        private float m_SideDamage;

        private SetUpVariables internal_Variables;

        public FireTrap(int x, int y, int damage, int sideDamage) : base(x, y)
        {

        }
    }
}
