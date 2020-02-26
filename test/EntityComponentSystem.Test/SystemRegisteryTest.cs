using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Xunit;

namespace PMDEvers.EntityComponentSystem.Test
{
    public class SystemRegisteryTest
    {
        [Fact]
        public void Add_returns_false_if_already_added()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();
            
            var result = registery.Add(system);
            var result2 = registery.Add(system);

            Assert.True(result);
            Assert.False(result2);
        }

        [Fact]
        public void Add_Set_Registery_On_System()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();
            
            var result = registery.Add(system);
            

            Assert.True(result);
            Assert.Equal(registery, system.Registery);
        }

        [Fact]
        public void Contains_Returns_False_If_System_Is_Not_Added()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();

            Assert.False(registery.Contains(system));
        }

        [Fact]
        public void Contains_Returns_True_If_System_Is_Added()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();

            registery.Add(system);

            Assert.True(registery.Contains(system));
        }

        [Fact]
        public void Get_Returns_System_If_added()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();

            registery.Add(system);

            var result = registery.Get<TestEntitySystem>();

            Assert.Equal(system, result);
        }

        [Fact]
        public void Get_Returns_Default_If_System_Is_Not_Added()
        {
            var registery = new EntityRegistery();

            var result = registery.Get<TestEntitySystem>();

            Assert.Equal(default, result);

        }

        [Fact]
        public void Remove_Returns_True_If_System_Is_Removed()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();

            registery.Add(system);

            Assert.True(registery.Contains(system));
            Assert.True(registery.Remove(system));
            Assert.False(registery.Contains(system));
        }

        [Fact]
        public void Remove_Returns_False_If_System_Could_Not_Be_Removed()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();

            Assert.False(registery.Contains(system));
            Assert.False(registery.Remove(system));
        }

        static Random rnd = new Random();

        [Fact]
        public void Update_Calls_Update_On_Added_Systems()
        {
            var registery = new EntityRegistery();
            var system = new TestEntitySystem();
            var delta = (float)rnd.NextDouble();
            
            registery.Add(system);

            registery.Update(delta);

            Assert.Equal(delta, system.Delta);
        }
    }
}
