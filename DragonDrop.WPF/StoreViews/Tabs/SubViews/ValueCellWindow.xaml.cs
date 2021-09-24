using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>l
    /// Interaction logic for ValueCellEditWindow.xaml
    /// </summary>
    public partial class ValueCellEditWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingWindow;

        public ValueCellEditWindow(IReloadableControl callingWindow, DefaultValueDto ogItem, int repoType)
        {
            _callingWindow = callingWindow;

            InitializeComponent();

            DataContext = new ValueCellEditViewModel(this, repoType, ogItem);

            entryBox.Focus();
        }


        public void RefreshParent() => _callingWindow.Reload();
        public void Stop() => Close();

        private void Close_Click(object sender, RoutedEventArgs e)=>Close();

        private void EntryBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) submitBtn.PerformClick();
        }
    }
}
