using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    public class StoneBlock : Block
    {
        public override TileDisplayInformation GetDisplayInfo()
        {
            return new TileDisplayInformation(TerrainTiles.SmoothStone, 125, 125, 125);
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
