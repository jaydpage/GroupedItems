using System.Windows;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace GroupedItemsTake2
{
    public class AddToParentPromptDialogViewModel
    {
        public bool Result { get; set; }
        public DelegateCommand<object> AddToEmptyCommand { get; private set; }
        public DelegateCommand<object> AddAtGroupLevelCommand { get; private set; }
        private readonly Window _window;
        public AddToParentPromptDialogViewModel(Window window)
        {
            _window = window;
            AddToEmptyCommand = new DelegateCommand<object>(obj => AddToEmptyGroup(), x => true);
            AddAtGroupLevelCommand = new DelegateCommand<object>(obj => AddAtGroupLevel(), x => true);      
        }

        private void AddAtGroupLevel()
        {
            Result = false;
            _window.Close();
        }

        private void AddToEmptyGroup()
        {
            Result = true;
            _window.Close();
        }
    }
}
