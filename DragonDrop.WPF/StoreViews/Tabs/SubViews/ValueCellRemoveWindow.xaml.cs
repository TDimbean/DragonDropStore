using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ValueCellRemoveWindow.xaml
    /// </summary>
    public partial class ValueCellRemoveWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingWindow;

        public ValueCellRemoveWindow(IReloadableControl callingWindow, DefaultValueDto ogItem, int repoType)
        {
            _callingWindow = callingWindow;

            InitializeComponent();

            DataContext = new ValueCellRemoveViewModel(this, ogItem, repoType);
        }

        public void RefreshParent() => _callingWindow.Reload();
        public void Stop() => Close();

        private void No_Click(object sender, RoutedEventArgs e) => Close();
    }
}
