using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel.Components
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Appearance))]
    [ProtoInclude(2, typeof(Mobile))]
    [ProtoInclude(3, typeof(ChunkLoading))]
    public abstract class Component
    {
        
    }
}
