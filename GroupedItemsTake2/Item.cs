using System;

namespace GroupedItemsTake2
{
    public class Item : IItem
    {
	    public static Item Create(string name)
	    {
		    var result = new Item(name, null);
		    return result;
	    }

        public Item(string name, IDisplayItem parent)
        {
            Parent = parent;
            Name = name;
            UID = Guid.NewGuid().ToString();
        }
        public string Name { get; set; }
        public IDisplayItem Parent { get; private set; }

        public void SetParent(IDisplayItem parent)
        {
            Parent = parent;
        }

        public IDisplayItem Copy()
        {
            return new Item(Name, Parent){UID = Guid.NewGuid().ToString()};
        }

        public Level Level
        {
            get
            {
                return Parent == null ? Level.Ungrouped : Level.Child;
            }
        }

        public string UID { get; private set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}