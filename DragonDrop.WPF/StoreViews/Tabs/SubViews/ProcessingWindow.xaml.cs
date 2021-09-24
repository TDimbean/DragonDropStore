using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ProcessingWindow.xaml
    /// </summary>
    public partial class ProcessingWindow : Window, IReloadableControl
    {
        private bool _custSortIsAscending = true;

        private IOrderDataService _ordServ;
        private IProductDataService _prodServ;
        private IOrderItemDataService _itemServ;

        private StoreView _storeView;

        public ProcessingWindow(IOrderDataService ordServ, IProductDataService prodServ, StoreView storeView)
        {
            storeView.Lock();
            _storeView = storeView;

            _ordServ = ordServ;
            _prodServ = prodServ;

            var container = new UnityContainer();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            _itemServ = container.Resolve<OrderItemDataService>();

            LoadData();

            InitializeComponent();
        }

        public void Reload() => LoadData();

        private void LoadData() =>
            DataContext = new ProcessingWindowViewModel(_ordServ);

        private void CustomerIdHeader_Click(object sender, RoutedEventArgs e)
        => ControlHelpers.SortDataGridByCustom(ref _custSortIsAscending, "CustomerId", custDG);

        private void CustomerId_Click(object sender, RoutedEventArgs e)
        {
            var ord = custDG.SelectedItem as Order;
            var custId = ord.CustomerId;

            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            var custServ = container.Resolve<CustomerDataService>();

            var cust = custServ.Get(custId);
            var custPop = new CustomerPop( cust, this, _storeView);
            custPop.Show();
        }

        private void ProcessHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            StopAnyOrderProcessing();

            var order = custDG.SelectedItem as Order;
            new ProcessOrderWindow(order.OrderId, _ordServ, _prodServ, _itemServ, this).Show();
        }

        private void StopAnyOrderProcessing()
        {
            foreach (var win in Application.Current.Windows)
            {
                if (win.GetType() == typeof(ProcessOrderWindow)) (win as Window).Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StopAnyOrderProcessing();
            _storeView.Unlock();
            _storeView.RefreshAdminOrders();
            _storeView.RefreshAdminProducts();
            base.OnClosing(e);
        }
    }
}
