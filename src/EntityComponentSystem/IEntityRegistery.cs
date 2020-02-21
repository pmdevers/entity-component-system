using System.Collections.Generic;

namespace PMDEvers.EntityComponentSystem
{
    public interface IEntityRegistery
    {
        int Count { get; }

        IEnumerable<IEntityRecord> All();

        IEntityRecord FindByName(string name);

        IEntityRecord Create(string name);

        bool Add(IEntityRecord record, IComponent component);

        bool Remove(IEntityRecord record);

        bool Remove(IEntityRecord record, IComponent component);

        bool Contains(IEntityRecord record);

        TComponent GetComponent<TComponent>(IEntityRecord entityRecord) where TComponent : class, IComponent;

        IEnumerable<T> GetComponentsOf<T>() where T : class, IComponent;

        IEnumerable<IComponent> GetComponents(IEntityRecord record);
    }
}