using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Item))]
    public abstract class Entity
    {
    }
}
