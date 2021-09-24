using DragonDrop.BLL.Interfaces.DataServices;
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
    /// Interaction logic for ProcessOrderWindow.xaml
    /// </summary>
    public partial class ProcessOrderWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingView;

        public ProcessOrderWindow
            (int ordId, IOrderDataService ordServ, IProductDataService prodServ, IOrderItemDataService itemServ,
            IReloadableControl callingView)
        {
            _callingView = callingView;

            DataContext = new ProcessOrderViewModel(ordId, ordServ, prodServ, itemServ, this);

            InitializeComponent();
        }

        private ProcessOrderViewModel GetViewModel => DataContext as ProcessOrderViewModel;

        public void RefreshGrids()
        {
            unpDG.Items.Refresh();
            procDG.Items.Refresh();
        }

        public void RefreshCheckBtn()
        {
            var vm = GetViewModel;
            checkBtn.IsEnabled = vm.CanCheck;
            checkBtnTxtBlk.Text =vm.CurScannedTitle;
        }

        private void QtyBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GetViewModel.CheckCommand.Execute();
        }

        private void PortBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GetViewModel.SetPortCommand.Execute();
        }
        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
            => ControlHelpers.HandlePortBoxInput(e);

        private void PortBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        public void RefreshParent() => _callingView.Reload();

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            GetViewModel.ShutDownCommand.Execute();
            base.OnClosing(e);
        }
    }
}
