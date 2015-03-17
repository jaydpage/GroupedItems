namespace GroupedItemsTake2
{
    public interface IGroup : IDislpayItem
    {
        void Add(IDislpayItem item);
        void Remove(IDislpayItem item);
    }
}