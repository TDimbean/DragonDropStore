using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class OrderEditViewModel : BindableBase
    {
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private IOrderDataService _service;
        private IPaymentMethodRepository _methRepo;
        private IShippingMethodRepository _shipRepo;
        private IOrderStatusRepository _statRepo;
        private Order _ogOrd;

        private string _defError = "This shouldn't be visible.";

        #region CustomerId

        private string _custEntryText;
        private string _custErrorText;
        private Visibility _custErrorVisibility;

        #endregion

        private int _payMethSelIndex;
        private int _shipMethSelIndex;
        private int _statSelIndex;

        #region Order Date

        private string _ordDateEntryText;
        private string _ordErrorText;
        private Visibility _ordErrorVisibility;
        private DateTime _selectedOrdDate;
        private DateTime _ordDispDate;
        private Visibility _ordCalVisibility;

        #endregion

        #region Shipping Date

        private string _shipDateEntryText;
        private string _shipErrorText;
        private Visibility _shipErrorVisibility;
        private DateTime _selectedShipDate;
        private DateTime _shipDispDate;
        private Visibility _shipCalVisibility;

        #endregion

        #endregion

        private IProductDataService _prodServ;
        private IOrderItemDataService _itemServ;
        private ISelfAndRelayReloadAndRemoteCloseControl _callingView;

        public OrderEditViewModel(IOrderDataService service, IPaymentMethodRepository methRepo,
                                IShippingMethodRepository shipRepo, IOrderStatusRepository statRepo,
                                int ogOrdId, IProductDataService prodServ, IOrderItemDataService itemServ, 
                                ISelfAndRelayReloadAndRemoteCloseControl callingView= null)
        {
            _service = service;
            _methRepo = methRepo;
            _shipRepo = shipRepo;
            _statRepo = statRepo;
            if (_asyncCalls) SetOgAsync(ogOrdId);
            else _ogOrd = _service.Get(ogOrdId);

            _prodServ = prodServ;
            _itemServ = itemServ;
            _callingView = callingView;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync) :
                new DelegateCommand(SubmitCommandExecute);

            #region CustomerId

            CustBoxLostFocusCommand = new DelegateCommand(CustBoxLostFocusCommandExecute);
            CustBoxTextChangedCommand = new DelegateCommand(CustBoxTextChangedCommandExecute);
            CustResetCommand = new DelegateCommand(CustResetCommandExecute);

            #endregion

            #region Order Date

            OrdDateBoxLostFocusCommand = new DelegateCommand(OrdDateBoxLostFocusCommandExecute);
            OrdDateBoxTextChangedCommand = new DelegateCommand(OrdDateBoxTextChangedCommandExecute);
            OrdDateResetCommand = new DelegateCommand(OrdDateResetCommandExecute);

            OrdDatePickerSelChangedCommand = new DelegateCommand(OrdDatePickerSelChangedCommandExecute);
            ToggleOrdCalendarCommand = new DelegateCommand(ToggleOrdCalendarCommandExecute);

            #endregion

            #region Shipping Date

            ShipDateBoxLostFocusCommand = new DelegateCommand(ShipDateBoxLostFocusCommandExecute);
            ShipDateBoxTextChangedCommand = new DelegateCommand(ShipDateBoxTextChangedCommandExecute);
            ShipDateResetCommand = new DelegateCommand(ShipDateResetCommandExecute);

            ShipDatePickerSelChangedCommand = new DelegateCommand(ShipDatePickerSelChangedCommandExecute);
            ToggleShipCalendarCommand = new DelegateCommand(ToggleShipCalendarCommandExecute);

            #endregion

            PayMethResetCommand = new DelegateCommand(PayMethResetCommandExecute);
            ShipMethResetCommand = new DelegateCommand(ShipMethResetCommandExecute);
            StatResetCommand = new DelegateCommand(StatResetCommandExecute);

            #endregion

            #region Init

            PayMethods = new List<PaymentMethod>(methRepo.GetAll());
            ShipMethods = new List<ShippingMethod>(shipRepo.GetAll());
            Statuses = new List<OrderStatus>(statRepo.GetAll());

            #region CustomerId

            CustEntryText = _ogOrd.CustomerId.ToString();
            CustErrorText = _defError;
            CustErrorVisibility = Visibility.Collapsed;

            #endregion

            PayMethSelIndex = _ogOrd.PaymentMethodId - 1;
            ShipMethSelIndex = _ogOrd.ShippingMethodId - 1;
            StatusSelIndex = _ogOrd.OrderStatusId;

            #region Order Date

            SelectedOrdDate = _ogOrd.OrderDate;
            OrdDispDate = SelectedOrdDate;
            OrdDateEntryText = SelectedOrdDate.ToShortDateString();
            OrdErrorText = _defError;
            OrdErrorVisibility = Visibility.Collapsed;
            OrdCalVisibility = Visibility.Collapsed;

            #endregion

            #region Shipping Date

            ShipErrorText = _defError;
            ShipErrorVisibility = Visibility.Collapsed;
            ShipCalVisibility = Visibility.Collapsed;
            if (_ogOrd.ShippingDate == null)
            {
                SelectedShipDate = DateTime.Now.AddDays(-10);
            ShipDispDate = SelectedShipDate;
                return;
            }
            SelectedShipDate = _ogOrd.ShippingDate.GetValueOrDefault();
            ShipDispDate = SelectedShipDate;
            ShipDateEntryText = SelectedShipDate.ToShortDateString();

            #endregion

            #endregion
        }

        private async void SetOgAsync(int ogOrdId)
        => _ogOrd = await _service.GetAsync(ogOrdId);

        #region Properties

        #region ViewModelProps

        public Visibility ProcOptionVis
        { get => _ogOrd.OrderStatusId == 0 ? Visibility.Visible : Visibility.Collapsed; }

        public Visibility ShipOptionVis
        { get => _ogOrd.OrderStatusId == 1 ? Visibility.Visible : Visibility.Collapsed; }

        #region CustomerIdEntry

        public string CustEntryText
        {
            get => _custEntryText;
            set => SetProperty(ref _custEntryText, value);
        }

        public string CustErrorText
        {
            get => _custErrorText;
            set => SetProperty(ref _custErrorText, value);
        }

        public Visibility CustErrorVisibility
        {
            get => _custErrorVisibility;
            set => SetProperty(ref _custErrorVisibility, value);
        }

        #endregion

        #region Combos

        public List<PaymentMethod> PayMethods { get; set; }
        public List<ShippingMethod> ShipMethods { get; set; }
        public List<OrderStatus> Statuses { get; set; }

        public PaymentMethod SelectedPayMethod { get; set; }
        public ShippingMethod SelectedShipMethod { get; set; }
        public OrderStatus SelectedStatus { get; set; }

        public int PayMethSelIndex
        {
            get => _payMethSelIndex;
            set => SetProperty(ref _payMethSelIndex, value);
        }

        public int ShipMethSelIndex
        {
            get => _shipMethSelIndex;
            set => SetProperty(ref _shipMethSelIndex, value);
        }

        public int StatusSelIndex
        {
            get => _statSelIndex;
            set => SetProperty(ref _statSelIndex, value);
        }

        #endregion

        #region Order DateEntry

        public string OrdDateEntryText
        {
            get => _ordDateEntryText;
            set => SetProperty(ref _ordDateEntryText, value);
        }

        public string OrdErrorText
        {
            get => _ordErrorText;
            set => SetProperty(ref _ordErrorText, value);
        }

        public Visibility OrdErrorVisibility
        {
            get => _ordErrorVisibility;
            set => SetProperty(ref _ordErrorVisibility, value);
        }

        public DateTime SelectedOrdDate
        {
            get => _selectedOrdDate;
            set => SetProperty(ref _selectedOrdDate, value);
        }

        public DateTime OrdDispDate
        {
            get => _ordDispDate;
            set => SetProperty(ref _ordDispDate, value);
        }

        public Visibility OrdCalVisibility
        {
            get => _ordCalVisibility;
            set => SetProperty(ref _ordCalVisibility, value);
        }

        #endregion

        #region Shipping DateEntry

        public string ShipDateEntryText
        {
            get => _shipDateEntryText;
            set => SetProperty(ref _shipDateEntryText, value);
        }

        public string ShipErrorText
        {
            get => _shipErrorText;
            set => SetProperty(ref _shipErrorText, value);
        }

        public Visibility ShipErrorVisibility
        {
            get => _shipErrorVisibility;
            set => SetProperty(ref _shipErrorVisibility, value);
        }

        public DateTime SelectedShipDate
        {
            get => _selectedShipDate;
            set => SetProperty(ref _selectedShipDate, value);
        }

        public DateTime ShipDispDate
        {
            get => _shipDispDate;
            set => SetProperty(ref _shipDispDate, value);
        }

        public Visibility ShipCalVisibility
        {
            get => _shipCalVisibility;
            set => SetProperty(ref _shipCalVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }
       
        #region Customer ID

        public DelegateCommand CustBoxLostFocusCommand { get; }
        public DelegateCommand CustBoxTextChangedCommand { get; }
        public DelegateCommand CustResetCommand { get; }

        #endregion

        public DelegateCommand PayMethResetCommand { get; }
        public DelegateCommand ShipMethResetCommand { get; }
        public DelegateCommand StatResetCommand { get; }

        #region Order Date

        public DelegateCommand OrdDateBoxLostFocusCommand { get; }
        public DelegateCommand OrdDateBoxTextChangedCommand { get; }
        public DelegateCommand OrdDateResetCommand { get; }

        public DelegateCommand OrdDatePickerSelChangedCommand { get; }
        public DelegateCommand ToggleOrdCalendarCommand { get; }

        #endregion

        #region Shipping Date

        public DelegateCommand ShipDateBoxLostFocusCommand { get; }
        public DelegateCommand ShipDateBoxTextChangedCommand { get; }
        public DelegateCommand ShipDateResetCommand { get; }

        public DelegateCommand ShipDatePickerSelChangedCommand { get; }
        public DelegateCommand ToggleShipCalendarCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private async void SubmitCommandExecuteAsync()
        {
            #region Converting Field Entry Data

            if (SelectedPayMethod == null) SelectedPayMethod = new PaymentMethod { PaymentMethodId = -1, Name = "Error" };
            if (SelectedShipMethod == null) SelectedShipMethod = new ShippingMethod { ShippingMethodId = -1, Name = "Error" };
            if (SelectedStatus == null) SelectedStatus = new OrderStatus { OrderStatusId = -1, Name = "Error" };

            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = _ogOrd.CustomerId;

            var shipString = ShipDateEntryText;
            var shipDate = new DateTime();
            var shipParses = DateTime.TryParse(shipString, out shipDate);
            
            #endregion

            var ord = new Order
            {
                OrderId = _ogOrd.OrderId,
                CustomerId = custId,
                PaymentMethodId = SelectedPayMethod.PaymentMethodId == -1 ?
                    _ogOrd.PaymentMethodId : SelectedPayMethod.PaymentMethodId,
                ShippingMethodId = SelectedShipMethod.ShippingMethodId == -1 ?
                    _ogOrd.ShippingMethodId : SelectedShipMethod.ShippingMethodId,
                OrderStatusId = SelectedStatus.OrderStatusId == -1 ?
                    _ogOrd.OrderStatusId : SelectedStatus.OrderStatusId,
                OrderDate = SelectedOrdDate,
                ShippingDate = shipParses ? SelectedShipDate : (DateTime?)null
            };

            var val = _service.ValidateOrder(ord);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Order Update from WPF form. Payment ID: " + _ogOrd.OrderId);
                await _service.UpdateAsync(ord);
                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList.Trim();
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Update Order", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SubmitCommandExecute()
        {
            #region Converting Field Entry Data

            if (SelectedPayMethod == null) SelectedPayMethod = new PaymentMethod { PaymentMethodId = -1, Name = "Error" };
            if (SelectedShipMethod == null) SelectedShipMethod = new ShippingMethod { ShippingMethodId = -1, Name = "Error" };
            if (SelectedStatus == null) SelectedStatus = new OrderStatus { OrderStatusId = -1, Name = "Error" };

            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = _ogOrd.CustomerId;

            var shipString = ShipDateEntryText;
            var shipDate = new DateTime();
            var shipParses = DateTime.TryParse(shipString, out shipDate);
            #endregion

            var ord = new Order
            {
                OrderId = _ogOrd.OrderId,
                CustomerId = custId,
                PaymentMethodId = SelectedPayMethod.PaymentMethodId == -1 ?
                    _ogOrd.PaymentMethodId : SelectedPayMethod.PaymentMethodId,
                ShippingMethodId = SelectedShipMethod.ShippingMethodId == -1 ?
                    _ogOrd.ShippingMethodId : SelectedShipMethod.ShippingMethodId,
                OrderStatusId = SelectedStatus.OrderStatusId == -1 ?
                    _ogOrd.OrderStatusId : SelectedStatus.OrderStatusId,
                OrderDate = SelectedOrdDate,
                ShippingDate = shipParses ? SelectedShipDate : (DateTime?)null
            };

            var val = _service.ValidateOrder(ord);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Order Update from WPF form. Payment ID: " + _ogOrd.OrderId);
                _service.Update(ord);
                if (_callingView== null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList.Trim();
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Update Order", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #region CustomerId

        private void CustBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(CustEntryText)) CustResetCommandExecute();
        }

        private void CustBoxTextChangedCommandExecute()
        {
            var custId = 0;
            var idParses = int.TryParse(CustEntryText, out custId);

            if (!idParses)
            {
                CustErrorText = "Invalid Customer ID format";
                CustErrorVisibility = Visibility.Visible;
                return;
            }

            var validation = _service.ValidateCustomerId(custId);
            if (!validation.isValid)
            {
                CustErrorText = validation.errorList.GetUntilOrEmpty(".");
                CustErrorVisibility = Visibility.Visible;
            }
            else
            {
                CustErrorText = _defError;
                CustErrorVisibility = Visibility.Collapsed;
            }
        }

        private void CustResetCommandExecute()
            => CustEntryText = _ogOrd.CustomerId.ToString();

        #endregion

        private void PayMethResetCommandExecute()
            => PayMethSelIndex = _ogOrd.PaymentMethodId - 1;

        private void ShipMethResetCommandExecute()
            => ShipMethSelIndex = _ogOrd.ShippingMethodId - 1;

        private void StatResetCommandExecute()
            => StatusSelIndex = _ogOrd.OrderStatusId;

        #region Order Date

        private void OrdDateBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(OrdDateEntryText))
            {
                OrdDateResetCommandExecute();
                return;
            }

            var val = new DateTime();
            var parses = DateTime.TryParse(OrdDateEntryText, out val);
            if (parses)
            {
                OrdDateEntryText = val.ToShortDateString();
                SelectedOrdDate = val;
                OrdDispDate = SelectedOrdDate;
            }
        }

        private void OrdDateBoxTextChangedCommandExecute()
        {
            DateTime val;
            var parses = DateTime.TryParse(OrdDateEntryText, out val);
            if(!parses)
            {
                OrdErrorText = "Please use a valid date format!";
                OrdErrorVisibility = Visibility.Visible;
                return;
            }

            var validation = _service.ValidateOrderDate(val);
            if (!validation.isValid)
            {
                OrdErrorText = validation.errorList.GetUntilOrEmpty(".");
                OrdErrorVisibility = Visibility.Visible;
            }
            else
            {
                OrdErrorText = _defError;
                OrdErrorVisibility = Visibility.Collapsed;
            }

        }

        private void OrdDatePickerSelChangedCommandExecute()
        {
            var newDate = SelectedOrdDate.ToShortDateString();
            OrdDispDate = SelectedOrdDate;

            if (OrdDateEntryText == newDate) return;
            OrdDateEntryText = newDate;
        }

        private void ToggleOrdCalendarCommandExecute()
        => OrdCalVisibility = OrdCalVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        private void OrdDateResetCommandExecute()
        {
            SelectedOrdDate = _ogOrd.OrderDate;
            OrdDispDate = SelectedOrdDate;
            OrdDateEntryText = SelectedOrdDate.ToShortDateString();
        }

        #endregion

        #region Shipping Date

        private void ShipDateBoxLostFocusCommandExecute()
        {
            var val = new DateTime();
            var parses = DateTime.TryParse(ShipDateEntryText, out val);
            if (parses)
            {
                ShipDateEntryText = val.ToShortDateString();
                SelectedShipDate = val;
                ShipDispDate = SelectedShipDate;
            }
        }

        private void ShipDateBoxTextChangedCommandExecute()
        {
            DateTime ordVal;
            var parses = DateTime.TryParse(OrdDateEntryText, out ordVal);
            if (!parses) return;

            DateTime val;
            parses = DateTime.TryParse(ShipDateEntryText, out val);
            var checkVal = !parses ? (DateTime?)null : val;

            var datesVal = _service.ValidateShippingDate(checkVal, ordVal);
            var statVal = _service.ValidateShipWithStatus(checkVal, StatusSelIndex);

            var isValid = datesVal.isValid && statVal.isValid;
            var errorList = new StringBuilder(datesVal.errorList).AppendLine(statVal.errorList).ToString();

            if (!isValid)
            {
                ShipErrorText = errorList.GetUntilOrEmpty(".");
                ShipErrorVisibility = Visibility.Visible;
            }
            else
            {
                ShipErrorText = _defError;
                ShipErrorVisibility = Visibility.Collapsed;
            }

        }

        private void ShipDatePickerSelChangedCommandExecute()
        {
            var newDate = SelectedShipDate.ToShortDateString();
            ShipDispDate = SelectedShipDate;

            if (ShipDateEntryText == newDate) return;
            ShipDateEntryText = newDate;
        }

        private void ToggleShipCalendarCommandExecute()
        => ShipCalVisibility = ShipCalVisibility == Visibility.Visible ?
            Visibility.Collapsed : Visibility.Visible;

        private void ShipDateResetCommandExecute()
        {
            if (_ogOrd.ShippingDate == null) return;
            SelectedShipDate = _ogOrd.ShippingDate.GetValueOrDefault();
            ShipDispDate = SelectedShipDate;
            ShipDateEntryText = SelectedShipDate.ToShortDateString();
        }

        #endregion

        #endregion
    }
}
