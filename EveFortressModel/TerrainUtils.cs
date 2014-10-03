using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    public enum TerrainType
    {
        Water,
        Grass,  
        Dirt,
    }

    public static class TerrainUtils
    {
        public static Dictionary<byte, TerrainType> LevelTypes = new Dictionary<byte, TerrainType>();

        public static Dictionary<TerrainType, byte> TypeLevelStarts = new Dictionary<TerrainType, byte>
        {
            { TerrainType.Water, 0},
            { TerrainType.Dirt, 100},
            { TerrainType.Grass, 115}
        };

        public static Dictionary<TerrainType, bool> Walkable = new Dictionary<TerrainType, bool>
        {
            { TerrainType.Water, false },
            { TerrainType.Dirt, true },
            { TerrainType.Grass, true },
        };

        public static Dictionary<TerrainType, string> Names = new Dictionary<TerrainType, string>();

        static TerrainUtils()
        {
            foreach(var key in TypeLevelStarts.Keys)
            {
                LevelTypes[TypeLevelStarts[key]] = key;
            }

            // fill in remaining lookup slots
            TerrainType CurrentType = LevelTypes[0];
            for (int i = 0; i <= 255; i++)
            {
                if (LevelTypes.ContainsKey((byte)i))
                {
                    CurrentType = LevelTypes[(byte)i];
                }
                else
                {
                    LevelTypes[(byte)i] = CurrentType;
                }
            }

            // fill in names
            var types = Enum.GetValues(typeof(TerrainType));
            foreach(var obj in types)
            {
                var type = (TerrainType)obj;
                Names[type] = Enum.GetName(typeof(TerrainType), type);
            }
        }

        static Dictionary<int, int> indexLookup = new Dictionary<int, int>
        {
            {1, 5},
            {2, 9},
            {3, 1},
            {4, 11},
            {5, 6},
            {6, 4},
            {7, 0},
            {8, 3},
            {9, 10},
            {10, 8},
            {11, 2},
            {12, 13},
            {13, 12},
            {14, 14},
        };

        public static int GetSpriteIndex(IEnumerable<int> corners)
        {
            var returnIndex = corners.Sum();
            if (returnIndex == 0 ||
                returnIndex == 15)
                throw new ArgumentException("Use solid tileset instead");

            return indexLookup[returnIndex];
        }
    }
}
