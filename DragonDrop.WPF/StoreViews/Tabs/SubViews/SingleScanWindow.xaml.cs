using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for SingleScanWindow.xaml
    /// </summary>
    public partial class SingleScanWindow : Window, IReceiveBarcodeAndRemoteCloseControl
    {
        private IReceiveBarcodeControl _callingView;
        private StoreView _storeView;

        public SingleScanWindow(IReceiveBarcodeControl callingView, StoreView storeView = null)
        {
            if (storeView != null) storeView.Lock();

            _callingView = callingView;
            _storeView = storeView;

            InitializeComponent();
            DataContext = new SingleScanViewModel(this as IReceiveBarcodeAndRemoteCloseControl);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            GetViewModel().ShutDownCommand.Execute();
            if (_storeView != null) _storeView.Unlock();
            base.OnClosing(e);
        }

        private SingleScanViewModel GetViewModel() => DataContext as SingleScanViewModel;

        private void PortBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GetViewModel().SetPortCommand.Execute();
        }
        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
            => ControlHelpers.HandlePortBoxInput(e);

        private void PortBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        public void SetBarcode(string code) => _callingView.SetBarcode(code);

        public void Stop() => Close();
    }
}
