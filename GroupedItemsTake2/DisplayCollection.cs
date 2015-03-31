using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Composite;
using NUnit.Framework;

namespace GroupedItemsTake2
{
	public class DisplayCollection : ObservableItemsCollection, IDisplayCollection
	{
        private ObservableCollection<IDislpayItem> _selectedItems;
        private List<IDislpayItem> _cutItems;

	    private void AddItem(IDislpayItem item, bool addToEmptyGroup)
        {
            if (!SelectedItems.Any()) AddAsUngrouped(item);
            else if (AreAnySelectedItemsAtTheTopLevel(SelectedItems) && !addToEmptyGroup) AddAsUngrouped(item);
            else if (AreSelectedItemsOfTheSameGroup(SelectedItems))
            {
                var group = GetItemGroup(SelectedItems.First());
                if (addToEmptyGroup)
                {
                    group = SelectedItems.First() as IGroup;                  
                }
                AddToGroup(item, group);
            }
        }

        private void AddAsUngrouped(IDislpayItem item)
        {
            item.SetParent(null);
            Add(item);
        }

        public void AddItemsPrompt(IEnumerable<IDislpayItem> items)
        {
            var result = PromptIfEmptyGroupIsSelected();
            AddItems(items, result);
        }

	    public void AddItems(IEnumerable<IDislpayItem> items, bool result)
	    {
	        foreach (var item in items)
	        {
	            AddItem(item, result);
	        }
	    }

	    private bool PromptIfEmptyGroupIsSelected()
	    {
	        if (SelectedItemIsAnEmptyGroup)
	        {
	            var view = new AddGroupPromptDialog();
                var vm = new AddGroupPromptDialogViewModel(view);
	            view.DataContext = vm;
	            view.ShowDialog();
	            return vm.Result;
	        }
	        return false;
	    }

	    private bool SelectedItemIsAnEmptyGroup
	    {
	        get
	        {
	            if (SelectedItems.Count != 1) return false;
                return SelectedItems.All(IsItemAParentWithoutChildren);
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

        public void MoveTo(IGroup group)
        {
            var itemsToGroup = GetDistinctItems(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            InsertItem(group);
            RemoveItems(GetItemsToRemove(SelectedItems));
            UpdateSelectedItems(new List<IDislpayItem>{group});
        }

        public void CutSelected()
        {
            _cutItems = GetDistinctItems(SelectedItems).ToList();
            RemoveItems(GetItemsToRemove(SelectedItems));
        }
        
        public void Paste()
        {
            AddItemsPrompt(_cutItems);
            _cutItems.Clear();
        }

        public void UnGroupSelected()
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
        
        public void MoveItemsOutOfGroup(ObservableCollection<IDislpayItem> items)
        {
            var itemsToGroup = GetDistinctItems(items).ToList();
            RemoveItems(GetGroupedItemsToRemove(items));
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

        public void DeleteSelected()
        {
            RemoveItems(SelectedItems);
        }

        public void MoveSelectedUp()
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

        public void MoveSelectedDown()
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

        public void DuplicateSelected()
        {
            foreach (var item in SelectedItems)
            {
                AddItem(item.Copy(), false);
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
            set
            {
                _selectedItems = value; 
            }
        }

		public void GroupSelected(string groupName)
		{
			var newGroup = Group.CreateGroup(groupName);
			MoveTo(newGroup);
		}
	}
}