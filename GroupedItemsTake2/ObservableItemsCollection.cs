using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GroupedItemsTake2
{
    public class ObservableItemsCollection : ObservableCollection<IDislpayItem>, IGroupingLogic
    {
        public void MoveDown(IEnumerable<IDislpayItem> selected)
        {
            var items = selected.OrderByDescending(IndexOf).ToList();

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

        public void MoveUp(IEnumerable<IDislpayItem> selected)
        {
            var items = selected.OrderBy(IndexOf).ToList();
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

        public int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selected)
        {
            var lowestIndex = int.MaxValue;
            foreach (var item in selected)
            {
                var index = IndexOf(item);
                if (!Contains(item)) continue;
                if (index < lowestIndex) lowestIndex = index;
            }
            return lowestIndex < 0 ? 0 : lowestIndex;
        }

        public bool AreOfTheSameGroup(IEnumerable<IDislpayItem> selected)
        {
            if (!selected.Any()) return false;
            var group = GetParent(selected.First());
            return selected.All(selectedItem => @group == GetParent(selectedItem));
        }

        public IGroup GetParent(IDislpayItem selected)
        {
            if (selected.Level == Level.Ungrouped) return null;
            if (IsAChild(selected)) return selected.Parent as IGroup;
            return selected as IGroup;
        }

        public IEnumerable<IDislpayItem> Clone(IEnumerable<IDislpayItem> selected)
        {
            var itemsToGroup = new List<IDislpayItem>();
            var selectedDistinct = GetDistinct(selected);
            itemsToGroup.AddRange(selectedDistinct.Select(item => (IDislpayItem)item.Clone()));
            return itemsToGroup;
        }

        public IEnumerable<IDislpayItem> GetDistinct(IEnumerable<IDislpayItem> selected)
        {
            var topSelectedParents = GetTopLevelParents(selected);
            var itemsWithNoSelectedParents = GetItemsWithoutSelectedParents(selected);
            return topSelectedParents.Concat(itemsWithNoSelectedParents).Distinct();
        }

        public IEnumerable<IDislpayItem> GetTopLevelParents(IEnumerable<IDislpayItem> selected)
        {
            var itemGroups = new List<IDislpayItem>();
            foreach (var item in selected)
            {
                if (!IsAParent(item)) continue;
                var topGroup = GetHighestLevelItem(item, selected);
                if (itemGroups.All(x => x.UID != topGroup.UID)) itemGroups.Add(topGroup);
            }
            return itemGroups;
        }

        public IEnumerable<IDislpayItem> GetMovableItems(IEnumerable<IDislpayItem> selected)
        {
            return GetDistinct(selected).Where(x => !IsTopLevelItem(x)).ToList();
        }

        public bool GetTopLevelItems(IEnumerable<IDislpayItem> selected)
        {
            return selected.Any(x => x.Level == Level.Ungrouped || x.Level == Level.Parent);
        }

        public bool IsTopLevelItem(IDislpayItem item)
        {
            return IsUngrouped(item) || IsOnlyAParent(item);
        }

        public bool IsAParent(IDislpayItem item)
        {
            if (IsOnlyAParent(item)) return true;
            return IsAParentChild(item);
        }      
        
        public bool IsChildlessParent(IDislpayItem item)
        {
            if (!IsAParent(item)) return false;
            var group = item as IGroup;
            return group != null && group.Count() == 0;
        }      

        public bool IsGrandParentless(IDislpayItem item)
        {
            if (item.Parent == null) return true;
            return item.Parent.Parent == null;
        }

        public bool IsAChild(IDislpayItem item)
        {
            if (IsOnlyAChild(item)) return true;
            return IsAParentChild(item);
        }
       
        private bool IsUngrouped(IDislpayItem item)
        {
            return item.Level == Level.Ungrouped;
        }

        private bool IsAParentChild(IDislpayItem item)
        {
            return item.Level == Level.ParentChild;
        }

        private static bool IsOnlyAChild(IDislpayItem item)
        {
            return item.Level == Level.Child;
        }

        private bool IsOnlyAParent(IDislpayItem item)
        {
            return item.Level == Level.Parent;
        }

        private IEnumerable<IDislpayItem> GetItemsWithoutSelectedParents(IEnumerable<IDislpayItem> selected)
        {
            var allSelectedGroups = selected.Where(IsAParent);
            var itemsWithoutSelectedGroups = new List<IDislpayItem>();
            foreach (var item in selected)
            {
                if (IsAChild(item))
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

        private static IDislpayItem GetHighestLevelItem(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems)
        {
            if (item.Parent == null) return item;
            if (selectedItems.Any(x => x.UID == item.Parent.UID))
                return GetHighestLevelItem(item.Parent, selectedItems);

            return item;
        }

        
    }
}
