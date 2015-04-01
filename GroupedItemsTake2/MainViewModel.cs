using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using GroupedItemsTake2.Annotations;
using Microsoft.Practices.Composite.Presentation.Commands;
using NUnit.Framework;

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
            DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => IsItemSelected);
            MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => IsItemSelected);
            GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => IsItemSelected);
            UnGroupCommand = new DelegateCommand<object>(obj => UngroupItems(), x => OnlyParentsSelected);
            MoveOutOfGroupCommand = new DelegateCommand<object>(obj => MoveOutOfGroup(), x => OnlyChildrenSelected);
            MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => IsItemSelected);
            DeleteCommand = new DelegateCommand<object>(obj => Delete(), x => IsItemSelected);
            CutCommand = new DelegateCommand<object>(obj => Cut(), x => IsItemSelected);
            PasteCommand = new DelegateCommand<object>(obj => Paste(), x => IsItemSelected);

            SelectedItems.CollectionChanged += SelectedItemsOnCollectionChanged;
		}

        private void SelectedItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UnGroupCommand.RaiseCanExecuteChanged();
            DuplicateCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            GroupCommand.RaiseCanExecuteChanged();
            MoveOutOfGroupCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            CutCommand.RaiseCanExecuteChanged();
            PasteCommand.RaiseCanExecuteChanged();
        }

	    private void Paste()
		{
			Items.Paste();
		}

		private void Cut()
		{
			Items.Cut();
		}

		private void DuplicateItem()
		{
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
			Items.Group(_groupNameGenerator.GenerateName());
		}

		private void UngroupItems()
		{
			Items.UnGroup();
		}

		private void MoveOutOfGroup()
		{
			Items.MoveOutOfGroup();
		}

		private void AddItem()
		{
			var newItem = Item.Create(_itemNameGenerator.GenerateItemName());
		    var newItems = new List<IDislpayItem> {newItem};
			Items.AddPrompt(newItems);
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

        public bool IsItemSelected
        {
            get { return SelectedItems.Any(); }
        }

	    public bool OnlyParentsSelected
	    {
	        get
	        {
	            if (!SelectedItems.Any()) return false;
	            return SelectedItems.All(x => Items.IsAParent(x));
	        }	      
	    }
        
        public bool OnlyChildrenSelected
	    {
            get
            {
                if (!SelectedItems.Any()) return false;
                return SelectedItems.All(x => Items.IsAChild(x));
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
