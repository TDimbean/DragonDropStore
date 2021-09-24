using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class PaymentAddViewModel : BindableBase
    {
        private IRelayReloadAndRemoteCloseControl _callingView;
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private IPaymentDataService _service;

        private string _defError = "This shouldn't be visible.";

        #region CustomerId

        private string _custEntryText;
        private string _custErrorText;
        private Visibility _custErrorVisibility;

        #endregion

        #region Amount

        private string _amountEntryText;
        private string _amountErrorText;
        private Visibility _amountErrorVisibility;

        #endregion

        #region Date

        private string _dateEntryText;
        private string _dateErrorText;
        private Visibility _dateErrorVisibility;
        private DateTime _selectedDate;
        private DateTime _dispDate;
        private Visibility _calVisibility;

        #endregion

        #endregion

        public PaymentAddViewModel(IPaymentDataService service, IPaymentMethodRepository methRepo,
            IRelayReloadAndRemoteCloseControl callingView=null)
        {
            _callingView = callingView;
            _service = service;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync) :
                new DelegateCommand(SubmitCommandExecute); ;

            #region CustomerId

            CustBoxGotFocusCommand = new DelegateCommand(CustBoxGotFocusCommandExecute);
            CustBoxLostFocusCommand = new DelegateCommand(CustBoxLostFocusCommandExecute);
            CustBoxTextChangedCommand = new DelegateCommand(CustBoxTextChangedCommandExecute);

            #endregion

            #region Amount

            AmountBoxGotFocusCommand = new DelegateCommand(AmountBoxGotFocusCommandExecute);
            AmountBoxLostFocusCommand = new DelegateCommand(AmountBoxLostFocusCommandExecute);
            AmountBoxTextChangedCommand = new DelegateCommand(AmountBoxTextChangedCommandExecute);

            #endregion

            #region Date

            DateBoxGotFocusCommand = new DelegateCommand(DateBoxGotFocusCommandExecute);
            DateBoxLostFocusCommand = new DelegateCommand(DateBoxLostFocusCommandExecute);
            DateBoxTextChangedCommand = new DelegateCommand(DateBoxTextChangedCommandExecute);
            DatePickerSelChangedCommand = new DelegateCommand(DatePickerSelChangedCommandExecute);
            ToggleCalendarCommand = new DelegateCommand(ToggleCalendarCommandExecute);

            #endregion

            #endregion

            #region Init

            Methods = new ObservableCollection<PaymentMethod>(methRepo.GetAll());

            #region CustomerId

            CustEntryText = "Customer ID";
            CustErrorText = _defError;
            CustErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Amount

            AmountEntryText = "Amount";
            AmountErrorText = _defError;
            AmountErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Date

            DateEntryText = "Date";
            DateErrorText = _defError;
            DateErrorVisibility = Visibility.Collapsed;
            DispDate = DateTime.Now;
            CalVisibility = Visibility.Collapsed;

            #endregion

            #endregion
        }

        #region Properties

        #region ViewModelProps

        public ObservableCollection<PaymentMethod> Methods { get; set; }

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

        public PaymentMethod SelectedMethod { get; set; }

        #region AmountEntry

        public string AmountEntryText
        {
            get => _amountEntryText;
            set => SetProperty(ref _amountEntryText, value);
        }

        public string AmountErrorText
        {
            get => _amountErrorText;
            set => SetProperty(ref _amountErrorText, value);
        }

        public Visibility AmountErrorVisibility
        {
            get => _amountErrorVisibility;
            set => SetProperty(ref _amountErrorVisibility, value);
        }

        #endregion

        #region DateEntry

        public string DateEntryText
        {
            get => _dateEntryText;
            set => SetProperty(ref _dateEntryText, value);
        }

        public string DateErrorText
        {
            get => _dateErrorText;
            set => SetProperty(ref _dateErrorText, value);
        }

        public Visibility DateErrorVisibility
        {
            get => _dateErrorVisibility;
            set => SetProperty(ref _dateErrorVisibility, value);
        }

        public DateTime SelectedDate
        {
            get=>_selectedDate;
            set=>SetProperty(ref _selectedDate,value);
        }

        public DateTime DispDate
        {
            get => _dispDate;
            set=>SetProperty(ref _dispDate,value);
        }

        public Visibility CalVisibility
        {
            get => _calVisibility;
            set => SetProperty(ref _calVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }

        #region Name

        public DelegateCommand CustBoxGotFocusCommand { get; }
        public DelegateCommand CustBoxLostFocusCommand { get; }
        public DelegateCommand CustBoxTextChangedCommand { get; }

        #endregion

        #region Amount

        public DelegateCommand AmountBoxGotFocusCommand { get; }
        public DelegateCommand AmountBoxLostFocusCommand { get; }
        public DelegateCommand AmountBoxTextChangedCommand { get; }

        #endregion

        #region Date

        public DelegateCommand DateBoxGotFocusCommand { get; }
        public DelegateCommand DateBoxLostFocusCommand { get; }
        public DelegateCommand DateBoxTextChangedCommand { get; }
        public DelegateCommand DatePickerSelChangedCommand { get; }
        public DelegateCommand ToggleCalendarCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private async void SubmitCommandExecuteAsync()
        {
            #region Converting Field Entry Data

            if (SelectedMethod == null) SelectedMethod = new PaymentMethod { PaymentMethodId = 0, Name = "Error" };

            var amount = 0m;
            var amountParses = decimal.TryParse(AmountEntryText.Replace("$", string.Empty), out amount);
            if (!amountParses) amount = 0m;

            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = -1;

            #endregion

            var pay = new Payment
            {
                CustomerId = custId,
                PaymentMethodId = SelectedMethod.PaymentMethodId,
                Amount = amount,
                Date = SelectedDate
            };

            var val = _service.ValidatePayment(pay);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Payment Registration from WPF form.");
                await _service.CreateAsync(pay);
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
                MessageBox.Show(valErrors.ToString(), "Failed to Register Payment", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SubmitCommandExecute()
        {
            #region Converting Field Entry Data
            if (SelectedMethod == null) SelectedMethod = new PaymentMethod { PaymentMethodId = 0, Name = "Error" };

            var amount = 0m;
            var amountParses = decimal.TryParse(AmountEntryText.Replace("$", string.Empty), out amount);
            if (!amountParses) amount = 0m;

            var custId = -1;
            var idParses = int.TryParse(CustEntryText, out custId);
            if (!idParses) custId = -1;

            #endregion

            var pay = new Payment
            {
                CustomerId = custId,
                PaymentMethodId = SelectedMethod.PaymentMethodId,
                Amount = amount,
                Date = SelectedDate
            };

            var val = _service.ValidatePayment(pay);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Payment Registration from WPF form.");
                _service.Create(pay);
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
                MessageBox.Show(valErrors.ToString(), "Failed to Register Payment", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var custId = 0;
            var idParses = int.TryParse(CustEntryText, out custId);

            if(!idParses)
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

        #region Amount

        private void AmountBoxGotFocusCommandExecute()
        {
            if (AmountEntryText == "Amount") AmountEntryText = string.Empty;
        }

        private void AmountBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(AmountEntryText)) AmountEntryText = "Amount";
            var val = 0m;
            var parses = decimal.TryParse(AmountEntryText, out val);
            if (parses) AmountEntryText = string.Format("{0:C}", val);
        }

        private void AmountBoxTextChangedCommandExecute()
        {
            decimal val;
            var parses = decimal.TryParse(AmountEntryText.Replace("$",""), out val);
            var checkVal = !parses ? (decimal?)null : val;

            var validation = _service.ValidateAmount(checkVal);
            if (!validation.isValid)
            {
                AmountErrorText = validation.errorList.GetUntilOrEmpty(".");
                AmountErrorVisibility = Visibility.Visible;
            }
            else
            {
                AmountErrorText = _defError;
                AmountErrorVisibility = Visibility.Collapsed;
            }

        }

        #endregion

        #region Date

        private void DateBoxGotFocusCommandExecute()
        {
            if (DateEntryText == "Date") DateEntryText = string.Empty;
        }

        private void DateBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(DateEntryText)) DateEntryText = "Date";
            var val = new DateTime();
            var parses = DateTime.TryParse(DateEntryText, out val);
            if (parses)
            {
                DateEntryText = val.ToShortDateString();
                SelectedDate = val;
                DispDate = val;
            }
        }

        private void DateBoxTextChangedCommandExecute()
        {
            DateTime val;
            var parses = DateTime.TryParse(DateEntryText, out val);
            var checkVal = !parses ? (DateTime?)null : val;

            var validation = _service.ValidateDate(checkVal);
            if (!validation.isValid)
            {
                DateErrorText = validation.errorList.GetUntilOrEmpty(".");
                DateErrorVisibility = Visibility.Visible;
            }
            else
            {
                DateErrorText = _defError;
                DateErrorVisibility = Visibility.Collapsed;
            }

        }

        private void DatePickerSelChangedCommandExecute()
        {
            var newDate = SelectedDate.ToShortDateString();
            DispDate = SelectedDate;

            if (DateEntryText == newDate) return;
            DateEntryText = newDate;
        }

        private void ToggleCalendarCommandExecute()
        => CalVisibility = CalVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        #endregion

        #endregion
    }
}
