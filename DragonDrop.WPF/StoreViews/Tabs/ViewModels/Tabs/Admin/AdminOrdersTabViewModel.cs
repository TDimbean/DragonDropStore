using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AdminOrdersTabViewModel : BindableBase
    {
        #region Fields

        private IOrderDataService _service;
        private List<Order> _orderList;

        private string _searchText;
        private Brush _searchColour;
        private FontStyle _searchStyle;

        private string _searchFont;
        private string _searchPlaceholder = "Showing all entries. Search...";

        private Visibility _searchFilterVisibility;

        private string _advCustText;

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

        private Dictionary<string, (bool, bool, bool, bool)> _queryOptions =
        new Dictionary<string, (bool, bool, bool, bool)>
        {
            { "custFilteredSortedAndPaged", (true, true, true, true) },
            { "custFilteredAndSorted", (true, true, true, false)},
            { "custFilteredAndPaged", (true, true, false, true)},
            { "custSortedAndPaged", (true, false, true, true)},
            { "custAndFiltered", (true, true, false, false)},
            { "custAndPaged", (true, false, false, true)},
            { "custAndSorted", (true, false, true, false)},
            { "cust", (true, false, false, false)},
            { "filterSortAndPage", (false, true, true, true)},
            { "sortAndPage", (false, false, true, true)},
            { "filterAndPage", (false, true, false, true)},
            { "page", (false, false, false, true)},
            { "filterAndSort", (false, true, true, false)},
            { "sort", (false, false, true, false)},
            { "filter", (false, true, false, false)},
            { "none", (false, false, false, false) }
        };

        private int _custId;
        private int _pgSize;
        private bool _desc;
        private string _sortBy;

        private (bool isCustFiltered, bool isAdvFilter, bool isAdvSort, bool isPaged) _query;

        #endregion

        public AdminOrdersTabViewModel(IOrderDataService service)
        {
                _service = service;

                #region Commands

                ToggleContinuousCommand = new DelegateCommand(ToggleContinuousCommandExecute);

                SearchGotFocusCommand = new DelegateCommand(SearchGotFocusCommandExecute);
                SearchLostFocusCommand = new DelegateCommand(SearchLostFocusCommandExecute);
                SearchTextChangedCommand = new DelegateCommand(SearchTextChangedCommandExecute);

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

        public List<Order> OrderList
        {
            get => _orderList;
            set => SetProperty(ref _orderList, value);
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
            get => new string[] { "ID", "Customer ID", "Placed On", "Shipped On", "Status", "Shipping Method", "Payment Method" };
        }

        public string AdvSortDirToggle
        {
            get => _advSortDirToggle;
            set => SetProperty(ref _advSortDirToggle, value);
        }

        public string AdvCustText
        {
            get => _advCustText;
            set => SetProperty(ref _advCustText, value);
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

        public DelegateCommand AdvCustTextChangedCommand { get; }

        public DelegateCommand AdvCalDateChangedCommand { get; }

        public DelegateCommand AdvAmountLostFocusCommand { get; }
        public DelegateCommand AdvDateLostFocusCommand { get; }

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
            if (_continuousSearch) SearchCommandExecute(false);
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

        private void SearchCommandExecute() => SearchCommandExecute(false);

        private void SearchCommandExecute(bool isNavReq)
        {
            _query = (!string.IsNullOrEmpty(AdvCustText),
                                    !string.IsNullOrEmpty(SearchText)
                                    && SearchText != _searchPlaceholder,
                                SelSortIndex != 0 ||
                                    (SelSortIndex == 0
                                    && AdvSortDirToggle == "Descending"),
                                AdvPageSizeText != "All");

            if (_query==_queryOptions["none"]) OrderList = SearchText == _searchPlaceholder ? 
                    new List<Order>(_service.GetAll()) :
                    new List<Order>(_service.GetAllFiltered(SearchText));

            else
            {
                if (!isNavReq) _pgIndex = 1;
                ExtractAdvValues();

                OrderList = new List<Order>(QueryService());

                if (_query.isPaged)
                {
                    _query.isPaged = false;
                    var totalItems = new List<Order>(QueryService()).Count;
                    if (_pgSize >= totalItems)
                    {
                        AdvPageSizeText = "All";
                        _pgCount = 1;
                    }
                    else _pgCount = totalItems % _pgSize == 0 ? totalItems / _pgSize : (totalItems / _pgSize) + 1;
                }
                else _pgCount = 1;
            }

            UpdateNavButtons();
        }

        private void PrevCommandExecute() => Scroll(false);

        private void NextCommandExecute() => Scroll(true);

        private void AdvVisChangedCommandExecute() => _isAdvSearchOn = !_isAdvSearchOn;

        private void ResetCommandExecute()
        {
            OrderList = new List<Order>(_service.GetAll());

            SearchText = _searchPlaceholder;
            SearchFont = "Segoe UI Historic";
            SearchStyle = FontStyles.Italic;
            SearchColour = Brushes.Gray;

            AdvSortDirToggle = "Ascending";
            SelSortIndex = 0;

            AdvPageSizeText = "All";

            ResultsPerPage = 0;

            PrevVisibility = Visibility.Hidden;
            NextVisibility = Visibility.Hidden;

            AdvPagingText = "Page: 1 of 1";

            ContSearchToggleText = "Off";
            _continuousSearch = false;

            AdvCustText = string.Empty;

            _isAdvSearchOn = false;
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

        private IEnumerable<Order> QueryService()
        {
            if (_query == _queryOptions["custFilteredSortedAndPaged"])
                        return _service.GetAllByCustomerIdFilteredSortedAndPaged
                            (_custId, SearchText, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["custFilteredAndSorted"])
                        return _service.GetAllByCustomerIdFilteredAndSorted
                            (_custId, SearchText, _sortBy, _desc);
            else if (_query == _queryOptions["custFilteredAndPaged"])
                        return _service.GetAllByCustomerIdFilteredAndPaged
                            (_custId, SearchText, _pgSize, _pgIndex);
            else if (_query == _queryOptions["custSortedAndPaged"])
                return _service.GetAllByCustomerIdSortedAndPaged(_custId, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["custAndFiltered"])
                        return _service.GetAllByCustomerIdAndFiltered(_custId, SearchText);
            else if (_query == _queryOptions["custAndPaged"])
                return _service.GetAllByCustomerIdAndPaged(_custId, _pgSize, _pgIndex);
            else if (_query == _queryOptions["custAndSorted"])
                return _service.GetAllByCustomerIdAndSorted(_custId, _sortBy, _desc);
            else if (_query == _queryOptions["filterSortAndPage"])
                        return _service.GetAllFilteredSortedAndPaged(SearchText, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["sortAndPage"])
                return _service.GetAllSortedAndPaged(_pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["filterAndPage"])
                        return _service.GetAllFilteredAndPaged(SearchText, _pgSize, _pgIndex);
            else if (_query == _queryOptions["page"])
                return _service.GetAllPaginated(_pgSize, _pgIndex);
            else if (_query == _queryOptions["filterAndSort"])
                        return _service.GetAllFilteredAndSorted(SearchText, _sortBy, _desc);
            else if (_query == _queryOptions["sort"])
                return _service.GetAllSorted(_sortBy, _desc);
            else if (_query == _queryOptions["filter"])
                        return _service.GetAllFiltered(SearchText);
            else if (_query == _queryOptions["cust"])
                return _service.GetAllByCustomerId(_custId);
            else  return _service.GetAll();
        }

        private void ExtractAdvValues()
        {
            _sortBy = null;
            switch (SelSortIndex)
            {
                case 1:
                    _sortBy = "custId";
                    break;
                case 2:
                    _sortBy = "ordDate";
                    break;
                case 3:
                    _sortBy = "shipDate";
                    break;
                case 4:
                    _sortBy = "stat";
                    break;
                case 5:
                    _sortBy = "shipMeth";
                    break;
                case 6:
                    _sortBy = "payMeth";
                    break;
                default:
                    _sortBy = "id";
                    break;
            }

            _custId = 0;
            var custIdParses = int.TryParse(AdvCustText, out _custId);
            if (_custId == 0 || !custIdParses) _query.isCustFiltered = false;

            _pgSize = AdvPageSizeText == "All" ? 0 : int.Parse(AdvPageSizeText);

            _desc = AdvSortDirToggle == "Descending";
        }

        #endregion

        #endregion
    }
}
