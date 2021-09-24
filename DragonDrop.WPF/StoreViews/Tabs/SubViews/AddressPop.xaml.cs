using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for AddressPop.xaml
    /// </summary>
    public partial class AddressPop : Window, IReloadableControl
    {
        private StoreView _storeView;
        private Customer _cust;

        private CustomerEditWindow _uniqueSub;

        public AddressPop(Customer cust, StoreView storeView=null)
        {
            _storeView = storeView;
            _cust = cust;

            InitializeComponent();

            LoadData();
        }

        private void LoadData()=> DataContext = new AddressPopViewModel(_cust);

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var newWin = new CustomerEditWindow(_cust.CustomerId, this as IReloadableControl, _storeView);
            if (_uniqueSub != null) _uniqueSub.Close();
            _uniqueSub = newWin;
            newWin.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public void Reload() => LoadData();
    }
}
