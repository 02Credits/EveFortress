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

        public WorldManager()
        {
            Chunks = SerializationUtils.DeserializeFileOrValue("Chunks.bin", new Dictionary<long, Dictionary<long, Chunk>>());
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

        public void Update()
        {
        }

        public void Dispose()
        {
            SerializationUtils.SerializeToFile("Chunks.bin", Chunks);
        }
    }
}
