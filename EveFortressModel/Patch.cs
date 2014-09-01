using ProtoBuf;
using System.Collections.Generic;

namespace EveFortressModel
{
    [ProtoContract]
    public class Patch
    {
        [ProtoMember(1)]
        private Dictionary<Point<byte>, BlockTypes> Changes { get; set; }
    }
}