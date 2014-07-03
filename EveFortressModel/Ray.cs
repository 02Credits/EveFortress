
using System;
namespace EveFortressModel
{
    public class Ray
    {
        public long StartX { get; set; }
        public long StartY { get; set; }
        public long StartZ { get; set; }

        public float DirX { get; set; }
        public float DirY { get; set; }
        public float DirZ { get; set; }

        public Ray(long startX, long startY, long startZ, long deltaX, long deltaY, long deltaZ)
        {
            var deltaLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            DirX = deltaX / (float)deltaLength;
            DirY = deltaY / (float)deltaLength;
            DirZ = deltaZ / (float)deltaLength;
        }

        public float? TimeAtX(long x)
        {
            return DirX == 0 ? new float?() : (x - StartX) / DirX;
        }
        public long XAtTime(float t)
        {
            return (long)(StartX + DirX * t);
        }
        public Tuple<long, long, long> PosAtX(long x)
        {
            var time = TimeAtX(x);
            if (time.HasValue)
            {
                return Tuple.Create(x, YAtTime(time.Value), ZAtTime(time.Value));
            }
            else
            {
                return null;
            }
        }

        public float? TimeAtY(long y)
        {
            return DirY == 0 ? new float?() : (y - StartY) / DirY;
        }
        public long YAtTime(float t)
        {
            return (long)(StartY + DirY * t);
        }
        public Tuple<long, long, long> PosAtY(long y)
        {
            var time = TimeAtY(y);
            if (time.HasValue)
            {
                return Tuple.Create(XAtTime(time.Value), y, ZAtTime(time.Value));
            }
            else
            {
                return null;
            }
        }

        public float? TimeAtZ(long z)
        {
            return (z - StartZ) / DirZ;
        }
        public long ZAtTime(float t)
        {
            return (long)(StartZ + DirZ * t);
        }
        public Tuple<long, long, long> PosAtZ(long z)
        {
            var time = TimeAtZ(z);
            if (time.HasValue)
            {
                return Tuple.Create(XAtTime(time.Value), YAtTime(time.Value), z);
            }
            else
            {
                return null;
            }
        }
    }
}
