using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GroupedItemsTake2.Annotations;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Logging;using GroupedItemsTake2.Interfaces;

using GroupedItemsTake2.Repository;
using log4net;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Win32;

namespace GroupedItemsTake2.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public DelegateCommand<object> AddCommand { get; private set; }
		public DelegateCommand<object> SaveCommand { get; private set; }
		public DelegateCommand<object> LoadCommand { get; private set; }
		public DelegateCommand<object> RemoveCommand { get; private set; }
		public DelegateCommand<object> DuplicateCommand { get; private set; }
		public DelegateCommand<object> EditCommand { get; private set; }
		public DelegateCommand<object> MoveUpCommand { get; private set; }
		public DelegateCommand<object> GroupCommand { get; private set; }
		public DelegateCommand<object> UnGroupCommand { get; private set; }
		public DelegateCommand<object> MoveOutOfGroupCommand { get; private set; }
		public DelegateCommand<object> MoveDownCommand { get; private set; }
		public DelegateCommand<object> DeleteCommand { get; private set; }
		public DelegateCommand<object> CopyCommand { get; private set; }
		public DelegateCommand<object> CutCommand { get; private set; }
		public DelegateCommand<object> PasteCommand { get; private set; }

        private readonly ILog _logger;
		private DisplayCollection _items;
		private IDisplayItem _selectedItem;
		private readonly GroupNameGenerator _groupNameGenerator;
		private readonly ItemNameGenerator _itemNameGenerator;
		private readonly RepositoryWriter _repositoryWriter;
		private readonly RepositoryReader _repositoryReader;

	    public MainViewModel()
		{
            _logger = new LogFactory().Create();
			_itemNameGenerator = new ItemNameGenerator();
			_groupNameGenerator = new GroupNameGenerator();
			_repositoryWriter = new RepositoryWriter();
            _repositoryReader = new RepositoryReader();
			_items = new DisplayCollection(); ;
			AddCommand = new DelegateCommand<object>(obj => Add(), x => true);
			SaveCommand = new DelegateCommand<object>(obj => Save(), x => true);
			LoadCommand = new DelegateCommand<object>(obj => Load(), x => true);
            DuplicateCommand = new DelegateCommand<object>(obj => DuplicateItem(), x => BelongToSameGroup);
            MoveUpCommand = new DelegateCommand<object>(obj => MoveUp(), x => BelongToSameGroup);
            MoveDownCommand = new DelegateCommand<object>(obj => MoveDown(), x => BelongToSameGroup);
            GroupCommand = new DelegateCommand<object>(obj => GroupItems(), x => BelongToSameGroup);
            UnGroupCommand = new DelegateCommand<object>(obj => UngroupItems(), x => OnlyParentsSelected);
            MoveOutOfGroupCommand = new DelegateCommand<object>(obj => MoveOutOfGroup(), x => OnlyChildrenSelected);
            DeleteCommand = new DelegateCommand<object>(obj => Delete(), x => IsItemSelected);
            CopyCommand = new DelegateCommand<object>(obj => Copy(), x => BelongToSameGroup);
            CutCommand = new DelegateCommand<object>(obj => Cut(), x => BelongToSameGroup);
            PasteCommand = new DelegateCommand<object>(obj => Paste(), x => CanPaste);

            SelectedItems.CollectionChanged += SelectedItemsOnCollectionChanged;
		}

	    private void Load()
	    {
            const string fileName = "New";
	        var result = PromptOpenFileDialog(fileName);
	        if (string.IsNullOrEmpty(result)) return;
	        Log("Load File: " + result);
	        var items = _repositoryReader.Read(result);
            Items.Clear();
            Items.AddItems(items);
	    }

	    private void Save()
	    {
            const string fileName = "New";
	        var result = PromptSaveFileDialog(fileName); 
            if(string.IsNullOrEmpty(result)) return;
	        Log("Save File: " + result);
	        _repositoryWriter.Write(_items, result);
	    }

	    private void Paste()
		{
            Log("Paste");
			Items.Paste();
		}

		private void Cut()
		{
            Log("Cut");
			Items.Cut();
		}
        
        private void Copy()
		{
            Log("Copy");
			Items.Copy();
		}

		private void DuplicateItem()
		{
            Log("Duplicate");
			Items.Duplicate();
		}

		private void MoveDown()
		{
            Log("Move Down");
			Items.MoveDown();
		}

		private void MoveUp()
		{
            Log("Move Up");
			Items.MoveUp();
		}

		private void Delete()
		{
            Log("Delete");
			Items.Delete();
		}

		private void GroupItems()
		{
            Log("Group");
			Items.Group(_groupNameGenerator.GenerateName());
		}

		private void UngroupItems()
		{
            Log("Ungroup");
			Items.UnGroup();
		}

		private void MoveOutOfGroup()
		{
            Log("Move out of group");
            Items.MoveItemsOutOfGroup();
		}

		private void Add()
		{
            Log("Add");
			var newItem = Item.Create(_itemNameGenerator.GenerateItemName());
		    var newItems = new List<IDisplayItem> {newItem};
			Items.AddItems(newItems);
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
            CopyCommand.RaiseCanExecuteChanged();
        }

        private string PromptSaveFileDialog(string fileName)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Items" + " (*.gi)|*.gi|" + "All Files" + "|*.*",
                DefaultExt = "gi",
                FileName = Path.GetFileName(fileName),
            };

            if ((bool)!dlg.ShowDialog()) return null;
            return dlg.FileName;
        }

        private string PromptOpenFileDialog(string fileName)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Items" + " (*.gi)|*.gi|" + "All Files" + "|*.*",
                DefaultExt = "gi",
                FileName = Path.GetFileName(fileName),
                Multiselect = false,
                CheckFileExists = true
            };

            if ((bool)!dlg.ShowDialog()) return null;
            return dlg.FileName;
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
        
        public bool CanPaste
        {
            get
            {
                if (IsItemSelected) return true;
                return !Items.Any();
            }
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
            get { return Items.OnlyChildrenSelected(); }	      
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
		    get
		    {
		        return Items.SelectedItems;
		    }
		    set
			{
				Items.SelectedItems = value;
				OnPropertyChanged("SelectedItems");
			}
		}

	    private void Log(string action)
	    {
	        var selectedText = Items.SelectedItems.Aggregate("SelectedItems: ", (current, item) => current + (item.Name + ", "));
	        _logger.Info(selectedText);
            _logger.Info(action);
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
