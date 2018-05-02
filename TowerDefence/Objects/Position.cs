using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence
{
    class Position
    {
        private int uniq_X;
        private int uniq_Y;

        public Position(int x, int y)
        {
           this.Uniq_X = x;
           this.Uniq_Y = y;
        }

        public float DistanceBetween(Position a)
        {
            return Math.Abs((this.uniq_X - a.uniq_X) * (this.uniq_X - a.uniq_X) + (this.Uniq_Y - a.Uniq_Y) * (this.Uniq_Y - a.uniq_Y));
        }

        public int Uniq_X { get => uniq_X; set => uniq_X = value; }
        public int Uniq_Y { get => uniq_Y; set => uniq_Y = value; }
    }

}
