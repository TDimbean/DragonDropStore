using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for AdminMenuTab.xaml
    /// </summary>
    public partial class AdminMenuTab : UserControl
    {
        private StoreView _storeView;

        public AdminMenuTab(StoreView storeView)
        {
            _storeView = storeView;

            InitializeComponent();
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            _storeView.SwitchToMain();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        => Application.Current.Shutdown();

        private void ModValues_Click(object sender, RoutedEventArgs e)
        => new EditDefaultsWindow().Show();

        private void TakeInv_Click(object sender, RoutedEventArgs e)
         => new TakeInventoryWindow(_storeView).Show();

        private void ProcOrders_Click(object sender, RoutedEventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();

            new ProcessingWindow(container.Resolve<OrderDataService>(),
                                 container.Resolve<ProductDataService>(), _storeView).Show();
        }

        private void ShipOrders_Click(object sender, RoutedEventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType<IOrderRepository, OrderRepository>();

            new ShipOrdersWindow(container.Resolve<OrderDataService>(), _storeView).Show();
        }

        private void ScanConfig_Click(object sender, RoutedEventArgs e)
            => new ScannerConfigWindow(_storeView).Show();
    }
}
