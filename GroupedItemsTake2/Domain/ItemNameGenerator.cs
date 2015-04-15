namespace GroupedItemsTake2.Domain
{
	public class ItemNameGenerator
	{
		private int _itemCount;

		public ItemNameGenerator()
		{
			_itemCount = 0;
		}

		public string GenerateItemName()
		{
			_itemCount++;
			return "Item" + _itemCount;
		}
	}
}