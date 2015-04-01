using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Composite;

namespace GroupedItemsTake2
{
	public class DisplayCollection : ObservableItemsCollection
	{
        private ObservableCollection<IDislpayItem> _selectedItems;
        private List<IDislpayItem> _cutItems;

	    private void Add(IDislpayItem item, bool addToEmptyGroup)
        {
            if (!SelectedItems.Any()) AddAsUngrouped(item);
            else if (GetTopLevelItems(SelectedItems) && !addToEmptyGroup) AddAsUngrouped(item);
            else if (AreOfTheSameGroup(SelectedItems))
            {
                var group = GetParent(SelectedItems.First());
                if (addToEmptyGroup)
                {
                    group = SelectedItems.First() as IGroup;                  
                }
                AddToGroup(item, group);
            }
        }

        public void AddPrompt(IEnumerable<IDislpayItem> items)
        {
            var result = PromptIfEmptyParentIsSelected();
            AddItems(items, result);
        }

        public void AddItems(IEnumerable<IDislpayItem> items, bool result)
        {
            foreach (var item in items)
            {
                Add(item, result);
            }
        }

        public void Insert(IDislpayItem item)
        {
            var lowestSelectedIndex = GetLowestSelectedIndex(SelectedItems);
            if (!SelectedItems.Any()) Insert(lowestSelectedIndex, item);
            else if (GetTopLevelItems(SelectedItems)) Insert(lowestSelectedIndex, item);
            else if (AreOfTheSameGroup(SelectedItems))
            {
                InsertInGroup(item, GetParent(SelectedItems.First()));
            }
        }

        public void Cut()
        {
            _cutItems = Clone(SelectedItems).ToList();
            Remove(GetDistinct(SelectedItems));
        }

        public void Paste()
        {
            AddPrompt(_cutItems);
            _cutItems.Clear();
        }

        public void Duplicate()
        {
            foreach (var item in SelectedItems)
            {
                Add(item.Copy(), false);
            }
        }

        public void Delete()
        {
            Remove(SelectedItems);
        }

        public void UnGroup()
        {
            var selectedParents = GetTopLevelParents(SelectedItems);
            foreach (var selectedParent in selectedParents)
            {
                UnGroup(selectedParent as IGroup);
            }
        }

        public void MoveOutOfGroup()
        {
            MoveOutOfGroup(SelectedItems);
        }

        public void MoveTo(IGroup group)
        {
            var itemsToGroup = Clone(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            Insert(group);
            Remove(GetDistinct(SelectedItems));
            UpdateSelection(new List<IDislpayItem> { group });
        }

        public void MoveUp()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveUp(ungroupedItems);
            if (childItems.Any())
            {
                var group = GetParent(childItems.First());
                group.MoveItemsUp(childItems);
            }
            UpdateSelection(items);
        }

        public void MoveDown()
        {
            var items = SelectedItems.OrderBy(IndexOf).ToList();
            var childItems = SelectedItems.Where(IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            MoveDown(ungroupedItems);
            if (childItems.Any())
            {
                var group = GetParent(childItems.First());
                group.MoveItemsDown(childItems);
            }
            UpdateSelection(items);
        }

        public void UpdateSelection(IEnumerable<IDislpayItem> items)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(items);
        }

        private void MoveOutOfGroup(ObservableCollection<IDislpayItem> items)
        {
            var itemsToGroup = Clone(items).ToList();
            Remove(GetMovableItems(items));
            foreach (var item in itemsToGroup)
            {
                if (IsTopLevelItem(item)) continue;
                if (IsGrandParentless(item))
                {
                    Insert(IndexOf(item.Parent), item);
                    item.SetParent(null);
                }
                else InsertInGroupAtParentIndex(item, GetParent(item.Parent));
            }
        }

        private void AddAsUngrouped(IDislpayItem item)
        {
            item.SetParent(null);
            Add(item);
        }

	    private bool PromptIfEmptyParentIsSelected()
	    {
	        if (!SelectedIsANotAEmptyParent) return false;
	        var view = new AddToParentPromptDialog();
	        var vm = new AddToParentPromptDialogViewModel(view);
	        view.DataContext = vm;
	        view.ShowDialog();
	        return vm.Result;
	    }

	    private bool SelectedIsANotAEmptyParent
	    {
	        get
	        {
	            if (SelectedItems.Count != 1) return false;
                return SelectedItems.All(IsChildlessParent);
	        }
	    }

        private void UnGroup(IGroup group)
        {
            MoveOutOfGroup(group.Items);
            RemoveItem(group);
        }

        private void Remove(IEnumerable<IDislpayItem> items)
        {
            foreach (var item in items.ToList())
            {
                RemoveItem(item);
            }
        }

        private void RemoveItem(IDislpayItem item)
        {
            if (IsAChild(item))
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

		public void Group(string groupName)
		{
			var newGroup = GroupedItemsTake2.Group.CreateGroup(groupName);
			MoveTo(newGroup);
		}
	}
}