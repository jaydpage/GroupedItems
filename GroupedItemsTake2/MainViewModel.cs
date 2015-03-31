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
		private readonly GroupNameGenerator _groupNameGenerator;
		private readonly ItemNameGenerator _itemNameGenerator;

		public MainViewModel()
		{
			_itemNameGenerator = new ItemNameGenerator();
			_groupNameGenerator = new GroupNameGenerator();
			_items = new DisplayCollection(); ;
			AddCommand = new DelegateCommand<object>(obj => AddItem(), x => true);
			DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => true);
			MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => Items.IsItemSelected);
			GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => Items.IsItemSelected);
			UnGroupCommand = new DelegateCommand<object>(obj => UngroupItems(), x => Items.IsItemSelected);
			MoveOutOfGroupCommand = new DelegateCommand<object>(obj => MoveOutOfGroup(), x => Items.IsItemSelected);
			MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => _items.IsItemSelected);
			DeleteCommand = new DelegateCommand<object>(obj => Delete(), x => true);
			CutCommand = new DelegateCommand<object>(obj => Cut(), x => true);
			PasteCommand = new DelegateCommand<object>(obj => Paste(), x => true);
		}

		private void Paste()
		{
			Items.Paste();
		}

		private void Cut()
		{
			Items.CutSelected();
		}

		private void DuplicateItem()
		{
			Items.DuplicateSelected();
		}

		private void MoveDown()
		{
			Items.MoveSelectedDown();
		}

		private void MoveUp()
		{
			Items.MoveSelectedUp();
		}

		private void Delete()
		{
			Items.DeleteSelected();
		}

		private void GroupItems()
		{
			Items.GroupSelected(_groupNameGenerator.GenerateName());
		}

		private void UngroupItems()
		{
			Items.UnGroupSelected();
		}

		private void MoveOutOfGroup()
		{
			Items.MoveSelectedItemsOutOfGroup();
		}

		private void AddItem()
		{
			var newItem = Item.Create(_itemNameGenerator.GenerateItemName());
			Items.AddItem(newItem);
		}

		public DisplayCollection Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
			}
		}

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
