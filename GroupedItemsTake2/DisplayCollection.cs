using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Practices.Composite;

namespace GroupedItemsTake2
{
    public class DisplayCollection : ObservableItemsCollection
    {
        private ObservableCollection<IDislpayItem> _selectedItems;

        public void AddItem(IDislpayItem item)
        {
            if (!SelectedItems.Any()) Add(item);
            else if (AreAnySelectedItemsAtTheTopLevel(SelectedItems)) Add(item);
            else if (AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                AddToGroup(item, GetItemGroup(SelectedItems.First()));
            }
        }
        
        public void InsertItem(IDislpayItem item)
        {
            var lowestSelectedIndex = GetLowestSelectedIndex(SelectedItems);
            if (!SelectedItems.Any()) Insert(lowestSelectedIndex, item);
            else if (AreAnySelectedItemsAtTheTopLevel(SelectedItems)) Insert(lowestSelectedIndex, item);
            else if (AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                InsertInGroup(item, GetItemGroup(SelectedItems.First()));
            }
        }

        public void GroupItems(IGroup group)
        {
            var itemsToGroup = CreateItemsToGroup(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            InsertItem(group);
            RemoveItems(GetItemsToRemove(SelectedItems));
            UpdateSelectedItems(new List<IDislpayItem>{group});
        }

        public void UnGroupSelectedItems()
        {
            var selectedParents = GetTopLevelSelectedParents(SelectedItems);
            foreach (var selectedParent in selectedParents)
            {
                UnGroup(selectedParent as IGroup);
            }
        }

        public void MoveSelectedItemsOutOfGroup()
        {
            MoveItemsOutOfGroup(SelectedItems);
        }
        
        public void MoveItemsOutOfGroup(ObservableCollection<IDislpayItem> observableCollection)
        {
            var itemsToGroup = CreateItemsToGroup(observableCollection).ToList();
            RemoveItems(GetGroupedItemsToRemove(observableCollection));
            foreach (var item in itemsToGroup)
            {
                if (IsItemAtTheTopLevel(item)) continue;
                if (ItemHasNoGrandParent(item))
                {
                    Insert(IndexOf(item.Parent), item);
                    item.SetParent(null);
                }
                else InsertInGroupAtParentIndex(item, GetItemGroup(item.Parent));
            }
        }

        private void UnGroup(IGroup group)
        {
            MoveItemsOutOfGroup(group.Items);
            RemoveItem(group);
        }

        private void RemoveItems(IEnumerable<IDislpayItem> items)
        {
            foreach (var item in items.ToList())
            {
                RemoveItem(item);
            }
        }

        private void RemoveItem(IDislpayItem item)
        {
            if (IsItemAChild(item))
            {
                var group = item.Parent as IGroup;
                if (group != null) group.Remove(item);
            }
            else Remove(item);
        }

        private void AddToGroup(IDislpayItem item, IGroup group)
        {
            group.Add(item);
        }
        
        private void InsertInGroup(IDislpayItem item, IGroup group)
        {
            group.Insert(item, SelectedItems);
        }
        
        private void InsertInGroupAtParentIndex(IDislpayItem item, IGroup group)
        {
            group.InsertAtParentIndex(item);
        }

        public void Delete()
        {
            RemoveItems(SelectedItems);
        }

        public void MoveUp()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(IsItemAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveItemsUp(ungroupedItems);
            if (childItems.Any())
            {
                var group = GetItemGroup(childItems.First());
                group.MoveItemsUp(childItems);
            }
            UpdateSelectedItems(items);
        }

        public void MoveDown()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(IsItemAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveItemsDown(ungroupedItems);
            if (childItems.Any())
            {
                var group = GetItemGroup(childItems.First());
                group.MoveItemsDown(childItems);
            }
            UpdateSelectedItems(items);
        }

        public void Duplicate()
        {
            foreach (var item in SelectedItems)
            {
                AddItem(item.Copy());
            }
        }

        public void UpdateSelectedItems(IEnumerable<IDislpayItem> items)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(items);
        }

        public ObservableCollection<IDislpayItem> SelectedItems
        {
            get
            {
                return _selectedItems ??
                     (_selectedItems = new ObservableCollection<IDislpayItem>());
            }
            set { _selectedItems = value; }
        }
    }
}