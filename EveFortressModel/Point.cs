﻿using ProtoBuf;
using System;

namespace EveFortressModel
{
    [ProtoContract]
    public struct Point<T>
        where T : IComparable
    {
        private T x;

        [ProtoMember(1)]
        public T X { get { return x; } set { x = value; } }

        private T y;

        [ProtoMember(2)]
        public T Y { get { return y; } set { y = value; } }

        public Point(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point<T>)
            {
                var otherPoint = (Point<T>)obj;
                return otherPoint.X.Equals(X) && otherPoint.Y.Equals(Y);
            }
            return false;
        }

        public static bool operator ==(Point<T> a, Point<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point<T> a, Point<T> b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
    }
}