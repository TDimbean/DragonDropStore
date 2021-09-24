using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for OrderDetailsPop.xaml
    /// </summary>
    public partial class OrderDetailsPop : Window
    {
        private int _ordId;
        private AdminOrdersTab _callingTab;
        private StoreView _storeView;

        public OrderDetailsPop(Order ord, AdminOrdersTab callingTab, StoreView storeView)
        {
            _storeView = storeView;
            _callingTab = callingTab;
            _ordId = ord.OrderId;
            InitializeComponent();

            DataContext = new OrderDetailsPopViewModel(ord);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            new OrderEditWindow(_ordId, _callingTab, _storeView).Show();
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
