namespace GroupedItemsTake2.Domain
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