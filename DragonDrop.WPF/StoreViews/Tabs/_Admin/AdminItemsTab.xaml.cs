using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for AdminItemsTab.xaml
    /// </summary>
    public partial class AdminItemsTab : UserControl, IReloadableControl
    {
        private StoreView _storeView;
        private UnityContainer _container;
        private OrderItem _selItem;

        public AdminItemsTab(StoreView storeView=null)
        {
            _storeView = storeView;

            InitializeComponent();

            _container = new UnityContainer();
            _container.RegisterType<IOrderRepository, OrderRepository>();
            _container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            _container.RegisterType<IProductRepository, ProductRepository>();
            _container.RegisterType<IOrderDataService, OrderDataService>();
            _container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            _container.RegisterType<IProductDataService, ProductDataService>();

            ResetBtn.Focus();

            LoadData();
        }

        private void LoadData() => DataContext = _container.Resolve<AdminItemsTabViewModel>();

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => ResetBtn.PerformClick();

        private void NewEntry_Click(object sender, RoutedEventArgs e) 
            => new ItemAddWindow(this as IReloadableControl, _storeView).Show();

        private void EditEntry_Click(object sender, RoutedEventArgs e)
            => new ItemEditWindow(_selItem.OrderId, _selItem.ProductId, this as IReloadableControl, _storeView).Show();

        #region Input Handling

        private void IntBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
                   => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as AdminItemsTabViewModel).SearchCommand.Execute();
            }
        }

        private void IdToggle_Click(object sender, RoutedEventArgs e)
             => ControlHelpers.ToggleBtnText(e, "Order ID", "Product ID");
        
        private void IntBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as AdminItemsTabViewModel).SearchCommand.Execute();
        }

        private void ResetAdvSort_Click(object sender, RoutedEventArgs e)
        {
            AdvSortCombo.SelectedIndex = 0;
            AdvSortDirToggle.Content = "Ascending";
        }

        private void ResetAdvPage_Click(object sender, RoutedEventArgs e)
         => AdvPageBox.Text = "All";

        private void ResetIdFilter_Click(object sender, RoutedEventArgs e)
        => IdBox.Text = string.Empty;

        #endregion

        private void CustDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as AdminItemsTabViewModel;
            _selItem = custDG.SelectedItem as OrderItem;
            vm.SelItem = _selItem;
            vm.UpdateDetailsCommand.Execute();
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_selItem != null)new OrderEditWindow(_selItem.OrderId, this, _storeView).Show();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selItem != null) new ProductEditWindow(_selItem.ProductId, this, _storeView).Show();
        }

        public void Reload()
        {
            LoadData();
        }
    }
}
