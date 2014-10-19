using EveFortressModel;
using EveFortressModel.Components;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressServer
{
    public class ChunkManager : IUpdateNeeded, IDisposeNeeded
    {

        private Dictionary<Point<long>, Chunk> Chunks { get; set; }

        private Dictionary<Point<long>, List<Patch>> Patches { get; set; }

        public ChunkManager()
        {
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
                chunk = Program.GetSystem<ChunkLoader>().LoadChunk(loc);
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
                    Program.GetSystem<EntitySystemManager>().HandleEntity(entity);
                }
            }

            foreach (var name in Program.GetSystem<PlayerManager>().Players.Keys)
            {
                var player = Program.GetSystem<PlayerManager>().Players[name];
                if (Program.GetSystem<PlayerManager>().Connections.ContainsKey(name))
                {
                    var connection = Program.GetSystem<PlayerManager>().Connections[name];
                    foreach (var subscribedChunkLocation in player.SubscribedChunks)
                    {
                        List<Patch> patch;
                        if (Patches.TryGetValue(subscribedChunkLocation, out patch))
                        {
                            Program.GetSystem<ClientMethods>().UpdateChunk(subscribedChunkLocation,
                                                              patch, connection);
                        }
                    }
                }
            }
            Patches.Clear();
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