using EveFortressModel;
using EveFortressModel.Components;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressServer
{
    public class ChunkManager : IUpdateNeeded, IDisposeNeeded
    {
        public const float NOISE_ROUGHNESS = 1f / 200f;
        public const byte NOISE_OCTAVES = 100;

        private Dictionary<Point<long>, Chunk> Chunks { get; set; }

        private Dictionary<Point<long>, List<Patch>> Patches { get; set; }

        public ChunkManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            Chunks = new Dictionary<Point<long>, Chunk>();
            Patches = new Dictionary<Point<long>, List<Patch>>();
        }

        public void NotifyOfChanges(long x, long y, TerrainType terrainType)
        {
            var loc = new Point<long>(x, y);
            var blockPos = Chunk.GetLocalCoords(x, y);
            if (!Patches.ContainsKey(loc))
                Patches[loc] = new List<Patch>();
            Patches[loc].Add(new Patch(blockPos, terrainType));
        }

        public Chunk GetChunk(long x, long y)
        {
            return GetChunk(new Point<long>(x, y));
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
            foreach (var chunk in Chunks.Values)
            {
                foreach (var entity in chunk.Entities.Values)
                {
                    Program.EntityManager.HandleEntity(entity);
                }
            }

            foreach (var name in Program.PlayerManager.Players.Keys)
            {
                var player = Program.PlayerManager.Players[name];
                if (Program.PlayerManager.Connections.ContainsKey(name))
                {
                    var connection = Program.PlayerManager.Connections[name];
                    foreach (var subscribedChunkLocation in player.SubscribedChunks)
                    {
                        List<Patch> patch;
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

        NoiseGen noiseGen = new NoiseGen(NOISE_ROUGHNESS, NOISE_OCTAVES);
        public Chunk LoadChunk(Point<long> loc)
        {
            var chunk = new Chunk();
            chunk.X = loc.X;
            chunk.Y = loc.Y;

            chunk.Level = new TerrainType[Chunk.DIAMETER * Chunk.DIAMETER];
            chunk.RandomSelection = new byte[Chunk.DIAMETER * Chunk.DIAMETER];
            chunk.Entities = new Dictionary<long, Entity>();

            for (byte x = 0; x < Chunk.DIAMETER; x++)
            {
                for (byte y = 0; y < Chunk.DIAMETER; y++)
                {
                    var worldX = chunk.X * Chunk.DIAMETER + x;
                    var worldY = chunk.Y * Chunk.DIAMETER + y;

                    var noise = noiseGen.GetNoise(worldX, worldY, 0);
                    var terrainType = TerrainUtils.LevelTypes[(byte)(noise * 255)];
                    chunk.Level[x * Chunk.DIAMETER + y] = terrainType;
                    chunk.RandomSelection[x * Chunk.DIAMETER + y] = (byte)Program.Random.Next(256);
                    if (terrainType == TerrainType.Grass)
                    {
                        if (Program.Random.Next(100) <= 1)
                        {
                            var entity = Program.EntityManager.NewEntity(new Point<long>(worldX, worldY),
                                new Appearance(new TileDisplayInformation("Tree", 0)),
                                new Mobile(),
                                new Synced());
                            chunk.Entities[entity.ID] = entity;
                        }
                    }
                }
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