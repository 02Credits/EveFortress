using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel.Components
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Synced))]
    [ProtoInclude(2, typeof(Appearance))]
    [ProtoInclude(3, typeof(Mobile))]
    public abstract class Component
    {
        
    }
}
