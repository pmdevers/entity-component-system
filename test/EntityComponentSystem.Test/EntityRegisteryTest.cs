using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Xunit;

namespace PMDEvers.EntityComponentSystem.Test
{
    public class EntityRegisteryTest
    {
        private static Random rnd = new Random();

        [Fact]
        public void Create_Returns_Entity_A_Valid_Entity()
        {
            var registery = new EntityRegistery();
            var entityId = registery.Create();
            Assert.True(Guid.TryParse(entityId.Name, out Guid _));
            Assert.Equal(registery, entityId.Registery);
        }

        [Fact]
        public void Create_With_Name_Returns_Entity_A_Valid_Entity()
        {
            var registery = new EntityRegistery();
            var randomName = Guid.NewGuid().ToString();
            var entityId = registery.Create(randomName);
            Assert.Equal(randomName, entityId.Name);
            Assert.Equal(registery, entityId.Registery);
        }

        [Fact]
        public void Find_Returns_The_Correct_Entity()
        {
            var registery = new EntityRegistery();
            
            for (int i = 0; i < 1000; i++)
            {
                registery.Create("Entity-" + i);
            }

            for (int i = 0; i < 100; i++)
            {
                var name = "Entity-" + rnd.Next(0, 1000);
                var entity = registery.FindByName(name);

                Assert.Equal(name, entity.Name);
                Assert.Equal(registery, entity.Registery);
            }
        }

        [Fact]
        public void Count_Returns_Number_Of_EntityRecords()
        {
            var registery = new EntityRegistery();
            var entities = rnd.Next(0, 1000);
            
            for (int i = 0; i < entities; i++)
            {
                registery.Create("Entity-" + i);
            }

            Assert.Equal(entities, registery.Count);
        }

        [Fact]
        public void Add_Sets_Component_EntityRecord()
        {
            var registery = new EntityRegistery();
            var record = registery.Create("Test Entity");
            
            var component = new TestComponent();
            registery.Add(record, component);

            Assert.Equal(record, component.Record);
        }

        [Fact]
        public void Remove_Returns_False_If_Component_Is_Not_From_Entity()
        {
            var registery = new EntityRegistery();
            var record1 = registery.Create("Test Entity 1");
            var record2 = registery.Create("Test Entity 2");
            var component1 = new TestComponent();
            record1.AddComponent(component1);
            record2.AddComponent(new TestComponent2());

            Assert.False(registery.Remove(record2, component1));
        }

        [Fact]
        public void Remove_Returns_False_If_Component_Is_NULL()
        {
            var registery = new EntityRegistery();
            var record1 = registery.Create();
            var component1 = new TestComponent();
            record1.AddComponent(component1);
            
            Assert.False(registery.Remove(record1, null));
        }

        [Fact]
        public void Remove_Returns_False_If_Entity_Is_NULL()
        {
            var registery = new EntityRegistery();
            var record1 = registery.Create();
            var component1 = new TestComponent();
            record1.AddComponent(component1);
            
            Assert.False(registery.Remove(null, null));
            Assert.False(registery.Remove((EntityRecord)null));
        }

        [Fact]
        public void All_Returns_All_Entities()
        {
            var registery = new EntityRegistery();
            var entities = rnd.Next(0, 1000);
            
            for (int i = 0; i < entities; i++)
            {
                registery.Create("Entity-" + i);
            }

            Assert.Equal(entities, registery.All().Count());
        }

        [Fact]
        public void Get_Component_Gets_Added_Components()
        {
            var registery = new EntityRegistery();
            var record = registery.Create("Test Entity");
            
            var component = new TestComponent();
            registery.Add(record, component);

            var registeryComponent = registery.GetComponent<TestComponent>(record);
            Assert.Equal(component, registeryComponent);

            var entityComponent = record.GetComponent<TestComponent>();
            Assert.Equal(component, entityComponent);
        }

        [Fact]
        public void GetComponent_Null_returns_default()
        {
            var registery = new EntityRegistery();
            var result = registery.GetComponent<TestComponent>(null);
            Assert.Null(result);
        }

