using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GroupedItemsTake2
{
    public class Group : ObservableCollection<IDislpayItem>, IGroup
    {
        public string Name { get; set; }
        public IDislpayItem Parent { get; private set; }

        public Group(string name, IDislpayItem parent)
        {
            Parent = parent;
            Name = name;
        }

        public new void Add(IDislpayItem item)
        {
            Items.Add(item);
            item.SetParent(this);
        }

        public new void Remove(IDislpayItem item)
        {
            Items.Remove(item);
        }

        public void AddRange(IEnumerable<IDislpayItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void SetParent(IDislpayItem parent)
        {
            Parent = parent;
        }

        public Level Level
        {
            get
            {
                var parent = Parent as Group;

                if (HasParent(parent) && HasChildren()) return Level.ParentChild;
                if (HasNoParent(parent) && HasChildren()) return Level.Parent;
                if (HasParent(parent) && HasNoChildren()) return Level.ParentChild;
                return Level.Parent;
            }
        }

        private bool HasNoChildren()
        {
            return !Items.Any();
        }

        private static bool HasNoParent(Group parent)
        {
            return parent == null;
        }

        private bool HasChildren()
        {
            return Items.Any();
        }

        private static bool HasParent(Group parent)
        {
            return parent != null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}