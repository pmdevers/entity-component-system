using System;
using System.Collections.Generic;
using System.Text;

using PMDEvers.EntityComponentSystem;

namespace EntityComponentSystem.Sample
{
    public class SampleSystem : EntitySystem
    {
        public override void Update(float delta)
        {
            foreach (var c in Registery.GetComponentsOf<SampleComponent>())
            {
                c.DeltaTime += delta;
                Console.WriteLine(c.DeltaTime);
            }

        }
    }
}
