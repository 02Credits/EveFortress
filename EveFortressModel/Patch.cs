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
        public byte Change { get; set; }

        public Patch() { }

        public Patch(Point<byte> position, byte change)
        {
            Position = position;
            Change = change;
        }
    }
}