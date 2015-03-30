using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public DelegateCommand<object> UnGroupCommand { get; private set; }
        public DelegateCommand<object> MoveOutOfGroupCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteCommand { get; private set; }
        public DelegateCommand<object> CutCommand { get; private set; }
        public DelegateCommand<object> PasteCommand { get; private set; }

        private DisplayCollection _items;
        private IDislpayItem _selectedItem;
        private int itemCount = 1;
        private int groupCount = 1;

        public MainViewModel()
        {
            _items = new DisplayCollection();;
            AddCommand = new DelegateCommand<object>(obj => AddItem(), x => true);
            DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => true);
            MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => IsItemSelected);
            GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => IsItemSelected);
            UnGroupCommand = new DelegateCommand<object>(obj => UngroupItems(), x => IsItemSelected);
            MoveOutOfGroupCommand = new DelegateCommand<object>(obj => MoveOutOfGroup(), x => IsItemSelected);
            MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => IsItemSelected);
            DeleteCommand = new DelegateCommand<object>(obj => Delete(), x => true);
            CutCommand = new DelegateCommand<object>(obj => Cut(), x => true);
            PasteCommand = new DelegateCommand<object>(obj => Paste(), x => true);
        }

        private void Paste()
        {
            Items.PasteItems();
        }

        private void Cut()
        {
            Items.CutSelectedItems();
        }

        private void DuplicateItem()
        {
            if (!IsItemSelected) return;
            Items.Duplicate();
        }

        private void MoveDown()
        {
            Items.MoveDown();
        }

        private void MoveUp()
        {
            Items.MoveUp();
        }
        
        private void Delete()
        {
            Items.Delete();
        }

        private void GroupItems()
        {
            var group = new Group("Group " + groupCount, null);
            groupCount++;
            Items.GroupSelectedItems(group);
        }
        
        private void UngroupItems()
        {
            Items.UnGroupSelectedItems();
        }
        
        private void MoveOutOfGroup()
        {
            Items.MoveItemsOutOfGroup(Items.SelectedItems);
        }


        private void AddItem()
        {
            Items.AddItem(new Item("Item" + itemCount, null));
            itemCount++;
        }

        public bool IsItemSelected
        {
            get { return SelectedItems.Any(); }
        }
        
        public bool AnySelectedItemsNotAtTopLevel
        {
            get { return SelectedItems.Any(x => !Items.IsItemAtTheTopLevel(x)); } 
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
            get { return Items.SelectedItems; }
            set
            {
                Items.SelectedItems = value;
                OnPropertyChanged("SelectedItems");
                OnPropertyChanged("IsItemSelected");
                OnPropertyChanged("AnySelectedItemsNotAtTopLevel");
            }
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
