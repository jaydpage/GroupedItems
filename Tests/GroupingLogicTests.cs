using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GroupingLogicTests
    {
        private static Item CreateItem()
        {
            return new Item("item", null);
        }

        private static Group CreateGroup()
        {
            return new Group("group", null);
        }

        [Test]
        public void givenUngroupedSelectedItems_GroupWillBeAddedAsParentAtFirstSelectedIndex()
        {
            var displayCollection = new DisplayCollection();

            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();
            var item3 = CreateItem();

            displayCollection.Add(item0);
            displayCollection.Add(item);
            displayCollection.Add(item1);
            displayCollection.Add(item2);
            displayCollection.Add(item3);


            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(item);
            selectedItems.Add(item1);
            selectedItems.Add(item2);

            displayCollection.GroupItems(group, selectedItems);

            const int expectedIndex = 1;

            Assert.That(!displayCollection.Contains(item));
            Assert.That(!displayCollection.Contains(item1));
            Assert.That(!displayCollection.Contains(item2));
            Assert.That(displayCollection.Contains(group));
            Assert.That(group.Items.Count == 3);
            Assert.AreEqual(group, group.Items[0].Parent);
            Assert.AreEqual(group, group.Items[1].Parent);
            Assert.AreEqual(group, group.Items[2].Parent);
            Assert.AreEqual(group.Level, Level.Parent);
            Assert.AreEqual(group.Level, Level.Parent);
            Assert.AreEqual(group, displayCollection[expectedIndex]);
        }

        [Test]
        public void givenSelectedItems_IfChildrenOfASelectedGroupAreSelectedTheyAreNotGroupedTwice()
        {
            var newgroup = CreateGroup();
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item1);
            displayCollection.Add(item2);
            displayCollection.GroupItems(newgroup, selectedItems);

            Assert.That(newgroup.Count() == 3);
            Assert.AreEqual(newgroup, newgroup.Items[0].Parent);
            Assert.AreEqual(newgroup, newgroup.Items[1].Parent);
            Assert.AreEqual(newgroup, newgroup.Items[2].Parent);
            Assert.AreEqual(newgroup.Items[0].Level, Level.ParentChild);
            Assert.AreEqual(newgroup.Items[1].Level, Level.Child);
            Assert.AreEqual(newgroup.Items[2].Level, Level.Child);
        }
        
        [Test]
        public void Given_SelectedChilderenOfSameGroup_WillBeGroupedTogether()
        {
            var newgroup = CreateGroup();
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.GroupItems(newgroup, selectedItems);

            Assert.AreEqual(newgroup, newgroup.Items[0].Parent);
            Assert.AreEqual(newgroup, newgroup.Items[1].Parent);
            Assert.AreEqual(group, newgroup.Parent);
            Assert.AreEqual(group.Level, Level.Parent);
            Assert.AreEqual(newgroup.Level, Level.ParentChild);
            Assert.AreEqual(newgroup.Items[0].Level, Level.Child);
            Assert.AreEqual(newgroup.Items[1].Level, Level.Child);
        }

        [Test]
        public void Given_AreAnySelectedItemsUngrouped_ShouldReturnCorretValue()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var selectedItems2 = new List<IDislpayItem>();
            selectedItems2.Add(group);

            Assert.AreEqual(true, GroupingLogic.AreAnySelectedItemsUngrouped(selectedItems));
            Assert.AreEqual(false, GroupingLogic.AreAnySelectedItemsUngrouped(selectedItems2));
        }

        [Test]
        public void Given_AreSelectedItemsOfTheSameGroup_ShouldReturnCorrectValue()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems2 = new List<IDislpayItem>();
            selectedItems2.Add(group);
            selectedItems2.Add(item0);
            selectedItems2.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            Assert.AreEqual(false, GroupingLogic.AreSelectedItemsOfTheSameGroup(selectedItems));
            Assert.AreEqual(true, GroupingLogic.AreSelectedItemsOfTheSameGroup(selectedItems2));
        }
        
        [Test]
        public void Given_GetSelectedItemGroup_ShouldReturnCorrectValue()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            Assert.AreEqual(group, GroupingLogic.GetSelectedItemGroup(selectedItems[0]));
            Assert.AreEqual(group, GroupingLogic.GetSelectedItemGroup(selectedItems[1]));
            Assert.AreEqual(group, GroupingLogic.GetSelectedItemGroup(selectedItems[2]));

        }
        
        [Test]
        public void Given_CreateItemsToGroup_ShouldReturnCorrectItems()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item1);
            displayCollection.Add(item2);

            var itemsToGroup = GroupingLogic.CreateItemsToGroup(selectedItems).ToList();

            Assert.AreEqual(3, itemsToGroup.Count());
            Assert.AreEqual(Level.Parent, itemsToGroup[0].Level);
            Assert.AreEqual(Level.Ungrouped, itemsToGroup[1].Level);
            Assert.AreEqual(Level.Ungrouped, itemsToGroup[2].Level);
        }
        
        [Test]
        public void ChildrenThatAreGroupedAreRemovedFromTheParentAfterGrouping()
        {
            var newgroup = CreateGroup();
            var group = CreateGroup();
            var item0 = CreateItem();
            item0.Name = "item0";
            var item = CreateItem();
            item.Name = "item";
            group.Add(item0);
            group.Add(item);

            var selectedItems = new List<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.GroupItems(newgroup, selectedItems);

            Assert.AreEqual(1, group.Count());
            Assert.AreEqual(newgroup, group.Items[0]);
            Assert.AreEqual(item0.Name, newgroup.Items[0].Name);
            Assert.AreEqual(item.Name, newgroup.Items[1].Name);
        }
    }
}
