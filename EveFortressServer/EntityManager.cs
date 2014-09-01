using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EveFortressServer
{
    public class EntityManager : IUpdateNeeded, IDisposeNeeded
    {
        public IDManager EntityIDManager { get; set; }
        public Dictionary<Type, EntitySystem> EntitySystems { get; set; }
        public List<Entity> Entities { get; set; }

        public EntityManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            EntitySystems = new Dictionary<Type, EntitySystem>();
            Entities = SerializationUtils.DeserializeFileOrValue("Entities.dat", new List<Entity>());
            EntityIDManager = new IDManager(SerializationUtils.DeserializeFileOrValue("CurrentEntityID.dat", 0));
        }

        public void AddSystem(EntitySystem system)
        {
            foreach(var type in system.ComponentSubscriptions)
            {
                EntitySystems[type] = system;
            }
        }

        public Entity AddEntity(Point<long> position, params Component[] components)
        {
            var entity = new Entity(EntityIDManager.GetNextID(), position);
            foreach(var component in components)
            {
                entity.AddComponent(component);
            }

            foreach (var connection in Program.PlayerManager.Connections.Values)
            {
                Program.ClientMethods.SendEntities(new List<EntityPatch> { entity.GetInitialPatch() }, connection);
            }

            return entity;
        }

        public void Update()
        {
            foreach(var entity in Entities)
            {
                EntitySystems[entity.GetType()].Update(entity);
            }
        }

        public void Dispose()
        {
            SerializationUtils.SerializeToFile("CurrentEntityID.dat", EntityIDManager.CurrentID);
            SerializationUtils.SerializeToFile("Entities.dat", Entities);
        }
    }
}
