using System;
using System.Collections.Generic;
using System.Linq;

namespace PMDEvers.EntityComponentSystem
{
    public class EntityRegistery
    {
        private static readonly object locker = new object();

        private readonly IDictionary<Type, EntitySystem> _systems = new Dictionary<Type, EntitySystem>();

        private readonly IDictionary<EntityRecord, IDictionary<Type, Component>> _entitys =
            new Dictionary<EntityRecord, IDictionary<Type, Component>>();

        public IEnumerable<Component> this[EntityRecord entity] => GetComponentsForRecord(entity).Values;

        public int Count => _entitys.Count;

        public IEnumerable<EntityRecord> All()
        {
            return _entitys.Keys;
        }

        public EntityRecord FindByName(string name)
        {
            return _entitys.Keys.FirstOrDefault(x => x.Name == name);
        }

        public bool Contains(string name)
        {
            return _entitys.Keys.Any(x => x.Name == name);
        }

        public bool Contains(EntityRecord entity)
        {
            return _entitys.ContainsKey(entity);
        }

        public EntityRecord Create(string name)
        {
            var entity = new EntityRecord(name, this);

            Add(entity);

            return entity;
        }

        public bool Add(EntityRecord entity, Component component)
        {
            lock (locker)
            {
                var components = GetComponentsForRecord(entity);

                if (component != null)
                {
                    var previousRecord = component.Record;
                    if (previousRecord != null)
                        if (!previousRecord.Equals(entity))
                            Remove(previousRecord, component);

                    var key = component.GetType();
                    if (!components.ContainsKey(key))
                        components.Add(key, component);
                    if (component.Record == null || !component.Record.Equals(entity))
                        component.Record = entity;
                }
            }

            return true;
        }

        public bool Remove(EntityRecord entity, Component component)
        {
            if (entity == null || component == null)
                return false;

            if (component.Record != null && !component.Record.Equals(entity))
                return false;

            var componentWasRemoved = false;
            var components = GetComponentsForRecord(entity);

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

        public bool Remove(EntityRecord entity)
        {
            if (entity == null || !Contains(entity))
                return false;

            bool entityDeleted = false;

            var components = GetComponentsForRecord(entity);

            lock (locker)
            {
                if (components != null && components.Count > 0)
                {
                    var values = components.Values;
                    for (int i = 0; i < values.Count - 1; i++)
                    {
                        var component = values.ElementAt(i);
                        Remove(entity, component);
                    }
                }

                entityDeleted = _entitys.Remove(entity);
            }

            return entityDeleted;
        }

        public IEnumerable<T> GetComponentsOf<T>() where T : Component
        {
            var type = typeof(T);
            return _entitys.Values.Where(x => x.ContainsKey(type)).Select(x => x[type]).Cast<T>();
        }

        public TComponent GetComponent<TComponent>(EntityRecord entity) where TComponent : Component
        {
            if (entity == null || !Contains(entity))
                return default;

            var components = GetComponentsForRecord(entity);
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

        public void Add(EntityRecord entity)
        {
            Add(entity, null);
        }

        public IEnumerable<Component> GetComponents(EntityRecord entity)
        {
            return GetComponentsForRecord(entity).Values;
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
        
        private IDictionary<Type, Component> GetComponentsForRecord(EntityRecord entity)
        {
            lock (locker)
            {
                if (!_entitys.ContainsKey(entity))
                    _entitys[entity] = new Dictionary<Type, Component>();

                return _entitys[entity];
            }
        }
    }
}