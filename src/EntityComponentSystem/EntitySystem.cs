using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PMDEvers.EntityComponentSystem
{
    public abstract class EntitySystem
    {
        public EntityRegistery Registery {get; set; }

        public abstract void Update(float delta);
    }
}
