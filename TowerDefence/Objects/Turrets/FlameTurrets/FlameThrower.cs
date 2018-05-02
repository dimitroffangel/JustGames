using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence.Objects.Turrets
{
    abstract class FlameThrower : Turret
    {
        public FlameThrower(int x, int y, TurretType turretType, TurretPlacement placement,
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {

        }
    }
}
