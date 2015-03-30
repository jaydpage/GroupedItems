using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupedItemsTake2
{
    public class Group : IGroup
    {
        public string Name { get; set; }
        public IDislpayItem Parent { get; private set; }
        public ObservableItemsCollection Items { get; private set; } 

        public Group(string name, IDislpayItem parent)
        {
            Parent = parent;
            Name = name;
            UID = Guid.NewGuid().ToString();
            Items = new ObservableItemsCollection();
        }

        public void Add(IDislpayItem item)
        {
            Items.Add(item);
            item.SetParent(this);
        }

        public void Insert(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems)
        {
            var lowestSelectedIndex = Items.GetLowestSelectedIndex(selectedItems);
            Items.Insert(lowestSelectedIndex, item);
            item.SetParent(this);
        }
        
        public void InsertAtParentIndex(IDislpayItem item)
        {
            var index = Items.IndexOf(item.Parent);
            Items.Insert(index, item);
            item.SetParent(this);
        }

        public void MoveItemsUp(IEnumerable<IDislpayItem> items)
        {
            Items.MoveItemsUp(items);
        }

        public void MoveItemsDown(IEnumerable<IDislpayItem> items)
        {
           Items.MoveItemsDown(items);
        }

        public void Remove(IDislpayItem item)
        {
            Items.Remove(item);
        }

        public int Count()
        {
            return Items.Count;
        }

        public bool Contains(IDislpayItem item)
        {
            return Items.Contains(item);
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
            foreach (var item in Items)
            {
                item.SetParent(this);
            }
        }

        public IDislpayItem Copy()
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