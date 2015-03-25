using System.Collections.Generic;
using System.Collections.ObjectModel;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class AddingLogicTests
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
        public void givenNoSelectedItems_ItemThatIsAddedShouldBeUngrouped()
        {
            var selectedItems = new ObservableCollection<IDislpayItem>();
            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            var item = CreateItem();
            Assert.That(displayCollection.Count == 0);
            displayCollection.AddItem(item);
            Assert.That(displayCollection.Count ==1);
            Assert.That(item.Level == Level.Ungrouped);
        }
        
        [Test]
        public void givenAnyUngroupedSelectedItems_ItemThatIsAddedShouldBeUngrouped()
        {
            var selectedItems = new ObservableItemsCollection();
            selectedItems.Add(CreateItem());
            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            var item = CreateItem();
            Assert.That(displayCollection.Count == 0);
            displayCollection.AddItem(item);
            Assert.That(displayCollection.Count ==1);
            Assert.That(item.Level == Level.Ungrouped);
        }
        
        [Test]
        public void givenSelectedItemsOfSameGroup_ItemThatIsAddedShouldBeAChildOfTheSameGroup()
        {
            var selectedItems = new ObservableCollection<IDislpayItem>();
            var group = CreateGroup();
            var item = CreateItem();
            var item4 = CreateItem();
            var item5 = CreateItem();
            var item6 = CreateItem();
            group.Add(item);
            group.Add(item4);
            group.Add(item5);
            group.Add(item6);
            selectedItems.Add(item);
            selectedItems.Add(item4);
            selectedItems.Add(item5);
            selectedItems.Add(item6);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            var item1 = CreateItem();
            displayCollection.AddItem(item1);
            Assert.AreEqual(Level.Child, item1.Level);
            Assert.AreEqual(item1.Parent, group);

            var displayCollection1 = new DisplayCollection();
            var group1 = CreateGroup();
            var item2 = CreateItem();
            var selectedItems1 = new ObservableCollection<IDislpayItem>();
            selectedItems1.Add(group1);
            displayCollection1.SelectedItems = selectedItems1;
            displayCollection1.Add(group1);
            displayCollection1.InsertItem(item2);
            Assert.AreEqual(Level.Ungrouped, item2.Level);
        }
        
        [Test]
        public void givenSelectedItemsOfDifferentLevels_ItemThatIsAddedAsUngrouped()
        {
            var selectedItems = new ObservableCollection<IDislpayItem>();
            var group = CreateGroup();
            var item = CreateItem();
            var item4 = CreateItem();
            var item5 = CreateItem();
            var item6 = CreateItem();
            group.Add(item);
            group.Add(item6);
            selectedItems.Add(item);
            selectedItems.Add(item4);
            selectedItems.Add(item5);
            selectedItems.Add(item6);
            selectedItems.Add(group);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            var item1 = CreateItem();
            displayCollection.AddItem(item1);
            Assert.AreEqual(Level.Ungrouped, item1.Level);
        }


        [Test]
        public void givenItemWithNoParentOrChildren_ShouldReturnItemLevelUngrouped()
        {
            var item = CreateItem();
            var expected = Level.Ungrouped;
            Assert.AreEqual(expected, item.Level);
        }
        
        [Test]
        public void givenItemInAGroup_ShouldReturnItemLevelChild()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            var expected = Level.Child;
            Assert.AreEqual(expected, item.Level);
        }
        
        [Test]
        public void givenAGroupWithAItem_ShouldReturnGroupLevelParent()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            var expected = Level.Parent;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void givenNestedGroup_ShouldReturnParentChild()
        {
            var item = CreateItem();
            var group = CreateGroup();
            var group1 = CreateGroup();
            group.Add(item);
            group1.Add(group);
            var expected = Level.ParentChild;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void givenGroupWithoutChildren_ShouldReturnParent()
        {
            var group = CreateGroup();
            var expected = Level.Parent;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void givenGroupWithParentandWithoutChildren_ShouldReturnParentChild()
        {
            var group = CreateGroup();
            var group1 = CreateGroup();
            group1.Add(group);
            var expected = Level.ParentChild;
            Assert.AreEqual(expected, group.Level);
        }
    }
}
