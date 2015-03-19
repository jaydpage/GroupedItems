using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupedItemsTake2
{
    public static class GroupingLogic
    {
        public static bool AreAnySelectedItemsAtTheTopLevel(IEnumerable<IDislpayItem> selected)
        {
            return selected.Any(x => x.Level == Level.Ungrouped || x.Level == Level.Parent);
        }
        
        public static bool AreSelectedItemsOfTheSameGroup(IEnumerable<IDislpayItem> selectedItems)
        {
            if (!selectedItems.Any()) return false;
            var group = GetSelectedItemGroup(selectedItems.First());
            return selectedItems.All(selectedItem => @group == GetSelectedItemGroup(selectedItem));
        }

        public static IGroup GetSelectedItemGroup(IDislpayItem selectedItem)
        {
            if (selectedItem.Level == Level.Ungrouped) return null;
            if (selectedItem.Level == Level.Child) return selectedItem.Parent as IGroup;
            return selectedItem as IGroup;
        }

        public static IEnumerable<IDislpayItem> CreateItemsToGroup(IEnumerable<IDislpayItem> selectedItems)
        {
            var itemsToGroup = new List<IDislpayItem>();
            var selectedDistinct = GetDistinctItemsToGroup(selectedItems);
            itemsToGroup.AddRange(selectedDistinct.Select(item => (IDislpayItem)item.Clone()));
            return itemsToGroup;
        } 
        
        public static IEnumerable<IDislpayItem> GetItemsToRemove(IEnumerable<IDislpayItem> selectedItems)
        {
            return GetDistinctItemsToGroup(selectedItems);
        }

        private static IEnumerable<IDislpayItem> GetDistinctItemsToGroup(IEnumerable<IDislpayItem> items)
        {
            var topSelectedParents = GetTopLevelSelectedParents(items);
            var itemsWithNoSelectedParents = GetItemsWithNoSelectedParents(items);
            return topSelectedParents.Concat(itemsWithNoSelectedParents).Distinct();
        }

        private static IEnumerable<IDislpayItem> GetTopLevelSelectedParents(IEnumerable<IDislpayItem> items)
        {
            var itemGroups = new List<IDislpayItem>();
            foreach (var item in items)
            {
                if (!IsItemAParent(item)) continue;
                var topGroup = GetTopLevelSelectedItem(item, items);
                if (itemGroups.All(x => x != topGroup)) itemGroups.Add(topGroup);
            }
            return itemGroups;
        }

        private static bool IsItemAParent(IDislpayItem item)
        {
            if (item.Level == Level.Parent) return true;
            return item.Level == Level.ParentChild;
        }
        
        public static bool IsItemAChild(IDislpayItem item)
        {
            if (item.Level == Level.Child) return true;
            return item.Level == Level.ParentChild;
        }

        private static IEnumerable<IDislpayItem> GetItemsWithNoSelectedParents(IEnumerable<IDislpayItem> items)
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
