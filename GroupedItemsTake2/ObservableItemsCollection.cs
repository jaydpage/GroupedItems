using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GroupedItemsTake2
{
    public class ObservableItemsCollection : ObservableCollection<IDislpayItem>
    {
        public void MoveItemsDown(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderByDescending(IndexOf).ToList();

            while (items.Any())
            {
                var current = items.First();
                items.Remove(current);
                var index = IndexOf(current);
                if (index >= Count - 1) return;
                RemoveAt(index);
                Insert(index + 1, current);
            }
        }

        public void MoveItemsUp(IEnumerable<IDislpayItem> selectedItems)
        {
            var items = selectedItems.OrderBy(IndexOf).ToList();
            while (items.Any())
            {
                var current = items.First();
                items.Remove(current);
                var index = IndexOf(current);
                if (index <= 0) return;
                RemoveAt(index);
                Insert(index - 1, current);
            }
        }

        public int GetLowestSelectedIndex(IEnumerable<IDislpayItem> selectedItems)
        {
            var lowestIndex = int.MaxValue;
            foreach (var item in selectedItems)
            {
                var index = IndexOf(item);
                if (!Contains(item)) continue;
                if (index < lowestIndex) lowestIndex = index;
            }
            return lowestIndex < 0 ? 0 : lowestIndex;
        }

        
    }
}
