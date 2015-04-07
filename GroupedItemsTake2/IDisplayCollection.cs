using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GroupedItemsTake2
{
    public interface IDisplayCollection
    {
        void Add(IDisplayItem item);
        void AddItems(IEnumerable<IDisplayItem> items);
        void Insert(IDisplayItem item);
        void Cut();
        void Paste();
        void Duplicate();
        void Delete();
        void UnGroup();
        void MoveItemsOutOfGroup();
        void MoveTo(IGroup group);
        void MoveUp();
        void MoveDown();
        void UpdateSelection(IEnumerable<IDisplayItem> items);
        void AddAsUngrouped(IDisplayItem item);
        ObservableCollection<IDisplayItem> SelectedItems { get; set; }
        int Count { get; }
        void Group(string groupName);
        bool Contains(IDisplayItem item);
        IEnumerator<IDisplayItem> GetEnumerator();
        bool IsAParent(IDisplayItem item);
        bool IsAChild(IDisplayItem item);
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}