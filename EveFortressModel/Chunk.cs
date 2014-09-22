using ProtoBuf;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressModel
{
    [ProtoContract]
    public class Chunk
    {
        public const byte DIAMETER_BITS = 6;
        public const byte DIAMETER = 1 << (DIAMETER_BITS - 1);

        [ProtoMember(1)]
        public TerrainType[] Level { get; set; }

        [ProtoMember(2)]
        public byte[] RandomSelection { get; set; }

        [ProtoMember(3)]
        public Dictionary<long, Entity> Entities { get; set; }

        [ProtoMember(4)]
        public long X { get; set; }

        [ProtoMember(5)]
        public long Y { get; set; }

        public Point<long> Loc { get { return new Point<long>(X, Y); } }

        public Chunk()
        {
            Entities = new Dictionary<long, Entity>();
        }

        public TerrainType GetTerrainLevel(Point<byte> loc)
        {
            return Level[loc.X * DIAMETER + loc.Y];
        }

        public byte GetRandomSelection(Point<byte> loc)
        {
            return RandomSelection[loc.X * DIAMETER + loc.Y];
        }

        public void Save()
        {
            SerializationUtils.SerializeToFile("World\\X" + X + "Y" + Y + ".bin", this);
        }

        public static Point<long> GetChunkCoords(Point<long> loc)
        {
            return GetChunkCoords(loc.X, loc.Y);
        }

        public static Point<long> GetChunkCoords(long x, long y)
        {
            var chunkX = x >> (DIAMETER_BITS - 1);
            var chunkY = y >> (DIAMETER_BITS - 1);
            return new Point<long>(chunkX, chunkY);
        }

        public static Point<byte> GetLocalCoords(Point<long> loc)
        {
            return GetLocalCoords(loc.X, loc.Y);
        }

        public static Point<byte> GetLocalCoords(long x, long y)
        {
            var blockX = x & (DIAMETER - 1);
            var blockY = y & (DIAMETER - 1);
            return new Point<byte>((byte)blockX, (byte)blockY);
        }

        public bool ContainsLoc(Point<long> loc)
        {
            return Loc.X - loc.X < DIAMETER &&
                   Loc.X - loc.X > 0 &&
                   Loc.Y - loc.Y < DIAMETER &&
                   Loc.Y - loc.Y > 0;
        }

        public void ApplyPatch(Patch patch)
        {
            Level[patch.Position.X * DIAMETER + patch.Position.Y] = patch.Change;
        }
    }
}