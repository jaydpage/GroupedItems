using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GroupedItemsTake2.Annotations;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Win32;
using NUnit.Framework;

namespace GroupedItemsTake2
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public DelegateCommand<object> AddCommand { get; private set; }
		public DelegateCommand<object> SaveCommand { get; private set; }
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
		private IDisplayItem _selectedItem;
		private readonly GroupNameGenerator _groupNameGenerator;
		private readonly ItemNameGenerator _itemNameGenerator;
		private readonly RepositoryWriter _repositoryWriter;

	    public MainViewModel()
		{
			_itemNameGenerator = new ItemNameGenerator();
			_groupNameGenerator = new GroupNameGenerator();
			_repositoryWriter = new RepositoryWriter();
			_items = new DisplayCollection(); ;
			AddCommand = new DelegateCommand<object>(obj => AddItem(), x => true);
			SaveCommand = new DelegateCommand<object>(obj => Save(), x => true);
            DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => BelongToSameGroup);
            MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => BelongToSameGroup);
            MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => BelongToSameGroup);
            GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => BelongToSameGroup);
            UnGroupCommand = new DelegateCommand<object>(obj => UngroupItems(), x => OnlyParentsSelected);
            MoveOutOfGroupCommand = new DelegateCommand<object>(obj => MoveOutOfGroup(), x => OnlyChildrenSelected);
            DeleteCommand = new DelegateCommand<object>(obj => Delete(), x => IsItemSelected);
            CutCommand = new DelegateCommand<object>(obj => Cut(), x => IsItemSelected);
            PasteCommand = new DelegateCommand<object>(obj => Paste(), x => IsItemSelected);

            SelectedItems.CollectionChanged += SelectedItemsOnCollectionChanged;
		}

	    private void Save()
	    {
            const string fileName = "New";

            var dlg = new SaveFileDialog
            {
                Filter = "Items" + " (*.gi)|*.gi|" + "All Files" + "|*.*",
                DefaultExt = "gi",
                FileName = Path.GetFileName(fileName),
            };

            if((bool)!dlg.ShowDialog()) return;
	        _repositoryWriter.Write(_items, dlg.FileName);
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
            Items.MoveItemsOutOfGroup();
		}

		private void AddItem()
		{
			var newItem = Item.Create(_itemNameGenerator.GenerateItemName());
		    var newItems = new List<IDisplayItem> {newItem};
			Items.AddItems(newItems);
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
        
        public bool BelongToSameGroup
	    {
            get
            {
                if (!SelectedItems.Any()) return false;
                return Items.BelongToSameGroup();
            }	      
	    }

	    public IDisplayItem SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged("SelectedItem");
			}
		}

		public ObservableCollection<IDisplayItem> SelectedItems
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
