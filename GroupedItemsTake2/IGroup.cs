using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GroupedItemsTake2
{
    public interface IGroup : IDisplayItem
    {
        ObservableItemsCollection Items { get; }
        void Add(IDisplayItem item);
        void Add(IEnumerable<IDisplayItem> items);
        void Insert(IDisplayItem item, IEnumerable<IDisplayItem> selectedItems);
        void InsertAtParentIndex(IDisplayItem item);
        void MoveItemsUp(IEnumerable<IDisplayItem> items);
        void MoveItemsDown(IEnumerable<IDisplayItem> items);
        void Remove(IDisplayItem item);
        int Count();
        bool Contains(IDisplayItem item);
    }
}