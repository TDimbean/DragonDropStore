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
    /// Interaction logic for AdminCustomersTab.xaml
    /// </summary>
    public partial class AdminCustomersTab : UserControl, IReloadableControl
    {
        private bool _adrSortIsAscending = true;
        private bool _emailSortIsAscending = true;
        private bool _citySortIsAscending = true;
        private bool _stateSortIsAscending = true;
        private bool _isAdvExpanded = true;
        private UnityContainer _container;
        private StoreView _storeView;

        private Style _visStyle = new Style(typeof(TextBlock));
        private Style _hidStyle = new Style(typeof(TextBlock));

        private Customer _selCust;

        public AdminCustomersTab(ICustomerDataService service, StoreView storeView = null)
        {
            InitializeComponent();

            _container = new UnityContainer();
            _container.RegisterType<ICustomerRepository, CustomerRepository>();
            _container.RegisterType<ICustomerDataService, CustomerDataService>();
            _storeView = storeView;

            ResetBtn.Focus();
            LoadData();

            _visStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
            _hidStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Hidden));

            Resources["EmailDeetStyle"]= _visStyle;
        }

        private void ResetCont_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ContSearchToggle.Content == "On") ContSearchToggle.PerformClick();
        }

        private void AdrDeetHeader_Click(object sender, RoutedEventArgs e)
            => DetailsPop.IsOpen = !DetailsPop.IsOpen;

        private void SortDataGridByCustom(ref bool isAsc, string sortCrit)
        {
            ControlHelpers.SortDataGridByCustom(ref isAsc, sortCrit, custDG);

            Resources["AdrDeetStyle"] = _hidStyle;
            Resources["CityDeetStyle"] = _hidStyle;
            Resources["StateDeetStyle"] = _hidStyle;
            Resources["EmailDeetStyle"] = _hidStyle;
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as AdminCustomersTabViewModel).SearchCommand.Execute();
        }

        private void SortByAdr_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _adrSortIsAscending, "Address");
            Resources["AdrDeetStyle"] = _visStyle;
        }

        private void SortByCity_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _citySortIsAscending, "City");
            Resources["CityDeetStyle"] = _visStyle;
        }

        private void SortByState_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _stateSortIsAscending, "State");
            Resources["StateDeetStyle"] = _visStyle;
        }

        private void SortByEmail_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _emailSortIsAscending, "Email");
            Resources["EmailDeetStyle"] = _visStyle;
        }

        private void LoadData() => DataContext = _container.Resolve<AdminCustomersTabViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        => new AddressPop(_selCust, _storeView).Show();

        private void NewEntry_Click(object sender, RoutedEventArgs e) => new CustomerAddWindow(this, _storeView).Show();

        private void EditEntry_Click(object sender, RoutedEventArgs e)
            => new CustomerEditWindow(_selCust.CustomerId, this, _storeView).Show();

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

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void ContSearchToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Off", "On");

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
                    => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);

        // Resets

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

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        public void Reload() => ResetBtn.PerformClick();

        private void CustDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => _selCust = custDG.SelectedItem as Customer;
    }
}
