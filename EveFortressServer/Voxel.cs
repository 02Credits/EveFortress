using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class Voxel
    {
        [ProtoMember(1)]
        public Block Block { get; set; }
        [ProtoMember(2)]
        public Entity Entity { get; set; }
        [ProtoMember(3)]
        public List<Item> ItemsOnGround { get; set; }

        public Voxel()
        {
            ItemsOnGround = new List<Item>();
        }
    }
}
