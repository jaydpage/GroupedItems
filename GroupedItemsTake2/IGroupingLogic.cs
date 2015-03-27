using System.Collections.Generic;

namespace GroupedItemsTake2
{
    public interface IGroupingLogic
    {
        bool AreAnySelectedItemsAtTheTopLevel(IEnumerable<IDislpayItem> selected);
        bool AreSelectedItemsOfTheSameGroup(IEnumerable<IDislpayItem> selectedItems);
        IGroup GetItemGroup(IDislpayItem selectedItem);
        IEnumerable<IDislpayItem> CreateItemsToGroup(IEnumerable<IDislpayItem> selectedItems);
        IEnumerable<IDislpayItem> GetItemsToRemove(IEnumerable<IDislpayItem> selectedItems);
        bool IsItemAParent(IDislpayItem item);
        bool DoesItemHaveAGrandParent(IDislpayItem item);
        bool IsItemAChild(IDislpayItem item);
        bool IsItemUngrouped(IDislpayItem item);
        void MoveItemsDown(IEnumerable<IDislpayItem> selectedItems);
        void MoveItemsUp(IEnumerable<IDislpayItem> selectedItems);
        int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems);
    }
}