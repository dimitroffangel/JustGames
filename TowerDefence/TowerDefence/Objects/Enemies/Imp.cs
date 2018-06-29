using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence.Objects.Enemies
{
    class Imp : Enemy
    {
        public Imp(int x, int y, ref SetUpVariables variables) : base(x, y, ref variables)
        {
        }

        protected override void SetStats()
        {
            this.Health = 70;
            this.MoveRate = 0.5f;
            this.InitialMoveRate = this.MoveRate;
        }
    }
}
