using System;
using System.Collections.Generic;
using System.Linq;

namespace PMDEvers.EntityComponentSystem
{
    public class EntityRegistery
    {
        private static readonly object locker = new object();

        private readonly IDictionary<Type, EntitySystem> _systems = new Dictionary<Type, EntitySystem>();

        private readonly IDictionary<EntityRecord, IDictionary<Type, Component>> _records =
            new Dictionary<EntityRecord, IDictionary<Type, Component>>();

        public IEnumerable<Component> this[EntityRecord record] => GetComponentsForRecord(record).Values;

        public int Count => _records.Count;

        public IEnumerable<EntityRecord> All()
        {
            return _records.Keys;
        }

        public EntityRecord FindByName(string name)
        {
            return _records.Keys.FirstOrDefault(x => x.Name == name);
        }

        public bool Contains(string name)
        {
            return _records.Keys.Any(x => x.Name == name);
        }

        public bool Contains(EntityRecord record)
        {
            return _records.ContainsKey(record);
        }

        public EntityRecord Create(string name)
        {
            var record = new EntityRecord(name, this);

            Add(record);

            return record;
        }

        public bool Add(EntityRecord record, Component component)
        {
            lock (locker)
            {
                var components = GetComponentsForRecord(record);

                if (component != null)
                {
                    var previousRecord = component.Record;
                    if (previousRecord != null)
                        if (!previousRecord.Equals(record))
                            Remove(previousRecord, component);

                    var key = component.GetType();
                    if (!components.ContainsKey(key))
                        components.Add(key, component);
                    if (component.Record == null || !component.Record.Equals(record))
                        component.Record = record;
                }
            }

            return true;
        }

        public bool Remove(EntityRecord record, Component component)
        {
            if (record == null || component == null)
                return false;

            if (component.Record != null && !component.Record.Equals(record))
                return false;

            var componentWasRemoved = false;
            var components = GetComponentsForRecord(record);

            lock (locker)
            {
                if (components.Count > 0)
                {
                    var key = component.GetType();
                    if (components.ContainsKey(key))
                        componentWasRemoved = components.Remove(key);
                }

                if (component.Record != null)
                    component.Record = null;
            }

            return componentWasRemoved;
        }

        public bool Remove(EntityRecord record)
        {
            if (record == null || !Contains(record))
                return false;

            bool recordDeleted = false;

            var components = GetComponentsForRecord(record);

            lock (locker)
            {
                if (components != null && components.Count > 0)
                {
                    var values = components.Values;
                    for (int i = 0; i < values.Count - 1; i++)
                    {
                        var component = values.ElementAt(i);
                        Remove(record, component);
                    }
                }

                recordDeleted = _records.Remove(record);
            }

            return recordDeleted;
        }

        public IEnumerable<T> GetComponentsOf<T>() where T : Component
        {
            var type = typeof(T);
            return _records.Values.Where(x => x.ContainsKey(type)).Select(x => x[type]).Cast<T>();
        }

        public TComponent GetComponent<TComponent>(EntityRecord record) where TComponent : Component
        {
            if (record == null || !Contains(record))
                return default;

            var components = GetComponentsForRecord(record);
            var result = default(TComponent);
            if (components.Count > 0)
            {
                var componentType = typeof(TComponent);

                if (components.ContainsKey(componentType))
                    result = (TComponent)components[componentType];
                else
                    foreach (var type in components.Keys)
                        if (typeof(TComponent).IsAssignableFrom(type))
                            return (TComponent)components[type];
            }

            return result;
        }

        public EntityRecord Create()
        {
            return Create(Guid.NewGuid().ToString());
        }

        public void Add(EntityRecord record)
        {
            Add(record, null);
        }

        public IEnumerable<Component> GetComponents(EntityRecord record)
        {
            return GetComponentsForRecord(record).Values;
        }

        public bool Add(EntitySystem system)
        {
            lock (locker)
            {
                var type = system.GetType();
                if(_systems.ContainsKey(type))
                    return false;

                system.Registery = this;
                _systems.Add(type, system);
                return true;
            }
        }

        public bool Contains(EntitySystem system)
        {
            return _systems.ContainsKey(system.GetType());
        }

        public bool Remove(EntitySystem system)
        {
            if (system == null)
                return false;

            lock (locker)
            {
                return _systems.Remove(system.GetType());
            }
        }

        public TSystem Get<TSystem>() where TSystem : EntitySystem
        {
            var result = default(TSystem);
            if (_systems.Count > 0)
            {
                var componentType = typeof(TSystem);

                if (_systems.ContainsKey(componentType))
                    result = (TSystem)_systems[componentType];
                else
                    foreach (var type in _systems.Keys)
                        if (typeof(TSystem).IsAssignableFrom(type))
                            return (TSystem)_systems[type];
            }

            return result;
        }

        public void Update(float delta)
        {
            var enumerator = _systems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var system = enumerator.Current;
                system.Value.Update(delta);
            }
            enumerator.Dispose();
        }
        
        private IDictionary<Type, Component> GetComponentsForRecord(EntityRecord record)
        {
            lock (locker)
            {
                if (!_records.ContainsKey(record))
                    _records[record] = new Dictionary<Type, Component>();

                return _records[record];
            }
        }
    }
}