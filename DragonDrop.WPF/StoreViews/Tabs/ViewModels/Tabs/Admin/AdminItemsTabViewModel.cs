using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AdminItemsTabViewModel : BindableBase
    {
        #region Fields

        #region ItemDetails

        private Visibility _shipTimeVis;

        private OrderItem _selItem;

        private string _orderIdLabel;
        private string _productIdLabel;

        private string _orderDateLbl;
        private string _shippingDateLbl;
        private int _orderStatusIdLbl;
        private int _shippingMethodIdLbl;
        private int _paymentMethodIdLbl;

        private string _prodNameLbl;
        private int _categoryIdLbl;
        private string _unitPriceLbl;
        private int _stockLbl;
        private string _descLbl;
        private string _barCodeLbl;
        private string _manufacturerLbl;

        #endregion

        private IOrderItemDataService _service;
        private IOrderDataService _ordService;
        private IProductDataService _prodService;
        private ObservableCollection<OrderItem> _itemList;

        private string _idText;
        private string _idToggleText;

        private string _advSortDirToggle;

        private int _selSortIndex;

        private string _advPageSizeText;

        private int _resultsPerPage;

        private Visibility _prevVisibility;
        private Visibility _nextVisibility;

        private string _advPagingText;

        private int _pgCount = 1;
        private int _pgIndex = 1;

        private Dictionary<string, (bool, bool, bool, bool)> _queryOptions =
        new Dictionary<string, (bool, bool, bool, bool)>
        {
            { "prodSortedAndPaged", (false, true, true, true) },
            { "prodAndSorted", (false, true, true, false)},
            { "prodAndPaged", (false, true, false, true)},
            { "ordSortedAndPaged", (true, false, true, true)},
            { "ordAndSorted", (true, false, true, false)},
            { "ordAndPaged", (true, false, false, true)},
            { "prod", (false, true, false, false)},
            { "ord", (true, false, false, false)},
            { "sortedAndPaged", (false, false, true, true)},
            { "page", (false, false, false, true)},
            { "sort", (false, false, true, false)},
            { "none", (false, false, false, false) }
        };

        private int _id;
        private int _pgSize;
        private bool _desc;
        private string _sortBy;

        private (bool isOrdFiltered, bool isProdFiltered, bool isAdvSort, bool isPaged) _query;

        #endregion

        public AdminItemsTabViewModel
            (IOrderItemDataService service, IOrderDataService ordService, IProductDataService prodService)
        {
            _service = service;
            _ordService = ordService;
            _prodService = prodService;

            #region Commands

            UpdateDetailsCommand = new DelegateCommand(UpdateDetailsCommandExecute);

            IdTextChangedCommand = new DelegateCommand(IdTextChangedCommandExecute);

            SearchCommand = new DelegateCommand(SearchCommandExecute);
            ResetCommand = new DelegateCommand(ResetCommandExecute);

            PrevCommand = new DelegateCommand(PrevCommandExecute);
            NextCommand = new DelegateCommand(NextCommandExecute);

            #endregion

            // Init
            ResetCommandExecute();
        }

        #region Properties

        #region ViewModel Props
        
        #region ItemDetails

        public Visibility ShipTimeVis
        {
            get => _shipTimeVis;
            set => SetProperty(ref _shipTimeVis, value);
        }

        public OrderItem SelItem
        {
            get => _selItem;
            set => SetProperty(ref _selItem, value);
        }

        public string OrderIdLabel
        {
            get => _orderIdLabel;
            set => SetProperty(ref _orderIdLabel, value);
        }

        public string ProductIdLabel
        {
            get => _productIdLabel;
            set => SetProperty(ref _productIdLabel, value);
        }

        public string OrderDateLbl
        {
            get=> _orderDateLbl;
            set=>SetProperty(ref _orderDateLbl, value);
        }

        public string ShippingDateLbl
        {
            get => _shippingDateLbl;
            set => SetProperty(ref _shippingDateLbl, value);
        }

        public int OrderStatusIdLbl
        {
            get => _orderStatusIdLbl;
            set => SetProperty(ref _orderStatusIdLbl, value);
        }

        public int ShippingMethodIdLbl
        {
            get => _shippingMethodIdLbl;
            set => SetProperty(ref _shippingMethodIdLbl, value);
        }

        public int PaymentMethodIdLbl
        {
            get => _paymentMethodIdLbl;
            set => SetProperty(ref _paymentMethodIdLbl, value);
        }

        public string ProdNameLbl
        {
            get => _prodNameLbl;
            set => SetProperty(ref _prodNameLbl, value);
        }

        public int CategoryIdLbl
        {
            get => _categoryIdLbl;
            set => SetProperty(ref _categoryIdLbl, value);
        }

        public string UnitPriceLbl
        {
            get => _unitPriceLbl;
            set => SetProperty(ref _unitPriceLbl, value);
        }

        public int StockLbl
        {
            get => _stockLbl;
            set => SetProperty(ref _stockLbl, value);
        }

        public string DescLbl
        {
            get => _descLbl;
            set => SetProperty(ref _descLbl, value);
        }

        public string BarCodeLbl
        {
            get => _barCodeLbl;
            set => SetProperty(ref _barCodeLbl, value);
        }

        public string ManufacturerLbl
        {
            get => _manufacturerLbl;
            set => SetProperty(ref _manufacturerLbl, value);
        }

        #endregion

        public ObservableCollection<OrderItem> ItemList
        {
            get => _itemList;
            set => SetProperty(ref _itemList, value);
        }

        public string IdToggleText
        {
            get => _idToggleText;
            set => SetProperty(ref _idToggleText, value);
        }

        public string AdvPagingText
        {
            get => _advPagingText;
            set => SetProperty(ref _advPagingText, value);
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
            get => new string[] { "Order ID", "Product ID", "Quantity" };
        }

        public string AdvSortDirToggle
        {
            get => _advSortDirToggle;
            set => SetProperty(ref _advSortDirToggle, value);
        }

        public string IdText
        {
            get => _idText;
            set => SetProperty(ref _idText, value);
        }
        
        #endregion

        #region Commands

        public DelegateCommand UpdateDetailsCommand { get; }

        public DelegateCommand IdTextChangedCommand { get; }

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand ResetCommand { get; }

        public DelegateCommand PrevCommand { get; }
        public DelegateCommand NextCommand { get; }

        #endregion

        #endregion

        #region Private/Not-Exposed Methods

        #region Commands

        private void IdTextChangedCommandExecute()
        {
            if (IdText.Length > 9) IdText = IdText.Substring(0, 9);
        }

        private void SearchCommandExecute() => SearchCommandExecute(false);

        private void SearchCommandExecute(bool isNavReq)
        {
            _query = (IdToggleText=="Order ID",
                    IdToggleText == "Product ID",
                    SelSortIndex != 0 ||
                        (SelSortIndex == 0
                        && AdvSortDirToggle == "Descending"),
                    AdvPageSizeText != "All");

            if (_query==_queryOptions["none"]) ItemList = new ObservableCollection<OrderItem>(_service.GetAll());

            else
            {
                if (!isNavReq) _pgIndex = 1;
                ExtractAdvValues();

                ItemList = new ObservableCollection<OrderItem>(QueryService());

                if (_query.isPaged)
                {
                    _query.isPaged = false;
                    var totalItems = new List<OrderItem>(QueryService()).Count;
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

        private void ResetCommandExecute()
        {
            ItemList = new ObservableCollection<OrderItem>(_service.GetAll().ToList());

            IdToggleText = "Order ID";

            AdvSortDirToggle = "Ascending";
            SelSortIndex = 0;

            AdvPageSizeText = "All";

            ResultsPerPage = 0;

            PrevVisibility = Visibility.Hidden;
            NextVisibility = Visibility.Hidden;

            AdvPagingText = "Page: 1 of 1";

            IdText = string.Empty;

            SelItem = null;
        }

        private void UpdateDetailsCommandExecute()
        {
            if(SelItem==null)
            {
                OrderIdLabel = string.Empty;
                ProductIdLabel = string.Empty;

                OrderDateLbl = string.Empty;
                ShippingDateLbl = string.Empty;
                OrderStatusIdLbl = 0;
                ShippingMethodIdLbl = 0;
                PaymentMethodIdLbl = 0;

                ProdNameLbl = string.Empty;
                CategoryIdLbl = 0;
                UnitPriceLbl = string.Empty;
                StockLbl = 0;
                DescLbl = string.Empty;
                BarCodeLbl = string.Empty;
                ManufacturerLbl = string.Empty;
                ShipTimeVis = Visibility.Hidden;

                return;
            }

            var ord = _ordService.Get(SelItem.OrderId);
            var prod = _prodService.Get(SelItem.ProductId);

            OrderIdLabel = ord.OrderId.ToString();
            ProductIdLabel = prod.ProductId.ToString();

            OrderDateLbl=ord.OrderDate.ToShortDateString();
            ShippingDateLbl = ord.ShippingDate.GetValueOrDefault().ToShortDateString();
            OrderStatusIdLbl=ord.OrderStatusId;
            ShippingMethodIdLbl=ord.ShippingMethodId;
            PaymentMethodIdLbl=ord.PaymentMethodId;

            ProdNameLbl = prod.Name;
            CategoryIdLbl = prod.CategoryId.GetValueOrDefault();
            UnitPriceLbl = string.Format("{0:C}",prod.UnitPrice.GetValueOrDefault());
            StockLbl = prod.Stock;
            DescLbl = prod.Description;
            BarCodeLbl = prod.BarCode.Substring(0,1) + " " +
                        prod.BarCode.Substring(1,5)+" "+
                        prod.BarCode.Substring(6,5)+" "+
                        prod.BarCode.Substring(11,1);
            ManufacturerLbl = prod.Manufacturer;
            ShipTimeVis = ShippingDateLbl == "1/1/0001" ? Visibility.Hidden : Visibility.Visible;
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

        private IEnumerable<OrderItem> QueryService()
        {
            if (_query == _queryOptions["ordSortedAndPaged"])
                return _service.GetAllByOrderIdSortedAndPaged(_id, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["prodSortedAndPaged"])
                return _service.GetAllByProductIdSortedAndPaged(_id, _pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["ordAndSorted"])
                return _service.GetAllByOrderIdAndSorted(_id, _sortBy, _desc);
            else if (_query == _queryOptions["prodAndSorted"])
                return _service.GetAllByProductIdAndSorted(_id, _sortBy, _desc);
            else if (_query == _queryOptions["ordAndPaged"])
                return _service.GetAllByOrderIdAndPaged(_id, _pgSize, _pgIndex);
            else if (_query == _queryOptions["prodAndPaged"])
                return _service.GetAllByProductIdAndPaged(_id, _pgSize, _pgIndex);
            else if (_query == _queryOptions["prod"])
                return _service.GetAllByProductId(_id);
            else if (_query == _queryOptions["ord"])
                return _service.GetAllByOrderId(_id);
            else if (_query == _queryOptions["sortedAndPaged"])
                return _service.GetAllSortedAndPaged(_pgSize, _pgIndex, _sortBy, _desc);
            else if (_query == _queryOptions["page"])
                return _service.GetAllPaginated(_pgSize, _pgIndex);
            else if (_query == _queryOptions["sort"])
                return _service.GetAllSorted(_sortBy, _desc);
            else return _service.GetAll();
        }

        private void ExtractAdvValues()
        {
            _sortBy = null;
            switch (SelSortIndex)
            {
                case 1:
                    _sortBy = "prodId";
                    break;
                case 2:
                    _sortBy = "qty";
                    break;
                default:
                    _sortBy = "ordId";
                    break;
            }

            _id = -1;
            var idParses = int.TryParse(IdText, out _id);
            if (_id == -1 || !idParses)
            {
                _query.isOrdFiltered = false;
                _query.isProdFiltered = false;
            }

            _pgSize = AdvPageSizeText == "All" ? 0 : int.Parse(AdvPageSizeText);

            _desc = AdvSortDirToggle == "Descending";
        }

        #endregion
                
        #endregion
    }
}
