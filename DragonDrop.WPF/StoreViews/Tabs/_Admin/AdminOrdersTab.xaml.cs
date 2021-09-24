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
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for AdminOrdersTab.xaml
    /// </summary>
    public partial class AdminOrdersTab : UserControl, IReloadableControl
    {
        private StoreView _storeView;

        private bool _custSortIsAscending = true;
        private bool _paySortIsAscending = true;
        private bool _shipSortIsAscending = true;
        private bool _shipDateSortIsAscending = true;
        private bool _isAdvExpanded = true;
        private UnityContainer _container;

        private Style _visStyle = new Style(typeof(TextBlock));
        private Style _hidStyle = new Style(typeof(TextBlock));

        public AdminOrdersTab(IOrderDataService service, StoreView storeView)
        {
            InitializeComponent();

            _storeView = storeView;

            _container = new UnityContainer();
            _container.RegisterType<IOrderRepository, OrderRepository>();
            _container.RegisterType<IOrderDataService, OrderDataService>();

            ResetBtn.Focus();

            LoadData();

            _visStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
            _hidStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Hidden));

            Resources["ShipMethDeetStyle"] = _visStyle;
        }

        private void LoadData() => DataContext = _container.Resolve<AdminOrdersTabViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ord = custDG.SelectedItem as Order;
            var custId = ord.CustomerId;

            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            var custServ = container.Resolve<CustomerDataService>();

            new CustomerPop(custServ.Get(custId), this, _storeView).Show();
        }

        private void CustomerIdHeader_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.SortDataGridByCustom(ref _custSortIsAscending, "CustomerId", custDG);

        private void NewEntry_Click(object sender, RoutedEventArgs e) => new OrderAddWindow(this, _storeView).Show();

        private void EditEntry_Click(object sender, RoutedEventArgs e)
            => new OrderEditWindow((custDG.SelectedItem as Order).OrderId, this, _storeView).Show();

        private void AdvSearchExpanderBtn_Click(object sender, RoutedEventArgs e)
        {
            _isAdvExpanded = !_isAdvExpanded;
            if (_isAdvExpanded)
            {
                AdvSearch.Visibility = Visibility.Collapsed;
                AdvSearchArrow.RenderTransform = new ScaleTransform(1, 1);
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

        private void AdvCust_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void ContSearchToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Off", "On");

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
                    => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);

        // Resets

        private void ResetContSearch_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ContSearchToggle.Content == "On") ContSearchToggle.PerformClick();
        }

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

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as AdminOrdersTabViewModel).SearchCommand.Execute();
            }
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as AdminOrdersTabViewModel).SearchCommand.Execute();
        }


        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        private void DetailsHeader_Click(object sender, RoutedEventArgs e)
            => DetailsPop.IsOpen = !DetailsPop.IsOpen;

        private void DetailsItem_Click(object sender, RoutedEventArgs e)
        {
            var ord = custDG.SelectedItem as Order;
            var deetPop = new OrderDetailsPop(ord, this, _storeView);
            deetPop.Show();
        }

        private void SortByShipDate_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _shipDateSortIsAscending, "ShippingDate");
            Resources["ShipDateDeetStyle"] = _visStyle;
        }

        private void SortByPayMeth_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _paySortIsAscending, "PaymentMethodId");
            Resources["PayMethDeetStyle"] = _visStyle;
        }

        private void SortByShipMeth_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _shipSortIsAscending, "ShippingMethodId");
            Resources["ShipMethDeetStyle"] = _visStyle;
        }

        private void SortDataGridByCustom(ref bool isAsc, string sortCrit)
        {
            ControlHelpers.SortDataGridByCustom(ref isAsc, sortCrit, custDG);

            Resources["ShipDateDeetStyle"] = _hidStyle;
            Resources["ShipMethDeetStyle"] = _hidStyle;
            Resources["PayMethDeetStyle"] = _hidStyle;
        }

        public void Reload() => ResetBtn.PerformClick();
    }
}
