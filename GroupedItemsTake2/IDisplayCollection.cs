using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GroupedItemsTake2
{
	public interface IDisplayCollection
	{
		void AddItem(IDislpayItem item);
		void InsertItem(IDislpayItem item);
		void MoveTo(IGroup group);
		void CutSelected();
		void Paste();
		void UnGroupSelected();
		void MoveSelectedItemsOutOfGroup();
		void MoveItemsOutOfGroup(ObservableCollection<IDislpayItem> items);
		void DeleteSelected();
		void MoveSelectedUp();
		void MoveSelectedDown();
		void DuplicateSelected();
		void UpdateSelectedItems(IEnumerable<IDislpayItem> items);
		ObservableCollection<IDislpayItem> SelectedItems { get; set; }
		int Count { get; }
		void MoveItemsDown(IEnumerable<IDislpayItem> selectedItems);
		void MoveItemsUp(IEnumerable<IDislpayItem> selectedItems);
		int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems);
		bool AreSelectedItemsOfTheSameGroup(IEnumerable<IDislpayItem> selectedItems);
		IGroup GetItemGroup(IDislpayItem selectedItem);
		IEnumerable<IDislpayItem> GetDistinctItems(IEnumerable<IDislpayItem> selectedItems);
		IEnumerable<IDislpayItem> GetTopLevelSelectedParents(IEnumerable<IDislpayItem> items);
		IEnumerable<IDislpayItem> GetItemsToRemove(IEnumerable<IDislpayItem> selectedItems);
		IEnumerable<IDislpayItem> GetGroupedItemsToRemove(IEnumerable<IDislpayItem> selectedItems);
		bool AreAnySelectedItemsAtTheTopLevel(IEnumerable<IDislpayItem> selected);
		bool IsItemAtTheTopLevel(IDislpayItem item);
		bool IsItemAParent(IDislpayItem item);
		bool ItemHasNoGrandParent(IDislpayItem item);
		bool IsItemAChild(IDislpayItem item);
		void Move(int oldIndex, int newIndex);
		event NotifyCollectionChangedEventHandler CollectionChanged;
		void Add(IDislpayItem item);
		void Clear();
		void CopyTo(IDislpayItem[] array, int index);
		bool Contains(IDislpayItem item);
		IEnumerator<IDislpayItem> GetEnumerator();
		int IndexOf(IDislpayItem item);
		void Insert(int index, IDislpayItem item);
		bool Remove(IDislpayItem item);
		void RemoveAt(int index);
		IDislpayItem this[int index] { get; set; }
	}
}