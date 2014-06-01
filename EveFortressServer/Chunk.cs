using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class Chunk
    {
        [ProtoMember(1)]
        public Voxel[, ,] Voxels { get; set; }
        [ProtoMember(2)]
        public long X { get; set; }
        [ProtoMember(3)]
        public long Y { get; set; }

        public Chunk() { }
    }
}
