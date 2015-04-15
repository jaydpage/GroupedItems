using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GroupedItemsTake2.Interfaces;

namespace GroupedItemsTake2.Domain
{
    public class ObservableItemsCollection : IEnumerable<IDisplayItem>, IGroupingLogic, INotifyCollectionChanged
    {
        private ObservableCollection<IDisplayItem> _items;

        public ObservableItemsCollection()
        {
            _items = new ObservableCollection<IDisplayItem>();
            _items.CollectionChanged +=ItemsCollectionChanged;
        }

        public void MoveDown(IEnumerable<IDisplayItem> selected)
        {
            var items = selected.OrderByDescending(IndexOf).ToList();
            while (items.Any())
            {
                var current = items.First();
                items.Remove(current);
                var index = IndexOf(current);
                if (index >= _items.Count - 1) return;
                Remove(index);
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
                Remove(index);
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
            var group = GetParent(selected.First());
            return GetDistinct(selected).All(selectedItem => group == GetParent(selectedItem));
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
            var topSelectedParents = GetHighestLevelParents(selected);
            var itemsWithNoSelectedParents = GetItemsWithoutSelectedParents(selected);
            return itemsWithNoSelectedParents.Concat(topSelectedParents).Distinct();
        }

        public IEnumerable<IDisplayItem> GetHighestLevelParents(IEnumerable<IDisplayItem> selected)
        {
            var itemGroups = new List<IDisplayItem>();
            foreach (var item in selected)
            {
                if (!IsAParent(item)) continue;
                var topGroup = GetHighestLevelItem(item, selected);
                if (itemGroups.All(x => x.UID != topGroup.UID)) itemGroups.Add(topGroup);
            }
            return itemGroups;
        }

        public IEnumerable<IDisplayItem> GetMovableItems(IEnumerable<IDisplayItem> selected)
        {
            return GetDistinct(selected).Where(x => !IsTopLevelItem(x)).ToList();
        }

        public bool AreAnyItemsTopLevelItems(IEnumerable<IDisplayItem> selected)
        {
            return selected.Any(x => x.Level == Level.Ungrouped || x.Level == Level.Parent);
        }

        public void Clear()
        {
            _items.Clear();
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
        
        public bool IsChildlessParent(IDisplayItem item)
        {
            if (!IsAParent(item)) return false;
            var group = item as IGroup;
            return group != null && group.Count() == 0;
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

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null) return;
            CollectionChanged.Invoke(sender, e);
        }

        public IEnumerator<IDisplayItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IDisplayItem item)
        {
            _items.Add(item);
        }

        public int Count()
        {
            return _items.Count;
        }

        public int IndexOf(IDisplayItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, IDisplayItem item)
        {
            _items.Insert(index, item);
        }

        private void Remove(int index)
        {
            _items.RemoveAt(index);
        }

        public void Remove(IDisplayItem item)
        {
            _items.Remove(item);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
