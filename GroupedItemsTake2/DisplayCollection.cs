using System.Collections.Generic;
using System.Linq;

namespace GroupedItemsTake2
{
    public class DisplayCollection : ObservableItemsCollection
    {
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
            var lowestSelectedIndex = GetLowestSelectedIndex();
            if (!SelectedItems.Any()) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreAnySelectedItemsAtTheTopLevel(SelectedItems)) Insert(lowestSelectedIndex, item);
            else if (GroupingLogic.AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                AddToGroup(item, GroupingLogic.GetSelectedItemGroup(SelectedItems.First()));
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

        private int GetLowestSelectedIndex()
        {
            var lowestIndex = int.MaxValue;
            foreach (var item in SelectedItems)
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
    }
}