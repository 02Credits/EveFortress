using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Synced))]
    [ProtoInclude(2, typeof(Appearance))]
    public abstract class Component
    {
        
    }
}
