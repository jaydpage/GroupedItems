using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GroupingLogicTests
    {
        ObservableItemsCollection _collection = new ObservableItemsCollection();

        private static Item CreateItem(string name = "item")
        {
            return new Item(name, null);
        }

        private static Group CreateGroup(string name = "group")
        {
            return new Group(name, null);
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


            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            displayCollection.SelectedItems = selectedItems;
            displayCollection.GroupSelectedItems(group);
            
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

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            displayCollection.AddItem(group);
            displayCollection.AddItem(item1);
            displayCollection.AddItem(item2);
            displayCollection.GroupSelectedItems(newgroup);

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

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            displayCollection.Add(group);
            displayCollection.GroupSelectedItems(newgroup);

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
            selectedItems2.Add(item0);

            Assert.AreEqual(true, _collection.AreAnySelectedItemsAtTheTopLevel(selectedItems));
            Assert.AreEqual(false, _collection.AreAnySelectedItemsAtTheTopLevel(selectedItems2));

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

            Assert.AreEqual(false, _collection.AreSelectedItemsOfTheSameGroup(selectedItems));
            Assert.AreEqual(true, _collection.AreSelectedItemsOfTheSameGroup(selectedItems2));
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

            Assert.AreEqual(group, _collection.GetItemGroup(selectedItems[0]));
            Assert.AreEqual(group, _collection.GetItemGroup(selectedItems[1]));
            Assert.AreEqual(group, _collection.GetItemGroup(selectedItems[2]));

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

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(group);
            selectedItems.Add(item1);
            selectedItems.Add(item2);
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            displayCollection.AddItem(group);
            displayCollection.AddItem(item1);
            displayCollection.AddItem(item2);

            var itemsToGroup = _collection.GetDistinctItems(selectedItems).ToList();

            Assert.AreEqual(3, itemsToGroup.Count());
            Assert.AreEqual(Level.Parent, itemsToGroup[0].Level);
            Assert.AreEqual(Level.Ungrouped, itemsToGroup[1].Level);
            Assert.AreEqual(Level.Ungrouped, itemsToGroup[2].Level);
        }
        
        [Test]
        public void ChildrenThatAreGroupedAreRemovedFromTheParentAfterGrouping()
        {
            var newgroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem>();
            selectedItems.Add(item0);
            selectedItems.Add(item);

            var displayCollection = new DisplayCollection();
            displayCollection.SelectedItems = selectedItems;
            displayCollection.Add(group);
            displayCollection.GroupSelectedItems(newgroup);

            Assert.AreEqual(1, group.Count());
            Assert.AreEqual(newgroup, group.Items[0]);
            Assert.AreEqual(item0.Name, newgroup.Items[0].Name);
            Assert.AreEqual(item.Name, newgroup.Items[1].Name);
        }

        [Test]
        public void GroupingUngroupedItemsWillHaveTheirParentSetToTheNewGroup()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item1 = CreateItem();
            group.Add(item0);
            group.Add(item1);

            Assert.AreEqual(group, item0.Parent);
            Assert.AreEqual(group, item1.Parent);
        }
        
        [Test]
        public void GroupingGroupedItemsWillMaintainParentsCorrectly()
        {
            var item1 = CreateItem("Item1");
            var item2 = CreateItem("Item2");
            var item3 = CreateItem("Item3");
            var item4 = CreateItem("Item4");
            var item5 = CreateItem("Item5");
            var item6 = CreateItem("Item6");
            var item7 = CreateItem("Item7");
            var displayCollection = new DisplayCollection{item1, item2, item3, item4, item5, item6, item7};

            var firstSelection = new ObservableCollection<IDislpayItem> { item1, item2 };
            var group1 = CreateGroup("Group1");
            displayCollection.SelectedItems = firstSelection;
            displayCollection.GroupSelectedItems(group1);

            var collectionGroup1 = displayCollection.First(x => x.Name == "Group1") as IGroup;
            Assert.AreEqual(group1, collectionGroup1.Items.First(x => x.Name == "Item1").Parent);
            Assert.AreEqual(group1, collectionGroup1.Items.First(x => x.Name == "Item2").Parent);
            Assert.True(displayCollection.Contains(group1));


            var secondSelection = new ObservableCollection<IDislpayItem> { item3, item4 };
            var group2 = CreateGroup("Group2");
            displayCollection.SelectedItems = secondSelection;
            displayCollection.GroupSelectedItems(group2);

            var collectionGroup2 = displayCollection.First(x => x.Name == "Group2") as IGroup;
            Assert.AreEqual(group2, collectionGroup2.Items.First(x => x.Name == "Item3").Parent);
            Assert.AreEqual(group2, collectionGroup2.Items.First(x => x.Name == "Item4").Parent);
            Assert.True(displayCollection.Contains(group2));


            var thirdSelection = new ObservableCollection<IDislpayItem> { group1, item1, item2, group2, item3, item4, item5 };
            var group3 = CreateGroup("Group3");
            displayCollection.SelectedItems = thirdSelection;
            displayCollection.GroupSelectedItems(group3);

            var collectionGroup3 = displayCollection.First(x => x.Name == "Group3") as IGroup;
            var collGroup1 = collectionGroup3.Items.First(x => x.Name == "Group1") as IGroup;

            Assert.AreEqual(group3, collectionGroup3.Items.First(x => x.Name == "Item5").Parent);
            Assert.AreEqual(group3, collectionGroup3.Items.First(x => x.Name == "Group2").Parent);
            Assert.AreEqual(group3, collectionGroup3.Items.First(x => x.Name == "Group1").Parent);
            Assert.True(displayCollection.Contains(group3));

            Assert.AreEqual(collGroup1.UID, collGroup1.Items.First(x => x.Name == "Item1").Parent.UID);
            Assert.AreEqual(collGroup1.UID, collGroup1.Items.First(x => x.Name == "Item2").Parent.UID);

            var forthSelection = new ObservableCollection<IDislpayItem> { group3, group1, item1, item2, group2, item3, item4, item5, item6 };
            var group4 = CreateGroup("Group4");
            displayCollection.SelectedItems = forthSelection;
            displayCollection.GroupSelectedItems(group4);

            var collectionGroup4 = displayCollection.First(x => x.Name == "Group4") as IGroup;
            var cgroup3 = collectionGroup4.Items.First(x => x.Name == "Group3") as IGroup;
            var cgroup2 = cgroup3.Items.First(x => x.Name == "Group2") as IGroup;
            var cgroup1 = cgroup3.Items.First(x => x.Name == "Group1") as IGroup;

            Assert.AreEqual(collectionGroup4.UID, collectionGroup4.Items.First(x => x.Name == "Item6").Parent.UID);
            Assert.AreEqual(collectionGroup4.UID, cgroup3.Parent.UID);

            Assert.AreEqual(group3.UID, cgroup2.Parent.UID);
            Assert.AreEqual(group3.UID, cgroup1.Parent.UID);
            Assert.AreEqual(group3.UID, cgroup3.Items.First(x => x.Name == "Item5").Parent.UID);

            Assert.AreEqual(cgroup1.UID, cgroup1.Items.First(x => x.Name == "Item1").Parent.UID);
            Assert.AreEqual(cgroup1.UID, cgroup1.Items.First(x => x.Name == "Item2").Parent.UID);

            Assert.AreEqual(cgroup2.UID, cgroup2.Items.First(x => x.Name == "Item3").Parent.UID);
            Assert.AreEqual(cgroup2.UID, cgroup2.Items.First(x => x.Name == "Item4").Parent.UID);
        }
    }
}
