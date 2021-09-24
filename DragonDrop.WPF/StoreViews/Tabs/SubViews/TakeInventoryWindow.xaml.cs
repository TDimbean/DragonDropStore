using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for TakeInventoryWindow.xaml
    /// </summary>
    public partial class TakeInventoryWindow : Window, IRemoteCloseControl
    {
        private StoreView _storeView;

        public TakeInventoryWindow(StoreView storeView = null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;

            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IProductRepository, ProductRepository>();
            DataContext = new TakeInventoryViewModel(container.Resolve<ProductDataService>(), this as IRemoteCloseControl);
        }

        public void RefreshGrid()=>incomingDG.Items.Refresh();

        public void RefreshCheckBtn()
        {
            var vm = GetViewModel();
            checkBtn.IsEnabled = vm.CanCheck;
            checkBtnTxtBlk.Text = vm.BtnText;
            accArrow1.Visibility = vm.AcceptMode ? Visibility.Visible : Visibility.Collapsed;
            }

        protected override void OnClosing(CancelEventArgs e)
        {
            GetViewModel().ShutDownCommand.Execute();
            if (_storeView != null)
            {
                _storeView.RefreshAdminProducts();
                _storeView.Unlock();
            }
            base.OnClosing(e);
        }

        private TakeInventoryViewModel GetViewModel() => DataContext as TakeInventoryViewModel;

        #region Handle UI Input

        private void QtyBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GetViewModel().CheckCommand.Execute();
        }

        private void PortBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GetViewModel().SetPortCommand.Execute();
        }
        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
            => ControlHelpers.HandlePortBoxInput(e);

        private void PortBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        #endregion

        public void Stop() => Close();

        private void IncomingDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var prod = incomingDG.SelectedItem as ProcessingProduct;
            if (prod == null) return;
            GetViewModel().SelectCommand.Execute(prod.ProductId);
            accArrow1.Visibility = Visibility.Collapsed;
        }

        private void IncomingDG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IncomingDG_SelectionChanged(null, null);
            accArrow1.Visibility = Visibility.Collapsed;
        }
    }
}
