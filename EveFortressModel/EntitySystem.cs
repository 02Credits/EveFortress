using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    public abstract class EntitySystem
    {
        public abstract List<Type> ComponentSubscriptions { get; }

        public abstract void Update(Entity entity);
    }
}
