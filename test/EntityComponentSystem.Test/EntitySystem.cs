using System;
using System.Collections.Generic;
using System.Text;

namespace PMDEvers.EntityComponentSystem.Test
{
    public class TestEntitySystem : EntitySystem
    {
        public override void Update(float delta)
        {
            Delta = delta;
        }

        public float Delta { get; private set; }
    }
}
