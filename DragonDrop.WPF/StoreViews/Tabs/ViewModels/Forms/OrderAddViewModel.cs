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
    public class OrderAddViewModel : BindableBase
    {
        private IRelayReloadAndRemoteCloseControl _callingView;
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private IOrderDataService _service;
        private IPaymentMethodRepository _methRepo;
        private IShippingMethodRepository _shipRepo;
        private IOrderStatusRepository _statRepo;

        private string _defError = "This shouldn't be visible.";

        #region CustomerId

        private string _custEntryText;
        private string _custErrorText;
        private Visibility _custErrorVisibility;

        #endregion

        #region OrderDate

        private string _ordDateEntryText;
        private string _ordDateErrorText;
        private Visibility _ordDateErrorVisibility;
        private DateTime _selectedOrdDate;
        private DateTime _ordDispDate;
        private Visibility _ordCalVisibility;

        #endregion

        #region ShippingDate

        private string _shipDateEntryText;
        private string _shipDateErrorText;
        private Visibility _shipDateErrorVisibility;
        private DateTime? _selectedShipDate;
        private DateTime? _shipDispDate;
        private Visibility _shipCalVisibility;

        #endregion

        #endregion

        public OrderAddViewModel(IOrderDataService service, IPaymentMethodRepository methRepo,
            IShippingMethodRepository shipRepo, IOrderStatusRepository statRepo, IRelayReloadAndRemoteCloseControl callingView = null)
        {
            _callingView = callingView;
            _service = service;
            _methRepo = methRepo;
            _statRepo = statRepo;
            _shipRepo = shipRepo;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync) :
                new DelegateCommand(SubmitCommandExecute);

            #region CustomerId

            CustBoxGotFocusCommand = new DelegateCommand(CustBoxGotFocusCommandExecute);
            CustBoxLostFocusCommand = new DelegateCommand(CustBoxLostFocusCommandExecute);
            CustBoxTextChangedCommand = new DelegateCommand(CustBoxTextChangedCommandExecute);

            #endregion
            
            #region Order Date

            OrdDateBoxGotFocusCommand = new DelegateCommand(OrdDateBoxGotFocusCommandExecute);
            OrdDateBoxLostFocusCommand = new DelegateCommand(OrdDateBoxLostFocusCommandExecute);
            OrdDateBoxTextChangedCommand = new DelegateCommand(OrdDateBoxTextChangedCommandExecute);
            OrdDatePickerSelChangedCommand = new DelegateCommand(OrdDatePickerSelChangedCommandExecute);
            ToggleOrdCalendarCommand = new DelegateCommand(ToggleOrdCalendarCommandExecute);

            #endregion

            #region Shipping Date

            StatusSelChangedCommand = new DelegateCommand(StatusSelChangedCommandExecute);

            ShipDateBoxGotFocusCommand = new DelegateCommand(ShipDateBoxGotFocusCommandExecute);
            ShipDateBoxLostFocusCommand = new DelegateCommand(ShipDateBoxLostFocusCommandExecute);
            ShipDateBoxTextChangedCommand = new DelegateCommand(ShipDateBoxTextChangedCommandExecute);
            ShipDatePickerSelChangedCommand = new DelegateCommand(ShipDatePickerSelChangedCommandExecute);
            ToggleShipCalendarCommand = new DelegateCommand(ToggleShipCalendarCommandExecute);

            #endregion

            #endregion

            #region Init

            Statuses = new List<OrderStatus>(statRepo.GetAll());
            PayMethods = new List<PaymentMethod>(methRepo.GetAll());
            ShipMethods = new List<ShippingMethod>(shipRepo.GetAll());

            #region CustomerId

            CustEntryText = "Customer ID";
            CustErrorText = _defError;
            CustErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Order Date

            OrdDateEntryText = "Date Placed";
            OrdErrorText = _defError;
            OrdDispDate = DateTime.Now;
            OrdDateErrorVisibility = Visibility.Collapsed;
            OrdCalVisibility = Visibility.Collapsed;

            #endregion

            #region Shipping Date

            ShipDateEntryText = "Date Shipped";
            ShipErrorText = _defError;
            SelectedShipDate = null;
            ShipDispDate = null;
            ShipDateErrorVisibility = Visibility.Collapsed;
            ShipCalVisibility = Visibility.Collapsed;

            #endregion

            #endregion
        }
       
        #region Properties

        #region ViewModelProps

        public List<PaymentMethod> PayMethods { get; set; }
        public List<ShippingMethod> ShipMethods { get; set; }
        public List<OrderStatus> Statuses { get; set; }

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

        #region Order DateEntry

        public string OrdDateEntryText
        {
            get => _ordDateEntryText;
            set => SetProperty(ref _ordDateEntryText, value);
        }

        public string OrdErrorText
        {
            get => _ordDateErrorText;
            set => SetProperty(ref _ordDateErrorText, value);
        }

        public Visibility OrdDateErrorVisibility
        {
            get => _ordDateErrorVisibility;
            set => SetProperty(ref _ordDateErrorVisibility, value);
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
            get => _shipDateErrorText;
            set => SetProperty(ref _shipDateErrorText, value);
        }

        public Visibility ShipDateErrorVisibility
        {
            get => _shipDateErrorVisibility;
            set => SetProperty(ref _shipDateErrorVisibility, value);
        }

        public DateTime? SelectedShipDate
        {
            get => _selectedShipDate;
            set => SetProperty(ref _selectedShipDate, value);
        }

        public DateTime? ShipDispDate
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

        public PaymentMethod SelectedPayMethod { get; set; }
        public ShippingMethod SelectedShipMethod { get; set; }
        public OrderStatus SelectedStatus { get; set; }

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }

        #region CustomerId

        public DelegateCommand CustBoxGotFocusCommand { get; }
        public DelegateCommand CustBoxLostFocusCommand { get; }
        public DelegateCommand CustBoxTextChangedCommand { get; }

        #endregion

        #region Order Date

        public DelegateCommand OrdDateBoxGotFocusCommand { get; }
        public DelegateCommand OrdDateBoxLostFocusCommand { get; }
        public DelegateCommand OrdDateBoxTextChangedCommand { get; }
        public DelegateCommand OrdDatePickerSelChangedCommand { get; }
        public DelegateCommand ToggleOrdCalendarCommand { get; }

        #endregion

        #region Shipping Date

        public DelegateCommand StatusSelChangedCommand { get; }

        public DelegateCommand ShipDateBoxGotFocusCommand { get; }
        public DelegateCommand ShipDateBoxLostFocusCommand { get; }
        public DelegateCommand ShipDateBoxTextChangedCommand { get; }
        public DelegateCommand ShipDatePickerSelChangedCommand { get; }
        public DelegateCommand ToggleShipCalendarCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private async void SubmitCommandExecuteAsync()
        {
            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = -1;

            var ord = new Order
            {
                CustomerId = custId,
                OrderDate = SelectedOrdDate,
                ShippingDate = SelectedShipDate == null ? null : SelectedShipDate,
                PaymentMethodId = SelectedPayMethod.PaymentMethodId,
                ShippingMethodId = SelectedShipMethod.ShippingMethodId,
                OrderStatusId = SelectedStatus.OrderStatusId,
            };

            var val = _service.ValidateOrder(ord);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Order Registration from WPF form.");
                await _service.CreateAsync(ord);
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
                MessageBox.Show(valErrors.ToString(), "Failed to Register Order", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void SubmitCommandExecute()
        {
            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = -1;

            var ord = new Order
            {
                CustomerId = custId,
                OrderDate = SelectedOrdDate,
                ShippingDate = SelectedShipDate == null ? null : SelectedShipDate,
                PaymentMethodId = SelectedPayMethod.PaymentMethodId,
                ShippingMethodId = SelectedShipMethod.ShippingMethodId,
                OrderStatusId = SelectedStatus.OrderStatusId,
            };

            var val = _service.ValidateOrder(ord);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Order Registration from WPF form.");
                _service.Create(ord);
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
                MessageBox.Show(valErrors.ToString(), "Failed to Register Order", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        #region CustomerId

        private void CustBoxGotFocusCommandExecute()
        {
            if (CustEntryText == "Customer ID") CustEntryText = string.Empty;
        }

        private void CustBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(CustEntryText)) CustEntryText = "Customer ID";
        }

        private void CustBoxTextChangedCommandExecute()
        {
            var custId = -1;
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

        #endregion

        #region Order Date

        private void OrdDateBoxGotFocusCommandExecute()
        {
            if (OrdDateEntryText == "Placed On") OrdDateEntryText = string.Empty;
        }

        private void OrdDateBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(OrdDateEntryText)) OrdDateEntryText = "Placed On";
            var val = new DateTime();
            var parses = DateTime.TryParse(OrdDateEntryText, out val);
            if (parses)
            {
                OrdDateEntryText = val.ToShortDateString();
                SelectedOrdDate = val;
            }
        }

        private void OrdDateBoxTextChangedCommandExecute()
        {
            DateTime val;
            var parses = DateTime.TryParse(OrdDateEntryText, out val);
            var checkVal = !parses ? (DateTime?)null : val;

            var validation = _service.ValidateOrderDate(checkVal.GetValueOrDefault());
            if (!validation.isValid)
            {
                OrdErrorText = validation.errorList.GetUntilOrEmpty(".");
                OrdDateErrorVisibility = Visibility.Visible;
            }
            else
            {
                OrdErrorText = _defError;
                OrdDateErrorVisibility = Visibility.Collapsed;
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

        #endregion

        #region Shipping Date

        private void StatusSelChangedCommandExecute()
        {
            if (SelectedStatus.OrderStatusId > 1) ShipDateBoxTextChangedCommandExecute();
        }

        private void ShipDateBoxGotFocusCommandExecute()
        {
            if (ShipDateEntryText == "Shipped") ShipDateEntryText = string.Empty;
        }

        private void ShipDateBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(ShipDateEntryText))
            {
                ShipDateEntryText = "Shipped";
                SelectedShipDate = null;
            }
            var val = new DateTime();
            var parses = DateTime.TryParse(ShipDateEntryText, out val);
            if (parses)
            {
                ShipDateEntryText = val.ToShortDateString();
                SelectedShipDate = val;
            }
        }

        private void ShipDateBoxTextChangedCommandExecute()
        {
            if (OrdDateEntryText == "Placed On") return;
            var ordVal = new DateTime();
            var parses = DateTime.TryParse(OrdDateEntryText, out ordVal);
            if (!parses) return;

            DateTime val;
            parses = DateTime.TryParse(ShipDateEntryText, out val);
            var checkVal = !parses ? (DateTime?)null : val;

            var statId = -1;

            try
            {
            statId = SelectedStatus.OrderStatusId;
            }
            catch
            { return; }

            var datesValidation = _service.ValidateShippingDate(checkVal, ordVal);
            var statValidation = _service.ValidateShipWithStatus(checkVal, statId);

            var isValid = datesValidation.isValid && statId!=-1 && statValidation.isValid;
            var errorList = new StringBuilder(datesValidation.errorList).AppendLine(statValidation.errorList).ToString();

            if (!isValid)
            {
                ShipErrorText = errorList.GetUntilOrEmpty(".");
                ShipDateErrorVisibility = Visibility.Visible;
            }
            else
            {
                ShipErrorText = _defError;
                ShipDateErrorVisibility = Visibility.Collapsed;
            }


        }

        private void ShipDatePickerSelChangedCommandExecute()
        {
            var newDate = SelectedShipDate.GetValueOrDefault().ToShortDateString();
            ShipDispDate = SelectedShipDate;

            if (ShipDateEntryText == newDate) return;
            ShipDateEntryText = newDate;
        }

        private void ToggleShipCalendarCommandExecute()
        => ShipCalVisibility = ShipCalVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        #endregion


        #endregion

    }
}
