using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DeletionAndUngroupingTests
    {
        private static Item CreateItem(string name = "item")
        {
            return new Item(name, null);
        }

        private static Group CreateGroup(string name = "group")
        {
            return new Group(name, null);
        }

        [Test]
        public void GivenTopLevelParent_AfterDeletingShouldBeRemovedFromList()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(1, displayCollection.Count());
            Assert.That(!displayCollection.Contains(group));
        }
        
        [Test]
        public void GivenChildItem_AfterDeletingShouldBeRemovedFromList()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(1, group.Count());
            Assert.That(!group.Contains(item0));
        }
        
        [Test]
        public void GivenParentChildItem_AfterDeletingShouldBeRemovedFromList()
        {
            var newGroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(newGroup);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(0, newGroup.Count());
            Assert.AreEqual(2, displayCollection.Count());
            Assert.That(!newGroup.Contains(group));
        }
        
        [Test]
        public void GivenUngroupedItem_AfterDeletingShouldBeRemovedFromList()
        {
            var item2 = CreateItem("item2");

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item2);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(0, displayCollection.Count());
            Assert.That(!displayCollection.Contains(item2));
        }
        
        [Test]
        public void GivenUngroupedItem_AfterUngroupingShouldHaveNoChange()
        {
            var item2 = CreateItem("item2");

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item2);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(1, displayCollection.Count());
            Assert.That(displayCollection.Contains(item2));
        }

        [Test]
        public void GivenChildItems_AfterUngrouping_ShouldBeRemovedFromParent()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(0, group.Count());
            Assert.That(!group.Contains(item0));
            Assert.That(!group.Contains(item2));
        }
        
        [Test]
        public void GivenChildItems_AfterUngrouping_ShouldBePlacedAtSameLevelAsParent()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(item0.Name, displayCollection[0].Name);
            Assert.AreEqual(item.Name, displayCollection[1].Name);
        }
        
        [Test]
        public void GivenParentChild_AfterUngrouping_ShouldBePlacedAtSameLevelAsParent()
        {
            var newGroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(newGroup);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(2, displayCollection.Count);
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(group.Name, displayCollection[0].Name);
            Assert.AreEqual(3, displayCollection.Count);
           
        }
        
        [Test]
        public void AfterUngroupingItems_IfTheyArePlacedAtTheUngroupedLevelTheirParentShouldBeNull()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            group.Add(item0);
            group.Add(item2);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(group);

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(group, item0.Parent);
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(null, displayCollection[0].Parent);
           
        }
        
        [Test]
        public void GivenDeepNestedChild_AfterUngrouping_ShouldBePlacedAtSameLevelAsParent()
        {
            var newerGroup = CreateGroup("newerGroup");
            var newGroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);
            newerGroup.Add(newGroup);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);

            var displayCollection = new DisplayCollection();
            displayCollection.Add(newerGroup);
            displayCollection.Add(item2);

            displayCollection.SelectedItems = selectedItems;
            Assert.That(newGroup.Contains(group));

            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(group.Name, newerGroup.Items[0].Name);
            Assert.AreEqual(2, newerGroup.Count());
            Assert.That(!newGroup.Contains(group));
           
        }
    }
}
