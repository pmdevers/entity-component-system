namespace PMDEvers.EntityComponentSystem
{
    public static class EntityRecordExtensions
    {
        public static TComponent GetComponent<TComponent>(this EntityRecord record) where TComponent : Component
        {
            return record.Registery.GetComponent<TComponent>(record);
        }

        public static bool AddComponent(this EntityRecord record, Component component)
        {
            return record.Registery.Add(record, component);
        }

        public static bool RemoveComponent(this EntityRecord record, Component component)
        {
            return record.Registery.Remove(record, component);
        }
    }
}