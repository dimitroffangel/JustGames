using System;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class FireTrapper : FlameThrower
    {
        private const int m_ExtendTime = 1;
        private DateTime m_CurrentExtendTime;
        private const int m_ExtendLimit = 4;
        private int m_CurrentExtensionCounter;
        private bool m_IsClone;
        
        public FireTrapper(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables, bool isCopied) : base(x, y, turretType, placement, ref variables)
        {
            CurrentExtendTime = DateTime.Now;
            m_CurrentExtensionCounter = 0;
            m_IsClone = isCopied;
        }

        public void TryExtendingExplosion()
        {
            if (m_IsClone || m_CurrentExtensionCounter >= m_ExtendLimit)
                return;

            // if the timer is up than extend with a field
            int trapBattleIndex = -1;

            if((DateTime.Now - m_CurrentExtendTime).TotalSeconds >= m_ExtendTime)
            {
                // extend
                // find the position index in the battlefield database
                for(int i = 0; i < internal_Variables.Battleground.Count; i++)
                {
                    var curPosition = internal_Variables.Battleground[i];

                    if(curPosition.X == X && curPosition.Y == Y)
                    {
                        trapBattleIndex = i;
                        break;
                    }
                }

                var newTrapPosition = internal_Variables
                    .Battleground[trapBattleIndex - (1 + m_CurrentExtensionCounter)];
                var fireTrap = new FireTrapper(newTrapPosition.X, newTrapPosition.Y, 
                    TurretType.FireBunker_Trap, m_Placement, ref internal_Variables, true);
                internal_Variables.FireTrappers.Add(fireTrap);
                Console.SetCursorPosition(newTrapPosition.X, newTrapPosition.Y);
                Console.Write("&");

                m_CurrentExtensionCounter++;
                m_CurrentExtendTime = DateTime.Now;
            }
        }

        public void Explode()
        {
            // explode when an enemy is in the traps' positions
        }

        protected override void SetStats()
        {
            this.Ammo = 10;
            this.FireRate = 0.7f;

            if (this.LevelDamage == 1)
            {
                this.Damage = SetUpVariables.Level_1_FireBunker_Damage;
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

        public DateTime CurrentExtendTime { get => m_CurrentExtendTime; set => m_CurrentExtendTime = value; }
    }
}
