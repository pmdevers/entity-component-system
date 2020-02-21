using System;
using System.Collections.Generic;
using System.Linq;

namespace PMDEvers.EntityComponentSystem
{
    public class EntityRegistery : IEntityRegistery
    {
        private static readonly object _locker = new object();

        private readonly IDictionary<IEntityRecord, IDictionary<Type, IComponent>> _records =
            new Dictionary<IEntityRecord, IDictionary<Type, IComponent>>();

        public IEnumerable<IComponent> this[IEntityRecord record] => GetComponentsForRecord(record).Values;

        public int Count => _records.Count;

        public IEnumerable<IEntityRecord> All()
        {
            return _records.Keys;
        }

        public IEntityRecord FindByName(string name)
        {
            return _records.Keys.FirstOrDefault(x => x.Name == name);
        }

        public bool Contains(string name)
        {
            return _records.Keys.Any(x => x.Name == name);
        }

        public bool Contains(IEntityRecord record)
        {
            return _records.ContainsKey(record);
        }

        public IEntityRecord Create(string name)
        {
            var record = new EntityRecord(name, this);

            Add(record);

            return record;
        }

        public bool Add(IEntityRecord record, IComponent component)
        {
            lock (_locker)
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

        public bool Remove(IEntityRecord record, IComponent component)
        {
            if (record == null || component == null)
                return false;

            if (component.Record != null && !component.Record.Equals(record))
                return false;

            var componentWasRemoved = false;
            var components = GetComponentsForRecord(record);

            lock (_locker)
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

        public bool Remove(IEntityRecord record)
        {
            if (record == null || !Contains(record))
                return false;

            bool recordDeleted = false;

            var components = GetComponentsForRecord(record);

            lock (_locker)
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

        public IEnumerable<T> GetComponentsOf<T>() where T : class, IComponent
        {
            var type = typeof(T);
            return _records.Values.Where(x => x.ContainsKey(type)).Select(x => x[type]).Cast<T>();
        }

        public TComponent GetComponent<TComponent>(IEntityRecord record) where TComponent : class, IComponent
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

        public IEntityRecord Create()
        {
            return Create(Guid.NewGuid().ToString());
        }

        public void Add(IEntityRecord record)
        {
            Add(record, null);
        }

        public IEnumerable<IComponent> GetComponents(IEntityRecord record)
        {
            return GetComponentsForRecord(record).Values;
        }

        private IDictionary<Type, IComponent> GetComponentsForRecord(IEntityRecord record)
        {
            lock (_locker)
            {
                if (!_records.ContainsKey(record))
                    _records[record] = new Dictionary<Type, IComponent>();

                return _records[record];
            }
        }
    }
}