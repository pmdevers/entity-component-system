using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PMDEvers.EntityComponentSystem.Test")]
namespace PMDEvers.EntityComponentSystem
{
    public class EntityRecord
    {
        internal EntityRecord(string name, EntityRegistery registry)
        {
            Name = name;
            Registery = registry;
        }

        public string Name { get; }
        public EntityRegistery Registery { get; }
    }
}