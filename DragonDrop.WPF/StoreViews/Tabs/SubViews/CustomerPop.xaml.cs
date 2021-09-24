using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for CustomerPop.xaml
    /// </summary>
    public partial class CustomerPop : Window
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public CustomerPop(Customer customer, IReloadableControl callingTab = null, StoreView storeView = null)
        {
            _storeView = storeView;
            _callingTab = callingTab;

            InitializeComponent();

            DataContext = new CustomerPopViewModel(customer);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AddressStack.Visibility == Visibility.Visible)
            {
                AddressStack.Visibility = Visibility.Collapsed;
                AdrBtnLabel.Content = "Address Details";
                return;
            }
            AddressStack.Visibility = Visibility.Visible;
            AdrBtnLabel.Content = "Hide Address";
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            new CustomerEditWindow((DataContext as CustomerPopViewModel).CustomerId, _callingTab, _storeView).Show();
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
