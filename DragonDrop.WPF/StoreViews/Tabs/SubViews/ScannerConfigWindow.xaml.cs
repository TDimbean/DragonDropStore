using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ScannerConfigWindow.xaml
    /// </summary>
    public partial class ScannerConfigWindow : Window
    {
        private StoreView _storeView;

        public ScannerConfigWindow(StoreView storeView = null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;

            InitializeComponent();
            DataContext = new ScannerConfigViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            (DataContext as ScannerConfigViewModel).ShutDownCommand.Execute();
            if (_storeView != null) _storeView.Unlock();
            base.OnClosing(e);
        }

        private void Entry_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
            => ControlHelpers.HandlePortBoxInput(e);

    }
}
