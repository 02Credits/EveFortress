using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Appearance))]
    public class SyncedComponent : Component
    {
    }
}
