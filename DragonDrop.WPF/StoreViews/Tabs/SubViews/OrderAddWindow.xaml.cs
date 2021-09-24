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
    /// Interaction logic for OrderAddWindow.xaml
    /// </summary>
    public partial class OrderAddWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        IReloadableControl _callingTab;
        StoreView _storeView;

        public OrderAddWindow(IReloadableControl callingTab, StoreView storeView)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;
            _callingTab = callingTab;
            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterType<IPaymentMethodRepository, PaymentMethodRepository>();
            container.RegisterType<IShippingMethodRepository, ShippingMethodRepository>();
            container.RegisterType<IOrderStatusRepository, OrderStatusRepository>();
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);

            DataContext = container.Resolve<OrderAddViewModel>();

            CustBox.Focus();
            CustBox.Text = "Customer ID";
        }

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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_storeView != null) _storeView.Unlock();
            base.OnClosing(e);
        }
    }
}
