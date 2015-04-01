using System.Collections.Generic;

namespace GroupedItemsTake2
{
    public interface IGroupingLogic
    {
        bool AreAnySelectedItemsAtTheTopLevel(IEnumerable<IDislpayItem> selected);
        bool AreSelectedItemsOfTheSameGroup(IEnumerable<IDislpayItem> selectedItems);
        IGroup GetParent(IDislpayItem selectedItem);
        IEnumerable<IDislpayItem> CloneSelected(IEnumerable<IDislpayItem> selectedItems);
        IEnumerable<IDislpayItem> GetDistinctItems(IEnumerable<IDislpayItem> selectedItems);
        bool IsItemAParent(IDislpayItem item);
        bool IsItemGrandParentless(IDislpayItem item);
        bool IsItemAChild(IDislpayItem item);
        bool IsTopLevelItem(IDislpayItem item);
        void MoveDown(IEnumerable<IDislpayItem> selectedItems);
        void MoveUp(IEnumerable<IDislpayItem> selectedItems);
        int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems);
        IEnumerable<IDislpayItem> GetTopLevelSelectedParents(IEnumerable<IDislpayItem> items);
    }
}