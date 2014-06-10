using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class GrassBlock : Block
    {
        public override TileDisplayInformation GetDisplayInfo()
        {
            return new TileDisplayInformation(TerrainTiles.Dirt, 100, 255, 0);
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
