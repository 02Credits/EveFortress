using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    [ProtoContract]
    public class Entity
    {
        [ProtoMember(1)]
        public long ID { get; set; }
        [ProtoMember(2)]
        public Dictionary<Type, Component> Components { get; set; }
        [ProtoMember(3)]
        public Point<long> Position { get; set; }
        public List<Type> UpdatedComponents { get; set; }

        public Entity() { }

        public Entity(long id, Point<long> position)
        {
            ID = id;
            Position = position;
            Components = new Dictionary<Type, Component>();
            UpdatedComponents = new List<Type>();
        }

        public bool HasComponent<T>()
        {
            return Components.ContainsKey(typeof(T));
        }

        public bool HasComponent(Type type)
        {
            return Components.ContainsKey(type);
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)Components[typeof(T)];
        }

        public void GetComponent<T>(Action<T> callBack) where T : Component
        {
            callBack((T)Components[typeof(T)]);
        }

        public T GetComponentOrDefault<T>() where T : Component
        {
            Component returnValue;
            Components.TryGetValue(typeof(T), out returnValue);
            return (T)returnValue;
        }

        public void AddComponent(Component component)
        {
            var type = component.GetType();
            Components[type] = component;
            if (!UpdatedComponents.Contains(type))
            {
                UpdatedComponents.Add(type);
            }
        }

        public void UpdateComponent<T>(Func<T,T> function) where T : Component
        {
            var type = typeof(T);
            Components[type] = function((T)Components[type]);
            if (!UpdatedComponents.Contains(type))
            {
                UpdatedComponents.Add(type);
            }
        }

        public EntityPatch GetPatch()
        {
            return new EntityPatch 
                { 
                    ID = ID, 
                    Position = Position,
                    UpdatedComponents = Components.Values.Where((c) => UpdatedComponents.Contains(c.GetType())).ToList() 
                };
        }

        public void ApplyPatch(EntityPatch patch)
        {
            Position = patch.Position;
            foreach (var component in patch.UpdatedComponents)
            {
                Components[component.GetType()] = component;
            }
        }
    }

    [ProtoContract]
    public class EntityPatch
    {
        [ProtoMember(1)]
        public long ID { get; set; }

        [ProtoMember(2)]
        public Point<long> PreviousPosition { get; set; }

        [ProtoMember(2)]
        public Point<long> Position { get; set; }

        [ProtoMember(3)]
        public List<Component> UpdatedComponents { get; set; }
    }
}
