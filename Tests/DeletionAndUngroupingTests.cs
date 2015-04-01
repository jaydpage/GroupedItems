using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public void GivenTopLevelParentAfterDeletingShouldBeRemovedFromList()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {group};

            var displayCollection = new DisplayCollection {group, item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(1, displayCollection.Count());
            Assert.That(!displayCollection.Contains(group));
        }
        
        [Test]
        public void GivenChildItemAfterDeletingShouldBeRemovedFromList()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {item0};

            var displayCollection = new DisplayCollection {group, item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(1, group.Count());
            Assert.That(!group.Contains(item0));
        }
        
        [Test]
        public void GivenParentChildItemAfterDeletingShouldBeRemovedFromList()
        {
            var newGroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);

            var selectedItems = new ObservableCollection<IDislpayItem> {group};

            var displayCollection = new DisplayCollection {newGroup, item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(0, newGroup.Count());
            Assert.AreEqual(2, displayCollection.Count());
            Assert.That(!newGroup.Contains(group));
        }
        
        [Test]
        public void GivenUngroupedItemAfterDeletingShouldBeRemovedFromList()
        {
            var item2 = CreateItem("item2");

            var selectedItems = new ObservableCollection<IDislpayItem> {item2};

            var displayCollection = new DisplayCollection {item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.Delete();

            Assert.AreEqual(0, displayCollection.Count());
            Assert.That(!displayCollection.Contains(item2));
        }
        
        [Test]
        public void GivenUngroupedItemAfterMovingOutOfGroupShouldHaveNoChange()
        {
            var item2 = CreateItem("item2");

            var selectedItems = new ObservableCollection<IDislpayItem> {item2};

            var displayCollection = new DisplayCollection {item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(1, displayCollection.Count());
            Assert.That(displayCollection.Contains(item2));
        }

        [Test]
        public void GivenChildItemsAfterMovingOutOfGroupShouldBeRemovedFromParent()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {item0, item};

            var displayCollection = new DisplayCollection {group, item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(0, group.Count());
            Assert.That(!group.Contains(item0));
            Assert.That(!group.Contains(item2));
        }
        
        [Test]
        public void GivenChildItemsAfterMovingOutOfGroupShouldBePlacedAtSameLevelAsParent()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {item0, item};

            var displayCollection = new DisplayCollection {group, item2};

            displayCollection.SelectedItems = selectedItems;
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(item0.Name, displayCollection[0].Name);
            Assert.AreEqual(item.Name, displayCollection[1].Name);
        }
        
        [Test]
        public void GivenParentChildAfterMovingOutOfGroupShouldBePlacedAtSameLevelAsParent()
        {
            var newGroup = CreateGroup("newGroup");
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);

            var selectedItems = new ObservableCollection<IDislpayItem> {@group};

            var displayCollection = new DisplayCollection {newGroup, item2};

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(2, displayCollection.Count);
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(group.Name, displayCollection[0].Name);
            Assert.AreEqual(3, displayCollection.Count);
           
        }
        
        [Test]
        public void GivenTopLevelGroupAfterUngroupingShouldBeRemovedFromItems()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {@group};

            var displayCollection = new DisplayCollection {item2};
            displayCollection.AddItems(new List<IDislpayItem> { group }, false);

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(2, displayCollection.Count);
            displayCollection.UnGroup();

            Assert.That(displayCollection.All(x => x.Name != group.Name));
            Assert.AreEqual(3, displayCollection.Count);           
        }
        
        [Test]
        public void GivenNestedGroupAfterUngroupingShouldBeRemovedFromItems()
        {
            var group = CreateGroup();
            var newGroup = CreateGroup("newGroup");
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);
            newGroup.Add(group);

            var selectedItems = new ObservableCollection<IDislpayItem> {@group};

            var displayCollection = new DisplayCollection {item2};
            displayCollection.AddItems(new List<IDislpayItem> { newGroup }, false);

            displayCollection.SelectedItems = selectedItems;
            displayCollection.UnGroup();

            Assert.That(newGroup.Items.All(x => x.Name != group.Name));
            Assert.AreEqual(2, newGroup.Items.Count);           
        }
        
        [Test]
        public void GivenTopLevelGroupAfterUngroupingItemsShouldBeAddedToCollection()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            var item = CreateItem();
            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDislpayItem> {group};

            var displayCollection = new DisplayCollection {item2};
            displayCollection.AddItems(new List<IDislpayItem> { group }, false);

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(2, displayCollection.Count);
            displayCollection.UnGroup();
     
            Assert.AreEqual(item0.Name, displayCollection[1].Name);
            Assert.AreEqual(item.Name, displayCollection[2].Name);           
        }
        
        [Test]
        public void GivenAnUngroupedParentShouldHaveItsParentHeirarchyUpdated()
        {
            var group = CreateGroup();
            var group1 = CreateGroup("Group1");
            var group2 = CreateGroup("Group2");
            var item = CreateItem();
            var item1 = CreateItem("Item1");
            
            group2.Add(item);
            group2.Add(item1);

            group1.Add(group2);
            group.Add(group1);
            

            var selectedItems = new ObservableCollection<IDislpayItem> {group, group1, group2, item, item1};

            var displayCollection = new DisplayCollection();
            displayCollection.AddItems(new List<IDislpayItem> { group }, false);

            displayCollection.SelectedItems = selectedItems;

            Assert.That(group2.Parent.UID == group1.UID);
            Assert.That(group2.Parent.Parent.UID == group.UID);
            Assert.That(group1.Parent.UID == group.UID);
            Assert.That(group1.Parent.Parent == null);
            Assert.That(group.Parent == null);

            displayCollection.UnGroup();

            //Assert.That(group2.Parent.UID == group1.UID);
            Assert.That(group2.Parent.Parent == null);


        }
        
        [Test]
        public void AfterMovingOutOfGroupIfTheyArePlacedAtTheUngroupedLevelTheirParentShouldBeNull()
        {
            var group = CreateGroup();
            var item0 = CreateItem("item0");
            var item2 = CreateItem("item2");
            group.Add(item0);
            group.Add(item2);

            var selectedItems = new ObservableCollection<IDislpayItem> {item0};

            var displayCollection = new DisplayCollection {group};

            displayCollection.SelectedItems = selectedItems;

            Assert.AreEqual(group, item0.Parent);
            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(null, displayCollection[0].Parent);         
        }
        
        [Test]
        public void GivenDeepNestedChildAfterMovingOutOfGroupShouldBePlacedAtSameLevelAsParent()
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

            var selectedItems = new ObservableCollection<IDislpayItem> {group};

            var displayCollection = new DisplayCollection {newerGroup, item2};

            displayCollection.SelectedItems = selectedItems;
            Assert.That(newGroup.Contains(group));

            displayCollection.MoveOutOfGroup();

            Assert.AreEqual(group.Name, newerGroup.Items[0].Name);
            Assert.AreEqual(2, newerGroup.Count());
            Assert.That(!newGroup.Contains(group));       
        }
    }
}
