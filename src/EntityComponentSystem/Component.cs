using System;
using System.Collections.Generic;
using System.Text;

namespace PMDEvers.EntityComponentSystem
{
    public abstract class Component
    {
        public EntityRecord Record { get; set; }
    }
}
