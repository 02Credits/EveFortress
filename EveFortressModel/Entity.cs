using ProtoBuf;
using System.Collections.Generic;

namespace EveFortressModel
{
    [ProtoContract]
    [ProtoInclude(2, typeof(Item))]
    public abstract class Entity
    {
        [ProtoMember(1)]
        public Point<long> Position { get; set; }

        public Entity() { }

        public Entity(Point<long> position)
        {
            Position = position;
        }
        
        public abstract List<TileDisplayInformation> GetDisplayInfo();
    }
}
