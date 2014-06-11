using EveFortressModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    [ProtoContract]
    public class DirtBlock : Block
    {
        public override TileDisplayInformation GetDisplayInfo()
        {
            var baseTile = new TileDisplayInformation(TerrainTiles.Dirt, 150, 75, 0);
            return baseTile;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
