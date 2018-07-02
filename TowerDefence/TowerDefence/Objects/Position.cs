﻿using System;
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

        public override bool Equals(object obj)
        {
            var position = obj as Position;
            return position != null &&
                   uniq_X == position.uniq_X &&
                   uniq_Y == position.uniq_Y &&
                   Uniq_X == position.Uniq_X &&
                   Uniq_Y == position.Uniq_Y;
        }

        public static bool operator ==(Position a, Position b)
        {
            if ((object)a == (object)b)
                return true;

            if ((object)a == null || (object)b == null)
                return false;


            return a.uniq_X == b.uniq_X && a.uniq_Y == b.Uniq_Y;
        }

        public static bool operator !=(Position a, Position b)
        {
            if ((object)a == (object)b)
                return false;

            if ((object)a == null || (object)b == null)
                return true;

            return a.uniq_X != b.Uniq_X || a.uniq_Y != b.Uniq_Y;
        }

        public int Uniq_X { get => uniq_X; set => uniq_X = value; }
        public int Uniq_Y { get => uniq_Y; set => uniq_Y = value; }
    }

    class PositionEqualityComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position a, Position b)
        {
            return a.Uniq_X == b.Uniq_X && a.Uniq_Y == b.Uniq_Y;
        }

        public int GetHashCode(Position obj)
        {
            string combined = obj.Uniq_X + " ; " + obj.Uniq_Y;
            return combined.GetHashCode();
        }
    }
}
