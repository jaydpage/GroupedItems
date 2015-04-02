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
	    private readonly ObservableItemsCollection _items;

	    public DisplayCollection()
	    {
	        _items = new ObservableItemsCollection();
            _items.CollectionChanged += ItemsOnCollectionChanged;
	    }

        public void Add(IDisplayItem item, bool addToEmptyGroup = false)
        {
            if (!SelectedItems.Any())
            {
                AddAsUngrouped(item);
                return;
            }
            if (_items.GetTopLevelItems(SelectedItems) && !addToEmptyGroup)
            {
                AddAsUngrouped(item);
                return;
            }
            if (!_items.AreOfTheSameGroup(SelectedItems)) return;
            var group = _items.GetParent(SelectedItems.First());
            if (addToEmptyGroup)
            {
                group = SelectedItems.First() as IGroup;                  
            }
            AddToGroup(item, group);
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
            var lowestSelectedIndex = _items.GetLowestSelectedIndex(SelectedItems);
            if (!SelectedItems.Any())
            {
                _items.Insert(lowestSelectedIndex, item);
                return;
            }
            if (_items.GetTopLevelItems(SelectedItems))
            {
                _items.Insert(lowestSelectedIndex, item);
                return;
            }
            if (_items.AreOfTheSameGroup(SelectedItems))
            {
                InsertInGroup(item, _items.GetParent(SelectedItems.First()));
            }
        }

        public void Cut()
        {
            _cutItems = _items.Clone(SelectedItems).ToList();
            Remove(_items.GetDistinct(SelectedItems));
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
            var selectedParents = _items.GetTopLevelParents(SelectedItems);
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
            var itemsToGroup = _items.Clone(SelectedItems).ToList();
            foreach (var item in itemsToGroup)
            {
                AddToGroup(item, group);
            }
            Insert(group);
            Remove(_items.GetDistinct(SelectedItems));
            UpdateSelection(new List<IDisplayItem> { group });
        }

        public void MoveUp()
        {
            var items = SelectedItems.OrderBy(_items.IndexOf).ToList();
            var childItems = SelectedItems.Where(_items.IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            _items.MoveUp(ungroupedItems);
            if (childItems.Any())
            {
                var group = _items.GetParent(childItems.First());
                group.MoveItemsUp(childItems);
            }
            UpdateSelection(items);
        }

        public void MoveDown()
        {
            var items = SelectedItems.OrderBy(_items.IndexOf).ToList();
            var childItems = SelectedItems.Where(_items.IsAChild);
            var ungroupedItems = items.Except(childItems).ToList();

            _items.MoveDown(ungroupedItems);
            if (childItems.Any())
            {
                var group = _items.GetParent(childItems.First());
                group.MoveItemsDown(childItems);
            }
            UpdateSelection(_items);
        }

        public void UpdateSelection(IEnumerable<IDisplayItem> items)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(_items);
        }

        private void MoveOutOfGroup(IEnumerable<IDisplayItem> items)
        {
            var itemsToGroup = _items.Clone(items).ToList();
            Remove(_items.GetMovableItems(items));
            foreach (var item in itemsToGroup)
            {
                if (_items.IsTopLevelItem(item)) continue;
                if (_items.IsGrandParentless(item))
                {
                    _items.Insert(_items.IndexOf(item.Parent), item);
                    item.SetParent(null);
                    continue;
                }
                InsertInGroupAtParentIndex(item, _items.GetParent(item.Parent));
            }
        }

        public void AddAsUngrouped(IDisplayItem item)
        {
            item.SetParent(null);
            _items.Add(item);
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
                return SelectedItems.All(_items.IsChildlessParent);
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
            if (_items.IsAChild(item))
            {
                var group = item.Parent as IGroup;
                if (group != null)
                {
                    group.Remove(item);
                }
                return;
            }
            _items.Remove(item);
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

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged == null) return;
            CollectionChanged.Invoke(sender, e);
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

	    public int Count { get { return _items.Count(); } }

	    public bool Contains(IDisplayItem item)
	    {
	        return _items.Contains(item);
	    }

	    public IEnumerator<IDisplayItem> GetEnumerator()
	    {
	        return _items.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }

	    public bool IsAParent(IDisplayItem item)
	    {
	        return _items.IsAParent(item);
	    }

	    public bool IsAChild(IDisplayItem item)
	    {
	        return _items.IsAChild(item);
	    }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}