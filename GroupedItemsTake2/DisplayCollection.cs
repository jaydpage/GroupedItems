using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.Composite;

namespace GroupedItemsTake2
{
    public class DisplayCollection : IEnumerable<IDisplayItem>, INotifyCollectionChanged
	{
        private ObservableCollection<IDisplayItem> _selectedItems;
        private List<IDisplayItem> _cutItems;
	    private ObservableItemsCollection _items;

	    public DisplayCollection()
	    {
	        _items = new ObservableItemsCollection();
            Items.CollectionChanged += ItemsOnCollectionChanged;
	    }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null) return;
            CollectionChanged.Invoke(sender, e);
        }

        public void Add(IDisplayItem item, bool addToEmptyGroup = false)
        {
            if (!SelectedItems.Any()) AddAsUngrouped(item);
            else if (Items.GetTopLevelItems(SelectedItems) && !addToEmptyGroup) AddAsUngrouped(item);
            else if (Items.AreOfTheSameGroup(SelectedItems))
            {
                var group = Items.GetParent(SelectedItems.First());
                if (addToEmptyGroup)
                {
                    group = SelectedItems.First() as IGroup;                  
                }
                AddToGroup(item, group);
            }
        }

        public void AddPrompt(IEnumerable<IDisplayItem> items)
        {
            var result = PromptIfEmptyParentIsSelected();
            AddItems(items, result);
        }

        public void AddItems(IEnumerable<IDisplayItem> items, bool result)
        {
            foreach (var item in items)
            {
                Add(item, result);
            }
        }

        public void Insert(IDisplayItem item)
        {
            var lowestSelectedIndex = Items.GetLowestSelectedIndex(SelectedItems);
            if (!SelectedItems.Any()) Items.Insert(lowestSelectedIndex, item);
            else if (Items.GetTopLevelItems(SelectedItems)) Items.Insert(lowestSelectedIndex, item);
            else if (Items.AreOfTheSameGroup(SelectedItems))
            {
                InsertInGroup(item, Items.GetParent(SelectedItems.First()));
            }
        }

        public void Cut()
        {
            _cutItems = Items.Clone(SelectedItems).ToList();
            Remove(Items.GetDistinct(SelectedItems));
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
                Add(item.Copy());
            }
        }

        public void Delete()
        {
            Remove(SelectedItems);
        }

        public void UnGroup()
        {
            var selectedParents = Items.GetTopLevelParents(SelectedItems);
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
            var itemsToGroup = Items.Clone(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            Insert(group);
            Remove(Items.GetDistinct(SelectedItems));
            UpdateSelection(new List<IDisplayItem> { group });
        }

        public void MoveUp()
        {
            var items = SelectedItems.OrderBy(Items.IndexOf).ToList();
            var childItems = SelectedItems.Where(Items.IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            Items.MoveUp(ungroupedItems);
            if (childItems.Any())
            {
                var group = Items.GetParent(childItems.First());
                group.MoveItemsUp(childItems);
            }
            UpdateSelection(items);
        }

        public void MoveDown()
        {
            var items = SelectedItems.OrderBy(Items.IndexOf).ToList();
            var childItems = SelectedItems.Where(Items.IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            Items.MoveDown(ungroupedItems);
            if (childItems.Any())
            {
                var group = Items.GetParent(childItems.First());
                group.MoveItemsDown(childItems);
            }
            UpdateSelection(items);
        }

        public void UpdateSelection(IEnumerable<IDisplayItem> items)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(items);
        }

        private void MoveOutOfGroup(IEnumerable<IDisplayItem> items)
        {
            var itemsToGroup = Items.Clone(items).ToList();
            Remove(Items.GetMovableItems(items));
            foreach (var item in itemsToGroup)
            {
                if (Items.IsTopLevelItem(item)) continue;
                if (Items.IsGrandParentless(item))
                {
                    Items.Insert(Items.IndexOf(item.Parent), item);
                    item.SetParent(null);
                }
                else InsertInGroupAtParentIndex(item, Items.GetParent(item.Parent));
            }
        }

        private void AddAsUngrouped(IDisplayItem item)
        {
            item.SetParent(null);
            Items.Add(item);
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
                return SelectedItems.All(Items.IsChildlessParent);
	        }
	    }

        private void UnGroup(IGroup group)
        {
            MoveOutOfGroup(group.Items);
            RemoveItem(group);
        }

        private void Remove(IEnumerable<IDisplayItem> items)
        {
            foreach (var item in items.ToList())
            {
                RemoveItem(item);
            }
        }

        private void RemoveItem(IDisplayItem item)
        {
            if (Items.IsAChild(item))
            {
                var group = item.Parent as IGroup;
                if (group != null) group.Remove(item);
            }
            else Items.Remove(item);
        }

        private void AddToGroup(IDisplayItem item, IGroup group)
        {
            group.Add(item);
        }
        
        private void InsertInGroup(IDisplayItem item, IGroup group)
        {
            group.Insert(item, SelectedItems);
        }
        
        private void InsertInGroupAtParentIndex(IDisplayItem item, IGroup group)
        {
            group.InsertAtParentIndex(item);
        }

        public ObservableCollection<IDisplayItem> SelectedItems
        {
            get
            {
                return _selectedItems ??
                     (_selectedItems = new ObservableCollection<IDisplayItem>());
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

	    public ObservableItemsCollection Items
	    {
	        get { return _items; }
	        set { _items = value; }
	    }

	    public int Count { get { return Items.Count(); } }

	    public bool Contains(IDisplayItem item)
	    {
	        return Items.Contains(item);
	    }

	    public IEnumerator<IDisplayItem> GetEnumerator()
	    {
	        return Items.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }

	    public bool IsAParent(IDisplayItem item)
	    {
	        return Items.IsAParent(item);
	    }

	    public bool IsAChild(IDisplayItem item)
	    {
	        return Items.IsAChild(item);
	    }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}