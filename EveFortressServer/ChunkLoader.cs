using EveFortressModel;
using EveFortressModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EveFortressServer
{
    public class ChunkLoader
    {
        public const float NOISE_ROUGHNESS = 1f / 200f;
        public const byte NOISE_OCTAVES = 100;

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
                            var entity = Program.GetSystem<EntitySystemManager>().NewEntity(new Point<long>(worldX, worldY),
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
    }
}
