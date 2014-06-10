using EveFortressModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AirBlock))]
    [ProtoInclude(2, typeof(DirtBlock))]
    [ProtoInclude(3, typeof(StoneBlock))]
    [ProtoInclude(4, typeof(GrassBlock))]
    public abstract class Block
    {
        public abstract TileDisplayInformation GetDisplayInfo();
        public abstract bool IsEmpty();
    }
}
