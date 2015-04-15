using GroupedItemsTake2.Domain;

namespace Tests
{
	public class TestBase
	{
		protected static Item CreateItem(string name = "item")
		{
			return new Item(name, null);
		}

		protected static Group CreateGroup(string name = "group")
		{
			return new Group(name, null);
		}
	}
}