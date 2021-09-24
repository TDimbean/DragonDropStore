using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for AdminPaymentsTab.xaml
    /// </summary>
    public partial class AdminPaymentsTab : UserControl, IReloadableControl
    {
        private bool _custSortIsAscending = true;
        private bool _isAdvExpanded = true;
        private UnityContainer _container;
        private StoreView _storeView;

        private Payment _selPay;

        public AdminPaymentsTab(IPaymentDataService service, StoreView storeView=null)
        {
            InitializeComponent();

            _container = new UnityContainer();
            _container.RegisterType<IPaymentRepository, PaymentRepository>();
            _container.RegisterType<IPaymentDataService, PaymentDataService>();
            _storeView = storeView;

            ResetBtn.Focus();

            LoadData();
        }

        private void LoadData() => DataContext = _container.Resolve<AdminPaymentsTabViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var custId = _selPay.CustomerId;

            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            var custServ = container.Resolve<CustomerDataService>();

            var cust = custServ.Get(custId);
            new CustomerPop(cust,this, _storeView).Show();
        }

        private void CustomerIdHeader_Click(object sender, RoutedEventArgs e)
        {
            var sortDir = _custSortIsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            _custSortIsAscending = !_custSortIsAscending;

            var dataView = CollectionViewSource.GetDefaultView(custDG.ItemsSource);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription("CustomerId", sortDir));
            dataView.Refresh();
        }

        private void NewEntry_Click(object sender, RoutedEventArgs e) => new PaymentAddWindow(this, _storeView).Show();

        private void EditEntry_Click(object sender, RoutedEventArgs e)
            => new PaymentEditWindow(_selPay.PaymentId, this, _storeView).Show();

        private void AdvSearchExpanderBtn_Click(object sender, RoutedEventArgs e)
        {
            _isAdvExpanded = !_isAdvExpanded;
            if (_isAdvExpanded)
            {
                AdvSearch.Visibility = Visibility.Collapsed;
                AdvSearchArrow.RenderTransform = new ScaleTransform(1, 1);
                AdvDatePop.IsOpen = false;
                custDG.Height = 760;
                return;
            }
            AdvSearch.Visibility = Visibility.Visible;
            AdvSearchArrow.RenderTransform = new ScaleTransform(1, -1);
            custDG.Height = 646;

            // GRID HEIGHT
            //Very wonky behaviour, don't change the default height. In theory, the minHeight(when Advanced Search is expanded) should
            //be the regular height - 94(Combined height of all 3 AdvSearch Rows). It was initially calculated with the def height
            //being 750, yet it seems the DataGrid only changed heights at certain intervals, so there's no use fiddling with it.
        }

        private void ToggleAdvDatePop_Click(object sender, RoutedEventArgs e)
            => AdvDatePop.IsOpen = !AdvDatePop.IsOpen;

        private void AdvAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9,.]");

        private void AdvCust_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void AdvAmountRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Over: ", "Under: ");

        private void AdvDateRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Before: ", "After: ");

        private void ContSearchToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Off", "On");

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
                    => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);

        public void HideAdvCal() => AdvDatePop.IsOpen = false;
        
        // Resets

        private void ResetAdvCust_Click(object sender, RoutedEventArgs e)
        {
            AdvCustBox.Text = string.Empty;
        }

        private void ResetAdvSort_Click(object sender, RoutedEventArgs e)
        {
            AdvSortCombo.SelectedIndex = 0;
            AdvSortDirToggle.Content = "Ascending";
        }

        private void ResetAdvPage_Click(object sender, RoutedEventArgs e)
        {
            AdvPageBox.Text = "All";
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            AdvDateBox.Text = string.Empty;
            AdvAmountBox.Text = string.Empty;
            ContSearchToggle.Content = "Off";
            AdvFilCombo.SelectedIndex = 0;
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as AdminPaymentsTabViewModel).SearchCommand.Execute();
        }

        private void AmountBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var decVal = 0m;
            var parses = decimal.TryParse(AdvAmountBox.Text, out decVal);

            if (parses) AdvAmountBox.Text = string.Format("{0:C}", decVal);

            FilterBox_KeyUp(sender, e);
        }

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as AdminPaymentsTabViewModel).SearchCommand.Execute();
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        public void Reload() => ResetBtn.PerformClick();

        private void CustDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => _selPay = custDG.SelectedItem as Payment;
    }
}
