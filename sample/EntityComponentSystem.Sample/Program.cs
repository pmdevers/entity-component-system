using System;

using PMDEvers.EntityComponentSystem;

namespace EntityComponentSystem.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var registery = new EntityRegistery();

            registery.AddSystem(new SampleSystem());

            var e = registery.Create();
            e.AddComponent(new SampleComponent());


            for (float delta = 0f; delta < 1000f; delta++)
            {
                registery.Update(delta);
            }
        }
    }
}
