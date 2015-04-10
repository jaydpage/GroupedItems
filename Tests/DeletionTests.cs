using System.Collections.ObjectModel;
using System.Linq;
using GroupedItemsTake2;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class DeletionTests : TestBase
	{
		[Test]
		public void GivenTopLevelParentAfterDeletingShouldBeRemovedFromList()
		{
			var group = CreateGroup();
			var item0 = CreateItem("item0");
			var item2 = CreateItem("item2");
			var item = CreateItem();
			@group.Add(item0);
			@group.Add(item);

			var selectedItems = new ObservableCollection<IDisplayItem> { @group };

			var displayCollection = new DisplayCollection { @group, item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.Delete();

			Assert.AreEqual(1, displayCollection.Count());
			Assert.That(!displayCollection.Contains(@group));
		}

		[Test]
		public void GivenUngroupedItemAfterDeletingShouldBeRemovedFromList()
		{
			var item2 = CreateItem("item2");

			var selectedItems = new ObservableCollection<IDisplayItem> { item2 };

			var displayCollection = new DisplayCollection { item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.Delete();

			Assert.AreEqual(0, displayCollection.Count());
			Assert.That(!displayCollection.Contains(item2));
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

			var selectedItems = new ObservableCollection<IDisplayItem> { group };

			var displayCollection = new DisplayCollection { newGroup, item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.Delete();

			Assert.AreEqual(0, newGroup.Count());
			Assert.AreEqual(2, displayCollection.Count());
			Assert.That(!newGroup.Contains(group));
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

			var selectedItems = new ObservableCollection<IDisplayItem> { item0 };

			var displayCollection = new DisplayCollection { group, item2 };

			displayCollection.SelectedItems = selectedItems;
			displayCollection.Delete();

			Assert.AreEqual(1, group.Count());
			Assert.That(!group.Contains(item0));
		}
	}
}