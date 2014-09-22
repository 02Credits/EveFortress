using ProtoBuf;
using System.Collections.Generic;

namespace EveFortressModel
{
    [ProtoContract]
    public class Patch
    {
        [ProtoMember(1)]
        public Point<byte> Position { get; set; }

        [ProtoMember(2)]
        public TerrainType Change { get; set; }

        public Patch() { }

        public Patch(Point<byte> position, TerrainType change)
        {
            Position = position;
            Change = change;
        }
    }
}