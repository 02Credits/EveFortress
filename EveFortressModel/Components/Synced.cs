using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    public class Synced : Component
    {
        public IEnumerable<Type> SyncedComponents { get; set; }

        public Synced(params Type[] syncedComponents)
        {
            SyncedComponents = syncedComponents;
        }
    }
}
