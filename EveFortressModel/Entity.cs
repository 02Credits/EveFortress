using EveFortressModel.Components;
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

        public Dictionary<Component, bool> Changes { get; set; }

        public Entity() { }

        public Entity(long id, Point<long> position)
        {
            ID = id;
            Position = position;
            Components = new Dictionary<Type, Component>();
            Changes = new Dictionary<Component, bool>();
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

        // State modifying Functions
        public void AddOrUpdateComponent(Component component)
        {
            var type = component.GetType();
            Components[type] = component;
            Changes[component] = true;
        }

        public void RemoveComponent(Component component)
        {
            Components.Remove(component.GetType());
            Changes[component] = false;
        }

        public void ApplyStateChanges(Dictionary<Component, bool> changes)
        {
            foreach (var component in changes.Keys)
            {
                if (changes[component])
                {
                    Components[component.GetType()] = component;
                }
                else
                {
                    Components.Remove(component.GetType());
                }
            }
        }
    }
}
