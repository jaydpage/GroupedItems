namespace GroupedItemsTake2
{
	public class GroupNameGenerator
	{
		private int _groupCount;

		public GroupNameGenerator()
		{
			_groupCount = 0;
		}

		public string GenerateName()
		{
            _groupCount++;
			var name = "Group " + _groupCount;
			return name;
		}
	}
}