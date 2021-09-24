using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
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
    /// Interaction logic for ItemAddWindow.xaml
    /// </summary>
    public partial class ItemAddWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public ItemAddWindow(IReloadableControl callingTab=null, StoreView storeView=null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;

            InitializeComponent();

            _callingTab = callingTab;

            var container = new UnityContainer();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterType<IProductDataService, ProductDataService>();
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);

            DataContext = container.Resolve<ItemAddViewModel>();

            OrdBox.Focus();
            OrdBox.Text = "Order ID";
        }

        private void IntBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void RefreshParent()
        {
            if (_callingTab != null) _callingTab.Reload();
        }

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_storeView != null) _storeView.Unlock();
            base.OnClosing(e);
        }
    }
}