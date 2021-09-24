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
    /// Interaction logic for AdminProductsTab.xaml
    /// </summary>
    public partial class AdminProductsTab : UserControl, IReloadableControl
    {
        private bool _codeSortIsAscending = true;
        private bool _descSortIsAscending = true;
        private bool _stockSortIsAscending = true;
        private bool _isAdvExpanded = true;
        private UnityContainer _container;


        private Style _visStyle = new Style(typeof(TextBlock));
        private Style _hidStyle = new Style(typeof(TextBlock));

        private Style _imgVisStyle = new Style(typeof(Image));
        private Style _imgHidStyle = new Style(typeof(Image));

        private StoreView _storeView;
        private Product _selProd;

        public AdminProductsTab(StoreView storeView = null)
        {
            _storeView = storeView;

            InitializeComponent();

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

        private void LoadData() => DataContext = _container.Resolve<AdminProductsTabViewModel>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_selProd != null) new ProductDetailsPop(_selProd, this, _storeView).Show();
        }

        private void NewEntry_Click(object sender, RoutedEventArgs e)
            => new ProductAddWindow(this as IReloadableControl, _storeView).Show();

        private void EditEntry_Click(object sender, RoutedEventArgs e)
            => new ProductEditWindow(_selProd.ProductId, this, _storeView).Show();

        #region Input Handling

        #region Multi-Column

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

        #endregion

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

        #endregion

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        public void Reload() => ResetBtn.PerformClick();

        private void CustDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
         => _selProd = custDG.SelectedItem as Product;
    }
}
