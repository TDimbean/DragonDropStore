using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ValueCellAddWindow.xaml
    /// </summary>
    public partial class ValueCellAddWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingWindow;

        public ValueCellAddWindow(IReloadableControl callingWindow, int repoType)
        {
            _callingWindow = callingWindow;

            InitializeComponent();

            DataContext = new ValueCellAddViewModel(this, repoType);
            entryBox.Focus();
        }


        public void RefreshParent() => _callingWindow.Reload();
        public void Stop() => Close();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void EntryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) submitBtn.PerformClick();
        }
    }
}
