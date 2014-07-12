using EveFortressModel;
using System;
using System.Collections.Generic;

namespace EveFortressServer
{
    public class WorldManager : IUpdateNeeded, IDisposeNeeded
    {
        Dictionary<Point<long>, Chunk> Chunks { get; set; } 
        Dictionary<Point<long>, List<Tuple<Point<byte>, BlockTypes>>> Patches { get; set; }

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

        public void Dispose()
        {
            foreach (var chunk in Chunks.Values)
            {
                chunk.Save();
            }
        }
    }
}
