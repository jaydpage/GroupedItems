using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.Composite;

namespace GroupedItemsTake2
{
    public class DisplayCollection : IEnumerable<IDisplayItem>, INotifyCollectionChanged, IDisplayCollection
    {
        private ObservableCollection<IDisplayItem> _selectedItems;
        private List<IDisplayItem> _cutItems;
	    private readonly ObservableItemsCollection _items;
        private bool _addToEmptyGroup;

	    public DisplayCollection()
	    {
	        _items = new ObservableItemsCollection();
            _items.CollectionChanged += ItemsOnCollectionChanged;
	    }

        public void Add(IDisplayItem item)
        {
            if (MustAddToTopLevel())
            {
                AddAsUngrouped(item);
                return;
            }
            AddToGroupOfSelectedItem(item);
        }

        public void AddItems(IEnumerable<IDisplayItem> items)
        {
            PromptIfEmptyParentIsSelected();
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Insert(IDisplayItem item)
        {
            if (MustInsertAtTopLevel())
            {
                InsertAtLowestSelectedIndex(item);
                return;
            }    
            InsertInGroupOfSelectedItem(item);          
        }

        public void Cut()
        {
            _cutItems = CloneSelectedItems();
            var itemsToRemove = GetDistinctSelectedItems();
            Remove(itemsToRemove);
        }

        public void Paste()
        {
            AddItems(_cutItems);
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
            var selectedParents = GetHighestSelectedParents();
            foreach (var selectedParent in selectedParents)
            {
                UnGroup(selectedParent as IGroup);
            }
        }

        public void MoveSelectedItemsOutOfGroup(IEnumerable<IDisplayItem> items)
        {
            MoveOutOfGroup(items);
        }

        public void MoveTo(IGroup group)
        {
            if (InvalidSelectionForGrouping()) return;
            AddSelectedToGroup(group);
            Insert(group);
            Remove(GetDistinctSelectedItems());
            UpdateSelection(new List<IDisplayItem> { group });
        }

        public void MoveUp()
        {
            var items = GetOrderedSelectedItems();
            var childItems = GetSelectedChildren();
            var ungroupedItems = items.Except(childItems).ToList();

            _items.MoveUp(ungroupedItems);
            MoveChildItemsUp(childItems);
            UpdateSelection(items);
        }

        public void MoveDown()
        {
            var items = GetOrderedSelectedItems();
            var childItems = GetSelectedChildren();
            var ungroupedItems = items.Except(childItems).ToList();

            _items.MoveDown(ungroupedItems);
            MoveChildItemsDown(childItems);
            UpdateSelection(items);
        }

        public void UpdateSelection(IEnumerable<IDisplayItem> items)
        {
            SelectedItems.Clear();
            SelectedItems.AddRange(items);
        }

        public void AddAsUngrouped(IDisplayItem item)
        {
            item.SetParent(null);
            _items.Add(item);
        }

	    private void PromptIfEmptyParentIsSelected()
	    {
	        if (!SelectedIsANotAEmptyParent) return;
	        var view = new AddToParentPromptDialog();
	        var vm = new AddToParentPromptDialogViewModel(view);
	        view.DataContext = vm;
	        view.ShowDialog();
	        _addToEmptyGroup = vm.Result;
	    }

	    private bool SelectedIsANotAEmptyParent
	    {
	        get
	        {
	            if (SelectedItems.Count != 1) return false;
                return SelectedItems.All(_items.IsChildlessParent);
	        }
	    }

        private void MoveOutOfGroup(IEnumerable<IDisplayItem> items)
        {
            var itemsToMove = Clone(items);
            Remove(_items.GetMovableItems(items));
            foreach (var item in itemsToMove)
            {
                InsertAtParentLevel(item);
            }
        }

        private void InsertAtParentLevel(IDisplayItem item)
        {
            if (_items.IsGrandParentless(item))
            {
                _items.Insert(IndexOf(item), item);
                item.SetParent(null);
                return;
            }
            InsertInGroupAtParentIndex(item, GetParent(item.Parent));
        }

        private void MoveChildItemsDown(IEnumerable<IDisplayItem> childItems)
        {
            if (!childItems.Any()) return;
            var group = GetParent(childItems.First());
            group.MoveItemsDown(childItems);
        }

        private void MoveChildItemsUp(IEnumerable<IDisplayItem> childItems)
        {
            if (!childItems.Any()) return;
            var group = GetParent(childItems.First());
            group.MoveItemsUp(childItems);
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

        private void InsertInGroupOfSelectedItem(IDisplayItem item)
        {
            var group = GetParentOfFirstSelectedItem();
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

        private bool InvalidSelectionForGrouping()
        {
            return !BelongToSameGroup();
        }

        private bool MustAddToTopLevel()
        {
            if (NoSelectedItems()) return true;
            return SelectedItemsHaveTopLevelItemsThatAreNotEmptyGroups();
        }

        private bool NoSelectedItems()
        {
            return !SelectedItems.Any();
        }

        private bool SelectedItemsHaveTopLevelItemsThatAreNotEmptyGroups()
        {
            return _items.AreAnyItemsTopLevelItems(SelectedItems) && !_addToEmptyGroup;
        }

        private IGroup GetParentOfFirstSelectedItem()
        {
            return GetParent(SelectedItems.First());
        }

        private void AddSelectedToGroup(IGroup @group)
        {
            var items = CloneSelectedItems();
            group.Add(items);
        }

         private void AddToGroupOfSelectedItem(IDisplayItem item)
        {
            var group = GetGroupToAddTo();
            group.Add(item);
        }

        private IGroup GetGroupToAddTo()
        {
            var group = GetParentOfFirstSelectedItem();
            if (_addToEmptyGroup)
            {
                group = SelectedItems.First() as IGroup;
            }
            return group;
        }

        private bool MustInsertAtTopLevel()
        {
            if (NoSelectedItems()) return true;
            return _items.AreAnyItemsTopLevelItems(SelectedItems);
        }

        private int GetLowestSelectedIndex()
        {
            return _items.GetLowestSelectedIndex(SelectedItems);
        }

        private void InsertAtLowestSelectedIndex(IDisplayItem item)
        {
            var lowestSelectedIndex = GetLowestSelectedIndex();
            _items.Insert(lowestSelectedIndex, item);
        }

        private IEnumerable<IDisplayItem> GetDistinctSelectedItems()
        {
            return _items.GetDistinct(SelectedItems);
        }

        private List<IDisplayItem> CloneSelectedItems()
        {
            return Clone(SelectedItems);
        }

        private List<IDisplayItem> Clone(IEnumerable<IDisplayItem> items)
        {
            return _items.Clone(items).ToList();
        } 

        private IEnumerable<IDisplayItem> GetHighestSelectedParents()
        {
            return _items.GetHighestLevelParents(SelectedItems);
        }

        private IGroup GetParent(IDisplayItem item)
        {
            return _items.GetParent(item);
        }

        private IEnumerable<IDisplayItem> GetSelectedChildren()
        {
            return SelectedItems.Where(_items.IsAChild);
        }

        private List<IDisplayItem> GetOrderedSelectedItems()
        {
            return SelectedItems.OrderBy(IndexOf).ToList();
        }

        private int IndexOf(IDisplayItem item)
        {
            return _items.IndexOf(item);
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

        public bool BelongToSameGroup()
        {
            return _items.BelongToTheSameGroup(SelectedItems);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}