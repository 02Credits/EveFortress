using EveFortressModel;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressServer
{
    public class WorldManager : IUpdateNeeded, IDisposeNeeded
    {
        private Dictionary<Point<long>, Chunk> Chunks { get; set; }

        private Dictionary<Point<long>, List<Tuple<Point<byte>, BlockTypes>>> Patches { get; set; }

        public WorldManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            Chunks = new Dictionary<Point<long>, Chunk>();
            Patches = new Dictionary<Point<long>, List<Tuple<Point<byte>, BlockTypes>>>();
        }

        public void NotifyOfChanges(long x, long y, long z, BlockTypes block)
        {
            var loc = new Point<long>(x, y, z);
            var blockPos = Chunk.GetBlockCoords(x, y, z);
            if (!Patches.ContainsKey(loc))
                Patches[loc] = new List<Tuple<Point<byte>, BlockTypes>>();
            Patches[loc].Add(Tuple.Create(blockPos, block));
        }

        public Chunk GetChunk(long x, long y, long z)
        {
            return GetChunk(new Point<long>(x, y, z));
        }

        public Chunk GetChunk(Point<long> loc)
        {
            Chunk chunk;
            if (Chunks.TryGetValue(loc, out chunk))
            {
                return chunk;
            }
            else
            {
                chunk = LoadChunk(loc);
                Chunks[loc] = chunk;
                return chunk;
            }
        }

        public void Update()
        {
            foreach (var name in Program.PlayerManager.Players.Keys)
            {
                var player = Program.PlayerManager.Players[name];
                if (Program.PlayerManager.Connections.ContainsKey(name))
                {
                    var connection = Program.PlayerManager.Connections[name];
                    foreach (var subscribedChunkLocation in player.SubscribedChunks)
                    {
                        List<Tuple<Point<byte>, BlockTypes>> patch;
                        if (Patches.TryGetValue(subscribedChunkLocation, out patch))
                        {
                            Program.ClientMethods.UpdateChunk(subscribedChunkLocation,
                                                              patch, connection);
                        }
                    }
                }
            }
            Patches.Clear();
        }

        public Chunk LoadChunk(Point<long> loc)
        {
            var chunk = new Chunk();
            chunk.X = loc.X;
            chunk.Y = loc.Y;
            chunk.Z = loc.Z;
            chunk.Blocks = SerializationUtils.DeserializeFileOrValue<Octree>(
                "World\\X" + chunk.X + "Y" + chunk.Y + "Z" + chunk.Z + ".bin", null);

            if (chunk.Blocks == null)
            {
                chunk.Blocks = new Octree(null, 0, 0, 0, Chunk.DIAMETER);

                Console.WriteLine("Generating chunk at " + chunk.X + ", " + chunk.Y);

                for (byte x = 0; x < Chunk.DIAMETER; x++)
                {
                    for (byte y = 0; y < Chunk.DIAMETER; y++)
                    {
                        var worldX = chunk.X * Chunk.DIAMETER + x;
                        var worldY = chunk.Y * Chunk.DIAMETER + y;
                        var noise = Utils.Noise.GetNoise((double)worldX / Chunk.INVERSE_NOISE_ROUGHNESS, (double)worldY / Chunk.INVERSE_NOISE_ROUGHNESS, 0);
                        var terrainLevel = (byte)((noise - 0.5f) * Chunk.TERRAIN_RANGE + Chunk.TERRAIN_HEIGHT);
                        for (byte z = 0; z <= Chunk.DIAMETER; z++)
                        {
                            var worldZ = chunk.Z * Chunk.DIAMETER + z;
                            if (worldZ == terrainLevel || worldZ == terrainLevel - 1)
                            {
                                if (worldZ < Chunk.TERRAIN_HEIGHT - 5)
                                {
                                    chunk.Blocks.SetBlock(x, y, z, BlockTypes.Sand);
                                }
                                else if (worldZ < Chunk.TERRAIN_HEIGHT - 4)
                                {
                                    chunk.Blocks.SetBlock(x, y, z, BlockTypes.Dirt);
                                }
                                else if (worldZ < Chunk.TERRAIN_HEIGHT + 7)
                                {
                                    chunk.Blocks.SetBlock(x, y, z, BlockTypes.Grass);
                                    var randomValue = Program.Random.Next(100);
                                    if (randomValue > 95)
                                    {
                                        Program.EntityManager.AddEntity(
                                            new Point<long>(worldX, worldY, worldZ),
                                            new Appearance(
                                                new TileDisplayInformation(EntityTiles.PalmTree, (byte)0, (byte)255, (byte)0)),
                                            new Synced(typeof(Appearance)));
                                    }
                                }
                                else
                                {
                                    chunk.Blocks.SetBlock(x, y, z, BlockTypes.Stone);
                                }
                            }
                            else if (worldZ <= terrainLevel)
                            {
                                chunk.Blocks.SetBlock(x, y, z, BlockTypes.Stone);
                            }
                            else
                            {
                                if (worldZ < Chunk.TERRAIN_HEIGHT - 6)
                                {
                                    chunk.Blocks.SetBlock(x, y, z, BlockTypes.Water);
                                }
                            }
                        }
                    }
                }
                chunk.Blocks.Simplify();
            }
            else
            {
                chunk.Blocks.UnPack();
            }

            return chunk;
        }

        public void Dispose()
        {
            foreach (var chunk in Chunks.Values)
            {
                chunk.Save();
            }
        }
    }
}