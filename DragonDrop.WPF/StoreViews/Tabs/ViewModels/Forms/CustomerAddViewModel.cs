using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class CustomerAddViewModel : BindableBase
    {
        private IRelayReloadAndRemoteCloseControl _callingView;
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private ICustomerDataService _service;

        private string _defError = "This shouldn't be visible.";

        #region Name

        private string _nameEntryText;
        private string _nameErrorText;
        private Visibility _nameErrorVisibility;

        #endregion

        #region Phone

        private string _phoneEntryText;
        private string _phoneErrorText;
        private Visibility _phoneErrorVisibility;

        #endregion

        #region Email

        private string _emailEntryText;
        private string _emailErrorText;
        private Visibility _emailErrorVisibility;

        #endregion

        #region Address

        private string _adrEntryText;
        private string _adrErrorText;
        private Visibility _adrErrorVisibility;

        #endregion

        #region City

        private string _cityEntryText;
        private string _cityErrorText;
        private Visibility _cityErrorVisibility;

        #endregion

        #region State

        private string _stateEntryText;
        private string _stateErrorText;
        private Visibility _stateErrorVisibility;

        #endregion

        #endregion

        public CustomerAddViewModel(ICustomerDataService service,
            IRelayReloadAndRemoteCloseControl callingView=null)
        {
            _callingView = callingView;
            _service = service;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync) 
                : new DelegateCommand(SubmitCommandExecute);

            #region Name

            NameBoxGotFocusCommand = new DelegateCommand(NameBoxGotFocusCommandExecute);
            NameBoxLostFocusCommand = new DelegateCommand(NameBoxLostFocusCommandExecute);
            NameBoxTextChangedCommand = new DelegateCommand(NameBoxTextChangedCommandExecute);

            #endregion

            #region Phone

            PhoneBoxGotFocusCommand = new DelegateCommand(PhoneBoxGotFocusCommandExecute);
            PhoneBoxLostFocusCommand = new DelegateCommand(PhoneBoxLostFocusCommandExecute);
            PhoneBoxTextChangedCommand = new DelegateCommand<TextChangedEventArgs>(PhoneBoxTextChangedCommandExecute);

            #endregion

            #region Email

            EmailBoxGotFocusCommand = new DelegateCommand(EmailBoxGotFocusCommandExecute);
            EmailBoxLostFocusCommand = new DelegateCommand(EmailBoxLostFocusCommandExecute);
            EmailBoxTextChangedCommand = new DelegateCommand(EmailBoxTextChangedCommandExecute);

            #endregion

            #region Address

            AdrBoxGotFocusCommand = new DelegateCommand(AdrBoxGotFocusCommandExecute);
            AdrBoxLostFocusCommand = new DelegateCommand(AdrBoxLostFocusCommandExecute);
            AdrBoxTextChangedCommand = new DelegateCommand(AdrBoxTextChangedCommandExecute);

            #endregion

            #region City

            CityBoxGotFocusCommand = new DelegateCommand(CityBoxGotFocusCommandExecute);
            CityBoxLostFocusCommand = new DelegateCommand(CityBoxLostFocusCommandExecute);
            CityBoxTextChangedCommand = new DelegateCommand(CityBoxTextChangedCommandExecute);

            #endregion

            #region State

            StateBoxGotFocusCommand = new DelegateCommand(StateBoxGotFocusCommandExecute);
            StateBoxLostFocusCommand = new DelegateCommand(StateBoxLostFocusCommandExecute);
            StateBoxTextChangedCommand = new DelegateCommand(StateBoxTextChangedCommandExecute);

            #endregion

            #endregion

            #region Init

            #region Name

            NameEntryText = "Name";
            NameErrorText = _defError;
            NameErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Phone

            PhoneEntryText = "Phone";
            PhoneErrorText = _defError;
            PhoneErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Email

            EmailEntryText = "E-mail";
            EmailErrorText = _defError;
            EmailErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Address

            AdrEntryText = "Address";
            AdrErrorText = _defError;
            AdrErrorVisibility = Visibility.Collapsed;

            #endregion

            #region City

            CityEntryText = "City";
            CityErrorText = _defError;
            CityErrorVisibility = Visibility.Collapsed;

            #endregion

            #region State

            StateEntryText = "State";
            StateErrorText = _defError;
            StateErrorVisibility = Visibility.Collapsed;

            #endregion


            #endregion
        }

        #region Properties

        #region ViewModelProps

        #region Name Entry

        public string NameEntryText
        {
            get => _nameEntryText;
            set => SetProperty(ref _nameEntryText, value);
        }

        public string NameErrorText
        {
            get => _nameErrorText;
            set => SetProperty(ref _nameErrorText, value);
        }

        public Visibility NameErrorVisibility
        {
            get => _nameErrorVisibility;
            set => SetProperty(ref _nameErrorVisibility, value);
        }

        #endregion

        #region Phone Entry

        public string PhoneEntryText
        {
            get => _phoneEntryText;
            set => SetProperty(ref _phoneEntryText, value);
        }

        public string PhoneErrorText
        {
            get => _phoneErrorText;
            set => SetProperty(ref _phoneErrorText, value);
        }

        public Visibility PhoneErrorVisibility
        {
            get => _phoneErrorVisibility;
            set => SetProperty(ref _phoneErrorVisibility, value);
        }

        #endregion

        #region E-mail Entry

        public string EmailEntryText
        {
            get => _emailEntryText;
            set => SetProperty(ref _emailEntryText, value);
        }

        public string EmailErrorText
        {
            get => _emailErrorText;
            set => SetProperty(ref _emailErrorText, value);
        }

        public Visibility EmailErrorVisibility
        {
            get => _emailErrorVisibility;
            set => SetProperty(ref _emailErrorVisibility, value);
        }

        #endregion

        #region Address Entry

        public string AdrEntryText
        {
            get => _adrEntryText;
            set => SetProperty(ref _adrEntryText, value);
        }

        public string AdrErrorText
        {
            get => _adrErrorText;
            set => SetProperty(ref _adrErrorText, value);
        }

        public Visibility AdrErrorVisibility
        {
            get => _adrErrorVisibility;
            set => SetProperty(ref _adrErrorVisibility, value);
        }

        #endregion

        #region City Entry

        public string CityEntryText
        {
            get => _cityEntryText;
            set => SetProperty(ref _cityEntryText, value);
        }

        public string CityErrorText
        {
            get => _cityErrorText;
            set => SetProperty(ref _cityErrorText, value);
        }

        public Visibility CityErrorVisibility
        {
            get => _cityErrorVisibility;
            set => SetProperty(ref _cityErrorVisibility, value);
        }

        #endregion

        #region State Entry

        public string StateEntryText
        {
            get => _stateEntryText;
            set => SetProperty(ref _stateEntryText, value);
        }

        public string StateErrorText
        {
            get => _stateErrorText;
            set => SetProperty(ref _stateErrorText, value);
        }

        public Visibility StateErrorVisibility
        {
            get => _stateErrorVisibility;
            set => SetProperty(ref _stateErrorVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }

        #region Name

        public DelegateCommand NameBoxGotFocusCommand { get; }
        public DelegateCommand NameBoxLostFocusCommand { get; }
        public DelegateCommand NameBoxTextChangedCommand { get; }

        #endregion

        #region Phone

        public DelegateCommand PhoneBoxGotFocusCommand { get; }
        public DelegateCommand PhoneBoxLostFocusCommand { get; }
        public DelegateCommand<TextChangedEventArgs> PhoneBoxTextChangedCommand { get; }

        #endregion

        #region E-mail

        public DelegateCommand EmailBoxGotFocusCommand { get; }
        public DelegateCommand EmailBoxLostFocusCommand { get; }
        public DelegateCommand EmailBoxTextChangedCommand { get; }

        #endregion

        #region Address

        public DelegateCommand AdrBoxGotFocusCommand { get; }
        public DelegateCommand AdrBoxLostFocusCommand { get; }
        public DelegateCommand AdrBoxTextChangedCommand { get; }

        #endregion

        #region City

        public DelegateCommand CityBoxGotFocusCommand { get; }
        public DelegateCommand CityBoxLostFocusCommand { get; }
        public DelegateCommand CityBoxTextChangedCommand { get; }

        #endregion

        #region State

        public DelegateCommand StateBoxGotFocusCommand { get; }
        public DelegateCommand StateBoxLostFocusCommand { get; }
        public DelegateCommand StateBoxTextChangedCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private void SubmitCommandExecute()
        {
            var cust = new Customer
            {
                Name = NameEntryText,
                Phone = PhoneEntryText,
                Email = EmailEntryText,
                Address = AdrEntryText,
                City = CityEntryText,
                State = StateEntryText
            };

            var val = _service.ValidateCustomer(cust);
                
            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Customer Registration from WPF form.");
                _service.Create(cust);
                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList;
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed To Register Customer",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SubmitCommandExecuteAsync()
        {
            var cust = new Customer
            {
                Name = NameEntryText,
                Phone = PhoneEntryText,
                Email = EmailEntryText,
                Address = AdrEntryText,
                City = CityEntryText,
                State = StateEntryText
            };

            var val = _service.ValidateCustomer(cust);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Customer Registration from WPF form.");
                await _service.CreateAsync(cust);
                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList;
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed To Register Customer",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #region Name

        private void NameBoxGotFocusCommandExecute()
        {
            if (NameEntryText == "Name") NameEntryText = string.Empty;
        }

        private void NameBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(NameEntryText)) NameEntryText = "Name";
        }

        private void NameBoxTextChangedCommandExecute()
        {
            var validation = _service.ValidateName(NameEntryText);
            if (!validation.isValid)
            {
                NameErrorText = validation.errorList.GetUntilOrEmpty(".");
                NameErrorVisibility = Visibility.Visible;
            }
            else
            {
                NameErrorText = _defError;
                NameErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Phone

        private void PhoneBoxGotFocusCommandExecute()
        {
            if (PhoneEntryText == "Phone") PhoneEntryText = string.Empty;
        }

        private void PhoneBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(PhoneEntryText)) PhoneEntryText = "Phone";
        }

        private void PhoneBoxTextChangedCommandExecute(TextChangedEventArgs e)
        {
            ControlHelpers.HandlePhoneNumberInput(e);

            var validation = _service.ValidatePhone(PhoneEntryText);
            if (!validation.isValid)
            {
                PhoneErrorText = validation.errorList.GetUntilOrEmpty(".");
                PhoneErrorVisibility = Visibility.Visible;
            }
            else
            {
                PhoneErrorText = _defError;
                PhoneErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region E-mail

        private void EmailBoxGotFocusCommandExecute()
        {
            if (EmailEntryText == "Email") EmailEntryText = string.Empty;
        }

        private void EmailBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(EmailEntryText)) EmailEntryText = "Email";
        }

        private void EmailBoxTextChangedCommandExecute()
        {
            var validation = _service.ValidateEmail(EmailEntryText);
            if (!validation.isValid)
            {
                EmailErrorText = validation.errorList.GetUntilOrEmpty(".");
                EmailErrorVisibility = Visibility.Visible;
            }
            else
            {
                EmailErrorText = _defError;
                EmailErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Address

        private void AdrBoxGotFocusCommandExecute()
        {
            if (AdrEntryText == "Address") AdrEntryText = string.Empty;
        }

        private void AdrBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(AdrEntryText)) AdrEntryText = "Address";
        }

        private void AdrBoxTextChangedCommandExecute()
        {
            var validation = _service.ValidateAddress(AdrEntryText);
            if (!validation.isValid)
            {
                AdrErrorText = validation.errorList.GetUntilOrEmpty(".");
                AdrErrorVisibility = Visibility.Visible;
            }
            else
            {
                AdrErrorText = _defError;
                AdrErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region City

        private void CityBoxGotFocusCommandExecute()
        {
            if (CityEntryText == "City") CityEntryText = string.Empty;
        }

        private void CityBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(CityEntryText)) CityEntryText = "City";
        }

        private void CityBoxTextChangedCommandExecute()
        {
            var validation = _service.ValidateCity(CityEntryText);
            if (!validation.isValid)
            {
                CityErrorText = validation.errorList.GetUntilOrEmpty(".");
                CityErrorVisibility = Visibility.Visible;
            }
            else
            {
                CityErrorText = _defError;
                CityErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region State

        private void StateBoxGotFocusCommandExecute()
        {
            if (StateEntryText == "State") StateEntryText = string.Empty;
        }

        private void StateBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(StateEntryText)) StateEntryText = "State";
        }

        private void StateBoxTextChangedCommandExecute()
        {
            var validation = _service.ValidateState(StateEntryText);
            if (!validation.isValid)
            {
                StateErrorText = validation.errorList.GetUntilOrEmpty(".");
                StateErrorVisibility = Visibility.Visible;
            }
            else
            {
                StateErrorText = _defError;
                StateErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #endregion
    }
}
