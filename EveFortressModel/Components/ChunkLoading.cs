using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel.Components
{
    [ProtoContract]
    public class ChunkLoading : Component
    {
        [ProtoMember(1)]
        public int SquareRadius { get; set; }

        public ChunkLoading() { }

        public ChunkLoading(int squareRadius)
        {
            SquareRadius = squareRadius;
        }
    }
}
