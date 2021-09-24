using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using DragonDrop.WPF.StoreViews.ViewModels;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserCartTab.xaml
    /// </summary>
    public partial class UserCartTab : UserControl, ICartableTab
    {
        private StoreViewModel _vm;

        public UserCartTab(StoreView storeView, StoreViewModel vm)
        {
            _vm = vm;

            InitializeComponent();

            DataContext = new UserCartTabViewModel();
        }

        public StoreViewModel ParentViewModel
        {
            get => _vm;
            set => _vm = value;
        }

        private UserOrderProd GetSelectedItem() => custDG.SelectedItem as UserOrderProd;

        private void Decrease_Click(object sender, RoutedEventArgs e)
            => _vm.DecreaseInCartCommand.Execute(GetSelectedItem().ProductId);

        private void Bump_Click(object sender, RoutedEventArgs e)
            => _vm.BumpInCartCommand.Execute(GetSelectedItem().ProductId);

        private void Remove_Click(object sender, RoutedEventArgs e)
            => _vm.RemoveFromCartCommand.Execute(GetSelectedItem().ProductId);

        private void Details_Click(object sender, RoutedEventArgs e)
            => new UserProdDetailWindow(GetSelectedItem().ProductId).Show();

        private void QtyBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void QtyBox_LostFocus(object sender, RoutedEventArgs e)
            => UpdQty(int.Parse((e.Source as TextBox).Text));

        private void QtyBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) UpdQty(int.Parse((e.Source as TextBox).Text));
        }

        private void UpdQty(int newQty)
            => _vm.ChangeQtyInCartCommand.Execute((GetSelectedItem().ProductId, newQty));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        => _vm.PurgeCommand.Execute();

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IProductDataService, ProductDataService>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterInstance(_vm);

            _vm.LockStoreCommand.Execute();
            container.Resolve<CheckoutWindow>().Show();
        }

        private void Wipe_Click(object sender, RoutedEventArgs e)
        {
            var decission = MessageBox.Show("Are you sure you want to clear your Cart of all the current items?",
                "Confirm Emptying", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (decission == MessageBoxResult.No) return;
            _vm.ClearCartCommand.Execute();
        }

        private void CustDG_LayoutUpdated(object sender, System.EventArgs e)
        { 
            var subtotal = 0m;
            var itemCount = 0;
            var prodCount = 0;

            foreach (var item in _vm.Cart)
            {
                prodCount++;
                itemCount += item.Quantity;
                subtotal += item.Price * item.Quantity;
            }

            totalProductsCountLbl.Content = prodCount;
            totalItemsCountLbl.Content = itemCount;

            var tax = subtotal / 100 * decimal.Parse(ConfigurationManager.AppSettings.Get("Tax"));

            subtotalValueLbl.Content= string.Format("{0:c}", subtotal);
            taxValueLbl.Content= string.Format("{0:c}", tax);
            totalValueLbl.Content = string.Format("{0:c}", subtotal + tax);
        }
    }
}
