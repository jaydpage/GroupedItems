using System.Collections.ObjectModel;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Interfaces;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class CommandRestrictionTests : TestBase
    {
        [Test]
        public void SelectedItemsBelongToSameGroup()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems = new ObservableCollection<IDisplayItem> {group, item1, item2, item0, item};

            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            displayCollection.AddAsUngrouped(group);
            displayCollection.AddAsUngrouped(item1);
            displayCollection.AddAsUngrouped(item2);

            Assert.AreEqual(true, displayCollection.BelongToSameGroup());
        }
        
        [Test]
        public void SelectedItemsDoNotBelongToSameGroup()
        {
            var newgroup = CreateGroup();
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);
            newgroup.Add(group);

            var selectedItems = new ObservableCollection<IDisplayItem> {group, item1, item2};

            var displayCollection = new DisplayCollection {SelectedItems = selectedItems};
            displayCollection.AddAsUngrouped(newgroup);
            displayCollection.AddAsUngrouped(item1);
            displayCollection.AddAsUngrouped(item2);

            Assert.AreEqual(false, displayCollection.BelongToSameGroup());
        }
        
        [Test]
        public void OnlyChildrenSelected()
        {
            var group = CreateGroup();
            var item0 = CreateItem();
            var item = CreateItem();
            var item1 = CreateItem();
            var item2 = CreateItem();

            group.Add(item0);
            group.Add(item);

            var selectedItems1 = new ObservableCollection<IDisplayItem> {item0, item};
            var selectedItems2 = new ObservableCollection<IDisplayItem> {group, item1, item2, item0, item};

            var displayCollection = new DisplayCollection {SelectedItems = selectedItems1};
            displayCollection.AddAsUngrouped(group);
            displayCollection.AddAsUngrouped(item1);
            displayCollection.AddAsUngrouped(item2);
            
            var displayCollection2 = new DisplayCollection {SelectedItems = selectedItems2};
            displayCollection2.AddAsUngrouped(group);
            displayCollection2.AddAsUngrouped(item1);
            displayCollection2.AddAsUngrouped(item2);

            Assert.AreEqual(false, displayCollection2.OnlyChildrenSelected());
            Assert.AreEqual(true, displayCollection.OnlyChildrenSelected());

        }
        
    }
}
