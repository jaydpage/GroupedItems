using System;

namespace GroupedItemsTake2
{
    public class Item : IItem
    {
        public Item(string name, IDislpayItem parent)
        {
            Parent = parent;
            Name = name;
            UID = Guid.NewGuid().ToString();
        }
        public string Name { get; set; }
        public IDislpayItem Parent { get; private set; }

        public void SetParent(IDislpayItem parent)
        {
            Parent = parent;
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