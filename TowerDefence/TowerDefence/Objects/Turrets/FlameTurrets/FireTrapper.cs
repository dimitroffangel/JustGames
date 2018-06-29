using System;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class FireTrapper : FlameThrower
    {
        private const int extendTime = 1;
        private DateTime currentExtendTime;
        private const int extendLimit = 4;
        private int currentExtensionCounter;
        private bool isCopy;

        private SetUpVariables internal_Variables;

        public FireTrapper(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables, bool isCopied) : base(x, y, turretType, placement, ref variables)
        {
            this.CurrentExtendTime = DateTime.Now;
            internal_Variables = variables;
            currentExtensionCounter = 0;
            isCopy = isCopied;
        }

        public void TryExtendingExplosion()
        {
            if (isCopy || this.currentExtensionCounter >= extendLimit)
                return;

            // if the timer is up than extend with a field
            int trapBattleIndex = -1;

            if((DateTime.Now - this.currentExtendTime).TotalSeconds >= extendTime)
            {
                // extend
                // find the position index in the battlefield database
                for(int i = 0; i < internal_Variables.Battleground.Count; i++)
                {
                    var curPosition = internal_Variables.Battleground[i];

                    if(curPosition.Uniq_X == Uniq_X && curPosition.Uniq_Y == Uniq_Y)
                    {
                        trapBattleIndex = i;
                        break;
                    }
                }

                var newTrapPosition = internal_Variables.Battleground[trapBattleIndex - (1 + this.currentExtensionCounter)];
                var fireTrap = new FireTrapper(newTrapPosition.Uniq_X, newTrapPosition.Uniq_Y, 
                    TurretType.FireBunker_Trap, m_Placement, ref internal_Variables, true);
                this.Variables.FireTrappers.Add(fireTrap);
                Console.SetCursorPosition(newTrapPosition.Uniq_X, newTrapPosition.Uniq_Y);
                Console.Write("&");

                this.currentExtensionCounter++;
                this.currentExtendTime = DateTime.Now;
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

        public DateTime CurrentExtendTime { get => currentExtendTime; set => currentExtendTime = value; }
        internal SetUpVariables Internal_Variables { get => internal_Variables; set => internal_Variables = value; }
    }
}
