using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
    }
}
