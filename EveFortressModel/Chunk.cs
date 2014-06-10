using EveFortressModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class Chunk
    {
        public readonly static byte WIDTH = 10;
        public readonly static byte HEIGHT = 10;
        public readonly static byte DEPTH = 100;

        [ProtoMember(1)]
        public Voxel[] Voxels { get; set; }
        [ProtoMember(2)]
        public long X { get; set; }
        [ProtoMember(3)]
        public long Y { get; set; }

        public Chunk() { }

        public Chunk(long chunkX, long chunkY)
        {
            X = chunkX;
            Y = chunkY;
            Voxels = new Voxel[WIDTH * HEIGHT * DEPTH];

            Console.WriteLine("Generating chunk at " + chunkX + ", " + chunkY);
            for (byte x = 0; x < WIDTH; x++)
            {
                for (byte y = 0; y < HEIGHT; y++)
                {
                    for (byte z = 0; z < DEPTH; z++)
                    {
                        if (z == DEPTH / 2)
                        {
                            var worldX = chunkX * WIDTH + x;
                            var worldY = chunkY * HEIGHT + y;
                            var xAndYSquared = worldX * worldX + worldY * worldY;
                            if (xAndYSquared < 36)
                            {
                                SetVoxel(x, y, z, new Voxel(new GrassBlock()));
                            }
                            else
                            {
                                SetVoxel(x, y, z, new Voxel(new DirtBlock()));
                            }
                        }
                        else if (z <= DEPTH / 2)
                        {
                            SetVoxel(x, y, z, new Voxel(new StoneBlock()));
                        }
                        else
                        {
                            SetVoxel(x, y, z, new Voxel(new AirBlock()));
                        }
                    }
                }
            }
        }

        public Voxel GetVoxel(byte x, byte y, byte z)
        {
            return Voxels[z * WIDTH * HEIGHT + y * HEIGHT + x];
        }

        public void SetVoxel(byte x, byte y, byte z, Voxel v)
        {
            v.X = x;
            v.Y = y;
            v.Z = z;
            Voxels[z * WIDTH * HEIGHT + y * HEIGHT + x] = v;
        }

        public List<TileDisplayInformation> GetDisplayInfo(byte x, byte y, byte z)
        {
            var voxel = GetVoxel(x, y, z);
            if (voxel.IsEmpty())
            {
                if (z > 0)
                {
                    var lowerVoxel = GetVoxel(x, y, (byte)(z - 1));
                    var returnList = new List<TileDisplayInformation>();
                    if (lowerVoxel.IsEmpty()) returnList.Add(TerrainTiles.EmptySpace);
                    else returnList.Add(lowerVoxel.Block.GetDisplayInfo());
                    returnList.Add(voxel.Block.GetDisplayInfo());
                }
            }
            return new List<TileDisplayInformation> { voxel.Block.GetDisplayInfo() };
        }

        public void ApplyPatch(List<Voxel> changes)
        {
            foreach (var change in changes)
            {
                SetVoxel(change.X, change.Y, change.Z, change);
            }
        }

        public static Tuple<long, long> GetChunkAtWorldCoords(long x, long y)
        {
            var voxelCoords = GetVoxelCoords(x, y);
            var chunkX = (x - voxelCoords.Item1) / Chunk.WIDTH;
            var chunkY = (y - voxelCoords.Item2) / Chunk.HEIGHT;
            return Tuple.Create(chunkX, chunkY);
        }

        public static Tuple<byte, byte> GetVoxelCoords(long x, long y)
        {
            var voxelX = x % Chunk.WIDTH;
            var voxelY = y % Chunk.HEIGHT;
            if (voxelX < 0) voxelX += Chunk.WIDTH;
            if (voxelY < 0) voxelY += Chunk.HEIGHT;
            return Tuple.Create((byte)voxelX, (byte)voxelY);
        }
    }
}
