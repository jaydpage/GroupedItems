using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tests
{
    public class DisplayCollection : ObservableCollection<IDislpayItem>
    {
        public void AddItem(Item item, List<IDislpayItem> selectedItems)
        {
            Add(item);
        }
    }
}