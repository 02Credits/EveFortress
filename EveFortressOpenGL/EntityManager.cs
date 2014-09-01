using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class EntityManager : IDisposeNeeded
    {
        public Dictionary<long, Entity> Entities { get; set; }

        public EntityManager()
        {
            Entities = new Dictionary<long, Entity>();
        }

        public void AddEntities(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                Entities[entity.ID] = entity;
            }
        }

        public void PatchEntity(EntityPatch patch)
        {
            Entities[patch.ID].ApplyPatch(patch);
        }

        public void Dispose()
        {
            Entities.Clear();
        }
    }
}
