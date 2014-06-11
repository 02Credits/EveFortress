using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ChunkManager : IUpdateNeeded, IResetNeeded
    {
        Dictionary<long, Dictionary<long, Chunk>> SubscribedChunks { get; set; }
        List<Tuple<long, long>> ChunkLocationsToSubscribeTo { get; set; }

        public ChunkManager()
        {
            SubscribedChunks = new Dictionary<long, Dictionary<long, Chunk>>();
            ChunkLocationsToSubscribeTo = new List<Tuple<long, long>>();
            Game.Updateables.Add(this);
            Game.Resetables.Add(this);
        }

        public List<List<TileDisplayInformation>> GetTiles(long left, long right, long top, long bottom, byte z)
        {
            var returnList = new List<List<TileDisplayInformation>>();
            for (long y = top; y <= bottom; y++)
            {
                for (long x = left; x <= right; x++)
                {
                    var chunkPos = Chunk.GetChunkAtWorldCoords(x, y);
                    var chunk = GetChunk(chunkPos.Item1, chunkPos.Item2);

                    if (chunk != null)
                    {
                        var voxelPos = Chunk.GetVoxelCoords(x, y);
                        returnList.Add(chunk.GetDisplayInfo(voxelPos.Item1, voxelPos.Item2, z));
                        continue;
                    }

                    if (!ChunkLocationsToSubscribeTo.Any((t) => t.Item1 == chunkPos.Item1 && t.Item2 == chunkPos.Item2))
                        ChunkLocationsToSubscribeTo.Add(chunkPos);

                    returnList.Add(new List<TileDisplayInformation> { TerrainTiles.None });
                }
            }
            return returnList;
        }

        Dictionary<Tuple<long, long>, DateTime> lastTimeFetched = new Dictionary<Tuple<long, long>, DateTime>();
        public Chunk GetChunk(long x, long y)
        {
            Chunk chunk = null;
            Dictionary<long, Chunk> chunksAtXPos;
            if (SubscribedChunks.TryGetValue(x, out chunksAtXPos))
            {
                chunksAtXPos.TryGetValue(y, out chunk);
            }
            if (chunk != null)
            {
                lastTimeFetched[Tuple.Create(x, y)] = DateTime.Now;
            }
            return chunk;
        }

        bool subscribing;
        public async void Update()
        {
            if (!subscribing)
            {
                subscribing = true;
                foreach (var chunkLocation in ChunkLocationsToSubscribeTo.ToList())
                {
                    var x = chunkLocation.Item1;
                    var y = chunkLocation.Item2;
                    if (!SubscribedChunks.ContainsKey(x))
                        SubscribedChunks[x] = new Dictionary<long, Chunk>();
                    SubscribedChunks[x][y] = await Game.ServerMethods.SubscribeToChunk(x, y);
                    ChunkLocationsToSubscribeTo.Remove(chunkLocation);
                }
                subscribing = false;
            }

            foreach (var x in SubscribedChunks.Keys.ToList())
            {
                foreach (var y in SubscribedChunks[x].Keys.ToList())
                {
                    DateTime timeFetched;
                    if (lastTimeFetched.TryGetValue(Tuple.Create(x, y), out timeFetched))
                    {
                        if (DateTime.Now - timeFetched > TimeSpan.FromSeconds(30))
                        {
                            SubscribedChunks[x].Remove(y);
                            await Game.ServerMethods.UnsubscribeToChunk(x, y);
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            SubscribedChunks.Clear();
            ChunkLocationsToSubscribeTo.Clear();
            subscribing = false;
        }
    }
}
