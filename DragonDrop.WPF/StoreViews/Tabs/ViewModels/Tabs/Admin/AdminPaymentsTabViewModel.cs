using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AdminPaymentsTabViewModel : BindableBase
    {
        #region Fields

        private IPaymentDataService _service;
        private List<Payment> _paymentList;

        private string _searchText;
        private Brush _searchColour;
        private FontStyle _searchStyle;

        private string _searchFont;
        private string _searchPlaceholder = "Showing all entries. Search...";

        private Visibility _amountFilterVisibility;
        private Visibility _dateFilterVisibility;
        private Visibility _searchFilterVisibility;

        private string _amountToggleText;
        private string _dateToggleText;

        private bool _filterOver;
        private bool _filterBefore;

        private int _selFilterIndex;

        private string _advCustText;
        private string _advAmountText;
        private string _advDateText;

        private DateTime _advCalDate;

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
        private bool _over;
        private bool _before;
        private int _pgSize;
        private bool _desc;
        private string _sortBy;
        private decimal _amount;
        private DateTime _date;

        private (bool isCustFiltered, bool isAdvFilter, bool isAdvSort, bool isPaged) _query;

        #endregion

        public AdminPaymentsTabViewModel(IPaymentDataService service)
        {
            _service = service;

            #region Commands

            ToggleContinuousCommand = new DelegateCommand(ToggleContinuousCommandExecute);

            SearchGotFocusCommand = new DelegateCommand(SearchGotFocusCommandExecute);
            SearchLostFocusCommand = new DelegateCommand(SearchLostFocusCommandExecute);
            SearchTextChangedCommand = new DelegateCommand(SearchTextChangedCommandExecute);

            FilterSelChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(FilterSelChangedCommandExecute);

            AdvCustTextChangedCommand = new DelegateCommand(AdvCustTextChangedCommandExecute);
            AdvAmountTextChangedCommand = new DelegateCommand(AdvAmountTextChangedCommandExecute);

            AdvCalDateChangedCommand = new DelegateCommand(AdvCalDateChangedCommandExecute);

            AdvAmountLostFocusCommand = new DelegateCommand(AdvAmountLostFocusCommandExecute);
            AdvDateLostFocusCommand = new DelegateCommand(AdvDateLostFocusCommandExecute);

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

        public List<Payment> PaymentList
        {
            get => _paymentList;
            set => SetProperty(ref _paymentList, value);
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
            get => new string[] { "ID", "Customer ID", "Amount", "Date", "Payment Method" };
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

        public string AdvAmountText
        {
            get => _advAmountText;
            set => SetProperty(ref _advAmountText, value);
        }

        public string AdvDateText
        {
            get => _advDateText;
            set => SetProperty(ref _advDateText, value);
        }

        public DateTime AdvCalDate
        {
            get => _advCalDate;
            set => SetProperty(ref _advCalDate, value);
        }

        public int SelFilterIndex
        {
            get => _selFilterIndex;
            set => SetProperty(ref _selFilterIndex, value);
        }

        public string[] FilterOptions
        {
            get => new string[] { "Search", "Amount", "Date" };
        }

        public bool FilterBefore
        {
            get => _filterBefore;
            set => SetProperty(ref _filterBefore, value);
        }

        public bool FilterOver
        {
            get => _filterOver;
            set => SetProperty(ref _filterOver, value);
        }

        public string AmountToggleText
        {
            get => _amountToggleText;
            set => SetProperty(ref _amountToggleText, value);
        }

        public string DateToggleText
        {
            get => _dateToggleText;
            set => SetProperty(ref _dateToggleText, value);
        }

        public Visibility SearchFilterVisibility
        {
            get => _searchFilterVisibility;
            set => SetProperty(ref _searchFilterVisibility, value);
        }

        public Visibility AmountFilterVisibility
        {
            get => _amountFilterVisibility;
            set => SetProperty(ref _amountFilterVisibility, value);
        }

        public Visibility DateFilterVisibility
        {
            get => _dateFilterVisibility;
            set => SetProperty(ref _dateFilterVisibility, value);
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

        public DelegateCommand AdvCustTextChangedCommand { get; }
        public DelegateCommand AdvAmountTextChangedCommand { get; }
        public DelegateCommand AdvDateTextChangedCommand { get; }

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
            #region Close Calendar

            if(e!=null)
                (((((((e.Source as ComboBox).Parent as StackPanel).Parent as DockPanel)
                    .Parent as StackPanel).Parent as StackPanel).Parent as StackPanel)
                    .Parent as AdminPaymentsTab).HideAdvCal();

            #endregion

            SearchFilterVisibility = Visibility.Collapsed;
            AmountFilterVisibility = Visibility.Collapsed;
            DateFilterVisibility = Visibility.Collapsed;

            switch (SelFilterIndex)
            {
                case 1:
                    AmountFilterVisibility = Visibility.Visible;
                    break;
                case 2:
                    DateFilterVisibility = Visibility.Visible;
                    break;
                default:
                    SearchFilterVisibility = Visibility.Visible;
                    break;
            }
        }

        private void AdvCustTextChangedCommandExecute()
        {
            if (AdvCustText.Length > 9) AdvCustText = AdvCustText.Substring(0, 9);
        }

        private void AdvAmountTextChangedCommandExecute()
        {
            if (AdvAmountText.Length > 15) AdvAmountText = AdvAmountText.Substring(0, 15);
        }

        private void AdvCalDateChangedCommandExecute() => ChangeAdvDate(AdvCalDate);

        private void AdvAmountLostFocusCommandExecute()
        {
            var val = 0m;
            var parses = decimal.TryParse(AdvAmountText, out val);
            if (parses) AdvAmountText = string.Format("{0:C}", val);
        }

        private void AdvDateLostFocusCommandExecute()
        {
            var val = new DateTime();
            var parses = DateTime.TryParse(AdvDateText, out val);
            if (parses) ChangeAdvDate(val);
        }

        private void SearchCommandExecute() => SearchCommandExecute(false);

        private void SearchCommandExecute(bool isNavReq)
        {
            if (!_isAdvSearchOn) PaymentList = new List<Payment>(_service.GetAllFiltered(SearchText));

            else
            {
                _query = (!string.IsNullOrEmpty(AdvCustText),
                    SelFilterIndex != 0 || 
                        (SelFilterIndex == 0 && 
                        !string.IsNullOrEmpty(SearchText) 
                        && SearchText != _searchPlaceholder),
                    SelSortIndex != 0 || 
                        (SelSortIndex == 0 
                        && AdvSortDirToggle == "Descending"),
                    AdvPageSizeText != "All");

                if (!isNavReq) _pgIndex = 1;
                ExtractAdvValues();

                PaymentList = new List<Payment>(QueryService());

                if (_query.isPaged)
                {
                    _query.isPaged = false;
                    var totalItems = new List<Payment>(QueryService()).Count;
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
            PaymentList = new List<Payment>(_service.GetAll());

            SearchText = _searchPlaceholder;
            SearchFont = "Segoe UI Historic";
            SearchStyle = FontStyles.Italic;
            SearchColour = Brushes.Gray;

            SearchFilterVisibility = Visibility.Visible;
            DateFilterVisibility = Visibility.Collapsed;
            AmountFilterVisibility = Visibility.Collapsed;

            AmountToggleText = "Under: ";
            DateToggleText = "After: ";

            AdvAmountText = string.Empty;
            AdvDateText = string.Empty;

            SelFilterIndex = 0;
            AdvCalDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

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

        private void ChangeAdvDate(DateTime date)
        {
            if (AdvCalDate != date) AdvCalDate = date;
            if (_date.ToShortDateString() != AdvDateText) AdvDateText = date.ToShortDateString();
        }

        private IEnumerable<Payment> QueryService()
        {
            if (_query == _queryOptions["custFilteredSortedAndPaged"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllByCustomerIdFilteredSortedAndPaged
                            (_custId, _amount, _over, _pgSize, _pgIndex, _sortBy, _desc);
                    case 2:
                        return _service.GetAllByCustomerIdFilteredSortedAndPaged
                            (_custId, _date, _before, _pgSize, _pgIndex, _sortBy, _desc);
                    default:
                        return _service.GetAllByCustomerIdFilteredSortedAndPaged
                            (_custId, SearchText, _pgSize, _pgIndex, _sortBy, _desc);
                }
            else if (_query == _queryOptions["custFilteredAndSorted"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllByCustomerIdFilteredAndSorted
                            (_custId, _amount, _over, _sortBy, _desc);
                    case 2:
                        return _service.GetAllByCustomerIdFilteredAndSorted
                            (_custId, _date, _before, _sortBy, _desc);
                    default:
                        return _service.GetAllByCustomerIdFilteredAndSorted
                            (_custId, SearchText, _sortBy, _desc);
                }
            else if (_query == _queryOptions["custFilteredAndPaged"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllByCustomerIdFilteredAndPaged
                            (_custId, _amount, _over, _pgSize, _pgIndex);
                    case 2:
                        return _service.GetAllByCustomerIdFilteredAndPaged
                            (_custId, _date, _before, _pgSize, _pgIndex);
                    default:
                        return _service.GetAllByCustomerIdFilteredAndPaged
                            (_custId, SearchText, _pgSize, _pgIndex);
                }
            else if (_query == _queryOptions["custSortedAndPaged"])
                return _service.GetAllByCustomerIdSortedAndPaged(_custId, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["custAndFiltered"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllByCustomerIdAndFiltered(_custId, _amount, _over);
                    case 2:
                        return _service.GetAllByCustomerIdAndFiltered(_custId, _date, _before);
                    default:
                        return _service.GetAllByCustomerIdAndFiltered(_custId, SearchText);
                }
            else if (_query == _queryOptions["custAndPaged"])
                return _service.GetAllByCustomerIdAndPaged(_custId, _pgSize, _pgIndex);
            else if (_query == _queryOptions["custAndSorted"])
                return _service.GetAllByCustomerIdAndSorted(_custId, _sortBy, _desc);
            else if (_query == _queryOptions["filterSortAndPage"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredSortedAndPaged(_amount, _over, _pgSize, _pgIndex, _sortBy, _desc);
                    case 2:
                        return _service.GetAllFilteredSortedAndPaged(_date, _before, _pgSize, _pgIndex, _sortBy, _desc);
                    default:
                        return _service.GetAllFilteredSortedAndPaged(SearchText, _pgSize, _pgIndex, _sortBy, _desc);
                }
            else if (_query == _queryOptions["sortAndPage"])
                return _service.GetAllSortedAndPaged(_pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["filterAndPage"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredAndPaged(_amount, _over, _pgSize, _pgIndex);
                    case 2:
                        return _service.GetAllFilteredAndPaged(_date, _before, _pgSize, _pgIndex);
                    default:
                        return _service.GetAllFilteredAndPaged(SearchText, _pgSize, _pgIndex);
                }
            else if (_query == _queryOptions["page"])
                return _service.GetAllPaginated(_pgSize, _pgIndex);
            else if (_query == _queryOptions["filterAndSort"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFilteredAndSorted(_amount, _over, _sortBy, _desc);
                    case 2:
                        return _service.GetAllFilteredAndSorted(_date, _before, _sortBy, _desc);
                    default:
                        return _service.GetAllFilteredAndSorted(SearchText, _sortBy, _desc);
                }
            else if (_query == _queryOptions["sort"])
                return _service.GetAllSorted(_sortBy, _desc);
            else if (_query == _queryOptions["filter"])
                switch (SelFilterIndex)
                {
                    case 1:
                        return _service.GetAllFiltered(_amount, _over);
                    case 2:
                        return _service.GetAllFiltered(_date, _before);
                    default:
                        return _service.GetAllFiltered(SearchText);
                }
            else if (_query == _queryOptions["cust"])
                return _service.GetAllByCustomerId(_custId);
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
                    _sortBy = "custId";
                    break;
                case 2:
                    _sortBy = "amount";
                    break;
                case 3:
                    _sortBy = "date";
                    break;
                case 4:
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

            _amount = 0m;
            var amountParses = !string.IsNullOrEmpty(AdvAmountText) ?
            decimal.TryParse(AdvAmountText.Replace("$", string.Empty), out _amount) : false;

            _over = AmountToggleText == "Over: ";

            _date = new DateTime();
            var dateParses = DateTime.TryParse(AdvDateText, out _date);

            _before = DateToggleText == "Before: ";

            if ((SelFilterIndex == 2 && !dateParses) || (SelFilterIndex == 1 && !amountParses)) _query.isAdvFilter = false;
        }

        #endregion

        #endregion
    }

}
