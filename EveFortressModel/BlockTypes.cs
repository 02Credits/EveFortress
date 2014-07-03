using System;

namespace EveFortressModel
{
    public enum BlockTypes
    {
        Unknown,
        None,
        Dirt,
        Stone,
        Grass,
        Water,
        Sand
    }

    public static class BlockDisplayer
    {
        public static Random Random = new Random();
        public static TileDisplayInformation GetDisplayInfo(BlockTypes type)
        {
            switch (type)
            {
                case BlockTypes.Dirt:
                    return new TileDisplayInformation(TerrainTiles.Dirt, 150, 75, 0);
                case BlockTypes.Grass:
                    return new TileDisplayInformation(TerrainTiles.Dirt, 100, 255, 0);
                case BlockTypes.Sand:
                    return new TileDisplayInformation(TerrainTiles.Sand, 255, 255, 0);
                case BlockTypes.Stone:
                    return new TileDisplayInformation(TerrainTiles.SmoothStone, 125, 125, 125);
                case BlockTypes.Water:
                    return new TileDisplayInformation(TerrainTiles.Water, 
                        (byte)(90 + Random.Next(20)), 
                        (byte)(190 + Random.Next(20)), 
                        (byte)(235 + Random.Next(20)));
                case BlockTypes.Unknown:
                    return new TileDisplayInformation(TerrainTiles.Checkered, 10, 10, 10);
                default:
                    return null;
            }
        }
    }
}
