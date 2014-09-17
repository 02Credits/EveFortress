using EveFortressModel;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressServer
{
    public class WorldManager : IUpdateNeeded, IDisposeNeeded
    {
        public const float NOISE_ROUGHNESS = 1f / 200f;
        public const byte NOISE_OCTAVES = 100;

        private Dictionary<Point<long>, Chunk> Chunks { get; set; }

        private Dictionary<Point<long>, List<Patch>> Patches { get; set; }

        public WorldManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            Chunks = new Dictionary<Point<long>, Chunk>();
            Patches = new Dictionary<Point<long>, List<Patch>>();
        }

        public void NotifyOfChanges(long x, long y, byte terrainType)
        {
            var loc = new Point<long>(x, y);
            var blockPos = Chunk.GetBlockCoords(x, y);
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

            chunk.TerrainLevel = new byte[Chunk.DIAMETER * Chunk.DIAMETER];

            for (byte x = 0; x < Chunk.DIAMETER; x++)
            {
                for (byte y = 0; y < Chunk.DIAMETER; y++)
                {
                    var worldX = chunk.X * Chunk.DIAMETER + x;
                    var worldY = chunk.Y * Chunk.DIAMETER + y;

                    var noise = noiseGen.GetNoise(worldX, worldY, 0);
                    chunk.TerrainLevel[x * Chunk.DIAMETER + y] = (byte)(noise * 255);
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