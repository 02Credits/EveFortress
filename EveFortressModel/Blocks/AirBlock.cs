using EveFortressModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class AirBlock : Block
    {
        [ProtoMember(1)]
        public Entity Entity { get; set; }
        [ProtoMember(2)]
        public List<Item> Items { get; set; }

        public override TileDisplayInformation GetDisplayInfo()
        {
            return TerrainTiles.EmptySpace;
        }

        public override bool IsEmpty()
        {
            return true;
        }
    }
}
