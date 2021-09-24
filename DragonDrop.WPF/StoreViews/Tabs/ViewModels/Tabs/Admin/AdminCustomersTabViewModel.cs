using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AdminCustomersTabViewModel : BindableBase
    {
        private string _visibleDetail;

        #region Fields

        private ICustomerDataService _service;
        private List<Customer> _customerList;

        private string _searchText;
        private Brush _searchColour;
        private FontStyle _searchStyle;

        private string _searchFont;
        private string _searchPlaceholder = "Showing all entries. Search...";

        private Visibility _searchFilterVisibility;

        private int _selFilterIndex;

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

        private int _pgSize;
        private bool _desc;
        private string _sortBy;

        private (bool isAdvFilter, bool isAdvSort, bool isPaged) _query;

        #endregion

        public AdminCustomersTabViewModel(ICustomerDataService service)
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

            VisibleDetail = "Email";
        }

        #region Properties

        #region ViewModel Props

        public string VisibleDetail
        {
            get => _visibleDetail;
            set => SetProperty(ref _visibleDetail, value);
        }

        public List<Customer> CustomerList
        {
            get => _customerList;
            set => SetProperty(ref _customerList, value);
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
            get => new string[] { "ID", "Name", "Phone", "Email", "Address", "City", "State" };
        }

        public string AdvSortDirToggle
        {
            get => _advSortDirToggle;
            set => SetProperty(ref _advSortDirToggle, value);
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
            _query = (!string.IsNullOrEmpty(SearchText)
                    && SearchText != _searchPlaceholder,
                SelSortIndex != 0 ||
                    (SelSortIndex == 0
                    && AdvSortDirToggle == "Descending"),
                AdvPageSizeText != "All");

        if (_query==_queryOptions["none"]) CustomerList = SearchText == _searchPlaceholder ?
                    new List<Customer>(_service.GetAll()) :
                    new List<Customer>(_service.GetAllFiltered(SearchText));

        else
        {

            if (!isNavReq) _pgIndex = 1;
            ExtractAdvValues();

            CustomerList = new List<Customer>(QueryService());

                if (_query.isPaged)
                {
                    _query.isPaged = false;
                    var totalItems = new List<Customer>(QueryService()).Count;
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
        CustomerList = new List<Customer>(_service.GetAll());

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

    private IEnumerable<Customer> QueryService()
    {
        if (_query == _queryOptions["filterSortAndPage"])
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
        else return _service.GetAll();
    }

    private void ExtractAdvValues()
    {
        _sortBy = null;
        switch (SelSortIndex)
        {
            case 1:
                _sortBy = "name";
                break;
            case 2:
                _sortBy = "phone";
                break;
            case 3:
                _sortBy = "email";
                break;
            case 4:
                _sortBy = "adr";
                break;
            case 5:
                _sortBy = "city";
                break;
            case 6:
                _sortBy = "state";
                break;
            default:
                _sortBy = "id";
                break;
        }

        _pgSize = AdvPageSizeText == "All" ? 0 : int.Parse(AdvPageSizeText);

        _desc = AdvSortDirToggle == "Descending";
    }

    #endregion

    #endregion
}
}
