using ProtoBuf;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressModel
{
    [ProtoContract]
    public class Chunk
    {
        public readonly static byte DIAMETER = 32;
        public readonly static byte TERRAIN_HEIGHT = 48;
        public readonly static byte TERRAIN_RANGE = 20;
        public readonly static float INVERSE_NOISE_ROUGHNESS = 200;

        [ProtoMember(1, AsReference=true)]
        public Octree Blocks { get; set; }
        [ProtoMember(2)]
        public long X { get; set; }
        [ProtoMember(3)]
        public long Y { get; set; }
        [ProtoMember(4)]
        public long Z { get; set; }
        public Tuple<long, long, long> Loc { get { return Tuple.Create(X, Y, Z); } }

        public Chunk() { }

        public Chunk(Tuple<long, long, long> loc)
            : this(loc.Item1, loc.Item2, loc.Item3) { }

        public Chunk(long chunkX, long chunkY, long chunkZ)
        {
            X = chunkX;
            Y = chunkY;
            Z = chunkZ;
            Blocks = SerializationUtils.DeserializeFileOrValue<Octree>("World\\X" + X + "Y" + Y + "Z" + Z + ".bin", null);
            if (Blocks == null)
            {
                Blocks = new Octree(null, 0, 0, 0, DIAMETER);

                Console.WriteLine("Generating chunk at " + chunkX + ", " + chunkY);
                for (byte x = 0; x < DIAMETER; x++)
                {
                    for (byte y = 0; y < DIAMETER; y++)
                    {
                        var worldX = chunkX * DIAMETER + x;
                        var worldY = chunkY * DIAMETER + y;
                        var noise = Utils.Noise.GetNoise((double)worldX / INVERSE_NOISE_ROUGHNESS, (double)worldY / INVERSE_NOISE_ROUGHNESS, 0);
                        var terrainLevel = (byte)((noise - 0.5f) * TERRAIN_RANGE + TERRAIN_HEIGHT);
                        for (byte z = 0; z <= DIAMETER; z++)
                        {
                            var worldZ = chunkZ * DIAMETER + z;
                            if (worldZ == terrainLevel || worldZ == terrainLevel - 1)
                            {
                                if (worldZ < TERRAIN_HEIGHT - 5)
                                {
                                    Blocks.SetBlock(x, y, z, BlockTypes.Sand);
                                }
                                else if (worldZ < TERRAIN_HEIGHT - 4)
                                {
                                    Blocks.SetBlock(x, y, z, BlockTypes.Dirt);
                                }
                                else if (worldZ < TERRAIN_HEIGHT + 7)
                                {
                                    Blocks.SetBlock(x, y, z, BlockTypes.Grass);
                                }
                                else
                                {
                                    Blocks.SetBlock(x, y, z, BlockTypes.Stone);
                                }
                            }
                            else if (worldZ <= terrainLevel)
                            {
                                Blocks.SetBlock(x, y, z, BlockTypes.Stone);
                            }
                            else
                            {
                                if (worldZ < TERRAIN_HEIGHT - 6)
                                {
                                    Blocks.SetBlock(x, y, z, BlockTypes.Water);
                                }
                            }
                        }
                    }
                }
                Blocks.Simplify();
            }
            else
            {
                Blocks.UnPack();
            }
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

        public BlockTypes GetBlock(Tuple<byte, byte, byte> loc)
        {
            return GetBlock(loc.Item1, loc.Item2, loc.Item3);
        }

        public BlockTypes GetBlock(byte x, byte y, byte z)
        {
            return Blocks.GetBlock(x, y, z);
        }

        public void SetBlock(Tuple<byte, byte, byte> loc, BlockTypes b)
        {
            SetBlock(loc.Item1, loc.Item2, loc.Item3, b);
        }

        public void SetBlock(byte x, byte y, byte z, BlockTypes b)
        {
            var octree = Blocks.SetBlock(x, y, z, b);
            octree.Parent.Simplify();
        }

        public void ApplyPatch(List<Tuple<byte, byte, byte, BlockTypes>> changes)
        {
            foreach (var change in changes)
            {
                SetBlock(change.Item1, change.Item2, change.Item3, change.Item4);
            }
        }

        public static Tuple<long, long, long> GetChunkCoords(long x, long y, long z)
        {
            var blockCoords = GetBlockCoords(x, y, z);
            var chunkX = (x - blockCoords.Item1) / DIAMETER;
            var chunkY = (y - blockCoords.Item2) / DIAMETER;
            var chunkZ = (z - blockCoords.Item3) / DIAMETER;
            return Tuple.Create(chunkX, chunkY, chunkZ);
        }

        public static Tuple<byte, byte, byte> GetBlockCoords(long x, long y, long z)
        {
            var blockX = x % DIAMETER;
            var blockY = y % DIAMETER;
            var blockZ = z % DIAMETER;
            if (blockX < 0) blockX += DIAMETER;
            if (blockY < 0) blockY += DIAMETER;
            if (blockZ < 0) blockZ += DIAMETER;
            return Tuple.Create((byte)blockX, (byte)blockY, (byte)blockZ);
        }
    }
}
