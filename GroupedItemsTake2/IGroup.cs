using System.Collections.ObjectModel;

namespace GroupedItemsTake2
{
    public interface IGroup : IDislpayItem
    {
        ObservableCollection<IDislpayItem> Items { get;}
        void Add(IDislpayItem item);
        void Remove(IDislpayItem item);
        int Count();
        bool Contains(IDislpayItem item);
    }
}