        [Fact]
        public void Contains_Should_Return_True_If_In_Registry()
        {
            var registery = new EntityRegistery();
            var record = registery.Create("Test Enntity");
            
            Assert.True(registery.Contains(record.Name));
            Assert.True(registery.Contains(record));
        }

        [Fact]
        public void Remove_Removes_A_Component_From_An_Entity()
        {
            var registery = new EntityRegistery();
            var record = registery.Create("Test Entity");
            registery.Add(record, new TestComponent());

            var component = record.GetComponent<TestComponent>();
            Assert.NotNull(record.GetComponent<TestComponent>());
            Assert.True(registery.Remove(record, component));
            Assert.Null(record.GetComponent<TestComponent>());
        }

        [Fact]
        public void Remove_Removes_An_Entity_From_The_Registery()
        {
            var registery = new EntityRegistery();
            var entityName = Guid.NewGuid().ToString();
            var record = registery.Create(entityName);
            record.AddComponent(new TestComponent());
            record.AddComponent(new TestComponent2());
            

            Assert.NotNull(registery.FindByName(entityName));

            Assert.True(registery.Remove(record));

            Assert.Null(registery.FindByName(entityName));
        }

        [Fact]
        public void Remove_Returns_True_If_Exists()
        {
            var registery = new EntityRegistery();
            var entityName = Guid.NewGuid().ToString();
            var record = registery.Create(entityName);
            
            Assert.True(registery.Remove(record));
        }

        [Fact]
        public void Remove_Returns_False_If_Not_Exists()
        {
            var registery = new EntityRegistery();
            var entityName = Guid.NewGuid().ToString();
            var record = new EntityRecord(entityName, registery);
            Assert.False(registery.Remove(record));
        }

        [Fact]
        public void GetComponents_Returns_All_Components_For_An_Entity()
        {
            var registery = new EntityRegistery();
            var record = registery.Create();
            record.AddComponent(new InheritedComponent());
            record.AddComponent(new TestComponent());
            record.AddComponent(new TestComponent2());
            var components = registery.GetComponents(record);
            Assert.Equal(3, components.Count());
        }

        [Fact]
        public void GetComponentOfT_Returns_Inheritance()
        {
            var registery = new EntityRegistery();
            var record = registery.Create();
            var component = new InheritedComponent();
            record.AddComponent(component);
            
            var testComponent = registery.GetComponent<TestComponent>(record);

            Assert.Equal(component, testComponent);
        }

        [Fact]
        public void Index_Returns_Entity()
        {
            var registery = new EntityRegistery();
            var record = registery.Create();
            var component = new TestComponent();

            record.AddComponent(component);

            Assert.Contains(component, registery[record]);
        }

        [Fact]
        public void RemoveComponent_Removes_Component_From_Entity()
        {
            var registery = new EntityRegistery();
            var record = registery.Create();
            var component = new TestComponent();
            record.AddComponent(component);

            Assert.NotNull(record.GetComponent<TestComponent>());
            Assert.True(record.RemoveComponent(component));

            Assert.Null(record.GetComponent<TestComponent>());
            Assert.False(record.RemoveComponent(component));
        }

        [Fact]
        public void GetComponentsOf_Returns_Correct_Number_Of_Components()
        {
            var registery = new EntityRegistery();
            
            var record = registery.Create();
            var record1 = registery.Create();
            var record2 = registery.Create();
            
            var component = new TestComponent();
            record.AddComponent(component);
            record.AddComponent(new TestComponent2());
            record1.AddComponent(component);
            record1.AddComponent(new TestComponent2());
            record2.AddComponent(component);
            record2.AddComponent(new TestComponent2());
            
            var testComponents = registery.GetComponentsOf<TestComponent>();
            var test2Components = registery.GetComponentsOf<TestComponent2>();
            
            Assert.Single(testComponents);
            Assert.Equal(3, test2Components.Count());
        }
    }
}
