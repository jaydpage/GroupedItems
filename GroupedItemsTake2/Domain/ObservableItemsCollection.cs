using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupedItemsTake2.Interfaces;

namespace GroupedItemsTake2.Domain
{
    public class ObservableItemsCollection : ObservableCollection<IDisplayItem>, IGroupingLogic
    {
        public void MoveDown(IEnumerable<IDisplayItem> selected)
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

        public void MoveUp(IEnumerable<IDisplayItem> selected)
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

        public int GetLowestSelectedIndex(IEnumerable<IDisplayItem> selected)
        {
            var lowestIndex = selected.Select(IndexOf).Concat(new[] {int.MaxValue}).Min();
            return lowestIndex < 0 ? 0 : lowestIndex;
        }

        public bool BelongToTheSameGroup(IEnumerable<IDisplayItem> selected)
        {
            if (!selected.Any()) return false;
            var group = GetParent(GetHighestSelectedItems(selected).First());
            return GetDistinct(selected).All(item => group == GetParent(item));
        }

        public IGroup GetParent(IDisplayItem selected)
        {
            if (IsUngrouped(selected)) return null;
            if (IsOnlyAParent(selected)) return null;
            if (IsAChild(selected)) return selected.Parent as IGroup;
            return selected as IGroup;
        }

        public IEnumerable<IDisplayItem> Clone(IEnumerable<IDisplayItem> selected)
        {
            var itemsToGroup = new List<IDisplayItem>();
            var selectedDistinct = GetDistinct(selected);
            itemsToGroup.AddRange(selectedDistinct.Select(item => (IDisplayItem)item.Clone()));
            return itemsToGroup;        
        }

        public IEnumerable<IDisplayItem> GetDistinct(IEnumerable<IDisplayItem> selected)
        {
            var topSelectedParents = GetHighestSelectedItems(selected.Where(IsAParent));
            var itemsWithNoSelectedParents = GetItemsWithoutSelectedParents(selected);
            return itemsWithNoSelectedParents.Concat(topSelectedParents).Distinct();
        }

        public IEnumerable<IDisplayItem> GetHighestSelectedItems(IEnumerable<IDisplayItem> selected)
        {
            var items = new List<IDisplayItem>();
            foreach (var item in selected)
            {
                var topItem = GetHighestLevelItem(item, selected);
                if (items.All(x => x.UID != topItem.UID)) items.Add(topItem);
            }
            return items;
        }

        public IEnumerable<IDisplayItem> GetMovableItems(IEnumerable<IDisplayItem> selected)
        {
            return GetDistinct(selected).Where(x => !IsTopLevelItem(x)).ToList();
        }

        public bool AreAnyItemsTopLevelItems(IEnumerable<IDisplayItem> selected)
        {
            return selected.Any(x => x.Level == Level.Ungrouped || x.Level == Level.Parent);
        }
        
        public bool AreAllTopLevelItemsParents()
        {
            if (!this.Any()) return false;
            return this.All(IsAParent);
        }

        public bool IsTopLevelItem(IDisplayItem item)
        {
            return IsUngrouped(item) || IsOnlyAParent(item);
        }

        public bool IsAParent(IDisplayItem item)
        {
            if (IsOnlyAParent(item)) return true;
            return IsAParentChild(item);
        }      

        public bool IsGrandParentless(IDisplayItem item)
        {
            if (item.Parent == null) return true;
            return item.Parent.Parent == null;
        }

        public bool IsAChild(IDisplayItem item)
        {
            if (IsOnlyAChild(item)) return true;
            return IsAParentChild(item);
        }
       
        private bool IsUngrouped(IDisplayItem item)
        {
            return item.Level == Level.Ungrouped;
        }

        private bool IsAParentChild(IDisplayItem item)
        {
            return item.Level == Level.ParentChild;
        }

        private static bool IsOnlyAChild(IDisplayItem item)
        {
            return item.Level == Level.Child;
        }

        private bool IsOnlyAParent(IDisplayItem item)
        {
            return item.Level == Level.Parent;
        }

        private IEnumerable<IDisplayItem> GetItemsWithoutSelectedParents(IEnumerable<IDisplayItem> selected)
        {
            var allSelectedGroups = selected.Where(IsAParent);
            var itemsWithoutSelectedGroups = new List<IDisplayItem>();
            foreach (var item in selected)
            {
                if (IsAChild(item))
                {
                    if (allSelectedGroups.All(x => x.UID != item.Parent.UID))
                    {
                        itemsWithoutSelectedGroups.Add(item);                        
                    }
                    continue;
                }
                itemsWithoutSelectedGroups.Add(item);            
            }
            return itemsWithoutSelectedGroups;
        }

        private static IDisplayItem GetHighestLevelItem(IDisplayItem item, IEnumerable<IDisplayItem> selectedItems)
        {
            if (item.Parent == null) return item;
            if (selectedItems.Any(x => x.UID == item.Parent.UID))
            {
                return GetHighestLevelItem(item.Parent, selectedItems);                
            }
            return item;
        }

        public bool IsChildlessParent(IDisplayItem item)
        {
            if (!IsAParent(item)) return false;
            var group = item as IGroup;
            return group != null && group.Count() == 0;
        }      
    }
}
