using System.Collections.Generic;

namespace GroupedItemsTake2
{
    public interface IGroupingLogic
    {
        bool GetTopLevelItems(IEnumerable<IDisplayItem> selected);
        bool AreOfTheSameGroup(IEnumerable<IDisplayItem> selected);
        IGroup GetParent(IDisplayItem selected);
        IEnumerable<IDisplayItem> Clone(IEnumerable<IDisplayItem> selected);
        IEnumerable<IDisplayItem> GetDistinct(IEnumerable<IDisplayItem> selected);
        bool IsAParent(IDisplayItem item);
        bool IsGrandParentless(IDisplayItem item);
        bool IsAChild(IDisplayItem item);
        bool IsTopLevelItem(IDisplayItem item);
        void MoveDown(IEnumerable<IDisplayItem> selected);
        void MoveUp(IEnumerable<IDisplayItem> selected);
        int GetLowestSelectedIndex(IEnumerable<IDisplayItem> selected);
        IEnumerable<IDisplayItem> GetTopLevelParents(IEnumerable<IDisplayItem> selected);
    }
}