using EveFortressModel;
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
        public byte X { get; set; }
        [ProtoMember(3)]
        public byte Y { get; set; }
        [ProtoMember(4)]
        public byte Z { get; set; }
        [ProtoMember(5)]
        public bool HasPosition { get; set; }

        public bool IsEmpty() { return Block.IsEmpty(); }

        public Voxel() { }
        public Voxel(Block block)
        {
            Block = block;
        }
        public Voxel(Block block, byte x, byte y, byte z)
            : this(block)
        {
            X = x;
            Y = y;
            Z = z;
            HasPosition = true;
        }
    }
}
