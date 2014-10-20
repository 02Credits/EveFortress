using EveFortressModel;
using EveFortressModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EveFortressServer
{
    public class EntitySystemManager : IDisposeNeeded, IUpdateNeeded
    {
        public IDManager EntityIDManager { get; set; }
        public Dictionary<Type, EntitySystem> EntitySystems { get; set; }

        public EntitySystemManager()
        {
            EntitySystems = new Dictionary<Type, EntitySystem>();
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
            var entity = CreateEntity(position, components);

            foreach (var connection in Program.GetSystem<PlayerManager>().Connections.Values)
            {
                Program.GetSystem<ClientMethods>().SendNewEntity(entity, connection);
            }

            return entity;
        }

        public Entity NewEntity(Point<long> position, params Component[] components)
        {
            return CreateEntity(position, components);
        }

        public void HandleEntity(Entity entity)
        {
            var entityComponentTypes = entity.Components.Keys;
            foreach (var system in EntitySystems.Values)
            {
                foreach (var type in system.ComponentSubscriptions)
                {
                    if (entityComponentTypes.Contains(type))
                    {
                        system.Update(entity);
                        break;
                    }
                }
            }
        }

        private Entity CreateEntity(Point<long> position, Component[] components)
        {
            var entity = new Entity(EntityIDManager.GetNextID(), position);
            foreach (var component in components)
            {
                entity.AddOrUpdateComponent(component);
            }
            return entity;
        }

        public void Dispose()
        {
            SerializationUtils.SerializeToFile("CurrentEntityID.dat", EntityIDManager.CurrentID);
        }
    }
}
