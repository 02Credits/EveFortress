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
        public byte[] TerrainLevel { get; set; }

        [ProtoMember(2)]
        public Dictionary<long, Entity> Entities { get; set; }

        [ProtoMember(3)]
        public long X { get; set; }

        [ProtoMember(4)]
        public long Y { get; set; }

        public Point<long> Loc { get { return new Point<long>(X, Y); } }

        public Chunk()
        {
        }

        public byte GetTerrainLevel(Point<byte> loc)
        {
            return TerrainLevel[loc.X * DIAMETER + loc.Y];
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

        public static Point<byte> GetBlockCoords(Point<long> loc)
        {
            return GetBlockCoords(loc.X, loc.Y);
        }

        public static Point<byte> GetBlockCoords(long x, long y)
        {
            var blockX = x & (DIAMETER - 1);
            var blockY = y & (DIAMETER - 1);
            return new Point<byte>((byte)blockX, (byte)blockY);
        }

        public void ApplyPatch(Patch patch)
        {
            TerrainLevel[patch.Position.X * DIAMETER + patch.Position.Y] = patch.Change;
        }
    }
}