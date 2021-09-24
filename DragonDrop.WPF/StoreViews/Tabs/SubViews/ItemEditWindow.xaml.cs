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
    /// Interaction logic for ItemEditWindow.xaml
    /// </summary>
    public partial class ItemEditWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public ItemEditWindow(int targetOrdId, int targetProdId, IReloadableControl callingTab=null, StoreView storeView = null)
        {
            if(storeView!=null)storeView.Lock();
            _storeView = storeView;
            _callingTab = callingTab;
            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IProductDataService, ProductDataService>();
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);

            var service = container.Resolve<OrderItemDataService>();
            
            container.RegisterInstance(service.Get(targetOrdId, targetProdId));

            DataContext = container.Resolve<ItemEditViewModel>();

            OrdBox.Focus();
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
