using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PMDEvers.EntityComponentSystem.Test")]
namespace PMDEvers.EntityComponentSystem
{
    internal struct EntityRecord : IEntityRecord
    {
        internal EntityRecord(string name, IEntityRegistery registry)
        {
            Name = name;
            Registery = registry;
        }

        public string Name { get; }
        public IEntityRegistery Registery { get; }
    }
}