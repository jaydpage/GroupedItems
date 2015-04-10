using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class MovingOutOfGroupTests : TestBase
	{
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

			var selectedItems = new ObservableCollection<IDisplayItem> { group };

			var displayCollection = new DisplayCollection { newerGroup, item2 };

			displayCollection.SelectedItems = selectedItems;
			Assert.That(newGroup.Contains(group));

			displayCollection.MoveItemsOutOfGroup();

			Assert.AreEqual(group.Name, newerGroup.Items.ElementAt(0).Name);
			Assert.AreEqual(2, newerGroup.Count());
			Assert.That(!newGroup.Contains(group));
		}

		[Test]
		public static void GivenUngroupedItemAfterMovingOutOfGroupShouldHaveNoChange()
		{
			var item2 = CreateItem("item2");

			var selectedItems = new ObservableCollection<IDisplayItem> { item2 };

			var displayCollection = new DisplayCollection { item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.MoveItemsOutOfGroup();

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

			var selectedItems = new ObservableCollection<IDisplayItem> { item0, item };

			var displayCollection = new DisplayCollection { group, item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.MoveItemsOutOfGroup();

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

			var selectedItems = new ObservableCollection<IDisplayItem> { item0, item };

			var displayCollection = new DisplayCollection { group, item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.MoveItemsOutOfGroup();

			Assert.AreEqual(item0.Name, displayCollection.ElementAt(0).Name);
			Assert.AreEqual(item.Name, displayCollection.ElementAt(1).Name);
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

			var selectedItems = new ObservableCollection<IDisplayItem> { @group };

			var displayCollection = new DisplayCollection { newGroup, item2 };

			displayCollection.SelectedItems = selectedItems;

			Assert.AreEqual(2, displayCollection.Count);
			displayCollection.MoveItemsOutOfGroup();

			Assert.AreEqual(group.Name, displayCollection.ElementAt(0).Name);
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

			var selectedItems = new ObservableCollection<IDisplayItem> { @group };

			var displayCollection = new DisplayCollection { item2 };
			displayCollection.AddItems(new List<IDisplayItem> { group });

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

			var selectedItems = new ObservableCollection<IDisplayItem> { group };

			var displayCollection = new DisplayCollection { item2 };
			displayCollection.AddItems(new List<IDisplayItem> { newGroup });

			displayCollection.SelectedItems = selectedItems;
			displayCollection.UnGroup();

			Assert.That(newGroup.Items.All(x => x.Name != group.Name));
			Assert.AreEqual(2, newGroup.Count());
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

			var selectedItems = new ObservableCollection<IDisplayItem> { group };

			var displayCollection = new DisplayCollection { item2 };
			displayCollection.AddItems(new List<IDisplayItem> { group });

			displayCollection.SelectedItems = selectedItems;

			Assert.AreEqual(2, displayCollection.Count);
			displayCollection.UnGroup();

			Assert.AreEqual(item0.Name, displayCollection.ElementAt(1).Name);
			Assert.AreEqual(item.Name, displayCollection.ElementAt(2).Name);
		}
	}
}
