using System.Collections.Generic;

namespace GroupedItemsTake2
{
    public interface IGroupingLogic
    {
        bool GetTopLevelItems(IEnumerable<IDislpayItem> selected);
        bool AreOfTheSameGroup(IEnumerable<IDislpayItem> selected);
        IGroup GetParent(IDislpayItem selected);
        IEnumerable<IDislpayItem> Clone(IEnumerable<IDislpayItem> selected);
        IEnumerable<IDislpayItem> GetDistinct(IEnumerable<IDislpayItem> selected);
        bool IsAParent(IDislpayItem item);
        bool IsGrandParentless(IDislpayItem item);
        bool IsAChild(IDislpayItem item);
        bool IsTopLevelItem(IDislpayItem item);
        void MoveDown(IEnumerable<IDislpayItem> selected);
        void MoveUp(IEnumerable<IDislpayItem> selected);
        int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selected);
        IEnumerable<IDislpayItem> GetTopLevelParents(IEnumerable<IDislpayItem> selected);
    }
}