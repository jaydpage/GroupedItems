using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GroupedItemsTake2
{
    public class ObservableItemsCollection : ObservableCollection<IDislpayItem>, IGroupingLogic
    {
        public void MoveDown(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderByDescending(IndexOf).ToList();

            while (items.Any())
            {
                var current = items.First();
                items.Remove(current);
                var index = IndexOf(current);
                if (index >= Count - 1) return;
                RemoveAt(index);
                Insert(index + 1, current);
            }
        }

        public void MoveUp(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderBy(IndexOf).ToList();
            while (items.Any())
            {
                var current = items.First();
                items.Remove(current);
                var index = IndexOf(current);
                if (index <= 0) return;
                RemoveAt(index);
                Insert(index - 1, current);
            }
        }

        public int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems)
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

        public bool AreSelectedItemsOfTheSameGroup(IEnumerable<IDislpayItem> selectedItems)
        {
            if (!selectedItems.Any()) return false;
            var group = GetParent(selectedItems.First());
            return selectedItems.All(selectedItem => @group == GetParent(selectedItem));
        }

        public IGroup GetParent(IDislpayItem selectedItem)
        {
            if (selectedItem.Level == Level.Ungrouped) return null;
            if (IsItemAChild(selectedItem)) return selectedItem.Parent as IGroup;
            return selectedItem as IGroup;
        }

        public IEnumerable<IDislpayItem> CloneSelected(IEnumerable<IDislpayItem> selectedItems)
        {
            var itemsToGroup = new List<IDislpayItem>();
            var selectedDistinct = GetDistinctItems(selectedItems);
            itemsToGroup.AddRange(selectedDistinct.Select(item => (IDislpayItem)item.Clone()));
            return itemsToGroup;
        }

        public IEnumerable<IDislpayItem> GetDistinctItems(IEnumerable<IDislpayItem> items)
        {
            var topSelectedParents = GetTopLevelSelectedParents(items);
            var itemsWithNoSelectedParents = GetItemsWithNoSelectedParents(items);
            return topSelectedParents.Concat(itemsWithNoSelectedParents).Distinct();
        }

        public IEnumerable<IDislpayItem> GetTopLevelSelectedParents(IEnumerable<IDislpayItem> items)
        {
            var itemGroups = new List<IDislpayItem>();
            foreach (var item in items)
            {
                if (!IsItemAParent(item)) continue;
                var topGroup = GetTopLevelSelectedItem(item, items);
                if (itemGroups.All(x => x.UID != topGroup.UID)) itemGroups.Add(topGroup);
            }
            return itemGroups;
        }

        public IEnumerable<IDislpayItem> GetGroupedItemsToRemove(IEnumerable<IDislpayItem> selectedItems)
        {
            return GetDistinctItems(selectedItems).Where(x => !IsTopLevelItem(x)).ToList();
        }

        public bool AreAnySelectedItemsAtTheTopLevel(IEnumerable<IDislpayItem> selected)
        {
            return selected.Any(x => x.Level == Level.Ungrouped || x.Level == Level.Parent);
        }

        public bool IsTopLevelItem(IDislpayItem item)
        {
            return IsItemUngrouped(item) || IsItemExclusivelyAParent(item);
        }

        public bool IsItemAParent(IDislpayItem item)
        {
            if (IsItemExclusivelyAParent(item)) return true;
            return IsItemAParentChild(item);
        }      
        
        public bool IsItemAParentWithoutChildren(IDislpayItem item)
        {
            if (!IsItemAParent(item)) return false;
            var group = item as IGroup;
            return group != null && group.Count() == 0;
        }      

        public bool IsItemGrandParentless(IDislpayItem item)
        {
            if (item.Parent == null) return true;
            return item.Parent.Parent == null;
        }

        public bool IsItemAChild(IDislpayItem item)
        {
            if (IsItemExclusivelyAChild(item)) return true;
            return IsItemAParentChild(item);
        }
       
        private bool IsItemUngrouped(IDislpayItem item)
        {
            return item.Level == Level.Ungrouped;
        }

        private bool IsItemAParentChild(IDislpayItem item)
        {
            return item.Level == Level.ParentChild;
        }

        private static bool IsItemExclusivelyAChild(IDislpayItem item)
        {
            return item.Level == Level.Child;
        }

        private bool IsItemExclusivelyAParent(IDislpayItem item)
        {
            return item.Level == Level.Parent;
        }

        private IEnumerable<IDislpayItem> GetItemsWithNoSelectedParents(IEnumerable<IDislpayItem> items)
        {
            var allSelectedGroups = items.Where(IsItemAParent);
            var itemsWithoutSelectedGroups = new List<IDislpayItem>();
            foreach (var item in items)
            {
                if (IsItemAChild(item))
                {
                    if (allSelectedGroups.All(x => x.UID != item.Parent.UID))
                        itemsWithoutSelectedGroups.Add(item);
                }
                else
                {
                    itemsWithoutSelectedGroups.Add(item);
                }
            }
            return itemsWithoutSelectedGroups;
        }

        private static IDislpayItem GetTopLevelSelectedItem(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems)
        {
            if (item.Parent == null) return item;
            if (selectedItems.Any(x => x.UID == item.Parent.UID))
                return GetTopLevelSelectedItem(item.Parent, selectedItems);

            return item;
        }

        
    }
}
