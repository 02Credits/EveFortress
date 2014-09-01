using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    [ProtoContract]
    public class Appearance : SyncedComponent
    {
        [ProtoMember(1)]
        public TileDisplayInformation DisplayInfo { get; set; }

        public Appearance() { }

        public Appearance(TileDisplayInformation displayInfo)
        {
            DisplayInfo = displayInfo;
        }
    }
}
