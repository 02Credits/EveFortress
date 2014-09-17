using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(2, typeof(Synced))]
    public abstract class Component
    {
        
    }
}
