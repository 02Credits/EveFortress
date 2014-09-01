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
        public readonly static byte TERRAIN_HEIGHT = 48;
        public readonly static byte TERRAIN_RANGE = 20;
        public readonly static float INVERSE_NOISE_ROUGHNESS = 200;

        [ProtoMember(1, AsReference = true)]
        public Octree Blocks { get; set; }

        [ProtoMember(2)]
        public long X { get; set; }

        [ProtoMember(3)]
        public long Y { get; set; }

        [ProtoMember(4)]
        public long Z { get; set; }

        public Point<long> Loc { get { return new Point<long>(X, Y, Z); } }

        public Chunk()
        {
        }

        public void Save()
        {
            Blocks.PackUp();
            SerializationUtils.SerializeToFile("World\\X" + X + "Y" + Y + "Z" + Z + ".bin", Blocks);
        }

        public bool ContainsLoc(long x, long y, long z)
        {
            return Blocks.ContainsLoc((byte)(x - X * DIAMETER), (byte)(y - Y * DIAMETER), (byte)(z - Z * DIAMETER));
        }

        public BlockTypes GetBlock(Point<byte> loc)
        {
            return GetBlock(loc.X, loc.Y, loc.Z);
        }

        public BlockTypes GetBlock(byte x, byte y, byte z)
        {
            return Blocks.GetBlock(x, y, z);
        }

        public void SetBlock(Point<byte> loc, BlockTypes b)
        {
            SetBlock(loc.X, loc.Y, loc.Z, b);
        }

        public void SetBlock(byte x, byte y, byte z, BlockTypes b)
        {
            var octree = Blocks.SetBlock(x, y, z, b);
            octree.Parent.Simplify();
        }

        public void ApplyPatch(List<Tuple<Point<byte>, BlockTypes>> changes)
        {
            foreach (var change in changes)
            {
                SetBlock(change.Item1, change.Item2);
            }
        }

        public static Point<long> GetChunkCoords(Point<long> loc)
        {
            return GetChunkCoords(loc.X, loc.Y, loc.Z);
        }

        public static Point<long> GetChunkCoords(long x, long y, long z)
        {
            var chunkX = x >> (DIAMETER_BITS - 1);
            var chunkY = y >> (DIAMETER_BITS - 1);
            var chunkZ = z >> (DIAMETER_BITS - 1);
            return new Point<long>(chunkX, chunkY, chunkZ);
        }

        public static Point<byte> GetBlockCoords(Point<long> loc)
        {
            return GetBlockCoords(loc.X, loc.Y, loc.Z);
        }

        public static Point<byte> GetBlockCoords(long x, long y, long z)
        {
            var blockX = x & (DIAMETER - 1);
            var blockY = y & (DIAMETER - 1);
            var blockZ = z & (DIAMETER - 1);
            return new Point<byte>((byte)blockX, (byte)blockY, (byte)blockZ);
        }
    }
}