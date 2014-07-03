using EveFortressModel;
using System;
using System.Collections.Generic;

namespace EveFortressServer
{
    public class WorldManager : IUpdateNeeded, IDisposeNeeded
    {
        Dictionary<Tuple<long, long, long>, Chunk> Chunks { get; set; } 
        Dictionary<Tuple<long, long, long>, List<Tuple<byte, byte, byte, BlockTypes>>> Patches { get; set; }

        public WorldManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            Chunks = new Dictionary<Tuple<long, long, long>, Chunk>();
            Patches = new Dictionary<Tuple<long, long, long>, List<Tuple<byte, byte, byte, BlockTypes>>>();
        }

        public void NotifyOfChanges(long x, long y, long z, BlockTypes block)
        {
            var loc = Tuple.Create(x, y, z);
            var blockPos = Chunk.GetBlockCoords(x, y, z);
            if (!Patches.ContainsKey(loc))
                Patches[loc] = new List<Tuple<byte, byte, byte, BlockTypes>>();
            Patches[loc].Add(Tuple.Create(blockPos.Item1, blockPos.Item2, blockPos.Item3, block));
        }

        public Chunk GetChunk(long x, long y, long z)
        {
            return GetChunk(Tuple.Create(x, y, z));
        }

        public Chunk GetChunk(Tuple<long, long, long> loc)
        {
            Chunk chunk;
            if (Chunks.TryGetValue(loc, out chunk))
            {
                return chunk;
            }
            else
            {
                chunk = new Chunk(loc);
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
                        List<Tuple<byte, byte, byte, BlockTypes>> patch;
                        if (Patches.TryGetValue(subscribedChunkLocation, out patch))
                        {
                            Program.ClientMethods.UpdateChunk(subscribedChunkLocation.Item1,
                                                              subscribedChunkLocation.Item2,
                                                              subscribedChunkLocation.Item3,
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
