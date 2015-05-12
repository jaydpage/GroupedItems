using System.Windows;
using GroupedItemsTake2.ViewModels;

namespace GroupedItemsTake2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DevExpress.Xpf.Core.ThemeManager.ApplicationThemeName = "Office2007Blue";
        }
    }
}
