using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window, ISelfAndRelayReloadAndRemoteCloseControl
    {
        IReloadableControl _callingTab;
        private UnityContainer _container;
        private StoreView _storeView;

        public OrderEditWindow(int targetId, IReloadableControl callingTab, StoreView storeView)
        {
            if(storeView!=null)storeView.Lock();
            _storeView = storeView;

            _callingTab = callingTab;
            InitializeComponent();

            _container = new UnityContainer();
            _container.RegisterType<IOrderRepository, OrderRepository>();
            _container.RegisterType<IOrderDataService, OrderDataService>();
            _container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            _container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            _container.RegisterType<IProductRepository, ProductRepository>();
            _container.RegisterType<IProductDataService, ProductDataService>();
            _container.RegisterType<IPaymentMethodRepository, PaymentMethodRepository>();
            _container.RegisterType<IShippingMethodRepository, ShippingMethodRepository>();
            _container.RegisterType<IOrderStatusRepository, OrderStatusRepository>();
            _container.RegisterInstance(targetId);
            _container.RegisterInstance(this as ISelfAndRelayReloadAndRemoteCloseControl);

            LoadData();

            CustBox.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(_storeView!=null)_storeView.Unlock();
            base.OnClosing(e);
        }

        private void LoadData()=>DataContext = _container.Resolve<OrderEditViewModel>();

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                => ControlHelpers.PopComboBoxPlaceholder(e.Source as ComboBox);

        private void CustBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");
        
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void Combo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as ComboBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void RefreshParent() => _callingTab.Reload();

        public void Stop() => Close();

        public void Reload() => LoadData();

        private void ShipOrder_Click(object sender, RoutedEventArgs e)
        {
            _container.Resolve<ShipSingleOrderWindow>().Show();
        }

        private void ProcOrder_Click(object sender, RoutedEventArgs e)
        {
            _container.Resolve<ProcessOrderWindow>().Show();
        }

    }
}
