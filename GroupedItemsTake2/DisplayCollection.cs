using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Composite;

namespace GroupedItemsTake2
{
    public class DisplayCollection : ObservableItemsCollection
    {
        private ObservableCollection<IDislpayItem> _selectedItems;

        public void AddItem(IDislpayItem item)
        {
            if (!SelectedItems.Any()) Add(item);
            else if (GroupingLogic.AreAnySelectedItemsAtTheTopLevel(SelectedItems)) Add(item);
            else if (GroupingLogic.AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                AddToGroup(item, GroupingLogic.GetSelectedItemGroup(SelectedItems.First()));
            }
        }
        
        public void InsertItem(IDislpayItem item)
        {
            var lowestSelectedIndex = GetLowestSelectedIndex(SelectedItems);
            if (!SelectedItems.Any()) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreAnySelectedItemsAtTheTopLevel(SelectedItems)) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                InsertInGroup(item, GroupingLogic.GetSelectedItemGroup(SelectedItems.First()));
            }
        }

        public void GroupItems(IGroup group)
        {
            var itemsToGroup = GroupingLogic.CreateItemsToGroup(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            InsertItem(group);
            RemoveItems(GroupingLogic.GetItemsToRemove(SelectedItems));
            UpdateSelectedItems(new List<IDislpayItem>{group});
        }

        private void RemoveItems(IEnumerable<IDislpayItem> items)
        {
            foreach (var item in items.ToList())
            {
                if(GroupingLogic.IsItemAChild(item))
                {
                    var group = item.Parent as IGroup;
                    if (group != null) group.Remove(item);
                }
                else Remove(item);
            }
        }

        private void AddToGroup(IDislpayItem item, IGroup group)
        {
            group.Add(item);
        }
        
        private void InsertInGroup(IDislpayItem item, IGroup group)
        {
            group.Insert(item, SelectedItems);
        }

        public void MoveUp()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(GroupingLogic.IsItemAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveItemsUp(ungroupedItems);
            if (childItems.Any())
            {
                var group = GroupingLogic.GetSelectedItemGroup(childItems.First());
                group.MoveItemsUp(childItems);
            }
            UpdateSelectedItems(items);
        }

        public void MoveDown()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(GroupingLogic.IsItemAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveItemsDown(ungroupedItems);
            if (childItems.Any())
            {
                var group = GroupingLogic.GetSelectedItemGroup(childItems.First());
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