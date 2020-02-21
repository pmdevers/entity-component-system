namespace PMDEvers.EntityComponentSystem
{
    public static class EntityRecordExtensions
    {
        public static TComponent GetComponent<TComponent>(this IEntityRecord record) where TComponent : class, IComponent
        {
            return record.Registery.GetComponent<TComponent>(record);
        }

        public static bool AddComponent(this IEntityRecord record, IComponent component)
        {
            return record.Registery.Add(record, component);
        }
    }
}