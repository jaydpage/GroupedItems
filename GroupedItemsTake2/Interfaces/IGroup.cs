using System.Collections.Generic;
using GroupedItemsTake2.Domain;

namespace GroupedItemsTake2.Interfaces
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