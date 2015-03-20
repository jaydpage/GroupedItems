using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GroupedItemsTake2
{
    public class DisplayCollection : ObservableCollection<IDislpayItem>
    {
        public void AddItem(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems)
        {
            if (!selectedItems.Any()) Add(item);
            else if (GroupingLogic.AreAnySelectedItemsAtTheTopLevel(selectedItems)) Add(item);
            else if (GroupingLogic.AreSelectedItemsOfTheSameGroup(selectedItems))
            {
                AddToGroup(item, GroupingLogic.GetSelectedItemGroup(selectedItems.First()));
            }
        }
        
        public void InsertItem(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems)
        {
            var lowestSelectedIndex = GetLowestSelectedIndex(selectedItems);
            if (!selectedItems.Any()) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreAnySelectedItemsAtTheTopLevel(selectedItems)) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreSelectedItemsOfTheSameGroup(selectedItems))
            {
                AddToGroup(item, GroupingLogic.GetSelectedItemGroup(selectedItems.First()));
            }
        }

        public void GroupItems(IGroup group, IEnumerable<IDislpayItem> selectedItems)
        {
            var itemsToGroup = GroupingLogic.CreateItemsToGroup(selectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            InsertItem(group, selectedItems);
            RemoveItems(GroupingLogic.GetItemsToRemove(selectedItems));
        }

        private void RemoveItems(IEnumerable<IDislpayItem> selectedItems)
        {
            foreach (var selectedItem in selectedItems.ToList())
            {
                if(GroupingLogic.IsItemAChild(selectedItem))
                {
                    var group = selectedItem.Parent as IGroup;
                    if (group != null) group.Remove(selectedItem);
                }
                else Remove(selectedItem);
            }
        }

        private int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems)
        {
            var lowestIndex = int.MaxValue;
            foreach (var item in selectedItems)
            {
                var index = IndexOf(item);
                if (!Contains(item)) continue;
                if (index < lowestIndex) lowestIndex = index;
            }
            return lowestIndex < 0 ? 0 : lowestIndex;
        }

        private void AddToGroup(IDislpayItem item, IGroup group)
        {
            group.Add(item);
        }

        public void MoveUp(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderBy(IndexOf).ToList();

            while (items.Any())
            {
                var current = items.First();
                var index = IndexOf(current);
                items.Remove(current);
                if (index <= 0) return;
                RemoveAt(index);
                Insert(index - 1, current);
            }
        }
        
        public void MoveDown(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderByDescending(IndexOf).ToList();

            while (items.Any())
            {
                var current = items.First();
                var index = IndexOf(current);
                items.Remove(current);
                if (index >= Count - 1) return;
                RemoveAt(index);
                Insert(index + 1, current);
            }
        }

        public void Duplicate(ObservableCollection<IDislpayItem> selectedItems)
        {
            foreach (var item in selectedItems)
            {
                AddItem(item.Copy(), selectedItems);
            }
        }
    }
}