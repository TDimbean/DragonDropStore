using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class UserStoreTabViewModel : BindableBase
    {
        #region Fields

        private IProductDataService _service;
        private List<Product> _productList;

        private string _searchText;
        private Brush _searchColour;
        private FontStyle _searchStyle;

        private string _searchFont;
        private string _searchPlaceholder = "Showing all entries. Search...";

        private Visibility _priceFilterVisibility;
        private Visibility _stockFilterVisibility;
        private Visibility _searchFilterVisibility;

        private string _priceToggleText;
        private string _stockToggleText;

        private bool _filterPriceOver;
        private bool _filterStockOver;

        private int _selFilterIndex;

        private string _advPriceText;
        private string _advStockText;

        private string _advSortDirToggle;

        private int _selSortIndex;

        private string _advPageSizeText;

        private int _resultsPerPage;

        private Visibility _prevVisibility;
        private Visibility _nextVisibility;

        private string _advPagingText;

        private string _contSearchToggleText;

        private int _pgCount = 1;
        private int _pgIndex = 1;

        private bool _isAdvSearchOn;

        private bool _continuousSearch;

        private Dictionary<string, (bool, bool, bool)> _queryOptions =
        new Dictionary<string, (bool, bool, bool)>
        {
            { "filterSortAndPage", (true, true, true)},
            { "sortAndPage", (false, true, true)},
            { "filterAndPage", (true, false, true)},
            { "page", (false, false, true)},
            { "filterAndSort", (true, true, false)},
            { "sort", (false, true, false)},
            { "filter", (true, false, false)},
            { "none", (false, false, false) }
        };

        private bool _priceOver;
        private bool _stockOver;
        private int _pgSize;
        private bool _desc;
        private string _sortBy;
        private decimal _price;
        private int _stock;

        private (bool isAdvFilter, bool isAdvSort, bool isPaged) _query;

        #endregion

        public UserStoreTabViewModel(IProductDataService service)
        {
            _service = service;

            #region Commands

            ToggleContinuousCommand = new DelegateCommand(ToggleContinuousCommandExecute);

            SearchGotFocusCommand = new DelegateCommand(SearchGotFocusCommandExecute);
            SearchLostFocusCommand = new DelegateCommand(SearchLostFocusCommandExecute);
            SearchTextChangedCommand = new DelegateCommand(SearchTextChangedCommandExecute);

            FilterSelChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(FilterSelChangedCommandExecute);

            AdvPriceTextChangedCommand = new DelegateCommand(AdvPriceTextChangedCommandExecute);

            AdvPriceLostFocusCommand = new DelegateCommand(AdvPriceLostFocusCommandExecute);

            SearchCommand = new DelegateCommand(SearchCommandExecute);
            ResetCommand = new DelegateCommand(ResetCommandExecute);

            PrevCommand = new DelegateCommand(PrevCommandExecute);
            NextCommand = new DelegateCommand(NextCommandExecute);

            AdvVisChangedCommand = new DelegateCommand(AdvVisChangedCommandExecute);

            #endregion

            // Init
            ResetCommandExecute();
        }

        #region Properties

        #region ViewModel Props

        public List<Product> ProductList
        {
            get => _productList;
            set => SetProperty(ref _productList, value);
        }

        public string AdvPagingText
        {
            get => _advPagingText;
            set => SetProperty(ref _advPagingText, value);
        }

        public string ContSearchToggleText
        {
            get => _contSearchToggleText;
            set => SetProperty(ref _contSearchToggleText, value);
        }

        public Visibility PrevVisibility
        {
            get => _prevVisibility;
            set => SetProperty(ref _prevVisibility, value);
        }

        public Visibility NextVisibility
        {
            get => _nextVisibility;
            set => SetProperty(ref _nextVisibility, value);
        }

        public int ResultsPerPage
        {
            get => _resultsPerPage;
            set => SetProperty(ref _resultsPerPage, value);
        }

        public string AdvPageSizeText
        {
            get => _advPageSizeText;
            set => SetProperty(ref _advPageSizeText, value);
        }

        public int SelSortIndex
        {
            get => _selSortIndex;
            set => SetProperty(ref _selSortIndex, value);
        }

        public string[] SortOptions
        {
            get => new string[] { "Name", "Category", "Price", "Stock", "Barcode", "Description" };
        }

        public string AdvSortDirToggle
        {
            get => _advSortDirToggle;
            set => SetProperty(ref _advSortDirToggle, value);
        }

        public string AdvPriceText
        {
            get => _advPriceText;
            set => SetProperty(ref _advPriceText, value);
        }

        public string AdvStockText
        {
            get => _advStockText;
            set => SetProperty(ref _advStockText, value);
        }

        public int SelFilterIndex
        {
            get => _selFilterIndex;
            set => SetProperty(ref _selFilterIndex, value);
        }

        public string[] FilterOptions
        {
            get => new string[] { "Search", "Price", "Stock" };
        }

        public bool FilterPriceOver
        {
            get => _filterPriceOver;
            set => SetProperty(ref _filterPriceOver, value);
        }

        public bool FilterStockOver
        {
            get => _filterStockOver;
            set => SetProperty(ref _filterStockOver, value);
        }

        public string PriceToggleText
        {
            get => _priceToggleText;
            set => SetProperty(ref _priceToggleText, value);
        }

        public string StockToggleText
        {
            get => _stockToggleText;
            set => SetProperty(ref _stockToggleText, value);
        }

        public Visibility SearchFilterVisibility
        {
            get => _searchFilterVisibility;
            set => SetProperty(ref _searchFilterVisibility, value);
        }

        public Visibility PriceFilterVisibility
        {
            get => _priceFilterVisibility;
            set => SetProperty(ref _priceFilterVisibility, value);
        }

        public Visibility StockFilterVisibility
        {
            get => _stockFilterVisibility;
            set => SetProperty(ref _stockFilterVisibility, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SearchFont
        {
            get => _searchFont;
            set => SetProperty(ref _searchFont, value);
        }

        public FontStyle SearchStyle
        {
            get => _searchStyle;
            set => SetProperty(ref _searchStyle, value);
        }

        public Brush SearchColour
        {
            get => _searchColour;
            set => SetProperty(ref _searchColour, value);
        }

        #endregion

        #region Commands

        public DelegateCommand ToggleContinuousCommand { get; }

        public DelegateCommand SearchGotFocusCommand { get; }
        public DelegateCommand SearchLostFocusCommand { get; }
        public DelegateCommand SearchTextChangedCommand { get; }

        public DelegateCommand<SelectionChangedEventArgs> FilterSelChangedCommand { get; }

        public DelegateCommand AdvPriceTextChangedCommand { get; }

        public DelegateCommand AdvPriceLostFocusCommand { get; }

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ResetCommand { get; }

        public DelegateCommand PrevCommand { get; }
        public DelegateCommand NextCommand { get; }

        public DelegateCommand AdvVisChangedCommand { get; }

        #endregion

        #endregion

        #region Private/Not-Exposed Methods

        #region Commands

        private void ToggleContinuousCommandExecute()
            => _continuousSearch = !_continuousSearch;

        private void SearchTextChangedCommandExecute()
        {
            if (SelFilterIndex == 0 && _continuousSearch) SearchCommandExecute(false);
        }

        private void SearchGotFocusCommandExecute()
        {
            SearchColour = Brushes.Black;
            SearchFont = "Adobe Heiti Std R";
            SearchStyle = FontStyles.Normal;

            if (SearchText == _searchPlaceholder) SearchText = string.Empty;
        }

        private void SearchLostFocusCommandExecute()
        {
            SearchColour = Brushes.Gray;
            SearchFont = "Segoe UI Historical";
            SearchStyle = FontStyles.Italic;

            if (string.IsNullOrEmpty(SearchText.Trim())) SearchText = _searchPlaceholder;
        }

        private void FilterSelChangedCommandExecute(SelectionChangedEventArgs e)
        {
            SearchFilterVisibility = Visibility.Collapsed;
            PriceFilterVisibility = Visibility.Collapsed;
            StockFilterVisibility = Visibility.Collapsed;

            switch (SelFilterIndex)
            {
                case 1:
                    PriceFilterVisibility = Visibility.Visible;
                    break;
                case 2:
                    StockFilterVisibility = Visibility.Visible;
                    break;
                default:
                    SearchFilterVisibility = Visibility.Visible;
                    break;
            }
        }

        private void AdvPriceTextChangedCommandExecute()
        {
            if (AdvPriceText.Length > 15) AdvPriceText = AdvPriceText.Substring(0, 15);
        }

        private void AdvPriceLostFocusCommandExecute()
        {
            var val = 0m;
            var parses = decimal.TryParse(AdvPriceText, out val);
            if (parses) AdvPriceText = string.Format("{0:C}", val);
        }

        private void SearchCommandExecute() => SearchCommandExecute(false);

        private void SearchCommandExecute(bool isNavReq)
        {
            _query = (SelFilterIndex != 0 ||
                        (SelFilterIndex == 0 &&
                        !string.IsNullOrEmpty(SearchText)
                        && SearchText != _searchPlaceholder),
                    SelSortIndex != 0 ||
                        (SelSortIndex == 0
                        && AdvSortDirToggle == "Descending"),
                    AdvPageSizeText != "All");

            if (_query == _queryOptions["none"]) ProductList = SearchText == _searchPlaceholder ?
                      new List<Product>(_service.GetAll()) :
                      new List<Product>(_service.GetAllFiltered(SearchText));

            else
            {
                if (!isNavReq) _pgIndex = 1;
                ExtractAdvValues();

                ProductList = new List<Product>(QueryService());

                if (_query.isPaged)
                {
                    _query.isPaged = false;
                    var totalItems = new List<Product>(QueryService()).Count;
                    if (_pgSize >= totalItems)
                    {
                        AdvPageSizeText = "All";
                        _pgCount = 1;
                    }
                    else _pgCount = totalItems % _pgSize == 0 ? totalItems / _pgSize : (totalItems / _pgSize) + 1;
                }
                else
                {
                    _pgCount = 1;
                }
            }

            UpdateNavButtons();
        }

        private void PrevCommandExecute() => Scroll(false);

        private void NextCommandExecute() => Scroll(true);

        private void AdvVisChangedCommandExecute() => _isAdvSearchOn = !_isAdvSearchOn;

        private void ResetCommandExecute()
        {
            ProductList = new List<Product>(_service.GetAll());

            SearchText = _searchPlaceholder;
            SearchFont = "Segoe UI Historic";
            SearchStyle = FontStyles.Italic;
            SearchColour = Brushes.Gray;

            SearchFilterVisibility = Visibility.Visible;
            PriceFilterVisibility = Visibility.Collapsed;
            StockFilterVisibility = Visibility.Collapsed;

            PriceToggleText = "Under: ";
            StockToggleText = "Under: ";

            AdvPriceText = string.Empty;
            AdvStockText = string.Empty;

            SelFilterIndex = 0;

            AdvSortDirToggle = "Ascending";
            SelSortIndex = 0;

            AdvPageSizeText = "All";

            ResultsPerPage = 0;

            PrevVisibility = Visibility.Hidden;
            NextVisibility = Visibility.Hidden;

            AdvPagingText = "Page: 1 of 1";

            ContSearchToggleText = "Off";
            _continuousSearch = false;
        }

        #endregion

        #region Methods

        private void Scroll(bool forward)
        {
            _pgIndex += forward ? 1 : -1;
            SearchCommandExecute(true);
        }

        private void UpdateNavButtons()
        {
            PrevVisibility = _pgIndex == 1 ? Visibility.Hidden : Visibility.Visible;
            NextVisibility = _pgIndex == _pgCount ? Visibility.Hidden : Visibility.Visible;

            AdvPagingText = "Page: " + _pgIndex + " of " + _pgCount;
        }

        private IEnumerable<Product> QueryService()
        {
            if (_query == _queryOptions["filterSortAndPage"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredSortedAndPaged(_price, _priceOver, _pgSize, _pgIndex, _sortBy, _desc);
                    case 2:
                        return _service.GetAllFilteredSortedAndPaged(_stock, _stockOver, _pgSize, _pgIndex, _sortBy, _desc);
                    default:
                        return _service.GetAllFilteredSortedAndPaged(SearchText, _pgSize, _pgIndex, _sortBy, _desc);
                }
            else if (_query == _queryOptions["sortAndPage"])
                return _service.GetAllSortedAndPaged(_pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["filterAndPage"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredAndPaged(_price, _priceOver, _pgSize, _pgIndex);
                    case 2:
                        return _service.GetAllFilteredAndPaged(_stock, _stockOver, _pgSize, _pgIndex);
                    default:
                        return _service.GetAllFilteredAndPaged(SearchText, _pgSize, _pgIndex);
                }
            else if (_query == _queryOptions["page"])
                return _service.GetAllPaginated(_pgSize, _pgIndex);
            else if (_query == _queryOptions["filterAndSort"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredAndSorted(_price, _priceOver, _sortBy, _desc);
                    case 2:
                        return _service.GetAllFilteredAndSorted(_stock, _stockOver, _sortBy, _desc);
                    default:
                        return _service.GetAllFilteredAndSorted(SearchText, _sortBy, _desc);
                }
            else if (_query == _queryOptions["sort"])
                return _service.GetAllSorted(_sortBy, _desc);
            else if (_query == _queryOptions["filter"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFiltered(_price, _priceOver);
                    case 2:
                        return _service.GetAllFiltered(_stock, _stockOver);
                    default:
                        return _service.GetAllFiltered(SearchText);
                }
            else if (_query == _queryOptions["none"])
                if (SearchText == _searchPlaceholder) return _service.GetAll();
                else return _service.GetAllFiltered(SearchText);
            else return _service.GetAllFiltered(SearchText);
        }

        private void ExtractAdvValues()
        {
            _sortBy = null;
            switch (SelSortIndex)
            {
                case 1:
                    _sortBy = "cat";
                    break;
                case 2:
                    _sortBy = "price";
                    break;
                case 3:
                    _sortBy = "stock";
                    break;
                case 4:
                    _sortBy = "code";
                    break;
                case 5:
                    _sortBy = "man";
                    break;
                default:
                    _sortBy = "name";
                    break;
            }

            _pgSize = AdvPageSizeText == "All" ? 0 : int.Parse(AdvPageSizeText);

            _desc = AdvSortDirToggle == "Descending";

            _price = 0m;
            var priceParses = !string.IsNullOrEmpty(AdvPriceText) ?
            decimal.TryParse(AdvPriceText.Replace("$", string.Empty), out _price) : false;

            _priceOver = PriceToggleText == "Over: ";

            _stock = 0;
            var stockParses = int.TryParse(AdvStockText, out _stock);

            _stockOver = StockToggleText == "Over: ";

            if ((SelFilterIndex == 2 && !stockParses) || (SelFilterIndex == 1 && !priceParses)) _query.isAdvFilter = false;
        }

        #endregion

        #endregion
    }
}
