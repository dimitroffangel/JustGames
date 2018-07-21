using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence
{
    class Position
    {
        private int m_X;
        private int m_Y;

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }


        public float DistanceBetween(Position a)
        {
            return ((this.m_X - a.m_X) * (this.m_X - a.m_X) + 
                (this.Y - a.m_Y) * (this.Y - a.m_Y));
        }

        public float DistanceBetweenNearbyBlocks()
        {
            Position a = new Position(0, 0);
            Position b = new Position(0, 1);
            return a.DistanceBetween(b);
        }

        public override bool Equals(object obj)
        {
            var position = obj as Position;
            return position != null &&
                   m_X == position.m_X &&
                   m_Y == position.m_Y &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 388665302;
            hashCode = hashCode * -1521134295 + m_X.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Y.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Position a, Position b)
        {
            if ((object)a == (object)b)
                return true;

            if ((object)a == null || (object)b == null)
                return false;


            return a.m_X == b.m_X && a.m_Y == b.Y;
        }

        public static bool operator !=(Position a, Position b)
        {
            if ((object)a == (object)b)
                return false;

            if (((object)a == null && (object)b != null) || 
                (object)a != null && (object)b == null)
                return true;

            if ((object)a == null && (object)b == null)
                return true;

            return a.m_X != b.X || a.m_Y != b.Y;
        }

        public int X { get => m_X; set => m_X = value; }
        public int Y { get => m_Y; set => m_Y = value; }
    }

    class PositionEqualityComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public int GetHashCode(Position obj)
        {
            string combined = obj.X + " ; " + obj.Y;
            return combined.GetHashCode();
        }
    }
}
