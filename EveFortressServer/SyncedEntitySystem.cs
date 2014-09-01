using EveFortressModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class SyncedEntitySystem : EntitySystem
    {
        public override List<Type> ComponentSubscriptions
        {
            get { return new List<Type> { typeof(Synced) }; }
        }

        public override void Update(Entity entity)
        {
            var synced = entity.GetComponent<Synced>();
            var patchedComponents = new List<SyncedComponent>();
            foreach (var componentType in synced.SyncedComponents)
            {
                patchedComponents.Add((SyncedComponent)entity.Components[componentType]);
            }
            entity.UpdatedComponents.Clear();

            if (patchedComponents.Count > 0)
            {
                var patch = new EntityPatch
                {
                    ID = entity.ID,
                    Position = entity.Position,
                    PatchedComponents = patchedComponents
                };

                foreach (var client in Program.PlayerManager.Connections.Values)
                {
                    Program.ClientMethods.PatchEntity(patch, client);
                }
            }
        }
    }
}
