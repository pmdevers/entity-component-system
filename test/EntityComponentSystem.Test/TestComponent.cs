using System;
using System.Collections.Generic;
using System.Text;

namespace PMDEvers.EntityComponentSystem.Test
{
    public class TestComponent : IComponent
    {
        public IEntityRecord Record { get; set; }
    }

    public class TestComponent2 : IComponent
    {
        public IEntityRecord Record { get; set; }
    }

    public class InheritedComponent : TestComponent
    {

    }
}
