using EveFortressModel;
using EveFortressModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class MobileEntitySystem : EntitySystem
    {
        public override List<Type> ComponentSubscriptions
        {
            get { return new List<Type> { typeof(Mobile) }; }
        }

        public override void Update(Entity entity)
        {
            var mobile = entity.GetComponent<Mobile>();
            entity.Position = new Point<long>(entity.Position.X, entity.Position.Y);
        }
    }
}
