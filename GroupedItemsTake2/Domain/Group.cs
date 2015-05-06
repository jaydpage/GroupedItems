using System;
using System.Collections.Generic;
using System.Linq;
using GroupedItemsTake2.Interfaces;

namespace GroupedItemsTake2.Domain
{
    public class Group : IGroup
    {
        public string Name { get; set; }
        public IDisplayItem Parent { get; private set; }
        public ObservableItemsCollection Items { get; private set; }

	    public static IGroup CreateGroup(string name)
	    {
		    var result = new Group(name, null);
		    return result;
	    }


        public Group(string name, IDisplayItem parent)
        {
            Parent = parent;
            Name = name;
            UID = Guid.NewGuid().ToString();
            Items = new ObservableItemsCollection();
        }

        public void Add(IDisplayItem item)
        {
            Items.Add(item);
            item.SetParent(this);
        }

        public void Add(IEnumerable<IDisplayItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Insert(IDisplayItem item, IEnumerable<IDisplayItem> selectedItems)
        {
            var lowestSelectedIndex = Items.GetLowestSelectedIndex(selectedItems);
            Items.Insert(lowestSelectedIndex, item);
            item.SetParent(this);
        }
        
        public void InsertAtParentIndex(IDisplayItem item)
        {
            var index = Items.IndexOf(item.Parent);
            Items.Insert(index, item);
            item.SetParent(this);
        }

        public void MoveItemsUp(IEnumerable<IDisplayItem> items)
        {
            Items.MoveUp(items);
        }

        public void MoveItemsDown(IEnumerable<IDisplayItem> items)
        {
           Items.MoveDown(items);
        }

        public void Remove(IDisplayItem item)
        {
            Items.Remove(item);
        }

        public int Count()
        {
            return Items.Count();
        }

        public bool Contains(IDisplayItem item)
        {
            return Items.Contains(item);
        }

        public void AddRange(IEnumerable<IDisplayItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void SetParent(IDisplayItem parent)
        {
            Parent = parent;
            foreach (var item in Items)
            {
                item.SetParent(this);
            }
        }

        public IDisplayItem Copy()
        {
            var newGroup = new Group(Name, Parent) { UID = Guid.NewGuid().ToString()};
            foreach (var item in Items)
            {
                newGroup.Add(item.Copy());
            }
            return newGroup;
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

        public string UID { get; private set; }
    }
}