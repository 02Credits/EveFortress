using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EveFortressServer
{
    public class WorldManager : IUpdateNeeded, IDisposeNeeded
    {
        Dictionary<long, Dictionary<long, Chunk>> Chunks { get; set; }
        Dictionary<Tuple<long, long>, List<Voxel>> Patches { get; set; }

        public WorldManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            Chunks = SerializationUtils.DeserializeFileOrValue("Chunks.bin", new Dictionary<long, Dictionary<long, Chunk>>());
            Patches = new Dictionary<Tuple<long, long>, List<Voxel>>();
        }

        public void NotifyOfChanges(long x, long y, Voxel voxel)
        {
            
        }

        public void NotifyOfChanges(Tuple<long, long> loc, Voxel voxel)
        {
            if (!Patches.ContainsKey(loc))
                Patches[loc] = new List<Voxel>();
            Patches[loc].Add(voxel);
        }

        public Chunk GetChunk(long x, long y)
        {
            Chunk chunk;
            Dictionary<long, Chunk> chunksAtX;
            if (Chunks.TryGetValue(x, out chunksAtX))
            {
                if (chunksAtX.TryGetValue(y, out chunk))
                {
                    return chunk;
                }
                else
                {
                    chunk = new Chunk(x, y);
                    chunksAtX[y] = chunk;
                    return chunk;
                }
            }
            else
            {
                chunk = new Chunk(x, y);
                Chunks[x] = new Dictionary<long, Chunk>();
                Chunks[x][y] = chunk;
                return chunk;
            }
        }

        public void SetVoxel(long x, long y, byte z, Voxel voxel)
        {
            var chunkCoords = Chunk.GetChunkAtWorldCoords(x, y);
            var chunk = GetChunk(chunkCoords.Item1, chunkCoords.Item2);
            var voxelCoords = Chunk.GetVoxelCoords(x, y);
            chunk.SetVoxel(voxelCoords.Item1, voxelCoords.Item2, z, voxel);
            NotifyOfChanges(chunkCoords, voxel);
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
                        List<Voxel> patch;
                        if (Patches.TryGetValue(subscribedChunkLocation, out patch))
                        {
                            Program.ClientMethods.UpdateChunk(subscribedChunkLocation.Item1,
                                                              subscribedChunkLocation.Item2,
                                                              patch, connection);
                        }
                    }
                }
            }
            Patches.Clear();
        }

        public void Dispose()
        {
            SerializationUtils.SerializeToFile("Chunks.bin", Chunks);
        }
    }
}
