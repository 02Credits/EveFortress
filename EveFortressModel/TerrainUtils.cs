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
        Dirt,
        Grass
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

        public static int GetSpriteIndex(bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            if ((topLeft && topRight && bottomLeft && bottomRight) ||
                (!topLeft && !topRight && !bottomLeft && !bottomRight))
                throw new ArgumentException("Use solid tileset instead");
            int returnIndex = -1;
            if (topLeft) returnIndex += 1;
            if (topRight) returnIndex += 2;
            if (bottomLeft) returnIndex += 4;
            if (bottomRight) returnIndex += 8;
            return returnIndex;
        }
    }
}
