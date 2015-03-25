using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GroupedItemsTake2
{
    public interface IGroup : IDislpayItem
    {
        ObservableItemsCollection Items { get; }
        void Add(IDislpayItem item);
        void Insert(IDislpayItem item, IEnumerable<IDislpayItem> selectedItems);
        void MoveItemsUp(IEnumerable<IDislpayItem> items);
        void MoveItemsDown(IEnumerable<IDislpayItem> items);
        void Remove(IDislpayItem item);
        int Count();
        bool Contains(IDislpayItem item);
    }
}