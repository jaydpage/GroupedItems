using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public void GivenNoSelectedItemsItemThatIsAddedShouldBeUngrouped()
        {
            var selectedItems = new ObservableCollection<IDisplayItem>();
            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            var item = CreateItem();
            Assert.That(displayCollection.Count == 0);
            displayCollection.AddItems(new List<IDisplayItem>{item});
            Assert.That(displayCollection.Count ==1);
            Assert.That(item.Level == Level.Ungrouped);
        }
        
        [Test]
        public void GivenAnyUngroupedSelectedItemsItemThatIsAddedShouldBeUngrouped()
        {
            var selectedItems = new ObservableCollection<IDisplayItem>{CreateItem()};
            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            var item = CreateItem();
            Assert.That(displayCollection.Count == 0);
            displayCollection.AddItems(new List<IDisplayItem> { item });
            Assert.That(displayCollection.Count ==1);
            Assert.That(item.Level == Level.Ungrouped);
        }
        
        [Test]
        public void GivenSelectedItemsOfSameGroupItemThatIsAddedShouldBeAChildOfTheSameGroup()
        {
            var selectedItems = new ObservableCollection<IDisplayItem>();
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

            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            var item1 = CreateItem();
            displayCollection.AddItems(new List<IDisplayItem> { item1 });
            Assert.AreEqual(Level.Child, item1.Level);
            Assert.AreEqual(item1.Parent, group);

            var displayCollection1 = new DisplayCollection();
            var group1 = CreateGroup();
            var item2 = CreateItem();
            var selectedItems1 = new ObservableCollection<IDisplayItem> {group1};
            displayCollection1.SelectedItems = selectedItems1;
            displayCollection1.Add(group1);
            displayCollection1.Insert(item2);
            Assert.AreEqual(Level.Ungrouped, item2.Level);
        }
        
        [Test]
        public void GivenSelectedItemsOfDifferentLevelsItemThatIsAddedAsUngrouped()
        {
            var selectedItems = new ObservableCollection<IDisplayItem>();
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

            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            var item1 = CreateItem();
            displayCollection.AddItems(new List<IDisplayItem> { item1 });
            Assert.AreEqual(Level.Ungrouped, item1.Level);
        }


        [Test]
        public void GivenItemWithNoParentOrChildrenShouldReturnItemLevelUngrouped()
        {
            var item = CreateItem();
            const Level expected = Level.Ungrouped;
            Assert.AreEqual(expected, item.Level);
        }
        
        [Test]
        public void GivenItemInAGroupShouldReturnItemLevelChild()
        {
            var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            const Level expected = Level.Child;
            Assert.AreEqual(expected, item.Level);
        }
        
        [Test]
        public void GivenAGroupWithAItemShouldReturnGroupLevelParent()
        {
             var item = CreateItem();
            var group = CreateGroup();
            group.Add(item);
            const Level expected = Level.Parent;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void GivenNestedGroupShouldReturnParentChild()
        {
            var item = CreateItem();
            var group = CreateGroup();
            var group1 = CreateGroup();
            group.Add(item);
            group1.Add(group);
            const Level expected = Level.ParentChild;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void GivenGroupWithoutChildrenShouldReturnParent()
        {
            var group = CreateGroup();
            const Level expected = Level.Parent;
            Assert.AreEqual(expected, group.Level);
        }
        
        [Test]
        public void GivenGroupWithParentandWithoutChildrenShouldReturnParentChild()
        {
            var group = CreateGroup();
            var group1 = CreateGroup();
            group1.Add(group);
            const Level expected = Level.ParentChild;
            Assert.AreEqual(expected, group.Level);
        }

        [Test]
        public void CutAndPasteTest()
        {
            var group = CreateGroup();
            var displayCollection = new DisplayCollection();
            displayCollection.AddItems(new List<IDisplayItem> { group });
            displayCollection.SelectedItems = new ObservableCollection<IDisplayItem> { group };

            Assert.That(displayCollection.Any(x => x.UID == group.UID));
            displayCollection.Cut();
            Assert.That(displayCollection.All(x => x.UID != group.UID));
            displayCollection.SelectedItems.Clear();
            displayCollection.Paste();
            Assert.That(displayCollection.Any(x => x.UID == group.UID));
        }
    }
}
