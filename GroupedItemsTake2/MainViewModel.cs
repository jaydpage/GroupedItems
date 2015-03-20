using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GroupedItemsTake2.Annotations;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace GroupedItemsTake2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public DelegateCommand<object> AddCommand { get; private set; }
        public DelegateCommand<object> RemoveCommand { get; private set; }
        public DelegateCommand<object> DuplicateCommand { get; private set; }
        public DelegateCommand<object> EditCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> GroupCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }

        private DisplayCollection _items;
        private ObservableCollection<IDislpayItem> _selectedItems;
        private IDislpayItem _selectedItem;
        private int itemCount = 1;
        private int groupCount = 1;

        public MainViewModel()
        {
            _items = new DisplayCollection();;
            AddCommand = new DelegateCommand<object>(obj => AddItem(), x => true);
            DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => true);
            MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => IsItemSelected());
            GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => IsItemSelected());
            MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => IsItemSelected());

        }

        private void DuplicateItem()
        {
            if (!IsItemSelected()) return;
            Items.Duplicate(SelectedItems);
        }

        private void MoveDown()
        {
            Items.MoveDown(SelectedItems);
        }

        private void MoveUp()
        {
            Items.MoveUp(SelectedItems);
        }

        private void GroupItems()
        {
            var group = new Group("Group " + groupCount, null);
            groupCount++;
            Items.GroupItems(group, SelectedItems);
        }


        private void AddItem()
        {
            Items.AddItem(new Item("Item" + itemCount, null), SelectedItems);
            itemCount++;
        }

        private bool IsItemSelected()
        {
            return SelectedItems.Any();
        }

        public DisplayCollection Items { get { return _items; } set { _items = value; } }

        public IDislpayItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<IDislpayItem> SelectedItems
        {
            get
            {
                return _selectedItems ??
                     (_selectedItems = new ObservableCollection<IDislpayItem>());
            }
            set { _selectedItems = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
           
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
