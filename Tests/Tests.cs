using System.Collections.Generic;
using System.Linq;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Interfaces;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Tests : TestBase
    {

        [Test]
        public void CanAddItemToGroup()
        {
            var item = CreateItem();
            var group = CreateGroup();
            Assert.AreEqual(0, group.Count());
            group.Add(item);
            const int expected = 1;
            Assert.AreEqual(expected, group.Count());
        }

	    [Test]
        public void GroupedItemKnowsItsParent()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            Assert.AreEqual(group, item.Parent);
        }
        
        [Test]
        public void GroupKnowsItsChildren()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            Assert.AreEqual(group.Items.First(), item);
        }

        [Test]
        public void CanAddGroupToGroup()
        {
            var item = CreateItem();
            var group = CreateGroup();
            Assert.AreEqual(0, group.Count());
            group.Add(item);
            const int expected = 1;
            Assert.AreEqual(expected, group.Count());
        }
        
        [Test]
        public void CanAssignGroupAndItemName()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            const string expectedItemName = "item";
            const string expectedGroupName = "group";
            Assert.AreEqual(expectedItemName, item.Name);
            Assert.AreEqual(expectedGroupName, group.Name);
        }

        [Test]
        public void CanAddGroupToDisplayCollection()
        {
            var displayCollection = new DisplayCollection();
            var group = CreateGroup();
            displayCollection.Add(group);
            const int expected = 1;
            Assert.AreEqual(expected, displayCollection.Count);
        }

        [Test]
        public void CanAddItemToDisplayCollection()
        {
            var displayCollection = new DisplayCollection();
            var item = CreateGroup();
            displayCollection.Add(item);
            const int expected = 1;
            Assert.AreEqual(expected, displayCollection.Count);
        }

        [Test]
        public void CanAddGroupAndItemToDisplayCollection()
        {
            var displayCollection = new DisplayCollection();
            var item = CreateItem();
            var group = CreateGroup();
            displayCollection.Add(item);
            displayCollection.Add(group);
            const int expected = 2;
            Assert.AreEqual(expected, displayCollection.Count);
        }

        [Test]
        public void CanAddCollectionOfIDislpayItemToGroup()
        {
            var collection = new List<IDisplayItem>();

            var item = CreateItem();
            var item2 = CreateItem();
            var item3 = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            group.Add(item2);
            group.Add(item3);


            var item4 = CreateItem();
            var item5 = CreateItem();
            var group2 = CreateGroup();
            group2.Add(item4);
            group2.Add(item5);

            group.Add(group2);

            var item6 = CreateItem();

            collection.Add(group);
            collection.Add(item6);

            var group3 = CreateGroup();
            group3.AddRange(collection);

            Assert.That(group3.Contains(group));
            Assert.That(group3.Contains(item6));
            Assert.That(group3.Count() == 2);

        }

    }
}
