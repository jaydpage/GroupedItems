using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupedItemsTake2;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Interfaces;
using NUnit.Framework;

namespace Tests
{
	public class UnclassifiedTests : TestBase
	{
        [Test]
        public void GivenItemWithNoParentOrChildrenShouldReturnItemLevelUngrouped()
        {
            var item = CreateItem();
            const Level expected = Level.Ungrouped;
            Assert.AreEqual(expected, item.Level);
        }
        
		[Test]
		public void AfterMovingOutOfGroupIfTheyArePlacedAtTheUngroupedLevelTheirParentShouldBeNull()
		{
			var group = CreateGroup();
			var item0 = CreateItem("item0");
			var item2 = CreateItem("item2");
			group.AddItem(item0);
			group.AddItem(item2);

			var selectedItems = new ObservableCollection<IDisplayItem> { item0 };

			var displayCollection = new DisplayCollection { group };

			displayCollection.SelectedItems = selectedItems;

			Assert.AreEqual(group, item0.Parent);
			displayCollection.MoveItemsOutOfGroup();

			Assert.AreEqual(null, displayCollection.ElementAt(0).Parent);
		}
		[Test]
		public void GivenAnUngroupedParentShouldHaveItsParentHeirarchyUpdated()
		{
			var group = CreateGroup();
			var group1 = CreateGroup("Group1");
			var group2 = CreateGroup("Group2");
			var item = CreateItem();
			var item1 = CreateItem("Item1");

			group2.AddItem(item);
			group2.AddItem(item1);

			group1.Add(group2);
			group.Add(group1);


			var selectedItems = new ObservableCollection<IDisplayItem> { group, group1, group2, item, item1 };

			var displayCollection = new DisplayCollection();
			displayCollection.AddItems(new List<IDisplayItem> { group });

			displayCollection.SelectedItems = selectedItems;

			Assert.That(group2.Parent.UID == group1.UID);
			Assert.That(group2.Parent.Parent.UID == group.UID);
			Assert.That(group1.Parent.UID == group.UID);
			Assert.That(group1.Parent.Parent == null);
			Assert.That(group.Parent == null);

			displayCollection.UnGroup();

			Assert.That(group2.Parent.Parent == null);
		}
	}
}