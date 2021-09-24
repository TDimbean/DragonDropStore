using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using DragonDrop.WPF.StoreViews.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserStoreTab.xaml
    /// </summary>
    public partial class UserStoreTab : UserControl, IReloadableControl
    {
        private StoreView _storeView;

        #region Fields

        private bool _codeSortIsAscending = true;
        private bool _descSortIsAscending = true;
        private bool _stockSortIsAscending = true;
        private bool _isAdvExpanded = true;
        private UnityContainer _container;


        private Style _visStyle = new Style(typeof(TextBlock));
        private Style _hidStyle = new Style(typeof(TextBlock));

        private Style _imgVisStyle = new Style(typeof(Image));
        private Style _imgHidStyle = new Style(typeof(Image));

        #endregion

        public UserStoreTab(StoreView storeView, IProductDataService service)
        {
            InitializeComponent();

            _storeView = storeView;

            _container = new UnityContainer();
            _container.RegisterType<IProductRepository, ProductRepository>();
            _container.RegisterType<IProductDataService, ProductDataService>();

            ResetBtn.Focus();

            LoadData();

            _visStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
            _hidStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Hidden));
            _imgVisStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
            _imgHidStyle.Setters.Add(new Setter(VisibilityProperty, Visibility.Hidden));

            Resources["StockDeetStyle"] = _visStyle;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var prod = custDG.SelectedItem as Product;
            (_storeView.DataContext as StoreViewModel).AddToCartCommand.Execute(prod.ProductId);
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            _storeView.SwitchToCart();
        }

        private void LoadData() => DataContext = _container.Resolve<AdminProductsTabViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var prod = custDG.SelectedItem as Product;

            var serv = _container.Resolve<ProductDataService>();
            
            var prodPop = new UserProdDetailWindow(prod.ProductId);
            prodPop.Show();
        }

        private void DetailsHeader_Click(object sender, RoutedEventArgs e)
        => DetailsPop.IsOpen = !DetailsPop.IsOpen;

        private void SortByDescription_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _descSortIsAscending, "Description");
            Resources["DescDeetStyle"] = _imgVisStyle;
        }

        private void SortByStock_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _stockSortIsAscending, "Stock");
            Resources["StockDeetStyle"] = _visStyle;
        }

        private void SortByCode_Click(object sender, RoutedEventArgs e)
        {
            SortDataGridByCustom(ref _codeSortIsAscending, "BarCode");
            Resources["CodeDeetStyle"] = _visStyle;
        }

        private void SortDataGridByCustom(ref bool isAsc, string sortCrit)
        {
            ControlHelpers.SortDataGridByCustom(ref isAsc, sortCrit, custDG);

            Resources["StockDeetStyle"] = _hidStyle;
            Resources["CodeDeetStyle"] = _hidStyle;
            Resources["DescDeetStyle"] = _imgHidStyle;
        }

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
        }

        private void AdvPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9,.]");

        private void AdvStock_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void AdvPriceRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Over: ", "Under: ");

        private void AdvStockRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Over: ", "Under: ");

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

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            AdvStockBox.Text = string.Empty;
            AdvPriceBox.Text = string.Empty;
            ContSearchToggle.Content = "Off";
            AdvFilCombo.SelectedIndex = 0;
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as AdminProductsTabViewModel).SearchCommand.Execute();
        }

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as AdminProductsTabViewModel).SearchCommand.Execute();
            }
        }

        private void PriceBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var decVal = 0m;
            var parses = decimal.TryParse(AdvPriceBox.Text, out decVal);

            if (parses) AdvPriceBox.Text = string.Format("{0:C}", decVal);

            FilterBox_KeyUp(sender, e);
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        public void Reload() => ResetBtn.PerformClick();
    }
}

